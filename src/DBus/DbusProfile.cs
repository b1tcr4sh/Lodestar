using System.Runtime.InteropServices;
using Mercurius;   
using Mercurius.Profiles;
using Mercurius.Configuration;
using Tmds.DBus;
using Mercurius.API;
using NLog;

namespace Mercurius.DBus {
    public class DbusProfile : IDbusProfile {
        public ObjectPath ObjectPath { get => _objectPath; }
        private ObjectPath _objectPath;
        private Profile modelProfile; 
        private ILogger logger;
        private ModrinthAPI _modrinth;
        private CurseforgeAPI _curseforge;

        private async Task<Profile> GetModelProfileAsync() => await ProfileManager.GetLoadedProfileAsync(modelProfile.Name);

        internal DbusProfile(Profile profile, ModrinthAPI modrinth, CurseforgeAPI curseforge) {
            _objectPath = new ObjectPath(String.Format($"/org/mercurius/profile/{profile.Name}"));
            modelProfile = profile;
            logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public async Task<ProfileInfo> GetProfileInfoAsync() {
            Profile profile = await GetModelProfileAsync();

            return new ProfileInfo {
                Name = profile.Name,
                MinecraftVersion = profile.MinecraftVersion,
                FilePath = profile.Path,
                IsServerSide = profile.ServerSide,
                Loader = profile.Loader
            };
        }
        public async Task<Mod[]> AddModAsync(string id, Repo service, bool ignoreDependencies) {
            Repository repo;
            switch (service) {
                case Repo.modrinth:
                    repo = _modrinth;
                    break;
                case Repo.curseforge:
                    repo = _curseforge;
                    break;
            }

            Profile profile = await GetModelProfileAsync();

            
            return (await profile.AddModAsync(repo, id, service, ignoreDependencies, false)).ToArray<Mod>();
        }
        public async Task<bool> RemoveModAsync(string id, bool force) {
            Profile profile = await GetModelProfileAsync();
            IEnumerable<Mod> mods = profile.Mods.Where<Mod>(mod => mod.VersionId == id);
        
            if (mods.Count() < 1) return false;

            if (mods.Count() > 1) {
                bool success = true;

                foreach (Mod mod in mods) {
                    if (!await profile.RemoveModFromListAsync(mod, force))
                        success = false;
                }

                return success;
            }

            return await profile.RemoveModFromListAsync(mods.ElementAt(0), force);
        }
        public async Task<bool> SyncAsync() {
            ModrinthAPI client = APIManager.Modrinth;
            Profile profile = await GetModelProfileAsync();

            try {
                await ProfileManager.SyncProfileAsync(profile, client);
            } catch (ProfileException) {
                return false;
            }
            return true;
        }
        public async Task<Mod[]> ListModsAsync() { // Throws error?
            Profile profile = await GetModelProfileAsync();

            return profile.Mods.ToArray<Mod>();
        }
        public async Task<ValidityReport> VerifyAsync() {
            Profile profile = await GetModelProfileAsync();
            ModrinthAPI client = APIManager.Modrinth;
            List<Mod> toRemove = new List<Mod>();
            List<string> toAdd = new List<string>();
            bool repaired = true;

            logger.Debug("Verifying profile {0} upon request", profile.Name);
            // Check for incompatible mods
            IEnumerable<Mod> incompatible = profile.Mods.Where<Mod>(mod => !mod.MinecraftVersion.Equals(profile.MinecraftVersion) || !mod.Loaders.Contains(profile.Loader));
            logger.Debug("Found {0} incompatible mods", incompatible.Count());

            if (incompatible.Count() > 0) {
                foreach (Mod mod in incompatible) {
                    toRemove.Add(mod);

                    try {
                        toAdd.Add(mod.ProjectId);
                    } catch (Exception e) {
                        logger.Warn(e);
                        logger.Trace(e.StackTrace);
                        repaired = false;
                    }
                }
            }

            await profile.RemoveModsFromListAsync(toRemove, true);

            await profile.AddModsAsync(client, toAdd.ToArray<string>(), Repo.modrinth, false);


            // Check for duplicates 
            logger.Info("Pruning duplicates...");
            toRemove = new List<Mod>();
            foreach (Mod mod in profile.Mods) {
                IEnumerable<Mod> matchingIds = profile.Mods.Where<Mod>(checking => mod.VersionId.Equals(checking.VersionId));

                logger.Debug("Found {0} duplicates of {1}", matchingIds.Count() - 1, mod.VersionId);

                if (matchingIds.Count() > 1) {
                    foreach(Mod duplicate in matchingIds.Skip(1)) {
                        toRemove.Add(duplicate);
                    }
                }
            }
            await profile.RemoveModsFromListAsync(toRemove, true);

            // Resolve dependencies
            string[] installedDeps = await profile.ResolveDependenciesAsync();

            return new ValidityReport {
                incompatible = incompatible.ToArray<Mod>(),
                missingDependencies = installedDeps,
                synced = profile.isSynced(),
                repaired = repaired
            };
        }
        public Task CheckForUpdatesAsync() {
            throw new NotImplementedException();
        }
        public Task UpdateModAsync(string id) { // Needs to be manager-side so it can fetch specific version
            throw new NotImplementedException();
        }
        public Task GenerateAsync(bool startFromCleanSlate) {
            throw new NotImplementedException();
        }
        public Task<Mod[]> AddVersionAsync(string id, Repo service, bool ignoreDependencies) {
            throw new NotImplementedException();
        }
    }

    [DBusInterface("org.mercurius.profile")]
    public interface IDbusProfile : IDBusObject {
        public Task<ProfileInfo> GetProfileInfoAsync();
        public Task<Mod[]> AddModAsync(string id, Repo service, bool ignoreDependencies);
        public Task<Mod[]> AddVersionAsync(string version, Repo service, bool ignoreDependencies);
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
        public bool repaired;
    }
}