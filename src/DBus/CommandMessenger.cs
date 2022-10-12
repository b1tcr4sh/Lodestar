using Tmds.DBus;

namespace Mercurius.DBus {
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