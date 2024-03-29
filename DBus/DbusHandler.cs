using System;
using System.Threading.Tasks;
using System.Reflection;
using Tmds.DBus;
using Mercurius.Configuration;
using Mercurius.Profiles;
using Microsoft.Extensions.Hosting;
using NLog;

namespace Mercurius.DBus {
    public class DbusHandler : IDisposable, IHostedService {
        public static async Task RegisterProfileAsync(DbusProfile profile) {
            await DbusConnection.RegisterObjectAsync(profile);
        }
        public static void DeregisterProfile(string name) {
            if (!Profile.Exists(name)) {
                throw new ProfileException($"Profile {name} doesn't exist!");
            }

            DbusConnection.UnregisterObject(new ObjectPath($"/org/mercurius/profile/{name}"));
        }

        private static Connection DbusConnection;

        private readonly ILogger logger;
        // private readonly IOptions<DaemonConfig> _config;
        public DbusHandler() {
            logger = LogManager.GetCurrentClassLogger();

            //  _logger = logger;
            //  _config = config;
         }

        public async Task StartAsync(CancellationToken cancellationToken) {
            logger.Info("Starting DBus Server Service....");

            Connection connection;

            if (SettingsManager.Settings.Dbus_System_Bus) {
                connection = new Connection(Address.Session!);
                logger.Info("Connected to DBus session bus");
            } else {
                ServerConnectionOptions server = new ServerConnectionOptions();
                connection = new Connection(server);

                await server.StartAsync("tcp:host=localhost,port=44881");
                logger.Info("DBus peer listening at tcp port 44881");
            }

            DbusConnection = connection;

            try {
                await connection.ConnectAsync();
            } catch (ConnectException e) {
                logger.Fatal("Could not establish connection to system bus... ?");
                logger.Fatal(e.Message);
                Environment.Exit(-1);
            } catch (DisconnectedException) {
                logger.Fatal("Could not establish connection to system bus... ?");
                Environment.Exit(-1);
            }

            if (SettingsManager.Settings.Dbus_System_Bus) {
                await connection.RegisterServiceAsync("org.mercurius.ProfileMessenger", () => {
                    logger.Fatal("Lost service name 'org.mercurius.ProfileMessenger, probably need to restart... ?");
                }, ServiceRegistrationOptions.Default);
                await connection.RegisterServiceAsync("org.mercurius.profile", () => {
                    logger.Fatal("Lost service name 'org.mercurius.profile, probably need to restart... ?");
                }, ServiceRegistrationOptions.Default);
            }

            

            await connection.RegisterObjectAsync(new ProfileMessenger());

            foreach (Profile profile in ProfileManager.GetLoadedProfiles().Values) {
                await connection.RegisterObjectAsync(new DbusProfile(profile));
            }
        }
        public Task StopAsync(CancellationToken cancellationToken) {
             logger.Info("Stopping Dbus Server Service.");
             return Task.CompletedTask;
         }
        public void Dispose() {
            logger.Info("Disposing DBus Server Service....");
        }
    }
}