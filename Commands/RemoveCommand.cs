using System;
using System.Threading.Tasks;
using Mercurius.Profiles;

namespace Mercurius.Commands {
    public class DeleteCommand : BaseCommand {
        public override string Name => "Remove";
        public override string Description => "Removes a profile";
        public override string Format => "remove <Profile Name>";
        public override Task Execute(string[] args) {
            Profile profile = ProfileManager.GetLoadedProfile(args[0]);
            
            Console.WriteLine($"Removing Profile {profile.Name}...");

            profile.Delete();
            profile.Dispose();
            return Task.CompletedTask;
        }
    } 
}