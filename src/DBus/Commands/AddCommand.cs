using System;
using System.Threading.Tasks;
using Mercurius.Modrinth;
using Mercurius.Modrinth.Models;
using Mercurius.Profiles;
using Mercurius.Configuration;
using Tmds.DBus;

namespace Mercurius.DBus.Commands {
    public class AddCommand : BaseCommand {
        public override string Name { get =>  "Add"; } 
        public override string Description { get => "Adds a mod to the selected profile."; } 
        public override string Format { get => "id<string>"; }
        public override bool TakesArgs { get => true; }
        public override ObjectPath ObjectPath { get => _path; }
        private ObjectPath _path = new ObjectPath("/org/mercurius/command/add");
        private APIClient client;  
        private bool ignoreDependencies;

        public override async Task<DbusResponse> ExecuteAsync(string[] args) {
            if (args.Length < 1) {
                return new DbusResponse {
                    Code = 1,
                    Data = "",
                    Message = "Insufficient args",
                    Type = DataType.Error
                };
            }

            // string query = string.Join(" ", args);
            // if (args.Contains<string>("-d") || args.Contains<string>("--ignore-dependencies")) {
            //     ignoreDependencies = true;
            //     query = string.Join(" ", args.Skip(Array.IndexOf<string>(args, "-d") + 1));
            // }

            if (ProfileManager.SelectedProfile == null) {
                Console.WriteLine("No profile is currently selected for install... ? (Select or create one)");
                return new DbusResponse {
                    Code = 2,
                    Data = "",
                    Message = "No profile is currently selected... ?",
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