using System;
using System.Threading.Tasks;
using Mercurius;
using Mercurius.Modrinth;
using Mercurius.Configuration;
using Mercurius.Profiles;
using Tmds.DBus;
using NLog;

namespace Mercurius.DBus.Commands {

    public class SyncCommand : BaseCommand {
        public override string Name { get => "Sync"; }
        public override string Description { get => "Syncronises local mods with profile."; }
        public override string Format { get => "[none]"; }
        public override bool TakesArgs { get => false; }
        public override ObjectPath ObjectPath { get => _objectPath; }
        private ObjectPath _objectPath = new ObjectPath("/org/mercurius/command/sync");
        private ILogger logger;

        public SyncCommand(ILogger _logger) : base(_logger) {
            logger = _logger;
        }

        private APIClient client = new APIClient();
        private List<Mod> installQueue = new List<Mod>();
        private Profile selectedProfile;
        public override async Task<DbusResponse> ExecuteAsync(string[] args) {
            selectedProfile = await ProfileManager.GetSelectedProfileAsync();

            if (selectedProfile is null) {
                Console.WriteLine("No Profile is Selected... ? (Create or Select One)");
                return new DbusResponse {
                    Code = 2,
                    Data = "",
                    Message = "No profile currently selected",
                    Type = DataType.Error
                };
            }

            await SyncModsFiles();

            // Super not error handled like at all at all
            return new DbusResponse {
                Code = 0,
                Data = "",
                Message = "Success",
                Type = DataType.None
            };
        }

        private async Task SyncModsFiles() {
            List<string> existingFiles = Directory.GetFiles($"{SettingsManager.Settings.Minecraft_Directory}/mods/").ToList<string>();
            List<string> modPaths = new List<string>();

            // if (existingFiles.Count <= 0) {
            //     // No mods to sync
            //     return;
            // }

            foreach (Mod mod in selectedProfile.Mods) {
                modPaths.Add($"{SettingsManager.Settings.Minecraft_Directory}/mods/{mod.FileName}");

                foreach (Mod dependency in mod.Dependencies) {
                    modPaths.Add($"{SettingsManager.Settings.Minecraft_Directory}/mods/{dependency.FileName}");
                }
            }
            
            

            List<string> keepers = existingFiles.Intersect<string>(modPaths).ToList<string>();

            foreach (string mod in keepers) {
                existingFiles.Remove(mod);
            }

            if (existingFiles.Count <= 0) {
                Console.WriteLine("There are no Residiual Mod jars to Remove");
            } else {
                Console.WriteLine("Removing Residual Mod jars...");
                foreach (string file in existingFiles)
                    File.Delete(file);
            }

            if (selectedProfile.Mods.Count <= 0) {
                Console.WriteLine("There is nothing to do...");
                return;
            }
            List<Mod> preQueue = new List<Mod>();
            preQueue.AddRange(selectedProfile.Mods);
            foreach (Mod mod in selectedProfile.Mods) {
                preQueue.AddRange(mod.Dependencies);
            }
            

            // Queue mods for install
            foreach (Mod mod in preQueue) {
                if (File.Exists($"{SettingsManager.Settings.Minecraft_Directory}/mods/{mod.FileName}")) {
                    Console.Write("{0}: {1} is already installed, reinstall? (y/N) > ", mod.Title, mod.ModVersion);

                    if (Console.ReadLine().ToLower().Equals("y")) {
                        installQueue.Add(mod);
                    }
                        
                } else
                    installQueue.Add(mod);
            }
                await Install();

                //TODO Resolve dependencies for mods in profile
        }
        private async Task<bool> Install() {
            if (installQueue.Count < 1) {
                Console.WriteLine("There is nothing to do...");
                return false;
            }

            Console.WriteLine("Mods Queued for Install: ");
            foreach (Mod modToInstall in installQueue) {
                Console.WriteLine("- {0}", modToInstall.Title);
            }

            Console.Write("\nContinue with Operation? (Y/n) > ");

            if (Console.ReadLine().ToLower().Equals("n")) {
                Console.WriteLine("Aborting...");
                return false;
            }
            
            foreach (Mod mod in installQueue) {
                await client.DownloadVersionAsync(mod);
            }
            return true;
        }
    }
}