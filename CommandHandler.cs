using System;
using System.Reflection;
using System.Collections.Generic;
using Mercurius.Modrinth;

namespace Mercurius.Commands {
    public class CommandHandler {
        public string[] Args { get; private set; }
        private string Command;
        public Dictionary<string, BaseCommand> Commands { get; private set; }
        public CommandHandler(string[] args) {
            Commands = GetCommands();
            if (args.Length == 0) HelpCommand();

            Command = args[0].ToLower();
            Args = args.Skip<string>(1).ToArray<string>();
        }

        public async Task ExecuteCommandAsync() {
            if (Command.Equals("help") || Command.Equals("-h") || Command.Equals("--help")) {
                HelpCommand();
            }

            if (Commands.ContainsKey(Command)) {
                if (Args.All<string>(item => item is null || item.Equals(string.Empty))) {
                    Console.WriteLine($"Insufficient Arguments Passesd for command {Command}\n{Commands.GetValueOrDefault(Command).Name}: {Commands.GetValueOrDefault(Command).Format}");
                    return;
                }

                await Commands.GetValueOrDefault(Command).Execute(Args);
            } else Console.WriteLine($"Command {Command} not found... ?");
        }

        private void HelpCommand() {
            Console.WriteLine("Mercurius - A package manager-like thing for Minecraft mods.\nThis app uses the Modrinth (https://modrinth.com) api to source mods, so uncountable thank yous to that team.");
            Console.WriteLine("\n{0, -10} {1, -30}", "Commands:", "Format:");
            
            foreach (KeyValuePair<string, BaseCommand> command in Commands) {
                Console.WriteLine(" {0, -10} {1, -30} {2, 20}", command.Value.Name, command.Value.Format, command.Value.Description);
            }

            System.Environment.Exit(0);
        }


        private Dictionary<string, BaseCommand> GetCommands() {
            Dictionary<string, BaseCommand> commands = new Dictionary<string, BaseCommand>();
            Type[] commandTypes = GetCommandTypes();

            foreach (Type commandObject in commandTypes) {
                BaseCommand command = commandObject.GetConstructor(new Type[0]).Invoke(new object[0]) as BaseCommand;
                commands.Add(command.Name.ToLower(), command);
            }

            return commands;
        }
        private static Type[] GetCommandTypes() {
            return Assembly.GetAssembly(typeof(BaseCommand)).GetTypes().Where(t => t.IsSubclassOf(typeof(BaseCommand))).ToArray<Type>();
        }
    }
}