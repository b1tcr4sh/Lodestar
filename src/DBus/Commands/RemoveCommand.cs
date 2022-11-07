using System;
using System.Threading.Tasks;
using Mercurius.Profiles;
using Tmds.DBus;
using NLog;

namespace Mercurius.DBus.Commands {
    public class RemoveCommand : BaseCommand {
        public override string Name => "RemoveMod";
        public override string Description => "Removes a Mod from a Profile.";
        public override string Format => "name<string>";
        public override bool TakesArgs { get => true; }
        public override ObjectPath ObjectPath { get => _objectPath; }
        private ObjectPath _objectPath = new ObjectPath("/org/mercurius/command/removemod");
        private ILogger logger;

        public RemoveCommand(ILogger _logger) : base(_logger) {
            logger = _logger;
        }
        public override async Task<DbusResponse> ExecuteAsync(string[] args) {
            // Profile selectedProfile = await ProfileManager.GetSelectedProfileAsync();

            List<Mod> allMods = new List<Mod>();

            List<Mod> matchingMods = new List<Mod>();
            // foreach (Mod mod in selectedProfile.Mods) {
            //     allMods.Add(mod);
            //     allMods.AddRange(mod.Dependencies);
            // }

            foreach (Mod mod in allMods) {
                if (mod.Title.ToLower().Equals(args[0])) {
                    matchingMods.Add(mod);
                }
            }
                    
            if (matchingMods.Count() <= 0) {
                // logger.Debug("There are No Mods Matching \"{0}\" in Profile {1}", args[0], selectedProfile.Name);
                return new DbusResponse {
                    Code = -1,
                    Data = "",
                    Message = "No mods matching supplied query",
                    Type = DataType.Error
                };
            }

            foreach (Mod mod in matchingMods) {
                // await selectedProfile.RemoveModFromListAsync(mod);
            }
            return new DbusResponse {
                Code = 0,
                Data = "",
                Message = "Success",
                Type = DataType.None
            };
        }
    } 
}