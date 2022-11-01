using System;
using System.Threading.Tasks;
using Mercurius.Profiles;
using Tmds.DBus;

namespace Mercurius.DBus.Commands {
    public class DeleteProfileCommand : BaseCommand {
        public override string Name => "DeleteProfile";
        public override string Description => "Deletes a profile";
        public override string Format => "name<string>";
        public override ObjectPath ObjectPath { get => _objectPath; }
        private ObjectPath _objectPath = new ObjectPath("/org/mercurius/command/deleteprofile");
        public override bool TakesArgs { get => true; }
        public override Task<DbusResponse> ExecuteAsync(string[] args) {
            Profile profile = ProfileManager.GetLoadedProfile(args[0]);
            // Handle profile not being loaded
            
            Console.WriteLine($"Removing Profile {profile.Name}...");

            profile.Delete();
            profile.Dispose();
            return Task.FromResult<DbusResponse>(new DbusResponse {
                Code = 0,
                Data = "",
                Message = "Success",
                Type = DataType.None
            });
        }
    } 
}