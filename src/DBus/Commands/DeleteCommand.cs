// using System;
// using System.Threading.Tasks;
// using Mercurius.Profiles;
// using Tmds.DBus;

// namespace Mercurius.DBus.Commands {
//     public class DeleteCommand : BaseCommand {
//         public override string Name => "Delete";
//         public override string Description => "Deletes a profile";
//         public override string Format => "name<string>";
//         public override ObjectPath ObjectPath { get => _objectPath; }
//         private ObjectPath _objectPath = new ObjectPath("/org/mercurius/command/delete");
//         public override bool TakesArgs { get => true; }
//         public override Task ExecuteAsync(string[] args) {
//             Profile profile = ProfileManager.GetLoadedProfile(args[0]);
            
//             Console.WriteLine($"Removing Profile {profile.Name}...");

//             profile.Delete();
//             profile.Dispose();
//             return Task.CompletedTask;
//         }
//     } 
// }