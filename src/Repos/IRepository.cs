using Tmds.DBus;

using Mercurius.Profiles;

namespace Mercurius.API {
    public interface IRepository : IDBusObject {
        public Task<Mod[]> SearchModAsync(string query, string version, string loader);
        // Search resource packs
        // Search plugins
    }
}