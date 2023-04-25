using Serilog;
using Tmds.DBus;

using Mercurius.Configuration;
using Mercurius.Profiles;

namespace Mercurius.DBus {
    public class DbusHandler : IDbusHandler {
        private ProfileManager _manager;
        private ILogger _logger;
        private Connection _connection;
        public DbusHandler(ProfileManager manager, ILogger logger) {
            _manager = manager;
            _logger = logger;
        }
        public async Task RegisterProfileAsync(DbusProfile profile) { 
            await _connection.RegisterObjectAsync(profile);
        }
        public void DeregisterProfile(string name) { 
            if (!_manager.ProfileExists(name)) {
                throw new ProfileException($"Profile {name} doesn't exist!");
            }

            _connection.UnregisterObject(new ObjectPath($"/org/mercurius/profile/{name}"));
        }
        public void setConnection(Connection dbus) { // I hate this, but I guess it workds for now
            _connection = dbus;
        }
    }
}