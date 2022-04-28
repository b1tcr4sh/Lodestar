using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mercurius.Profiles {
    public static class ProfileManager {
        public static Dictionary<string, Profile> LoadedProfiles { get; private set;}
        public static Profile SelectedProfile { get; private set; }
        private static string ProfilePath;
        public static void InitializeDirectory(string profileDirectoryPath) {
            LoadedProfiles = new Dictionary<string, Profile>();

            ProfilePath = profileDirectoryPath;
            if (!Directory.Exists(ProfilePath)) Directory.CreateDirectory(ProfilePath); 

            LoadAllProfiles();
        }

        public static Profile SelectProfile(string name) {
            foreach (KeyValuePair<string, Profile> profile in LoadedProfiles) {
                if (profile.Key.Equals(name)) {
                    SelectedProfile = profile.Value;
                    return profile.Value;
                }
            }
            throw new ProfileException($"Profile {name} Not Found.");
        }
        private static void LoadAllProfiles() {
            string[] files = Directory.GetFiles(ProfilePath);

            foreach (string file in files) {
                string contents = File.ReadAllText(file, Encoding.ASCII);
                Profile profile = JsonSerializer.Deserialize<Profile>(contents);

                if (!LoadedProfiles.ContainsKey(profile.Name.ToLower()))
                    LoadedProfiles.Add(profile.Name.ToLower(), profile); 
            }
        }
        public static async Task<Profile> LoadProfileAsync(string name) {
            string[] files = Directory.GetFiles(ProfilePath);

            foreach (string file in files) {
                try {
                    string fileContents = await File.ReadAllTextAsync(file, Encoding.ASCII);
                    Profile profile = JsonSerializer.Deserialize<Profile>(fileContents);

                    if (profile.Name.Replace(" ", "_").Equals(name)) return profile;
                } catch (Exception e) {
                    Console.WriteLine(@$"Error occurred loading profile at {file}: {e.Message}");
                } 
            }
            throw new ProfileException($"Profile {name} not found!");
        }
        public static async Task CreateDefaultProfileAsync(string name, string minecraftVersion) {
            Profile profile = new Profile {
                Name = name,
                MinecraftVersion = minecraftVersion,
                ClientType = ClientType.ClientSide,
                Mods = new Mod[0],
                UnknownMods = null
            };
            await WriteProfileAsync(profile);
            SelectedProfile = profile;
        }
        public static Profile CreateProfile(string name, string minecraftVersion, ClientType clientType, bool select = false) {
            Profile profile = new Profile {
                Name = name,
                MinecraftVersion = minecraftVersion,
                ClientType = clientType
            };
            if (select) SelectedProfile = profile;

            return profile;
        }
        internal static async Task WriteProfileAsync(Profile profile) {
            using FileStream stream = new FileStream($@"./Profiles/{profile.Name.Replace(" ", "_")}.profile.json", FileMode.CreateNew, FileAccess.Write);

            await JsonSerializer.SerializeAsync<Profile>(stream, profile, new JsonSerializerOptions { IncludeFields = true, WriteIndented = true });
            stream.Close();
        }
        internal static async Task OverwriteProfileAsync(Profile profile, string existingProfileName) {
            using FileStream stream = new FileStream($@"./Profiles/{existingProfileName.Replace(" ", "_")}.profile.json", FileMode.Create, FileAccess.Write);

            await JsonSerializer.SerializeAsync<Profile>(stream, profile, new JsonSerializerOptions { IncludeFields = true, WriteIndented = true });
            stream.Close();
        }
        internal static bool RemoveProfileFileAsync(string profileName) {
            if (File.Exists($"{ProfilePath}/{profileName.Replace(" ", "_")}.json")) {
                return false;
            }

            File.Delete($"{ProfilePath}/{profileName.Replace(" ", "_")}.json");
            return true;
        }
    }
}