using Tmds.DBus;
using System.Runtime.InteropServices;

namespace Mercurius.DBus {
    [StructLayout(LayoutKind.Sequential)]
    public struct DbusResponse {
        public String Message { get; set; }
        public int Code { get; set; }
        public object Data { get; set; }
        public DataType Type { get; set; }
    }

    public enum DataType {
        None = 0,
        ModDefinition = 1,
        Profile = 1,
        Error = 2
    }

    [DBusInterface("org.mercurius.commandmessenger")]
    public interface ICommandMessenger : IDBusObject {
        Task<ObjectPath[]> ListCommandsAsync();
    }


    public class CommandMessenger : ICommandMessenger {
        private static readonly ObjectPath _objectPath = new ObjectPath("/org/mercurius/commandmessenger");

        public Task<ObjectPath[]> ListCommandsAsync() {
            List<ObjectPath> paths = new List<ObjectPath>();

            foreach (KeyValuePair<string, BaseCommand> command in CommandManager.GetCommands() ) {
                paths.Add(command.Value.ObjectPath);
            }

            return Task.FromResult<ObjectPath[]>(paths.ToArray<ObjectPath>());
        }
        public ObjectPath ObjectPath { get => _objectPath; }
    }
}