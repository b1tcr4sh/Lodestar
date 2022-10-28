using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mercurius.Modrinth.Models;
using Mercurius.Modrinth;
using Mercurius.Configuration;
using Mercurius.DBus.Commands;
using Mercurius.DBus;
using NLog;

namespace Mercurius.Profiles {
    public static class ProfileManager {
        private static Dictionary<string, Profile> LoadedProfiles;
        public static Profile SelectedProfile { get; private set; }
        private static string ProfilePath;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static void InitializeDirectory() {
            LoadedProfiles = new Dictionary<string, Profile>();
            if (SettingsManager.Settings is null) {
                logger.Fatal("Couldn't find configuration file!");
                System.Environment.Exit(1);
            } 
            ProfilePath = SettingsManager.Settings.Profile_Directory;
            if (!Directory.Exists(ProfilePath)) {
                Directory.CreateDirectory(ProfilePath);
                logger.Debug("Created Profiles Directory at {0}", ProfilePath);
            } 
        }
        public static void InitializeDirectory(string path) {
            LoadedProfiles = new Dictionary<string, Profile>();
            ProfilePath = @path;
            if (!Directory.Exists(ProfilePath)) Directory.CreateDirectory(ProfilePath); 
        }
        
        public static Profile GetLoadedProfile(string name) {
            if (LoadedProfiles.Keys.Contains(name)) return LoadedProfiles[name];
            else throw new ProfileException($"Profile {name} doesn't exist!");
        }
        public static IReadOnlyDictionary<string, Profile> GetLoadedProfiles() {
            if (LoadedProfiles is null || LoadedProfiles.Count <= 0) return new Dictionary<string, Profile>() as IReadOnlyDictionary<string, Profile>;

            return LoadedProfiles as IReadOnlyDictionary<string, Profile>;
        }
        public static bool SelectProfile(string name) {
            foreach (KeyValuePair<string, Profile> profile in LoadedProfiles) {                
                if (profile.Key.Equals(name)) {
                    SelectedProfile = profile.Value;

                    logger.Debug($"Selected profile {name}");
                    return true;
                }
            }
            return false;
        }
        public static void LoadAllProfiles() {
            LoadedProfiles = new Dictionary<string, Profile>();
            string[] files = Directory.GetFiles(ProfilePath);
            
            foreach (string file in files) {
                try {
                    string contents = File.ReadAllText(file, Encoding.ASCII);
                    Profile profile = JsonSerializer.Deserialize<Profile>(contents);

                    if (profile.Name.Contains(" "))
                        logger.Warn($"Profile at {profile.Path} was unable to be loaded (Name contained spaces)");
                    else if (!LoadedProfiles.ContainsKey(profile.Name))
                        LoadedProfiles.Add(profile.Name, profile); 
                } catch (Exception e) {
                    logger.Warn(@$"Error occurred loading profile at {file}");
                    logger.Trace(e.Message);
                }
            }

            foreach (KeyValuePair<string, Profile> profile in LoadedProfiles) {
                if (!File.Exists($@"{ProfilePath}/{profile.Key.ToLower()}.profile.json"))
                    LoadedProfiles.Remove(profile.Key);
            }

            logger.Info($"Loaded {LoadedProfiles.Count} profiles");
        }
        public static async Task<bool> AddModAsync(APIClient client, string id, bool ignoreDependencies) {
            // string id;
            // SearchModel search = await client.SearchAsync(query);
            // if (!query.ToLower().Equals(search.hits[0].title.ToLower())) {
            //     return false;
            // } else id = search.hits[0].project_id;

            // logger.Debug("Attempting to add mod {0} to profile {1}", query, SelectedProfile.Name);

            ProjectModel project = await client.GetProjectAsync(id);
            VersionModel[] versions = await client.ListVersionsAsync(project);

            VersionModel[] viableVersions = versions.Where<VersionModel>((version) => version.game_versions.Contains<string>(ProfileManager.SelectedProfile.MinecraftVersion)).ToArray<VersionModel>();
            viableVersions = viableVersions.Where<VersionModel>((version) => version.loaders.Contains(SelectedProfile.Loader.ToLower())).ToArray<VersionModel>();

            if (viableVersions.Count() < 1) {
                Console.WriteLine("There were no valid installation candidates.  Aborting...");
                logger.Info("Found no installation candidates for install");
                return false;
            }

            VersionModel version = await client.GetVersionInfoAsync(viableVersions[0].id);

            Mod mod = new Mod(version, project);
            
            if (version.dependencies.Count() > 0 && !ignoreDependencies) {
                Console.WriteLine("Revolving Dependencies...");

                foreach (Dependency dependency in version.dependencies) {
                    VersionModel dependencyVersion = await client.GetVersionInfoAsync(dependency.version_id);
                    ProjectModel dependencyProject = await client.GetProjectAsync(dependencyVersion.project_id);

                    Mod dependencyMod = new Mod(dependencyVersion, dependencyProject);
                    mod.AddDependency(dependencyMod);
                }
            }
            Console.WriteLine("Updating Profile...");
            await SelectedProfile.UpdateModListAsync(mod);
            logger.Info("Successfully added mod {0} to profile {1}", mod.Title, SelectedProfile.Name);
            return true;
        }
        
        public static async Task<Profile> LoadProfileAsync(string name) {
            string[] files = Directory.GetFiles(ProfilePath);

            foreach (string file in files) {
                logger.Debug("Attempting to load profile from {0}", file);

                try {
                    string fileContents = await File.ReadAllTextAsync(file, Encoding.ASCII);
                    Profile profile = JsonSerializer.Deserialize<Profile>(fileContents);

                    if (profile.Name.ToLower().Equals(name.ToLower())) {
                        Console.WriteLine($"Loaded profile {profile.Name}");
                        logger.Debug("Loaded profile {0} at {1}", profile.Name, profile.Path);

                        LoadedProfiles.Add(profile.Name, profile);
                        return profile;
                    }
                } catch (Exception e) {
                    logger.Warn(@$"Error occurred loading profile at {file}:");
                    logger.Trace(e.Message);
                } 
            }
            throw new ProfileException($"Profile {name} not found!");
        }
        
        internal static async Task WriteProfileAsync(Profile profile) {
            if (File.Exists($@"{ProfilePath}/{profile.Name}.profile.json")) return;

            logger.Debug("Writing New Profile {0} to {1}", profile, ProfilePath);
            using FileStream stream = new FileStream($@"{ProfilePath}/{profile.Name}.profile.json", FileMode.CreateNew, FileAccess.Write);

            await JsonSerializer.SerializeAsync<Profile>(stream, profile, new JsonSerializerOptions { IncludeFields = true, WriteIndented = true });
            stream.Close();
        }
        internal static async Task OverwriteProfileAsync(Profile profile, string existingProfileName) {
            if (!File.Exists($@"{ProfilePath}/{existingProfileName.ToLower()}.profile.json")) throw new ProfileException($"Profile Expected at {ProfilePath}/{existingProfileName.ToLower()}.profile.json Doesnt' Exist!");
            
            logger.Debug("Overwriting Profile {0} at {1}", profile, ProfilePath);
            using FileStream stream = new FileStream($@"{ProfilePath}/{existingProfileName.ToLower()}.profile.json", FileMode.Create, FileAccess.Write);

            await JsonSerializer.SerializeAsync<Profile>(stream, profile, new JsonSerializerOptions { IncludeFields = true, WriteIndented = true });
            stream.Close();
        }
        internal static bool DeleteProfileFile(string profileName) {
            if (!File.Exists($"{ProfilePath}/{profileName.ToLower()}.profile.json")) {
                logger.Debug("Attempted to Delete Profile {0}, but File Didn't Exist... ?", profileName);
                return false;
            }

            File.Delete($"{ProfilePath}/{profileName.ToLower()}.profile.json");
            logger.Debug("Deleted Profile at {0}", $"{ProfilePath}/{profileName.ToLower()}.profile.json");
            return true;
        }
        internal static void UnloadProfile(Profile profile) {
            // if (SelectedProfile is null) {
            //     Console.WriteLine("No profile selected... ?");
            //     return;
            // }
            // else 
            if (SelectedProfile is not null && SelectedProfile.Equals(profile)) {
                SelectedProfile = null;
                Console.WriteLine($"Profile {profile.Name} deselected");
            }

            if (LoadedProfiles.ContainsKey(profile.Name)) {
                LoadedProfiles.Remove(profile.Name);
                logger.Debug("Unloaded Profile {0} at {1}", profile.Name, profile.Path);
                LoadAllProfiles();
            } else throw new ProfileException($"Profile {profile.Name} doesn't exist!");
        }
    }
}