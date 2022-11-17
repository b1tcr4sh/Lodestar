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

            ServerConnectionOptions server = new ServerConnectionOptions();
            Connection connection = new Connection(server);

            DbusConnection = connection;

            await connection.ConnectAsync();
            await connection.RegisterObjectAsync(new ProfileMessenger());

            foreach (Profile profile in ProfileManager.GetLoadedProfiles().Values) {
                await connection.RegisterObjectAsync(new DbusProfile(profile));
            }
            string boundAddress = await server.StartAsync("tcp:host=localhost,port=44881");
            logger.Info($"Dbus Server Listening at {boundAddress}");
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