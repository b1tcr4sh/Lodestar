using Tmds.DBus;
using System.Runtime.InteropServices;
using Mercurius.Profiles;

namespace Mercurius.DBus {
    [StructLayout(LayoutKind.Sequential)]
    public struct DbusResponse {
        public String Message { get; set; }
        public int Code { get; set; }
        public object Data { get; set; }
        public DataType Type { get; set; }
    }

    public enum DataType {
        None = 0,
        ModDefinition = 1,
        Profile = 1,
        Error = 2
    }

    [DBusInterface("org.mercurius.ProfileMessenger")]
    public interface IProfileMessenger : IDBusObject {
        Task<ObjectPath[]> ListProfilesAsync();
        Task<ObjectPath> CreateProfileAsync(string name, string minecraftVersion, ModLoader loader, bool serverSide);
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
        public ObjectPath ObjectPath { get => _objectPath; }
    }
}