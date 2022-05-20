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
        public static IReadOnlyDictionary<string, Profile> GetLoadedProfiles() {
            if (LoadedProfiles is null || LoadedProfiles.Count <= 0) return new Dictionary<string, Profile>() as IReadOnlyDictionary<string, Profile>;

            return LoadedProfiles as IReadOnlyDictionary<string, Profile>;
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
            LoadedProfiles = new Dictionary<string, Profile>();
            string[] files = Directory.GetFiles(ProfilePath);

            foreach (string file in files) {
                string contents = File.ReadAllText(file, Encoding.ASCII);
                Profile profile = JsonSerializer.Deserialize<Profile>(contents);

                if (profile.Name.Contains(" "))
                    Console.WriteLine($"Profile at {profile.Path} was unable to be loaded (Name contained spaces)");
                else if (!LoadedProfiles.ContainsKey(profile.Name))
                    LoadedProfiles.Add(profile.Name, profile); 
            }

            foreach (KeyValuePair<string, Profile> profile in LoadedProfiles) {
                if (!File.Exists($@"{ProfilePath}/{profile.Key.ToLower()}.profile.json"))
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

                    if (profile.Name.ToLower().Equals(name.ToLower())) {
                        Console.WriteLine($"Loaded profile {profile.Name}");
                        LoadedProfiles.Add(profile.Name, profile);
                        return profile;
                    }
                } catch (Exception e) {
                    Console.WriteLine(@$"Error occurred loading profile at {file}: {e.Message}");
                } 
            }
            throw new ProfileException($"Profile {name} not found!");
        }
        
        internal static async Task WriteProfileAsync(Profile profile) {
            if (File.Exists($@"{ProfilePath}/{profile.Name}.profile.json")) return;
            using FileStream stream = new FileStream($@"{ProfilePath}/{profile.Name}.profile.json", FileMode.CreateNew, FileAccess.Write);

            await JsonSerializer.SerializeAsync<Profile>(stream, profile, new JsonSerializerOptions { IncludeFields = true, WriteIndented = true });
            stream.Close();
        }
        internal static async Task OverwriteProfileAsync(Profile profile, string existingProfileName) {
            if (!File.Exists($@"./Profiles/{existingProfileName.ToLower()}.profile.json")) throw new ProfileException($"Profile {existingProfileName} doesn't exist!");
            using FileStream stream = new FileStream($@"./Profiles/{existingProfileName.ToLower()}.profile.json", FileMode.Create, FileAccess.Write);

            await JsonSerializer.SerializeAsync<Profile>(stream, profile, new JsonSerializerOptions { IncludeFields = true, WriteIndented = true });
            stream.Close();
        }
        internal static bool DeleteProfileFile(string profileName) {
            if (!File.Exists($"{ProfilePath}/{profileName.ToLower()}.profile.json")) {
                return false;
            }

            File.Delete($"{ProfilePath}/{profileName.ToLower()}.profile.json");
            return true;
        }
        internal static void UnloadProfile(Profile profile) {
            // if (SelectedProfile is null) {
            //     Console.WriteLine("No profile selected... ?");
            //     return;
            // }
            // else 
            if (SelectedProfile.Equals(profile)) {
                SelectedProfile = null;
                Console.WriteLine($"Profile {profile.Name} deselected");
            }

            if (LoadedProfiles.ContainsKey(profile.Name)) {
                LoadedProfiles.Remove(profile.Name);
                LoadAllProfiles();
            } else throw new ProfileException($"Profile {profile.Name} doesn't exist!");
        }
    }
}