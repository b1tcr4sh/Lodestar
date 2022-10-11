using System;
using System.Threading.Tasks;
using Mercurius.Profiles;

namespace Mercurius.Commands {
    public class DeleteCommand : BaseCommand {
        public override string Name => "Delete";
        public override string Description => "Deletes a profile";
        public override string Format => "delete <Profile Name>";
        public override int ArgsQuantity => 1;
        public override Task Execute(string[] args) {
            Profile profile = ProfileManager.GetLoadedProfile(args[0]);
            
            Console.WriteLine($"Removing Profile {profile.Name}...");

            profile.Delete();
            profile.Dispose();
            return Task.CompletedTask;
        }
    } 
}