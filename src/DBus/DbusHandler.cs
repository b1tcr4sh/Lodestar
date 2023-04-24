using Serilog;

using Mercurius.Configuration;
using Mercurius.Profiles;

namespace Mercurius.DBus {
    public class DbusHandler : IDbusHandler {
        private ProfileManager _manager;
        private ILogger _logger;
        public DbusHandler(ProfileManager manager, ILogger logger) {
            _manager = manager;
            _logger = logger;
        }
        public async Task RegisterProfileAsync(DbusProfile profile) {
            
        }
        public void DeregisterProfile(string name) {

        }
    }
}