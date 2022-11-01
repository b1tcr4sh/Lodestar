using System;
using System.Reflection;
using NLog;

namespace Mercurius.DBus {
    public static class CommandManager {
        public static Logger CommandLogger;
        public static Dictionary<string, BaseCommand> GetCommands() {
            CommandLogger = LogManager.GetCurrentClassLogger();

            Dictionary<string, BaseCommand> commands = new Dictionary<string, BaseCommand>();
            Type[] commandTypes = GetCommandTypes();

            foreach (Type commandObject in commandTypes) {
                BaseCommand command = commandObject.GetConstructor(new Type[] {typeof(ILogger)}).Invoke(new object[] {CommandLogger}) as BaseCommand;
                commands.Add(command.Name.ToLower(), command);
            }

            return commands;
        }
        private static Type[] GetCommandTypes() {
            return Assembly.GetAssembly(typeof(BaseCommand)).GetTypes().Where(t => t.IsSubclassOf(typeof(BaseCommand))).ToArray<Type>();
        }
    }
}