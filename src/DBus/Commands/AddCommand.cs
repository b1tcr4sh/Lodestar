using System;
using System.Threading.Tasks;
using Mercurius.Modrinth;
using Mercurius.Modrinth.Models;
using Mercurius.Profiles;
using Mercurius.Configuration;
using Tmds.DBus;
using NLog;

namespace Mercurius.DBus.Commands {
    public class AddCommand : BaseCommand {
        public override string Name { get =>  "Add"; } 
        public override string Description { get => "Adds a mod to the selected profile."; } 
        public override string Format { get => "id<string>, service<enum>, ignoreDependencies<bool>"; }
        public override bool TakesArgs { get => true; }
        public override ObjectPath ObjectPath { get => _path; }
        private ObjectPath _path = new ObjectPath("/org/mercurius/command/add");
        private APIClient client;  
        private ILogger logger;

        public AddCommand(ILogger _logger) : base(_logger) {
            logger = _logger;
        }

        public override async Task<DbusResponse> ExecuteAsync(string[] args) {
            bool ignoreDependencies;
            Repo service;


            if (args.Length < 2) {
                return new DbusResponse {
                    Code = 1,
                    Data = "",
                    Message = "Insufficient args",
                    Type = DataType.Error
                };
            }

            if (ProfileManager.SelectedProfile == null) {
                logger.Debug("No profile is currently selected for install... ? (Select or create one)");
                return new DbusResponse {
                    Code = 2,
                    Data = "",
                    Message = "No profile is currently selected... ?",
                    Type = DataType.Error
                };
            } 

            if (!Boolean.TryParse(args[2], out ignoreDependencies)) {
                return new DbusResponse {
                    Code = -1,
                    Data = "",
                    Message = "<ignoreDependencies> could not be resolved to boolean",
                    Type = DataType.Error
                };
            }
            if (!Enum.TryParse<Repo>(args[1], out service)) {
                return new DbusResponse {
                    Code = -1,
                    Data = "",
                    Message = "<service> could not be resolved to enum",
                    Type = DataType.Error
                };
            }

            client = new APIClient();

            try {
                await ProfileManager.AddModAsync(client, args[0], service, ignoreDependencies);
            } catch (Exception e) {
                return new DbusResponse {
                    Code = -1,
                    Data = e.StackTrace,
                    Message = e.Message,
                    Type = DataType.Error
                };
            }
            
            return new DbusResponse {
                Code = 0,
                Data = "",
                Message = "Successfully added mod to profile",
                Type = DataType.None
            };
        }
    
    }
}