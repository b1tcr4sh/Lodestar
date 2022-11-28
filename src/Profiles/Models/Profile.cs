using Mercurius.Configuration;
using Mercurius.DBus;
using Mercurius.Modrinth.Models;
using Mercurius.Modrinth;
using Tmds.DBus;
using System.Threading.Tasks;
using NLog;

// TODO: Make profile respond to local metadata changes
// either reload before any operation or make reload command?  (don't really want to do the latter)

namespace Mercurius.Profiles {
    public class Profile : IDisposable {
        public string Name { get; set; }
        public string MinecraftVersion { get; set; }
        public bool ServerSide { get; set; }
        public ModLoader Loader { get; set; }
        public List<Mod> Mods { get; set; }

        public string Path { get => string.Format("{0}{1}.profile.json", SettingsManager.Settings.Profile_Directory, Name); } //"{SettingsManager.Settings.Profile_Directory}/{this.Name}.profile.json";
        private ILogger logger = LogManager.GetCurrentClassLogger();
        private bool _disposed = false;

        internal static async Task<Profile> CreateNewAsync(string name, string minecraftVersion, ModLoader loader, bool serverSide) {
            Profile profile = new Profile {
                Name = name,
                MinecraftVersion = minecraftVersion,
                ServerSide = serverSide,
                Loader = loader,
                Mods = new List<Mod>()
            };
            await ProfileManager.WriteProfileAsync(profile);
            await ProfileManager.LoadProfileAsync(profile.Name);

            return profile;
        }

        internal async Task<Profile> UpdateAsync(Profile oldProfile, Profile newProfile) {
            if (oldProfile.Equals(newProfile)) return oldProfile;

            await ProfileManager.OverwriteProfileAsync(newProfile, newProfile.Name);
            await ProfileManager.LoadProfileAsync(newProfile.Name);
            return ProfileManager.GetLoadedProfile(newProfile.Name);
        }
        internal async Task UpdateModListAsync(List<Mod> mods) {
            if (Mods is null) {
                Mods = mods;
            } else {
                Mods.AddRange(mods);
            }

            await ProfileManager.OverwriteProfileAsync(this, this.Name);
        }
        internal async Task UpdateModListAsync(Mod mod) {
            Mods.Add(mod);

            await ProfileManager.OverwriteProfileAsync(this, this.Name);
        }
        internal async Task<bool> RemoveModFromListAsync(Mod modToRemove, bool force) {

            IEnumerable<Mod> dependants = Mods.Where<Mod>(mod => mod.DependencyVersions.Contains<string>(modToRemove.VersionId));
       
            if (dependants.Count() > 0 && !force) {
                throw new DependencyException($"{modToRemove.Title} is a dependency!");
            }

            bool success = Mods.Remove(modToRemove);
            await ProfileManager.OverwriteProfileAsync(this, this.Name);

            return success;
        }
        public async Task<Mod> AddModAsync(APIClient client, string projectId, Repo service, bool ignoreDependencies) {
            logger.Debug("Attempting to add mod {0} to profile {1}", projectId, Name);

            ProjectModel project = await client.GetProjectAsync(projectId);
            VersionModel[] versions = await client.ListVersionsAsync(project);

            VersionModel[] viableVersions = versions.Where<VersionModel>((version) => version.game_versions.Contains<string>(MinecraftVersion)).ToArray<VersionModel>();
            viableVersions = viableVersions.Where<VersionModel>((version) => version.loaders.Contains(Loader.ToString().ToLower())).ToArray<VersionModel>();

            if (viableVersions.Count() < 1) {
                logger.Debug("Found no installation candidates for install");
                throw new Exception("Found no valid installation candidates");
            }

            VersionModel version = await client.GetVersionInfoAsync(viableVersions[0].id);

            Mod mod = new Mod(version, project);

            List<Mod> modsToAdd = new List<Mod>();
            
            // resolve dependencies
            if (version.dependencies.Count() > 0 && !ignoreDependencies) {
                logger.Debug("Resolving Dependencies...");

                foreach (Dependency dependency in version.dependencies) {
                    VersionModel dependencyVersion = await client.GetVersionInfoAsync(dependency.version_id);
                    ProjectModel dependencyProject = await client.GetProjectAsync(dependencyVersion.project_id);

                    Mod dependencyMod = new Mod(dependencyVersion, dependencyProject);
                    // mod.AddDependency(dependencyMod);
                    mod.AddDependency(dependency.version_id);
                    // await AddModAsync(client, dependency.project_id, service, false);
                    modsToAdd.Add(dependencyMod);
                }
            }
            modsToAdd.Add(mod);

            await UpdateModListAsync(modsToAdd);
            logger.Info("Successfully added mod {0} to profile {1}", mod.Title, Name);
            return mod;
        }
        public async Task<Mod> AddVersionAsync(APIClient client, string versionId, bool ignoreDependencies) {
            VersionModel version = await client.GetVersionInfoAsync(versionId);
            ProjectModel project = await client.GetProjectAsync(version.project_id);

            Mod mod = new Mod(version, project);

            List<Mod> modsToAdd = new List<Mod>();
            
            // resolve dependencies
            if (version.dependencies.Count() > 0 && !ignoreDependencies) {
                logger.Debug("Resolving Dependencies...");

                foreach (Dependency dependency in version.dependencies) {
                    VersionModel dependencyVersion = await client.GetVersionInfoAsync(dependency.version_id);
                    ProjectModel dependencyProject = await client.GetProjectAsync(dependencyVersion.project_id);

                    Mod dependencyMod = new Mod(dependencyVersion, dependencyProject);
                    // mod.AddDependency(dependencyMod);
                    mod.AddDependency(dependency.version_id);
                    // await AddModAsync(client, dependency.project_id, service, false);
                    modsToAdd.Add(dependencyMod);
                }
            }
            modsToAdd.Add(mod);

            await UpdateModListAsync(modsToAdd);
            logger.Info("Successfully added mod {0} to profile {1}", mod.Title, Name);
            return mod;
        }
        public async Task<string[]> ResolveDependenciesAsync() {
            APIClient client = new APIClient();
            List<string> installedDependencies = new List<string>();

            foreach (Mod mod in Mods) {
                if (mod.DependencyVersions.Count() > 0) {
                    logger.Debug("{0} has {1} listed dependencies", mod.Title, mod.DependencyVersions.Count());

                    List<string> unmetDeps = new List<string>();

                    foreach (string dependency in mod.DependencyVersions) {
                        bool depencencyMet = Mods.Any<Mod>(checking => checking.VersionId.Equals(dependency));

                        if (!depencencyMet) {
                            unmetDeps.Add(dependency);
                            logger.Info("dependency {0} is unmet!", dependency);
                        }
                    }

                    if (unmetDeps.Count() < 1) {
                        logger.Info("All dependencies were met!");
                        return installedDependencies.ToArray<string>();
                    }

                    logger.Info("Adding missing dependencies...");
                    foreach (string unmet in unmetDeps) {
                        installedDependencies.Add(unmet);
                        await AddVersionAsync(client, unmet, false);
                    }
                }
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
                ProfileManager.DeleteProfileFile(Name);
            else {
                logger.Debug($"No file exists for profile {Name}... Unloading...");
                ProfileManager.UnloadProfile(this);
                return;
            }

            // Dipose/unload
            ProfileManager.UnloadProfile(this);   
            Dispose();         
        }
        public void Dispose() {
            Dispose(false);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing) {
            if (_disposed) return;
            if (disposing) {
                ProfileManager.UnloadProfile(this);
            }

            _disposed = true;
        }
    }
    public enum ClientType {
        ClientSide, ServerSide
    }
    public enum ModLoader {
        forge, fabric, quilt
    }
}