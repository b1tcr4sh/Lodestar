using Mercurius;   
using Mercurius.Profiles;
using Mercurius.Configuration;
using Tmds.DBus;

namespace Mercurius.DBus {
    public class DbusProfile : IProfile {
        public Task<string> GetNameAsync { get => Task.FromResult<string>(modelProfile.Name); }
        public Task<string> GetMinecraftVersionAsync { get => Task.FromResult<string>(modelProfile.MinecraftVersion); }
        public Task<bool> IsServerSideAsync { get => Task.FromResult<bool>(modelProfile.ServerSide); }
        public Task<string> GetLoaderAsync { get => Task.FromResult<string>(modelProfile.Loader); }
        public Task<Mod[]> GetModListAsync { get => Task.FromResult<Mod[]>(modelProfile.Mods.ToArray<Mod>()); }
        public Task<string> GetPathAsync { get => Task.FromResult<string>(modelProfile.Path); }
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
        public Task<string> GetNameAsync { get; }
        public Task<string> GetMinecraftVersionAsync { get; }
        public Task<bool> IsServerSideAsync { get; }
        public Task<string> GetLoaderAsync { get; }
        public Task<Mod[]> GetModListAsync { get; }
        public Task<string> GetPathAsync { get; }
    }
}