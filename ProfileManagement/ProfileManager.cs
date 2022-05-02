using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mercurius.Configuration;

namespace Mercurius.Profiles {
    public static class ProfileManager {
        private static Dictionary<string, Profile> LoadedProfiles;
        public static Profile SelectedProfile { get; private set; }
        private static string ProfilePath;
        public static void InitializeDirectory() {
            LoadedProfiles = new Dictionary<string, Profile>();
            ProfilePath = SettingsManager.Settings.Profile_Directory;
            if (!Directory.Exists(ProfilePath)) Directory.CreateDirectory(ProfilePath); 
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
        public static Profile SelectProfile(string name) {
            foreach (KeyValuePair<string, Profile> profile in LoadedProfiles) {
                if (profile.Key.Equals(name)) {
                    SelectedProfile = profile.Value;

                    Console.WriteLine($"Selected profile {name}");
                    return profile.Value;
                }
            }
            throw new ProfileException($"Profile {name} Not Found.");
        }
        public static void LoadAllProfiles() {
            string[] files = Directory.GetFiles(ProfilePath);

            foreach (string file in files) {
                string contents = File.ReadAllText(file, Encoding.ASCII);
                Profile profile = JsonSerializer.Deserialize<Profile>(contents);

                if (!LoadedProfiles.ContainsKey(profile.Name))
                    LoadedProfiles.Add(profile.Name, profile); 
            }

            foreach (KeyValuePair<string, Profile> profile in LoadedProfiles) {
                if (!File.Exists($@"{ProfilePath}/{profile.Key.ToLower().Replace(" ", "_")}.profile.json"))
                    LoadedProfiles.Remove(profile.Key);
            }

            Console.WriteLine($"Loaded {LoadedProfiles.Count} profiles");
        }
        public static async Task<Profile> LoadProfileAsync(string name) {
            string[] files = Directory.GetFiles(ProfilePath);

            foreach (string file in files) {
                try {
                    string fileContents = await File.ReadAllTextAsync(file, Encoding.ASCII);
                    Profile profile = JsonSerializer.Deserialize<Profile>(fileContents);

                    if (profile.Name.ToLower().Equals(name.ToLower())) return profile;
                } catch (Exception e) {
                    Console.WriteLine(@$"Error occurred loading profile at {file}: {e.Message}");
                } 
            }
            throw new ProfileException($"Profile {name} not found!");
        }
        
        internal static async Task WriteProfileAsync(Profile profile) {
            if (File.Exists($@"{ProfilePath}/{profile.Name.ToLower().Replace(" ", "_")}.profile.json")) return;
            using FileStream stream = new FileStream($@"{ProfilePath}/{profile.Name.ToLower().Replace(" ", "_")}.profile.json", FileMode.CreateNew, FileAccess.Write);

            await JsonSerializer.SerializeAsync<Profile>(stream, profile, new JsonSerializerOptions { IncludeFields = true, WriteIndented = true });
            stream.Close();
        }
        internal static async Task OverwriteProfileAsync(Profile profile, string existingProfileName) {
            if (!File.Exists($@"./Profiles/{existingProfileName.ToLower().Replace(" ", "_")}.profile.json")) throw new ProfileException($"Profile {existingProfileName} doesn't exist!");
            using FileStream stream = new FileStream($@"./Profiles/{existingProfileName.ToLower().Replace(" ", "_")}.profile.json", FileMode.Create, FileAccess.Write);

            await JsonSerializer.SerializeAsync<Profile>(stream, profile, new JsonSerializerOptions { IncludeFields = true, WriteIndented = true });
            stream.Close();
        }
        internal static bool DeleteProfileFile(string profileName) {
            if (!File.Exists($"{ProfilePath}/{profileName.ToLower().Replace(" ", "_")}.profile.json")) {
                return false;
            }

            File.Delete($"{ProfilePath}/{profileName.ToLower().Replace(" ", "_")}.profile.json");
            return true;
        }
        internal static void UnloadProfile(Profile profile) {
            if (SelectedProfile.Equals(profile)) {
                SelectedProfile = null;
            }

            if (LoadedProfiles.ContainsKey(profile.Name)) {
                LoadedProfiles.Remove(profile.Name);
            }
        }
    }
}