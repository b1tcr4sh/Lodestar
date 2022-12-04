using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mercurius.Modrinth.Models;
using Mercurius.Modrinth;
using Mercurius.Configuration;
using Mercurius.DBus;
using NLog;

namespace Mercurius.Profiles {
    public static class ProfileManager {
        private static Dictionary<string, Profile> LoadedProfiles;
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
        
        public static async Task<Profile> GetLoadedProfileAsync(string name) {
            if (!LoadedProfiles.Keys.Contains(name)) throw new ProfileException($"Profile {name} doesn't exist!");

            Profile foundProfile = LoadedProfiles[name];

            if (await foundProfile.VerifyLocalFileAsync()) {
                return foundProfile;
            } else {
                return LoadedProfiles[name];
            }
        }
        public static IReadOnlyDictionary<string, Profile> GetLoadedProfiles() {
            if (LoadedProfiles is null || LoadedProfiles.Count <= 0) return new Dictionary<string, Profile>() as IReadOnlyDictionary<string, Profile>;

            return LoadedProfiles as IReadOnlyDictionary<string, Profile>;
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
                    else if (!LoadedProfiles.ContainsKey(profile.Name)) {
                        profile.GenerateChecksum();
                        LoadedProfiles.Add(profile.Name, profile); 
                    }
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
        
        public static async Task<Profile> LoadProfileAsync(string name) {
            string[] files = Directory.GetFiles(ProfilePath);

            foreach (string file in files) {
                logger.Debug("Attempting to load profile from {0}", file);

                try {
                    string fileContents = await File.ReadAllTextAsync(file, Encoding.ASCII);
                    Profile profile = JsonSerializer.Deserialize<Profile>(fileContents);

                    if (profile.Name.ToLower().Equals(name.ToLower())) {
                        logger.Debug("Loaded profile {0} at {1}", profile.Name, profile.Path);

                        profile.GenerateChecksum();

                        LoadedProfiles.Add(profile.Name, profile);
                        return profile;
                    }
                } catch (Exception e) {
                    logger.Warn(@$"Error occurred loading profile at {file}:");
                    logger.Warn(e.Message);
                } 
            }
            throw new ProfileException($"Profile {name} wasn't found!");
        }
        
        public static async Task<Profile> LoadProfilFromFileAsync(string path) {
            logger.Debug("Attempting to load profile from {0}", path);

            Profile profile;

                try {
                    string fileContents = await File.ReadAllTextAsync(path, Encoding.ASCII);
                    profile = JsonSerializer.Deserialize<Profile>(fileContents);
                    logger.Debug("Loaded profile {0} at {1}", profile.Name, profile.Path);

                    profile.GenerateChecksum();
                    LoadedProfiles.Add(profile.Name, profile);
                } catch (Exception e) {
                    logger.Warn(@$"Error occurred loading profile at {path}:");
                    logger.Warn(e.Message);

                    throw new ProfileException("Profile failed to load");
                } 
            return profile;
        }
        internal static async Task WriteProfileAsync(Profile profile) {
            if (File.Exists($@"{ProfilePath}/{profile.Name}.profile.json")) throw new ProfileException($"File {profile.Name} already has exisint file!");

            logger.Debug("Writing New Profile {0} to {1}", profile, ProfilePath);
            using FileStream stream = new FileStream($@"{ProfilePath}/{profile.Name}.profile.json", FileMode.CreateNew, FileAccess.Write);

            await JsonSerializer.SerializeAsync<Profile>(stream, profile, new JsonSerializerOptions { IncludeFields = true, WriteIndented = true });
            stream.Close();

            profile.GenerateChecksum();
        }
        internal static async Task OverwriteProfileAsync(Profile profile, string existingProfileName) {
            if (!File.Exists($@"{ProfilePath}/{existingProfileName.ToLower()}.profile.json")) throw new ProfileException($"Profile Expected at {ProfilePath}/{existingProfileName.ToLower()}.profile.json Doesnt' Exist!");
            
            logger.Debug("Overwriting Profile {0} at {1}", profile, ProfilePath);
            using FileStream stream = new FileStream(profile.Path, FileMode.Create, FileAccess.Write);

            await JsonSerializer.SerializeAsync<Profile>(stream, profile, new JsonSerializerOptions { IncludeFields = true, WriteIndented = true });
            stream.Close();

            profile.GenerateChecksum();
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
            if (LoadedProfiles.ContainsKey(profile.Name)) {
                LoadedProfiles.Remove(profile.Name);
                logger.Debug("Unloaded Profile {0} at {1}", profile.Name, profile.Path);
                // LoadAllProfiles();
            } else throw new ProfileException($"Profile {profile.Name} doesn't exist!");
        }
        internal static async Task SyncProfileAsync(Profile profile, APIClient client) {
            logger.Info("Syncing {0}", profile.Name);


            List<string> existingFiles = Directory.GetFiles($"{SettingsManager.Settings.Minecraft_Directory}/mods/").ToList<string>();
            List<string> modPaths = new List<string>();

            // if (existingFiles.Count <= 0) {
            //     // No mods to sync
            //     return;
            // }

            foreach (Mod mod in profile.Mods) {
                modPaths.Add($"{SettingsManager.Settings.Minecraft_Directory}/mods/{mod.FileName}");

                // foreach (Mod dependency in mod.DependencyVersions) {
                //     modPaths.Add($"{SettingsManager.Settings.Minecraft_Directory}/mods/{dependency.FileName}");
                // }
            }
            
            List<string> keepers = existingFiles.Intersect<string>(modPaths).ToList<string>();

            foreach (string mod in keepers) {
                existingFiles.Remove(mod);
            }

            if (existingFiles.Count <= 0) {
                logger.Info("There are no Residiual Mod jars to Remove");
            } else {
                logger.Info("Removing Residual Mod jars...");
                foreach (string filePath in existingFiles) {
                    File.Delete(filePath);
                    logger.Debug("Deleted mod jar at {0}", filePath);
                }
                    
            }

            if (profile.Mods.Count <= 0) {
                logger.Info("Profile has no mods to sync!");
                throw new ProfileException("Profile has no mods to sync!");
            }

            logger.Debug("Queuing {0} mods for install", profile.Mods.Count);
            List<Mod> preQueue = new List<Mod>();
            preQueue.AddRange(profile.Mods);
            // foreach (Mod mod in selectedProfile.Mods) { // Dependencies are already root level now, so no need to collect them before install
            //     preQueue.AddRange(mod.DependencyVersions);                
            // }
            

            // Queue mods for install
            List<Mod> installQueue = new List<Mod>();

            foreach (Mod mod in preQueue) {
                if (File.Exists($"{SettingsManager.Settings.Minecraft_Directory}/mods/{mod.FileName}")) {
                    logger.Debug("{0}: {1} is already installed, skipping...", mod.Title, mod.ModVersion);
                        
                } else
                    installQueue.Add(mod);
            }
                logger.Debug("Attempting to install mods...");
                foreach (Mod mod in installQueue) {
                    await client.DownloadVersionAsync(mod);
                }
        }
    }
}