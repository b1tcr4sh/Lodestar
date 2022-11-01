using System;
using System.Threading.Tasks;
using Mercurius;
using Mercurius.Modrinth;
using Mercurius.Configuration;
using Mercurius.Profiles;
using Tmds.DBus;
using NLog;

namespace Mercurius.DBus.Commands {
    public class GenerateCommand : BaseCommand {
        public override string Name { get => "generate"; }
        public override string Description { get => "generates mods for an empty profile from mod jars."; }
        public override string Format { get => "[none]"; }
        public override bool TakesArgs { get => false; }
        public override ObjectPath ObjectPath { get => _objectPath; }
        private ObjectPath _objectPath = new ObjectPath("/org/mercurius/command/generate");
        public override async Task<DbusResponse> ExecuteAsync(string[] args) {
            if (ProfileManager.SelectedProfile is null) {
                // No profile selected
                return new DbusResponse {
                    Code = 2,
                    Data = "",
                    Message = "No profile currently selected",
                    Type = DataType.Error
                };
            }

            ILogger logger = LogManager.GetCurrentClassLogger();

            List<string> existingFiles = Directory.GetFiles($"{SettingsManager.Settings.Minecraft_Directory}/mods/").ToList<string>();

            APIClient client = new APIClient();

            foreach (string path in existingFiles) {
                string query = path.Substring(path.LastIndexOf("/") + 1);

                logger.Info("Trying to generate a mod from {0}", path);                

                await ProfileManager.AddModAsync(client, query, false);
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