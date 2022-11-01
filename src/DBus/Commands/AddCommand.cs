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
        public override string Format { get => "id<string>, ignoreDependencies<bool>"; }
        public override bool TakesArgs { get => true; }
        public override ObjectPath ObjectPath { get => _path; }
        private ObjectPath _path = new ObjectPath("/org/mercurius/command/add");
        private APIClient client;  
        private bool ignoreDependencies;
        private ILogger logger;

        internal AddCommand(ILogger _logger) : base(_logger) {
            logger = _logger;
        }

        public override async Task<DbusResponse> ExecuteAsync(string[] args) {
            if (args.Length < 2) {
                return new DbusResponse {
                    Code = 1,
                    Data = "",
                    Message = "Insufficient args",
                    Type = DataType.Error
                };
            }

            if (ProfileManager.SelectedProfile == null) {
                Console.WriteLine("No profile is currently selected for install... ? (Select or create one)");
                return new DbusResponse {
                    Code = 2,
                    Data = "",
                    Message = "No profile is currently selected... ?",
                    Type = DataType.Error
                };
            } 

            if (!Boolean.TryParse(args[1], out ignoreDependencies)) {
                return new DbusResponse {
                    Code = -1,
                    Data = "",
                    Message = "ignoreDependencies could not be resolved to boolean",
                    Type = DataType.Error
                };
            }



            client = new APIClient();

            if (!await ProfileManager.AddModAsync(client, args[0], ignoreDependencies)) {
                return new DbusResponse {
                    Code = -1,
                    Data = "",
                    Message = "Adding mod to profile failed",
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