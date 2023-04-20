using System.Security.Cryptography;
using NLog;

using Mercurius.Configuration;
using Mercurius.DBus;
using Mercurius.API;
using Mercurius.API.Modrinth;

namespace Mercurius.Profiles {
    public class Profile : IDisposable {
        public string Name { get; set; }
        public string MinecraftVersion { get; set; }
        public bool ServerSide { get; set; }
        public ModLoader Loader { get; set; }
        public List<Mod> Mods { get; set; }
        public string Path { get => string.Format("{0}{1}.profile.json", SettingsManager.Settings.Profile_Directory, Name); } //"{SettingsManager.Settings.Profile_Directory}/{this.Name}.profile.json";
        
        public ProfileManager Manager;
        public APIs Apis;
        private ILogger logger = LogManager.GetCurrentClassLogger();
        private bool _disposed = false;
        private string checksum;


        internal static Profile CreateNew(string name, string minecraftVersion, ModLoader loader, bool serverSide, ProfileManager manager, APIs apis) {
            return new Profile(name, minecraftVersion, loader, serverSide, manager, apis); 
        }
        internal Profile(string name, string minecraftVersion, ModLoader loader, bool serverSide, ProfileManager manager, APIs apis) {
            Name = name;
            MinecraftVersion = minecraftVersion;
            ServerSide = serverSide;
            Loader = loader;
            Mods = new List<Mod>();
            Manager = manager;
            Apis = apis;

            Manager.WriteProfileAsync(this).GetAwaiter().GetResult();
            Manager.LoadProfileAsync(Name).GetAwaiter().GetResult();
        } 

        public bool Exists() {
            return Manager.GetLoadedProfiles().Keys.Contains(Name);
        }

        public void GenerateChecksum() {
            using (MD5 md5Instance = MD5.Create()) {
                using (var stream = File.OpenRead(Path)) {
                   byte[] raw = md5Instance.ComputeHash(stream);
                   checksum = BitConverter.ToString(raw).Replace("-","").ToLower();

                   logger.Debug("Generated new checksum for {0}: {1}", Name, checksum);
                }
            }
        }
        public async Task<bool> VerifyLocalFileAsync() {
            if (!File.Exists(Path)) {
                DbusHandler.DeregisterProfile(Name, Manager);
                Delete();
                
                throw new ProfileException($"Profile file at {Path} expected");
            }

            string hashResult;

            using (MD5 md5Instance = MD5.Create()) {
                using (var stream = File.OpenRead(Path)) {
                   byte[] rawHashResult = md5Instance.ComputeHash(stream);
                   hashResult = BitConverter.ToString(rawHashResult).Replace("-","").ToLower();
                }
            }

            if (hashResult.Equals(checksum)) {
                logger.Debug("Checksum matches local file");
                return true;
            } else {
                logger.Debug("New checksum found, reloading profile...");

                Manager.UnloadProfile(this);
                Profile reloaded = await Manager.LoadProfileAsync(Name);

                DbusHandler.DeregisterProfile(Name, Manager);
                await DbusHandler.RegisterProfileAsync(new DbusProfile(reloaded));
                Dispose();
                return false;
            }
        }

        internal async Task<Profile> UpdateAsync(Profile oldProfile, Profile newProfile) {
            if (oldProfile.Equals(newProfile)) return oldProfile;

            await Manager.OverwriteProfileAsync(newProfile, newProfile.Name);
            await Manager.LoadProfileAsync(newProfile.Name);
            return await Manager.GetLoadedProfileAsync(newProfile.Name);
        }
        internal async Task UpdateModListAsync(List<Mod> mods) {
            if (Mods is null) {
                Mods = mods;
            } else {
                Mods.AddRange(mods);
            }

            await Manager.OverwriteProfileAsync(this, this.Name);
        }
        internal async Task UpdateModListAsync(Mod mod) {
            Mods.Add(mod);

            await Manager.OverwriteProfileAsync(this, this.Name);
        }
        internal async Task<bool> RemoveModFromListAsync(Mod modToRemove, bool force) {

            IEnumerable<Mod> dependants = Mods.Where<Mod>(mod => mod.DependencyVersions.ContainsKey(modToRemove.VersionId));
       
            if (dependants.Count() > 0 && !force) {
                throw new DependencyException($"{modToRemove.Title} is a dependency!");
            }

            bool success = Mods.Remove(modToRemove);
            await Manager.OverwriteProfileAsync(this, this.Name);

            return success;
        }
        internal async Task<bool> RemoveModsFromListAsync(IEnumerable<Mod> modsToRemove, bool force) {
            bool success = true;

            foreach (Mod modToRemove in modsToRemove) {
                IEnumerable<Mod> dependants = Mods.Where<Mod>(mod => mod.DependencyVersions.ContainsKey(modToRemove.VersionId));
       
                if (dependants.Count() > 0 && !force) {
                    logger.Warn("{0} is a dependency!", modToRemove.Title);
                } else {
                    if (!Mods.Remove(modToRemove)) {
                        success = false;
                    }
                }
            }

            await Manager.OverwriteProfileAsync(this, this.Name);
            return success;            
        }
        public async Task<IReadOnlyList<Mod>> AddLatestModVersionAsync(string projectId, Remote service, bool ignoreDependencies, bool dryRun) {
            if (dryRun) logger.Debug("Attempting to add mod {0} to profile {1}", projectId, Name);
            else logger.Debug("Dry running fetch for mod {0}", projectId);

            Repository client = Apis.Get(service);

            ProjectModel project = await client.GetModProjectAsync(projectId);
            VersionModel[] versions = await client.ListVersionsAsync(project.id);

            VersionModel[] viableVersions = versions.Where<VersionModel>((version) => version.game_versions.Contains<string>(MinecraftVersion)).ToArray<VersionModel>();
            viableVersions = viableVersions.Where<VersionModel>((version) => version.loaders.Contains(Loader.ToString().ToLower())).ToArray<VersionModel>();

            if (viableVersions.Count() < 1) {
                logger.Debug("Found no installation candidates for install");
                throw new Exception("Found no valid installation candidates");
            }

            VersionModel version = await client.GetVersionInfoAsync(viableVersions[0].id);

            if (Mods.Any<Mod>(mod => mod.VersionId.Equals(version.id))) {
                throw new ProfileException($"Profile already contains {project.title}");
            }

            Mod mod = new Mod(version, project);

            List<Mod> modsToAdd = new List<Mod>();
            
            // resolve dependencies
            if (version.dependencies.Count() > 0 && !ignoreDependencies) {
                logger.Debug("Resolving Dependencies...");

                foreach (Dependency dependency in version.dependencies) {
                    if (dependency.version_id is null) {
                        throw new VersionInvalidException("A dependency was null from Mopdrinth...?");
                    }

                    VersionModel dependencyVersion;
                    ProjectModel dependencyProject;

                    try {
                        dependencyVersion = await client.GetVersionInfoAsync(dependency.version_id);
                        dependencyProject = await client.GetModProjectAsync(dependencyVersion.project_id);                        
                    } catch (VersionInvalidException) {
                        logger.Warn("Version could not be found... ?");
                        break;
                    } catch (ProjectInvalidException) {
                        logger.Warn("Project could not be found...?");
                        break;
                    }

                    if (Mods.Any<Mod>(mod => mod.VersionId.Equals(dependencyVersion.id))) {
                        logger.Warn($"Profile already contains {dependencyProject.title}, skipping...");
                    } else {
                        Mod dependencyMod = new Mod(dependencyVersion, dependencyProject);
                        mod.AddDependency(dependency.version_id);
                        modsToAdd.Add(dependencyMod);
                    }
                }
            }
            modsToAdd.Add(mod);

            if (!dryRun) {
                await UpdateModListAsync(modsToAdd);
                logger.Info("Successfully added mod {0} to profile {1}", mod.Title, Name);
            }
            return modsToAdd;
        }
        public async Task<Mod[]> AddModsAsync(string[] projectIds, Remote service, bool ignoreDependencies) {
            List<Mod> modsToAdd = new List<Mod>();
            Repository client = Apis.Get(service);

            foreach (string projectId in projectIds) {
                logger.Debug("Attempting to add mod {0} to profile {1}", projectId, Name);

                ProjectModel project = await client.GetModProjectAsync(projectId);
                VersionModel[] versions = await client.ListVersionsAsync(project.id);

                VersionModel[] viableVersions = versions.Where<VersionModel>((version) => version.game_versions.Contains<string>(MinecraftVersion)).ToArray<VersionModel>();
                viableVersions = viableVersions.Where<VersionModel>((version) => version.loaders.Contains(Loader.ToString().ToLower())).ToArray<VersionModel>();

                if (viableVersions.Count() < 1) {
                    logger.Debug("Found no installation candidates for install");
                    throw new Exception("Found no valid installation candidates");
                }

                VersionModel version = await client.GetVersionInfoAsync(viableVersions[0].id);

                Mod mod = new Mod(version, project);

                // resolve dependencies
                if (version.dependencies.Count() > 0 && !ignoreDependencies) {
                    logger.Debug("Resolving Dependencies...");

                    foreach (Dependency dependency in version.dependencies) {
                        VersionModel dependencyVersion = await client.GetVersionInfoAsync(dependency.version_id);
                        ProjectModel dependencyProject = await client.GetModProjectAsync(dependencyVersion.project_id);

                        Mod dependencyMod = new Mod(dependencyVersion, dependencyProject);
                        mod.AddDependency(dependency.version_id);
                        modsToAdd.Add(dependencyMod);
                    }
                }
                modsToAdd.Add(mod);
                logger.Info("Successfully added mod {0} to profile {1}", mod.Title, Name);
            }

            await UpdateModListAsync(modsToAdd);
            return modsToAdd.ToArray<Mod>();
        }
        public async Task<Mod> AddModVersionAsync(ModrinthAPI client, string versionId, bool ignoreDependencies) {
            VersionModel version = await client.GetVersionInfoAsync(versionId);
            ProjectModel project = await client.GetModProjectAsync(version.project_id);

            Mod mod = new Mod(version, project);

            List<Mod> modsToAdd = new List<Mod>();
            
            // resolve dependencies
            if (version.dependencies.Count() > 0 && !ignoreDependencies) {
                logger.Debug("Resolving Dependencies...");

                foreach (Dependency dependency in version.dependencies) {
                    VersionModel dependencyVersion = await client.GetVersionInfoAsync(dependency.version_id);
                    ProjectModel dependencyProject = await client.GetModProjectAsync(dependencyVersion.project_id);

                    Mod dependencyMod = new Mod(dependencyVersion, dependencyProject);
                    mod.AddDependency(dependency.version_id);
                    modsToAdd.Add(dependencyMod);
                }
            }
            modsToAdd.Add(mod);

            await UpdateModListAsync(modsToAdd);
            logger.Info("Successfully added mod {0} to profile {1}", mod.Title, Name);
            return mod;
        }
        public async Task<string[]> ResolveDependenciesAsync() {
            ModrinthAPI client = APIManager.Modrinth;
            List<string> installedDependencies = new List<string>();
            List<string> unmetDeps = new List<string>();

            foreach (Mod mod in Mods) {
                logger.Debug("{0} has {1} listed dependencies", mod.Title, mod.DependencyVersions.Count());
                if (mod.DependencyVersions.Count() > 0) {
                    foreach (string dependency in mod.DependencyVersions) {
                        bool depencencyMet = Mods.Any<Mod>(checking => checking.VersionId.Equals(dependency));

                        if (!depencencyMet) {
                            unmetDeps.Add(dependency);
                            logger.Info("dependency {0} is unmet!", dependency);
                        }
                    }
                }
            }

            if (unmetDeps.Count() < 1) {
                logger.Info("All dependencies were met!");
                return installedDependencies.ToArray<string>();
            }

            logger.Info("Adding missing dependencies...");
            foreach (string unmet in unmetDeps) {
                installedDependencies.Add(unmet);
                await AddModVersionAsync(client, unmet, false);
            }
            return installedDependencies.ToArray<string>();
        }
        public bool isSynced() {
            foreach (Mod mod in Mods) {
                if (!mod.CheckFileExists()) {
                    return false;
                }
            }

            return true;
        }
        // internal async Task<Mod> GenerateFromModFiles(APIClient client) {
        //     List<string> existingFiles = Directory.GetFiles($"{SettingsManager.Settings.Minecraft_Directory}/mods/").ToList<string>();

        //     if (Mods.Count >= 1) {
        //         foreach (Mod mod in Mods) {
        //             if (!mod.FileExists()) {
        //                 await RemoveModFromListAsync(mod);

        //                 foreach (Mod dependency in mod.Dependencies) {
        //                     if (dependency.FileExists()) {
        //                         File.Delete($"{SettingsManager.Settings.Minecraft_Directory}/mods/{dependency.FileName}");
        //                     }
        //                 }
        //             } else {
        //                 existingFiles.Remove($"{SettingsManager.Settings.Minecraft_Directory}/mods/{mod.FileName}");
                        
        //                 foreach (Mod dependency in mod.Dependencies) {
        //                     if (mod.FileExists()) {
        //                         existingFiles.Remove($"{SettingsManager.Settings.Minecraft_Directory}/mods/{dependency.FileName}");
        //                     }
        //                 }   
        //             }
        //         }
        //     }

        //     SearchModel searchRes = await client.SearchAsync(name);

        //     List<Hit> candidates = new List<Hit>();

        //     foreach (Hit hit in searchRes.hits) {
        //         if (name.Contains(hit.title.ToLower()) && hit.versions.Contains(selectedProfile.MinecraftVersion)) {
        //             return await ProfileManager.FetchModAsync(client, hit.project_id, Repo.modrinth, false);
        //         }
        //     }
        //     throw new Exception("No valid install candidates found!");
        // }
        internal void Delete() {
            if (File.Exists(Path))
                Manager.DeleteProfileFile(Name);
                
            // Dipose/unload   
            Dispose(true);         
        }
        public void Dispose() {
            Dispose(false);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing) {
            if (_disposed) return;
            if (disposing) {
                Manager.UnloadProfile(this);
            }

            _disposed = true;
        }
    }
}