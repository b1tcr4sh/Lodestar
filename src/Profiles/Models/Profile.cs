using Mercurius.Configuration;
using Mercurius.DBus;
using Mercurius.Modrinth.Models;
using Mercurius.Modrinth;
using Tmds.DBus;
using System.Threading.Tasks;
using NLog;

namespace Mercurius.Profiles {
    public class Profile {
        // Make a reference to it's respective json file, with methods to update, delete, etc.
        public string Name { get; set; }
        public string MinecraftVersion { get; set; }
        public bool ServerSide { get; set; }
        public ModLoader Loader { get; set; }
        public List<Mod> Mods { get; set; }

        public string Path { get => string.Format("{0}{1}.profile.json", SettingsManager.Settings.Profile_Directory, Name); } //"{SettingsManager.Settings.Profile_Directory}/{this.Name}.profile.json";
        // private bool _disposed = false;
        private ILogger logger = LogManager.GetCurrentClassLogger();

        internal static async Task<Profile> CreateNewAsync(string name, string minecraftVersion, ModLoader loader, bool serverSide) {
            Profile profile = new Profile {
                Name = name,
                MinecraftVersion = minecraftVersion,
                ServerSide = serverSide,
                Loader = loader,
                Mods = new List<Mod>()
                // _objectPath = new ObjectPath($"/org/mercurius/profile/{name}")
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
        internal async Task RemoveModFromListAsync(Mod modToRemove) {

            // List<Mod> parentsWithRemoveableDependency = Mods.Where<Mod>((mod) => mod.Dependencies.Where<Mod>((dependency) => dependency.Title.ToLower().Equals(modToRemove.Title))).ToList<Mod>();

            List<Mod> parentsWithRemoveableDependency = new List<Mod>();
            foreach (Mod mod in Mods) {
                bool containsRemoveableDep = false;
                foreach (string dependency in mod.DependencyVersions) {
                    if (dependency.Equals(modToRemove.VersionId)) {
                        containsRemoveableDep = true;
                    }
                }
                if (containsRemoveableDep) {
                    parentsWithRemoveableDependency.Add(mod);
                }
            }

            if (parentsWithRemoveableDependency.Count() <= 0) {
                logger.Debug("Removing {0}...", modToRemove.Title);
                Mods.Remove(modToRemove);

                await ProfileManager.OverwriteProfileAsync(this, this.Name);
                return;
            }
            
            foreach (Mod parent in parentsWithRemoveableDependency) {
                logger.Debug("Removing {0} as dependency of mod {1}", modToRemove.Title, parent.Title);
                Mods.Remove(parent);
                // parent.DependencyVersions.Remove(modToRemove.VersionId);

                await UpdateModListAsync(parent);
            }

            // foreach (Mod parent in Mods) {
            //     if (parent.Dependencies.Contains(modToRemove)) {
            //         Mods.Remove(parent);

            //         parent.Dependencies.Remove(modToRemove);
            //         await UpdateModListAsync(parent);
            //         Console.WriteLine(Mods);
            //         return;
            //     }
            // }

            // Mods.Remove(modToRemove);
            // await ProfileManager.OverwriteProfileAsync(this, this.Name);
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
        }

        // public void Dispose() {
        //     Dispose(false);
        //     GC.SuppressFinalize(this);
        // }
        // private void Dispose(bool disposing) {
        //     if (_disposed) return;
        //     if (disposing) {
        //         ProfileManager.UnloadProfile(this);
        //     }

        //     _disposed = true;
        // }
    }
    public enum ClientType {
        ClientSide, ServerSide
    }
    public enum ModLoader {
        forge, fabric, quilt
    }
}