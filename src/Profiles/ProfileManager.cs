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
        // private static Profile SelectedProfile; 
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

        // public static async Task<Profile> GetSelectedProfileAsync(bool regenIfMissing = false) {
        //     if (SelectedProfile is null) {
        //         throw new ProfileException("No Profile Selected!");
        //     } 

        //     try {
        //         // Reload selected profile before returning it to make sure it's in sync with local json
        //         string path = SelectedProfile.Path;

        //         UnloadProfile(SelectedProfile);
        //         Profile profile = await LoadProfilFromFileAsync(path);
        //         SelectProfile(profile.Name);
        //     } catch (ProfileException e) {
        //         if (!regenIfMissing) {
        //             return null;
        //         } else {
        //             logger.Warn(e.Message);
        //         }

        //         await Profile.CreateNewAsync(SelectedProfile.Name, SelectedProfile.MinecraftVersion, SelectedProfile.Loader, SelectedProfile.ServerSide, true);
        //         logger.Info("Selected profile failed to reload.  Presumably this means that filename changed?");
        //     } 
            

        //     return SelectedProfile;
        // }
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
        // public static Profile SelectProfile(string name) {
        //     foreach (KeyValuePair<string, Profile> profile in LoadedProfiles) {                
        //         if (profile.Key.Equals(name)) {
        //             SelectedProfile = profile.Value;

        //             logger.Debug($"Selected profile {name}");
        //             return profile.Value;
        //         }
        //     }
        //     throw new Exception("Profile does not exist... ?");
        // }
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
        
        public static async Task<Profile> LoadProfileAsync(string name) {
            string[] files = Directory.GetFiles(ProfilePath);

            foreach (string file in files) {
                logger.Debug("Attempting to load profile from {0}", file);

                try {
                    string fileContents = await File.ReadAllTextAsync(file, Encoding.ASCII);
                    Profile profile = JsonSerializer.Deserialize<Profile>(fileContents);

                    if (profile.Name.ToLower().Equals(name.ToLower())) {
                        logger.Debug("Loaded profile {0} at {1}", profile.Name, profile.Path);

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

                        LoadedProfiles.Add(profile.Name, profile);
                } catch (Exception e) {
                    logger.Warn(@$"Error occurred loading profile at {path}:");
                    logger.Warn(e.Message);

                    throw new ProfileException("Profile failed to load");
                } 
            return profile;
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
            // if (SelectedProfile is not null && SelectedProfile.Equals(profile)) {
            //     SelectedProfile = null;
            //     logger.Info($"Profile {profile.Name} deselected");
            // }

            if (LoadedProfiles.ContainsKey(profile.Name)) {
                LoadedProfiles.Remove(profile.Name);
                logger.Debug("Unloaded Profile {0} at {1}", profile.Name, profile.Path);
                // LoadAllProfiles();
            } else throw new ProfileException($"Profile {profile.Name} doesn't exist!");
        }
    }
    public enum Repo {
        modrinth,
        curseforge,
        custom
    }
}