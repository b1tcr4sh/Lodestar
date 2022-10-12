using System;
using System.Threading.Tasks;
using System.Reflection;
using Tmds.DBus;
using Mercurius.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mercurius.DBus {
    public class DbusHandler : IDisposable, IHostedService {
        private readonly ILogger _logger;
        private readonly IOptions<DaemonConfig> _config;
        public DbusHandler(ILogger<DaemonService> logger, IOptions<DaemonConfig> config) {
             _logger = logger;
             _config = config;
         }

        public async Task StartAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("Starting DBus Server Service....");

            ServerConnectionOptions server = new ServerConnectionOptions();
            using (Connection connection = new Connection(server)) {
                await connection.ConnectAsync();
                await connection.RegisterObjectAsync(new CommandMessenger());

                await connection.RegisterObjectsAsync(CommandManager.GetCommands().Values);

                // string boundAddress = await server.StartAsync($"tcp:host=localhost,port={_config.Value.DBusPort}");
                string boundAddress = await server.StartAsync("tcp:host=localhost,port=44881");
                _logger.LogInformation($"Dbus Server Listening at {boundAddress}");
            }
        }
        public Task StopAsync(CancellationToken cancellationToken) {
             _logger.LogInformation("Stopping Dbus Server Service.");
             return Task.CompletedTask;
         }
        public void Dispose() {
            _logger.LogInformation("Disposing DBus Server Service....");
        }
    }
}