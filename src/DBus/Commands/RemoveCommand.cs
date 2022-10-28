// using System;
// using System.Threading.Tasks;
// using Mercurius.Profiles;
// using Tmds.DBus;

// namespace Mercurius.DBus.Commands {
//     public class RemoveCommand : BaseCommand {
//         public override string Name => "Remove";
//         public override string Description => "Removes a Mod from a Profile.";
//         public override string Format => "name<string>";
//         public override bool TakesArgs { get => true; }
//         public override ObjectPath ObjectPath { get => _objectPath; }
//         private ObjectPath _objectPath = new ObjectPath("/org/mercurius/command/remove");
//         public override async Task ExecuteAsync(string[] args) {
//             string query = string.Join<string>(" ", args);

//             List<Mod> allMods = new List<Mod>();

//             List<Mod> matchingMods = new List<Mod>();
//             foreach (Mod mod in ProfileManager.SelectedProfile.Mods) {
//                 allMods.Add(mod);
//                 allMods.AddRange(mod.Dependencies);
//             }

//             foreach (Mod mod in allMods) {
//                 if (mod.Title.ToLower().Equals(query)) {
//                     matchingMods.Add(mod);
//                 }
//             }
                    
//             if (matchingMods.Count() <= 0) {
//                 Console.WriteLine("There are No Mods Matching \"{0}\" in Profile {1}", query, ProfileManager.SelectedProfile.Name);
//                 return;
//             }

//             foreach (Mod mod in matchingMods) {
//                 await ProfileManager.SelectedProfile.RemoveModFromListAsync(mod);
//             }
//         }
//     } 
// }