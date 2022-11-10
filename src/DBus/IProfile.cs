using System.Runtime.InteropServices;
using Mercurius;   
using Mercurius.Profiles;
using Mercurius.Configuration;
using Tmds.DBus;

namespace Mercurius.DBus {
    public class DbusProfile : IProfile {
        public Task<ProfileInfo> GetProfileInfoAsync() {
            return Task.FromResult<ProfileInfo>(new ProfileInfo {
                Name = modelProfile.Name,
                MinecraftVersion = modelProfile.MinecraftVersion,
                FilePath = modelProfile.Path,
                IsServerSide = modelProfile.ServerSide,
                Loader = modelProfile.Loader
            });
        }
        public ObjectPath ObjectPath { get => _objectPath; }
        private ObjectPath _objectPath;
        private Profile modelProfile;

        internal DbusProfile(Profile profile) {
            _objectPath = new ObjectPath(String.Format($"/org/mercurius/profile/{profile.Name}"));
            modelProfile = profile;
        }

    }

    [DBusInterface("org.mercurius.profile")]
    public interface IProfile : IDBusObject {
        public Task<ProfileInfo> GetProfileInfoAsync();
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProfileInfo {
        public string Name;
        public string MinecraftVersion;
        public bool IsServerSide;
        public ModLoader Loader;
        public string FilePath;
    }
}