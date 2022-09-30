using System;
using System.Threading.Tasks;
using Mercurius.Profiles;

namespace Mercurius.Commands {
    public class RemoveCommand : BaseCommand {
        public override string Name => "Remove";
        public override string Description => "Removes a Mod from a Profile.";
        public override string Format => "remove <Mod Name>";
        public override int ArgsQuantity => 1;
        public override async Task Execute(string[] args) {
            string query = string.Join<string>(" ", args);

            List<Mod> allMods = new List<Mod>();

            List<Mod> matchingMods = new List<Mod>();
            foreach (Mod mod in ProfileManager.SelectedProfile.Mods) {
                allMods.Add(mod);
                allMods.AddRange(mod.Dependencies);
            }

            foreach (Mod mod in allMods) {
                if (mod.Title.ToLower().Equals(query)) {
                    matchingMods.Add(mod);
                }
            }
                    
            if (matchingMods.Count() <= 0) {
                Console.WriteLine("There are No Mods Matching \"{0}\" in Profile {1}", query, ProfileManager.SelectedProfile.Name);
                return;
            }

            foreach (Mod mod in matchingMods) {
                await ProfileManager.SelectedProfile.RemoveModFromListAsync(mod);
            }
        }
    } 
}