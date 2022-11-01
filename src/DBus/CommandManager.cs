using System;
using System.Reflection;
using NLog;

namespace Mercurius.DBus {
    public static class CommandManager {
        public static Logger CommandLogger = LogManager.GetCurrentClassLogger();
        public static Dictionary<string, BaseCommand> GetCommands() {
            Dictionary<string, BaseCommand> commands = new Dictionary<string, BaseCommand>();
            Type[] commandTypes = GetCommandTypes();

            foreach (Type commandObject in commandTypes) {
                BaseCommand command = commandObject.GetConstructor(new Type[0]).Invoke(new object[] {CommandLogger as object}) as BaseCommand;
                commands.Add(command.Name.ToLower(), command);
            }

            return commands;
        }
        private static Type[] GetCommandTypes() {
            return Assembly.GetAssembly(typeof(BaseCommand)).GetTypes().Where(t => t.IsSubclassOf(typeof(BaseCommand))).ToArray<Type>();
        }
    }
}