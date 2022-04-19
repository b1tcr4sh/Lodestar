using System.Reflection;
using Mercurius.Commands;

namespace Mercurius {
    public static class CommandHandler {
        private static Dictionary<string, MethodInfo> Commands;
        public static void HandleCommand(string alias, string[] args) {
            foreach (string commandName in Commands.Keys) {
                if (commandName.Equals(alias)) {
                    Commands.GetValueOrDefault(alias).Invoke(Commands.GetValueOrDefault(alias), args);
                } else Console.WriteLine($"Command {alias} not found");
            }
        }
        public static void RegisterCommands() {
            Type[] commandTypes = GetCommandTypes();

            Commands = new Dictionary<string, MethodInfo>();

            foreach (Type commandType in commandTypes) {
                Commands.Add(commandType.GetMember("Alias").ToString(), commandType.GetMethod("Execute"));
            }
        }
        private static Type[] GetCommandTypes() {
            return Assembly.GetAssembly(typeof(IBaseCommand)).GetTypes().Where(t => t.IsSubclassOf(typeof(IBaseCommand))).ToArray<Type>();
        }
    }
}