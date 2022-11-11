using System.Runtime.InteropServices;
using Mercurius;   
using Mercurius.Profiles;
using Mercurius.Configuration;
using Tmds.DBus;
using Mercurius.Modrinth;

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
        public async Task<Mod> AddModAsync(string id, Repo service, bool ignoreDependencies) {
            APIClient client = new APIClient();
            try {
                return await modelProfile.AddModAsync(client, id, service, ignoreDependencies);
            } catch (ProfileException e) {
                throw new Exception(e.Message); // Not handled temporarily
            }
        }
        // public Task<string> SyncAsync() {
            
        // }
        public Task<Mod[]> GetModListAsync() {
            return Task.FromResult<Mod[]>(modelProfile.Mods.ToArray<Mod>());
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
        public Task<Mod> AddModAsync(string id, Repo service, bool ignoreDependencies);
        // public Task<string> SyncAsync();
        public Task<Mod[]> GetModListAsync();
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