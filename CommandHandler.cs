using System.Reflection;
using System.Collections.Generic;
using Mercurius.Commands;

namespace Mercurius {
    public static class CommandHandler {
        private static Dictionary<string, MethodInfo> Commands;
        public static void HandleCommand(string alias, string[] args) {
            Dictionary<string, MethodInfo> Commands2 = Commands;

            foreach (KeyValuePair<string, MethodInfo> commandPair in Commands2) {
                Console.WriteLine(commandPair.Key);
                if (commandPair.Key.Equals(alias)) {
                    Commands.GetValueOrDefault(alias).Invoke(Commands.GetValueOrDefault(alias).GetType().GetConstructor(new Type[0]).Invoke(new object[0]), args);
                } else Console.WriteLine($"Command {alias} not found");
            }
            

        }
        public static void RegisterCommands() {
            Type[] commandTypes = GetCommandTypes();

            Commands = new Dictionary<string, MethodInfo>();

            foreach (Type commandType in commandTypes) {
                Commands.Add(commandType.Name.ToLower(), commandType.GetMethod("Execute"));
            }
        }
        private static Type[] GetCommandTypes() {
            return Assembly.GetAssembly(typeof(BaseCommand)).GetTypes().Where(t => t.IsSubclassOf(typeof(BaseCommand))).ToArray<Type>();
        }
    }
}