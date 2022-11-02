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
        private ILogger logger;

        public GenerateCommand(ILogger _logger) : base(_logger) {
            logger = _logger;
        }
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

            List<string> existingFiles = Directory.GetFiles($"{SettingsManager.Settings.Minecraft_Directory}/mods/").ToList<string>();
            if (existingFiles.Count < 1) {
                return new DbusResponse {
                    Code = -1,
                    Data = "",
                    Message = "No mod files to generate from... ?"
                };
            }

            if (ProfileManager.SelectedProfile.Mods.Count >= 1) {
                foreach (Mod mod in ProfileManager.SelectedProfile.Mods) {
                    if (!mod.FileExists()) {
                        await ProfileManager.SelectedProfile.RemoveModFromListAsync(mod);
                    } else {
                        existingFiles.Remove($"{SettingsManager.Settings.Minecraft_Directory}/mods/{mod.FileName}");
                    }
                }
            }

            APIClient client = new APIClient();

            // TODO: Rework to open mod file, grab name/id then use search to find mod.
            // should search both services and allow user choice

            foreach (string path in existingFiles) {
                logger.Info("Trying to generate a mod from {0}", path);  

                try {
                    await Mod.GenerateFromNameAsync(parseFileName(path), client);
                    
                } catch (Exception e) {
                    return new DbusResponse {
                        Code = -1,
                        Data = e.StackTrace,
                        Message = e.Message,
                        Type = DataType.Error
                    };
                }
            }

            return new DbusResponse {
                Code = 0,
                Data = "",
                Message = "Success",
                Type = DataType.None
            };
        }

        private string parseFileName(String path) {
            string filename = path.Substring(path.LastIndexOf("/") + 1).ToLower();

            if (filename.Contains("1.")) {
                filename = filename.Substring(0, filename.IndexOf("1."));
            }

            return filename;
        }
    }
}