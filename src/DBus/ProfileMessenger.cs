using Tmds.DBus;
using System.Runtime.InteropServices;
using Mercurius.Profiles;
using NLog;

namespace Mercurius.DBus {
    [DBusInterface("org.mercurius.ProfileMessenger")]
    public interface IProfileMessenger : IDBusObject {
        Task<string[]> ListProfilesAsync();
        Task<bool> CreateProfileAsync(string name, string minecraftVersion, ModLoader loader, bool serverSide);
        Task DeleteProfileAsync(string name);
    }


    public class ProfileMessenger : IProfileMessenger {
        private static readonly ObjectPath _objectPath = new ObjectPath("/org/mercurius/ProfileMessenger");
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        public Task<string[]> ListProfilesAsync() {
            List<string> names = new List<string>();

            foreach (Profile profile in ProfileManager.GetLoadedProfiles().Values) {
                names.Add(profile.Name);
            }
            return Task.FromResult<string[]>(names.ToArray<string>());
        }

        public async Task<bool> CreateProfileAsync(string name, string minecraftVersion, ModLoader loader, bool serverSide) {
            Profile profile;
            
            try {
                profile = await Profile.CreateNewAsync(name, minecraftVersion, loader, serverSide);
            } catch (ProfileException e) {
                logger.Warn("Failed to create profile... ?");
                logger.Warn(e.Message);
                logger.Trace(e.StackTrace);
                return false;
            }

            DbusProfile dbusProfile = new DbusProfile(profile);
            await DbusHandler.RegisterProfileAsync(dbusProfile);
            
            return true;
        }
        public async Task DeleteProfileAsync(string name) {
            if (!Profile.Exists(name)) {
                throw new ProfileException($"Profile {name} doesn't exist!");
            }

            Profile profile = await ProfileManager.GetLoadedProfileAsync(name);

            profile.Delete();
        }
        public ObjectPath ObjectPath { get => _objectPath; }
    }
}