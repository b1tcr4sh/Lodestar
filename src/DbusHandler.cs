using System;
using System.Threading.Tasks;
using Tmds.DBus;
using Mercurius;
using Mercurius.Commands;

namespace Mercurius.Dbus {
    public class DbusHandler {
        public async Task InstantiateDbusServer() {
            ServerConnectionOptions server = new ServerConnectionOptions();
            using (Connection connection = new Connection(server)) {
                await connection.ConnectAsync();
                await connection.RegisterObjectAsync(new CommandMessenger());

                string boundAddress = await server.StartAsync("tcp:host=localhost,port=44881");
                MCSLogger.logger.Info($"Dbus server listening at {boundAddress}");
            }
        }
    }

    [DBusInterface("org.mercurius.commandhandler")]
    interface ICommandMessenger : IDBusObject {
        public Task<string> ExecuteCommand(string command, string[] args);
    }

    class CommandMessenger : ICommandMessenger {
        public static readonly ObjectPath Path = new ObjectPath("org/mercurius/commandhandler");
        private CommandHandler handler;

        public CommandMessenger() {
            handler = new CommandHandler();
        }

        public async Task<string> ExecuteCommand(string command, string[] args) {
            return await handler.ExecuteCommandAsync(args.Prepend<string>(command).ToArray<string>());
        }
        public ObjectPath ObjectPath { get { return Path; } }
    }
}