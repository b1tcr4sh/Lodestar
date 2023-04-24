using Tmds.DBus;
using Serilog;

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
        private static ILogger _logger = Log.Logger;
        private DbusHandler dbusHandler;
        
        private ProfileManager _manager;

        public ProfileMessenger(ProfileManager manager, DbusHandler handler) {
            _manager = manager;
            dbusHandler = handler;
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
                profile = Profile.CreateNew(name, minecraftVersion, loader, serverSide, _manager);
            } catch (ProfileException e) {
                _logger.Warning("Failed to create profile... ?");
                _logger.Warning(e.Message);
                _logger.Warning(e.StackTrace);
                return false;
            }

            DbusProfile dbusProfile = new DbusProfile(profile);
            await dbusHandler.RegisterProfileAsync(dbusProfile);
            
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