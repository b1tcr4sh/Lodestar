using System;
using System.Threading.Tasks;
using Mercurius.Profiles;
using Mercurius.Configuration;
using Tmds.DBus;

namespace Mercurius.DBus.Commands {
    public class CreateCommand : BaseCommand {
        public override string Name => "Create";
        public override string Description => "Generates a new profile.";
        public override string Format => "name<string> version<string> loader<string> isServerSide<bool>";
        public override bool TakesArgs { get => true; }
        public override ObjectPath ObjectPath { get => _objectPath; }
        private ObjectPath _objectPath = new ObjectPath("/org/mercurius/command/create");
        public override async Task ExecuteAsync(string[] args) {
            if (args.Length < 4) {
                // insufficient args passed error
                return;
            }

            bool serverSide;

            if (!Boolean.TryParse(args[3], out serverSide)) {
                // incorrect type passed for server side
                return;
            }

            await Profile.CreateNewAsync(args[0], args[1], args[2], serverSide, true);
            Console.WriteLine($"Created and selected new profile {args[0]}");
        }   
    }
}