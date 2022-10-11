using System;
using System.Threading.Tasks;
using Tmds.DBus;
using Mercurius;
using Mercurius.Commands;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mercurius.Dbus {
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

    [DBusInterface("org.mercurius.commandmessenger")]
    public interface ICommandMessenger : IDBusObject {
        // public Task<string> ExecuteCommandAsync(string command, string[] args);
        public Task<String> AddCommandAsync(string query);
    }

    public class CommandMessenger : ICommandMessenger {
        public static readonly ObjectPath Path = new ObjectPath("/org/mercurius/commandmessenger");
        private CommandHandler handler;

        public CommandMessenger() {
            handler = new CommandHandler();
        }

        // public async Task<string> ExecuteCommandAsync(string command, string[] args) {
        //     return await handler.ExecuteCommandAsync(args.Prepend<string>(command).ToArray<string>());
        // }

        public async Task<string> AddCommandAsync(string query) {
            return await handler.ExecuteCommandAsync(new string[] {"add", query});
        }
        public ObjectPath ObjectPath { get { return Path; } }
    }
}