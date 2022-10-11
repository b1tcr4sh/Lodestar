using System;
using System.Reflection;
using System.Collections.Generic;
using Mercurius.Modrinth;
using Mercurius.Dbus;

namespace Mercurius.Commands {
    public class CommandHandler {
        public string[] Args { get; private set; }
        private string Command;
        public Dictionary<string, BaseCommand> Commands { get; private set; }

        public CommandHandler() {
            Commands = GetCommands();
        }

        public async Task<string> ExecuteCommandAsync(string[] args) {
            ParseCommand(args);

            if (Command.Equals("help") || Command.Equals("-h") || Command.Equals("--help")) {
                return HelpMessage();
            }

            if (Commands.ContainsKey(Command)) {
                // if (Args.All<string>(item => item is null || item.Equals(string.Empty) || )) {
                if (Args.Length < Commands.GetValueOrDefault(Command).ArgsQuantity) {
                    // Console.WriteLine($"Insufficient Arguments Passesd for command {Command}\n{Commands.GetValueOrDefault(Command).Name}: {Commands.GetValueOrDefault(Command).Format}");
                    return $"Insufficient Arguments Passesd for command {Command}\n{Commands.GetValueOrDefault(Command).Name}: {Commands.GetValueOrDefault(Command).Format}";
                }

                return await Commands.GetValueOrDefault(Command).Execute(Args);
            } else return $"Command {Command} not found... ?";
        }

        public string HelpMessage() {            
            StringWriter writer = new StringWriter();
            writer.WriteLine("Mercurius - A package manager-like thing for Minecraft mods.\nThis app uses the Modrinth (https://modrinth.com) api to source mods, so uncountable thank yous to that team");
            writer.WriteLine("\n{0, -10} {1, -30}", "Commands:", "Format:");

            foreach (KeyValuePair<string, BaseCommand> command in Commands) {
                writer.WriteLine(" {0, -10} {1, -30} {2, 20}", command.Value.Name, command.Value.Format, command.Value.Description);
            }

            return writer.ToString();
        }

        private void ParseCommand(string[] args) {
            Command = args[0].ToLower();
            Args = args.Skip<string>(1).ToArray<string>();
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