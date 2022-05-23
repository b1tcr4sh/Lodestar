using System;
using System.Threading.Tasks;
using Mercurius.Profiles;

namespace Mercurius.Commands {
    public class ListCommand : BaseCommand {
        public override string Name => "List";
        public override string Description => "Lists either currently loaded profiles or mods of selected profile.";
        public override string Format => "list [mods | profiles]";
        public override Task Execute(string[] args)
        {
            if (args.Length == 0) {
                Console.WriteLine("What to list? ... (mods or profiles)");
                return Task.CompletedTask;
            }

            switch (args[0].ToLower()) {
                case "mods":
                case "mod":
                    ListMods();
                    break;
                case "profiles":
                case "profile":
                    ListProfiles();
                    break;
                default:
                    Console.WriteLine("What to list? ... (mods or profiles)");
                    break;
            }

            return Task.CompletedTask;
        }
        private void ListMods() {
            if (ProfileManager.SelectedProfile is null) {
                Console.WriteLine("No profile is currently selected... ?");
                return;
            }
            if (ProfileManager.SelectedProfile.Mods.Count <= 0) {
                Console.WriteLine($"Profile {ProfileManager.SelectedProfile.Name} contains no mods");
                return;
            }

            Console.WriteLine("Listing {0} mods in currently loaded profile {1}\n", ProfileManager.SelectedProfile.Mods.Count, ProfileManager.SelectedProfile.Name);
            Console.WriteLine("{0, -30} {1, -20} {2, 15}", "Mod Name", "Mod Version", "Filename");

            foreach (Mod mod in ProfileManager.SelectedProfile.Mods) {
                Console.WriteLine("{0, -30} {1, -20} {2, 15}", mod.Title, mod.ModVersion, mod.FileName);
            }
        }
        private void ListProfiles() {
            IReadOnlyDictionary<string, Profile> profiles = ProfileManager.GetLoadedProfiles();

            if (profiles.Count <= 0) {
                Console.WriteLine("There are 0 profiles loaded.  (Is this intentional... ?)");
                return;
            }
            Console.WriteLine($"Listing {profiles.Count} currrently loaded profile(s)\n");
            Console.WriteLine("{0, -30} {1, -20} {2, 15}", "Profile Name", "Minecraft Version", "Loader");

            foreach (KeyValuePair<string, Profile> pair in ProfileManager.GetLoadedProfiles()) {
                Console.WriteLine("{0, -30} {1, -20} {2, 15}", pair.Value.Name, pair.Value.MinecraftVersion, pair.Value.Loader);
            }
        }
    }
}