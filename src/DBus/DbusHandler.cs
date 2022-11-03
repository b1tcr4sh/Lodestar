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
            using (Connection connection = new Connection(server)) {
                await connection.ConnectAsync();
                await connection.RegisterObjectAsync(new CommandMessenger());

                await connection.RegisterObjectsAsync(CommandManager.GetCommands().Values);
                // await connection.RegisterObjectsAsync(ProfileManager.GetLoadedProfiles().Values);  Throws exception??

                // string boundAddress = await server.StartAsync($"tcp:host=localhost,port={_config.Value.DBusPort}");
                string boundAddress = await server.StartAsync("tcp:host=localhost,port=44881");
                logger.Info($"Dbus Server Listening at {boundAddress}");
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