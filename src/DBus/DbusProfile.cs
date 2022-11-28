using System.Runtime.InteropServices;
using Mercurius;   
using Mercurius.Profiles;
using Mercurius.Configuration;
using Tmds.DBus;
using Mercurius.Modrinth;
using NLog;

namespace Mercurius.DBus {
    public class DbusProfile : IDbusProfile {
        public ObjectPath ObjectPath { get => _objectPath; }
        private ObjectPath _objectPath;
        private Profile modelProfile;
        private ILogger logger;

        internal DbusProfile(Profile profile) {
            _objectPath = new ObjectPath(String.Format($"/org/mercurius/profile/{profile.Name}"));
            modelProfile = profile;
            logger = NLog.LogManager.GetCurrentClassLogger();
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
            } catch (HttpRequestException e) {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound) {
                    throw new Exception("Invalid mod id");
                } else {
                    throw new Exception($"failed to connect: {e.StatusCode}");
                }
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
        public async Task<bool> SyncAsync() {
            APIClient client = new APIClient();
            try {
                await ProfileManager.SyncProfileAsync(modelProfile, client);
            } catch (ProfileException) {
                return false;
            }
            return true;
        }
        public Task<Mod[]> ListModsAsync() {
            foreach (Mod mod in modelProfile.Mods) {
                mod.CheckFileExists();
            }

            return Task.FromResult<Mod[]>(modelProfile.Mods.ToArray<Mod>());
        }
        public async Task<ValidityReport> VerifyAsync() {
            APIClient client = new APIClient();
            List<Mod> toRemove = new List<Mod>();
            List<Mod> toAdd = new List<Mod>();


            IEnumerable<Mod> incompatible = modelProfile.Mods.Where<Mod>(mod => !mod.MinecraftVersion.Equals(modelProfile.MinecraftVersion));
        
            if (incompatible.Count() > 0) {
                logger.Debug("Found {0} incompatible mods", incompatible.Count());
                foreach (Mod mod in incompatible) {
                    toRemove.Add(mod);

                    await modelProfile.AddModAsync(client, mod.ProjectId, Repo.modrinth, false);
                }
            }

            string[] installedDeps = await modelProfile.ResolveDependenciesAsync();


            foreach (Mod mod in modelProfile.Mods) {
                IEnumerable<Mod> matchingIds = modelProfile.Mods.Where<Mod>(checking => mod.VersionId.Equals(checking.VersionId));

                if (matchingIds.Count() > 1) {
                    logger.Debug("Found {0} duplicates of {1}", matchingIds.Count(), mod.VersionId);
                    foreach(Mod duplicate in matchingIds.Skip(1)) {
                        toRemove.Add(duplicate);
                    }
                }
            }

            foreach (Mod removeable in toRemove) {
                await modelProfile.RemoveModFromListAsync(removeable, true);
            }

            return new ValidityReport {
                incompatible = incompatible.ToArray<Mod>(),
                missingDependencies = installedDeps,
                synced = modelProfile.isSynced()
            };
        }
        public Task CheckForUpdatesAsync() {
            throw new NotImplementedException();
        }
        public Task UpdateModAsync(string id) {
            throw new NotImplementedException();
        }
        public Task GenerateAsync(bool startFromCleanSlate) {
            throw new NotImplementedException();
        }
    }

    [DBusInterface("org.mercurius.profile")]
    public interface IDbusProfile : IDBusObject {
        public Task<ProfileInfo> GetProfileInfoAsync();
        public Task<Mod> AddModAsync(string id, Repo service, bool ignoreDependencies);
        public Task<bool> RemoveModAsync(string id, bool force);
        public Task<bool> SyncAsync();
        public Task<Mod[]> ListModsAsync();
        public Task<ValidityReport> VerifyAsync(); // Should check to make sure all dependencies are met and everything is compatible; auto fix incompatibilities or return false if can't
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

    [StructLayout(LayoutKind.Sequential)]
    public struct ValidityReport {
        public Mod[] incompatible;
        public string[] missingDependencies;
        public bool synced;
    }
}