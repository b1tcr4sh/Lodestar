using Microsoft.Extensions.Hosting;
using Tmds.DBus;
using Serilog;

using Mercurius.Configuration;
using Mercurius.Profiles;

namespace Mercurius.DBus {
    public class DbusServer : IHostedService, IDisposable {
        private Connection connection;
        private readonly ILogger _logger;
        private ProfileManager _manager;
        private IDbusHandler _handler;
        public DbusServer(ProfileManager manager, ILogger logger, IDbusHandler handler) {
            _logger = logger;
            _manager = manager;
            _handler = handler;
        }
        public async Task StartAsync(CancellationToken cancellationToken) {
            _logger.Information("Starting DBus Server Service....");


            if (SettingsManager.Settings.Dbus_System_Bus) {
                connection = new Connection(Address.Session!);
                _logger.Information("Connected to DBus session bus");
            } else {
                ServerConnectionOptions server = new ServerConnectionOptions();
                connection = new Connection(server);

                await server.StartAsync("tcp:host=localhost,port=44881");
                _logger.Information("DBus peer listening at tcp port 44881");
            }

            try {
                await connection.ConnectAsync();
            } catch (ConnectException e) {
                _logger.Fatal("Could not establish connection to system bus... ?");
                _logger.Fatal(e.Message);
                Environment.Exit(-1);
            } catch (DisconnectedException) {
                _logger.Fatal("Could not establish connection to system bus... ?");
                Environment.Exit(-1);
            }

            _handler.SetConnection(connection);  // I hate this, but I guess it workds for now

            if (SettingsManager.Settings.Dbus_System_Bus) {
                await connection.RegisterServiceAsync("org.mercurius.ProfileMessenger", () => {
                    _logger.Fatal("Lost service name 'org.mercurius.ProfileMessenger, probably need to restart... ?");
                }, ServiceRegistrationOptions.Default);
                await connection.RegisterServiceAsync("org.mercurius.profile", () => {
                    _logger.Fatal("Lost service name 'org.mercurius.profile, probably need to restart... ?");
                }, ServiceRegistrationOptions.Default);
            }

            

            await connection.RegisterObjectAsync(new ProfileMessenger(_manager, _handler));

            foreach (Profile profile in _manager.GetLoadedProfiles().Values) {
                await connection.RegisterObjectAsync(new DbusProfile(profile));
            }
        }
        public Task StopAsync(CancellationToken cancellationToken) {
             _logger.Information("Stopping Dbus Server Service.");
             return Task.CompletedTask;
         }
        public void Dispose() {
            _logger.Information("Disposing DBus Server Service....");
        }
    }
}