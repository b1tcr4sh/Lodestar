using Mercurius.Profiles;
using Tmds.DBus;
using NLog;

namespace Mercurius.DBus.Commands {
    public class SelectCommand : BaseCommand {
        public override string Name { get => "select"; }
        public override string Description { get => "selects a loaded profile."; }
        public override string Format { get => "name<string>"; }
        public override bool TakesArgs { get => true; }
        public override ObjectPath ObjectPath { get => _objectPath; }
        private ObjectPath _objectPath = new ObjectPath("/org/mercurius/command/select");
        private ILogger logger;

        public SelectCommand(ILogger _logger) : base(_logger) {
            logger = _logger;
        }

        public override Task<DbusResponse> ExecuteAsync(string[] args) {
            if (args.Length <= 0) {
                // Insufficient args
                return Task.FromResult<DbusResponse>(new DbusResponse {
                    Code = 1,
                    Data = "",
                    Message = "Insufficient args",
                    Type = DataType.Error        
                });
            }

            try {
                ProfileManager.SelectProfile(args[0]); 
            } catch (Exception e) {
                return Task.FromResult<DbusResponse>(new DbusResponse {
                    Code = -1,
                    Data = "",
                    Message = e.Message,
                    Type = DataType.Error
                });
            }

            Console.WriteLine("Selected profile {0}", args[0]);
            return Task.FromResult<DbusResponse>(new DbusResponse {
                Code = 0,
                Data = "",
                Message = "Success",
                Type = DataType.None
            });
        }
    }
}