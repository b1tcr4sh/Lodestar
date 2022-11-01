using System;
using System.Threading.Tasks;
using Mercurius.Profiles;
using Mercurius.Configuration;
using Tmds.DBus;
using NLog;

namespace Mercurius.DBus.Commands {
    public class CreateProfileCommand : BaseCommand {
        public override string Name => "CreateProfile";
        public override string Description => "Generates a new profile.";
        public override string Format => "name<string> version<string> loader<string> isServerSide<bool>";
        public override bool TakesArgs { get => true; }
        public override ObjectPath ObjectPath { get => _objectPath; }
        private ObjectPath _objectPath = new ObjectPath("/org/mercurius/command/createprofile");
        private ILogger logger;

        internal CreateProfileCommand(ILogger _logger) : base(_logger) {
            logger = _logger;
        }

        public override async Task<DbusResponse> ExecuteAsync(string[] args) {
            logger = LogManager.GetCurrentClassLogger();

            if (args.Length < 4) {
                // insufficient args passed error
                return new DbusResponse {
                    Code = 1,
                    Data = "",
                    Message = "Insufficient args",
                    Type = DataType.Error
                };
            }

            bool serverSide;

            if (!Boolean.TryParse(args[3], out serverSide)) {
                // incorrect type passed for server side
                return new DbusResponse {
                    Code = -1,
                    Data = "",
                    Message = "Malformed isServerSide arg",
                    Type = DataType.Error
                };
            }

            try {
                await Profile.CreateNewAsync(args[0], args[1], args[2], serverSide, false);
            } catch (Exception e) {
                return new DbusResponse {
                    Code = -1,
                    Data = "",
                    Message = e.Message,
                    Type = DataType.Error
                };
            }


            logger.Info($"Created new profile {args[0]}");

            return new DbusResponse {
                Code = 0,
                Data = "",
                Message = "Success",
                Type = DataType.None
            };
        }   
    }
}