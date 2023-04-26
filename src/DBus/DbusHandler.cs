using Serilog;
using Tmds.DBus;

using Mercurius.Configuration;
using Mercurius.Profiles;

namespace Mercurius.DBus {
    public class DbusHandler : IDbusHandler {
        private ILogger _logger;
        private Connection _connection;
        public DbusHandler(ILogger logger) {
            _logger = logger;
        }
        public async Task RegisterProfileAsync(DbusProfile profile) { 
            await _connection.RegisterObjectAsync(profile);
        }
        public void DeregisterProfile(string name) { 
            _connection.UnregisterObject($"/org/mercurius/profile/{name}");
        }
        public void SetConnection(Connection dbus) { // I hate this, but I guess it workds for now
            if (_connection is not null) {
                throw new Exception("Connection is already set for this handler");
            }
            _connection = dbus;
        }
    }
}