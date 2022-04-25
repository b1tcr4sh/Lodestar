using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mercurius.Profiles {
    public static class ProfileManager {
        public static Dictionary<string, Profile> LoadedProfiles { get; private set;}
        private static string ProfilePath;
        public static void InitializeDirectory(string profileDirectoryPath) {
            ProfilePath = profileDirectoryPath;
            if (!Directory.Exists(ProfilePath)) Directory.CreateDirectory(ProfilePath); 
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
        public static async Task WriteProfile(Profile profile) {
            using FileStream stream = new FileStream($@"./Profiles/{profile.Name.Replace(" ", "_")}.profile.json", FileMode.CreateNew, FileAccess.Write);

            await JsonSerializer.SerializeAsync<Profile>(stream, profile, new JsonSerializerOptions { IncludeFields = true, WriteIndented = true });
            stream.Close();
        }
    }
}