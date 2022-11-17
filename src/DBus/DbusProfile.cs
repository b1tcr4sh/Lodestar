using System.Runtime.InteropServices;
using Mercurius;   
using Mercurius.Profiles;
using Mercurius.Configuration;
using Tmds.DBus;
using Mercurius.Modrinth;

namespace Mercurius.DBus {
    public class DbusProfile : IDbusProfile {
        public ObjectPath ObjectPath { get => _objectPath; }
        private ObjectPath _objectPath;
        private Profile modelProfile;

        internal DbusProfile(Profile profile) {
            _objectPath = new ObjectPath(String.Format($"/org/mercurius/profile/{profile.Name}"));
            modelProfile = profile;
        }

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
        public async Task<bool> RemoveModAsync(string id, bool force) {
            IEnumerable<Mod> mods = modelProfile.Mods.Where<Mod>(mod => mod.VersionId == id);
        
            if (mods.Count() < 1) return false;

            if (mods.Count() > 1) {
                bool success = true;

                foreach (Mod mod in mods) {
                    if (!await modelProfile.RemoveModFromListAsync(mod, force))
                        success = false;
                }

                return success;
            }

            return await modelProfile.RemoveModFromListAsync(mods.ElementAt(0), force);
        }
        public async Task<DbusResponse> SyncAsync() {
            APIClient client = new APIClient();
            try {
                await ProfileManager.SyncProfileAsync(modelProfile, client);
            } catch (ProfileException e) {
                return new DbusResponse {
                    Message = e.Message,
                    Code = -1,
                    Data = "",
                    Type = DataType.Error
                };
            }

            return new DbusResponse {
                Code = 0,
                Message = "Success",
                Data = ObjectPath,
                Type = DataType.Profile
            };
        }
        public Task<Mod[]> ListModsAsync() {
            foreach (Mod mod in modelProfile.Mods) {
                mod.CheckFileExists();
            }

            return Task.FromResult<Mod[]>(modelProfile.Mods.ToArray<Mod>());
        }
    }

    [DBusInterface("org.mercurius.profile")]
    public interface IDbusProfile : IDBusObject {
        public Task<ProfileInfo> GetProfileInfoAsync();
        public Task<Mod> AddModAsync(string id, Repo service, bool ignoreDependencies);
        public Task<bool> RemoveModAsync(string id, bool force);
        public Task<DbusResponse> SyncAsync();
        public Task<Mod[]> ListModsAsync();
        public Task<bool> VerifyAsync(); // Should check to make sure all dependencies are met and everything is compatible; auto fix incompatibilities or return false if can't
        public Task CheckForUpdatesAsync(); // Should return struct describing mods and if they're outdated
        public Task UpdateModAsync(string id); // Should fetch newest compatible version of mod
        public Task GenerateAsync(bool startFromCleanSlate); // Should generate mod metadata from mod files (properly this time)
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