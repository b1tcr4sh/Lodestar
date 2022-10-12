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
        public override string Description { get => "Adds a Mod to the Selected Profile."; } 
        public override string Format { get => "Add <Mod Name>"; }
        public override bool TakesArgs { get => false; }
        public override ObjectPath ObjectPath { get => _path; }
        private ObjectPath _path = new ObjectPath("/org/mercurius/command/add");
        private APIClient client;  
        private bool ignoreDependencies;

        public override async Task ExecuteAsync(string[] args) {
            if (args.Length < 1) throw new ArgumentException("Insuffcient Arguments Provided.");

            string query = string.Join(" ", args);
            if (args.Contains<string>("-d") || args.Contains<string>("--ignore-dependencies")) {
                ignoreDependencies = true;
                query = string.Join(" ", args.Skip(Array.IndexOf<string>(args, "-d") + 1));
            }

            if (ProfileManager.SelectedProfile == null) {
                Console.WriteLine("No profile is currently selected for install... ? (Select or create one)");
                return;
            } 

            client = new APIClient();

            await ProfileManager.AddModAsync(client, query, ignoreDependencies);
        }
    
    }
}