using System;
using System.Threading.Tasks;
using Mercurius.Profiles;
using Mercurius.Configuration;
using Tmds.DBus;

namespace Mercurius.DBus.Commands {
    public class CreateCommand : BaseCommand {
        public override string Name => "Create";
        public override string Description => "Generates a new profile according to entered values.";
        public override string Format => "create <Profile Name>";
        public override bool TakesArgs { get => true; }
        public override ObjectPath ObjectPath { get => _objectPath; }
        private ObjectPath _objectPath = new ObjectPath("/org/mercurius/command/create");
        private string minecraftVersion;
        private string loader;
        private bool serverSide;
        public override async Task ExecuteAsync(string[] args) {
            string name = string.Join(" ", args);
            if (name.Contains(" ")) {
                name = name.Replace(" ", "_").ToLower();
            }

            Console.WriteLine($"Creating profile {name}");
            CreationDialog();

            await Profile.CreateNewAsync(name, minecraftVersion, loader, serverSide, true);
            Console.WriteLine($"Created and selected new profile {name}");
        }   
        private void CreationDialog() {
            Console.Write("Minecraft Version > ");
            minecraftVersion = Console.ReadLine();

            loader = CollectLoader();

            Console.Write("Server Side (y/N) > ");
            string response = Console.ReadLine().ToLower();
            if (response.Equals("y"))
                serverSide = true;
            else
                serverSide = false;

            Console.WriteLine();
        }
        private string CollectLoader() {
            Console.Write("Loader (1. Fabric, 2. Forge) > ");
            string loaderResponse = Console.ReadLine();
            if (loaderResponse.Equals("1"))
                return "Fabric";
            else if (loaderResponse.Equals("2"))
                return "Forge";
            else {
                Console.WriteLine("Invalid loader selection (Not 1 or 2)");
                return CollectLoader();
            }
        }
    }
}