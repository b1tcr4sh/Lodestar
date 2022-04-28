using Mercurius.Configuration;
using System.Threading.Tasks;

namespace Mercurius.Profiles {
    public class Profile {
        // Make a reference to it's respective json file, with methods to update, delete, etc.

        public string Path { get => $"{SettingsManager.Settings.Profile_Directory}/{this.Name}"; private set { Path = value; } }
        public string Name { get; set; }
        public string MinecraftVersion { get; set; }
        public ClientType ClientType { get; set; }
        public bool ContainsUnknownMods = false;
        public Mod[] Mods { get; set; }
        public UnknownMod[] UnknownMods = null;

        public async Task<Profile> UpdateAsync(Profile oldProfile, Profile newProfile) {
            if (oldProfile.Equals(newProfile)) return oldProfile;

            await ProfileManager.OverwriteProfileAsync(newProfile, newProfile.Name);
            return await ProfileManager.LoadProfileAsync(newProfile.Name);
        }
        public bool Delete(string name) {
            return ProfileManager.RemoveProfileFileAsync(name);
        }
    }
    public enum ClientType {
        ClientSide, ServerSide
    }
}