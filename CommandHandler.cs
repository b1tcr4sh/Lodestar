using System;
using System.Reflection;
using System.Collections.Generic;

namespace Mercurius.Commands {
    public class CommandHandler {
        public string[] Args { get; private set; }
        private string Command;
        public CommandHandler(string[] args) {
            if (args is null) throw new ArgumentNullException("args was null");
            
            Args = args.Skip<string>(1).ToArray<string>();
            GetCommands();
        }

        private Dictionary<string, BaseCommand> GetCommands() {
            List<BaseCommand> commandObjects = new List<BaseCommand>();
            Type[] commandTypes = GetCommandTypes();

            foreach (Type commandObject in commandTypes) {
                BaseCommand command = commandObject.GetConstructor(new Type[0]).Invoke(new object[0]) as BaseCommand;
                commandObjects.Add(command);

                Console.WriteLine(command.Name);
            }


            return null;
        }

        public async Task ExecuteCommand() {
            
        }

        private static Type[] GetCommandTypes() {
            return Assembly.GetAssembly(typeof(BaseCommand)).GetTypes().Where(t => t.IsSubclassOf(typeof(BaseCommand))).ToArray<Type>();
        }
    }
}