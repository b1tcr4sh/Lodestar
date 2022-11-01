using System;
using System.Threading.Tasks;
using Mercurius.Profiles;
using Tmds.DBus;
using NLog;

namespace Mercurius.DBus.Commands {
    public class DeleteProfileCommand : BaseCommand {
        public override string Name => "DeleteProfile";
        public override string Description => "Deletes a profile";
        public override string Format => "name<string>";
        public override ObjectPath ObjectPath { get => _objectPath; }
        private ObjectPath _objectPath = new ObjectPath("/org/mercurius/command/deleteprofile");
        public override bool TakesArgs { get => true; }
        private ILogger logger;

        public DeleteProfileCommand(ILogger _logger) : base(_logger) {
            logger = _logger;
        }
        public override Task<DbusResponse> ExecuteAsync(string[] args) {
            Profile profile;

            try {
                profile = ProfileManager.GetLoadedProfile(args[0]);
            } catch (Exception e) {
                return Task.FromResult<DbusResponse>(new DbusResponse {
                    Code = -1,
                    Data = e.StackTrace,
                    Message = e.Message,
                    Type = DataType.Error
                });
            }
            
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