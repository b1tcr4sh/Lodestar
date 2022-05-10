using System;
using System.Threading.Tasks;
using Mercurius.Profiles;

namespace Mercurius.Commands {
    public class DeleteCommand : BaseCommand {
        public override string Name => "Delete";
        public override string Description => "Deletes a profile";
        public override string Format => "delete <Profile Name>";
        public override Task Execute(string[] args) {
            Profile profile = ProfileManager.GetLoadedProfile(args[0]);
            
            Console.WriteLine($"Deleting Profile {profile.Name}...");

            profile.Delete();
            profile.Dispose();
            return Task.CompletedTask;
        }
    } 
}