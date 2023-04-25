using Microsoft.Extensions.Hosting;

using Mercurius.Configuration;
using Mercurius.Profiles;

namespace Mercurius.DBus {
    public interface IDbusHandler {
        public Task RegisterProfileAsync(DbusProfile profile);
        public void DeregisterProfile(string name);
    }
}