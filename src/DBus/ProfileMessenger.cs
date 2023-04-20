using Tmds.DBus;
using NLog;

using Mercurius.Profiles;

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
        
        private ProfileManager _manager;

        public ProfileMessenger(ProfileManager manager) {
            _manager = manager;
        }

        public Task<string[]> ListProfilesAsync() {
            List<string> names = new List<string>();

            foreach (Profile profile in _manager.GetLoadedProfiles().Values) {
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
            if (!_manager.ProfileExists(name)) {
                throw new ProfileException($"Profile {name} doesn't exist!");
            }

            Profile profile = await _manager.GetLoadedProfileAsync(name);

            profile.Delete();
        }
        public ObjectPath ObjectPath { get => _objectPath; }
    }
}