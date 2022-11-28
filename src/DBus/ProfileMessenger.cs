using Tmds.DBus;
using System.Runtime.InteropServices;
using Mercurius.Profiles;

namespace Mercurius.DBus {
    [DBusInterface("org.mercurius.ProfileMessenger")]
    public interface IProfileMessenger : IDBusObject {
        Task<ObjectPath[]> ListProfilesAsync();
        Task<ObjectPath> CreateProfileAsync(string name, string minecraftVersion, ModLoader loader, bool serverSide);
        Task DeleteProfileAsync(string name);
    }


    public class ProfileMessenger : IProfileMessenger {
        private static readonly ObjectPath _objectPath = new ObjectPath("/org/mercurius/ProfileMessenger");

        public Task<ObjectPath[]> ListProfilesAsync() {
            List<ObjectPath> paths = new List<ObjectPath>();

            foreach (Profile profile in ProfileManager.GetLoadedProfiles().Values) {
                paths.Add(new ObjectPath($"/org/mercurius/profile/{profile.Name}"));
            }
            return Task.FromResult<ObjectPath[]>(paths.ToArray<ObjectPath>());
        }

        public async Task<ObjectPath> CreateProfileAsync(string name, string minecraftVersion, ModLoader loader, bool serverSide) {
            Profile profile;
            
            try {
                profile = await Profile.CreateNewAsync(name, minecraftVersion, loader, serverSide);
            } catch (Exception e) {
                throw new DBusException("ProfileCreationFailure", e.Message);
            }

            DbusProfile dbusProfile = new DbusProfile(profile);
            await DbusHandler.RegisterProfileAsync(dbusProfile);
            
            return dbusProfile.ObjectPath;
        }
        public Task DeleteProfileAsync(string name) {
            if (!Profile.Exists(name)) {
                throw new ProfileException($"Profile {name} doesn't exist!");
            }

            Profile profile = ProfileManager.GetLoadedProfile(name);

            profile.Delete();

            return Task.CompletedTask;
        }
        public ObjectPath ObjectPath { get => _objectPath; }
    }
}