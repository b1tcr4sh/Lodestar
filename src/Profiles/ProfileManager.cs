using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog;
using Microsoft.Extensions.Hosting;

using Mercurius.API;
using Mercurius.Configuration;
using Mercurius.DBus;

namespace Mercurius.Profiles {
    public class ProfileManager {
        private Dictionary<string, Profile> LoadedProfiles;
        private string ProfilePath;
        private ILogger _logger;
        internal IDbusHandler dbusHandler;
        [JsonIgnore]
        public APIs Apis;
        public ProfileManager(APIs apis, ILogger logger, IDbusHandler handler) {
            Apis = apis;
            dbusHandler = handler;
            _logger = logger;

            InitializeDirectory();
            LoadAllProfiles();
        }

        private void InitializeDirectory() {
            LoadedProfiles = new Dictionary<string, Profile>();
            if (SettingsManager.Settings is null) {
                _logger.Fatal("Couldn't find configuration file!");
                System.Environment.Exit(1);
            } 
            ProfilePath = SettingsManager.Settings.Profile_Directory;
            if (!Directory.Exists(ProfilePath)) {
                Directory.CreateDirectory(ProfilePath);
                _logger.Debug("Created Profiles Directory at {0}", ProfilePath);
            } 
        }
        private void InitializeDirectory(string path) {
            LoadedProfiles = new Dictionary<string, Profile>();
            ProfilePath = @path;
            if (!Directory.Exists(ProfilePath)) Directory.CreateDirectory(ProfilePath); 
        }
        
        public async Task<Profile> GetLoadedProfileAsync(string name) {
            if (!LoadedProfiles.Keys.Contains(name)) throw new ProfileException($"Profile {name} doesn't exist!");

            Profile foundProfile = LoadedProfiles[name];

            if (await foundProfile.VerifyLocalFileAsync()) {
                return foundProfile;
            } else {
                return LoadedProfiles[name];
            }
        }
        public IReadOnlyDictionary<string, Profile> GetLoadedProfiles() {
            if (LoadedProfiles is null || LoadedProfiles.Count <= 0) return new Dictionary<string, Profile>() as IReadOnlyDictionary<string, Profile>;

            return LoadedProfiles as IReadOnlyDictionary<string, Profile>;
        }
        public void LoadAllProfiles() {
            LoadedProfiles = new Dictionary<string, Profile>();
            string[] files = Directory.GetFiles(ProfilePath);
            
            foreach (string file in files) {
                try {
                    string contents = File.ReadAllText(file, Encoding.ASCII);
                    Profile profile = JsonSerializer.Deserialize<Profile>(contents);

                    if (profile.Name.Contains(" "))
                        _logger.Warning($"Profile at {profile.Path} was unable to be loaded (Name contained spaces)");
                    else if (!LoadedProfiles.ContainsKey(profile.Name)) {
                        profile.GenerateChecksum();
                        profile.Manager = this;
                        profile.Apis = Apis;
                        LoadedProfiles.Add(profile.Name, profile); 
                    }
                } catch (Exception e) {
                    _logger.Warning(@$"Error occurred loading profile at {file}");
                    _logger.Warning(e.Message);
                }
            }

            foreach (KeyValuePair<string, Profile> profile in LoadedProfiles) {
                if (!File.Exists($@"{ProfilePath}/{profile.Key.ToLower()}.profile.json"))
                    LoadedProfiles.Remove(profile.Key);
            }

            _logger.Information($"Loaded {LoadedProfiles.Count} profiles");
        }
        
        public async Task<Profile> LoadProfileAsync(string name) {
            string[] files = Directory.GetFiles(ProfilePath);

            foreach (string file in files) {
                _logger.Debug("Attempting to load profile from {0}", file);

                try {
                    string fileContents = await File.ReadAllTextAsync(file, Encoding.ASCII);
                    Profile profile = JsonSerializer.Deserialize<Profile>(fileContents);

                    if (profile.Name.ToLower().Equals(name.ToLower())) {
                        _logger.Debug("Loaded profile {0} at {1}", profile.Name, profile.Path);

                        profile.GenerateChecksum();
                        profile.Manager = this;
                        profile.Apis = Apis;

                        LoadedProfiles.Add(profile.Name, profile);
                        return profile;
                    }
                } catch (Exception e) {
                    _logger.Warning(@$"Error occurred loading profile at {file}:");
                    _logger.Warning(e.Message);
                } 
            }
            throw new ProfileException($"Profile {name} wasn't found!");
        }
        public async Task<Profile> LoadProfilFromFileAsync(string path) {
            _logger.Debug("Attempting to load profile from {0}", path);

            Profile profile;

                try {
                    string fileContents = await File.ReadAllTextAsync(path, Encoding.ASCII);
                    profile = JsonSerializer.Deserialize<Profile>(fileContents);
                    _logger.Debug("Loaded profile {0} at {1}", profile.Name, profile.Path);

                    profile.GenerateChecksum();
                    profile.Manager = this;
                    profile.Apis = Apis;
                    LoadedProfiles.Add(profile.Name, profile);
                } catch (Exception e) {
                    _logger.Warning(@$"Error occurred loading profile at {path}:");
                    _logger.Warning(e.Message);

                    throw new ProfileException("Profile failed to load");
                } 
            return profile;
        }
        public bool ProfileExists(string name) {
            return GetLoadedProfiles().Keys.Contains(name);
        }

        internal async Task WriteProfileAsync(Profile profile) {
            if (File.Exists($@"{ProfilePath}/{profile.Name}.profile.json")) throw new ProfileException($"File {profile.Name} already has exisint file!");

            _logger.Debug("Writing New Profile {0} to {1}", profile, ProfilePath);
            using FileStream stream = new FileStream($@"{ProfilePath}/{profile.Name}.profile.json", FileMode.CreateNew, FileAccess.Write);

            await JsonSerializer.SerializeAsync<Profile>(stream, profile, new JsonSerializerOptions { IncludeFields = true, WriteIndented = true });
            stream.Close();

            profile.GenerateChecksum();
        }
        internal async Task OverwriteProfileAsync(Profile profile, string existingProfileName) {
            if (!File.Exists($@"{ProfilePath}/{existingProfileName.ToLower()}.profile.json")) throw new ProfileException($"Profile Expected at {ProfilePath}/{existingProfileName.ToLower()}.profile.json Doesnt' Exist!");
            
            _logger.Debug("Overwriting Profile {0} at {1}", profile, ProfilePath);
            using FileStream stream = new FileStream(profile.Path, FileMode.Create, FileAccess.Write);

            await JsonSerializer.SerializeAsync<Profile>(stream, profile, new JsonSerializerOptions { IncludeFields = true, WriteIndented = true });
            stream.Close();

            profile.GenerateChecksum();
        }
        internal bool DeleteProfileFile(string profileName) {
            if (!File.Exists($"{ProfilePath}/{profileName.ToLower()}.profile.json")) {
                _logger.Debug("Attempted to Delete Profile {0}, but File Didn't Exist... ?", profileName);
                return false;
            }

            File.Delete($"{ProfilePath}/{profileName.ToLower()}.profile.json");
            _logger.Debug("Deleted Profile at {0}", $"{ProfilePath}/{profileName.ToLower()}.profile.json");
            return true;
        }
        internal void UnloadProfile(Profile profile) {
            if (LoadedProfiles.ContainsKey(profile.Name)) {
                LoadedProfiles.Remove(profile.Name);
                _logger.Debug("Unloaded Profile {0} at {1}", profile.Name, profile.Path);
                // LoadAllProfiles();
            } else throw new ProfileException($"Profile {profile.Name} doesn't exist!");
        }
        internal async Task SyncProfileAsync(Profile profile) {
            _logger.Information("Syncing {0}", profile.Name);


            List<string> existingFiles = Directory.GetFiles($"{SettingsManager.Settings.Minecraft_Directory}/mods/").Where(file => file.Substring(file.Length - 4).Equals(".jar")).ToList<string>();
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
                _logger.Information("There are no Residiual Mod jars to Remove");
            } else {
                _logger.Information("Removing Residual Mod jars...");
                foreach (string filePath in existingFiles) {
                    File.Delete(filePath);
                    _logger.Debug("Deleted mod jar at {0}", filePath);
                }
                    
            }

            if (profile.Mods.Count <= 0) {
                _logger.Information("Profile has no mods to sync!");
                throw new ProfileException("Profile has no mods to sync!");
            }

            _logger.Debug("Queuing {0} mods for install", profile.Mods.Count);
            List<Mod> preQueue = new List<Mod>();
            preQueue.AddRange(profile.Mods);
            // foreach (Mod mod in selectedProfile.Mods) { // Dependencies are already root level now, so no need to collect them before install
            //     preQueue.AddRange(mod.DependencyVersions);                
            // }
            

            // Queue mods for install
            List<Mod> installQueue = new List<Mod>();

            foreach (Mod mod in preQueue) {
                if (File.Exists($"{SettingsManager.Settings.Minecraft_Directory}/mods/{mod.FileName}")) {
                    _logger.Debug("{0}: {1} is already installed, skipping...", mod.Title, mod.ModVersion);
                        
                } else
                    installQueue.Add(mod);
            }
            _logger.Debug("Attempting to install mods...");
            foreach (Mod mod in installQueue) {
                Repository client = Apis.Get(mod.Repo);

                await client.DownlodModAsync(mod);
            }
        }
    }
}