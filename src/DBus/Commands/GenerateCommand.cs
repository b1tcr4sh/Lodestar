using System;
using System.Threading.Tasks;
using Mercurius;
using Mercurius.Modrinth;
using Mercurius.Configuration;
using Mercurius.Profiles;
using Mercurius.Modrinth.Models;
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
            // Profile selectedProfile = await ProfileManager.GetSelectedProfileAsync();

            // if (selectedProfile is null) {
            //     // No profile selected
            //     return new DbusResponse {
            //         Code = 2,
            //         Data = "",
            //         Message = "No profile currently selected",
            //         Type = DataType.Error
            //     };
            // }

            List<string> existingFiles = Directory.GetFiles($"{SettingsManager.Settings.Minecraft_Directory}/mods/").ToList<string>();
            if (existingFiles.Count < 1) {
                return new DbusResponse {
                    Code = -1,
                    Data = "",
                    Message = "No mod files to generate from... ?"
                };
            }

            // if (selectedProfile.Mods.Count >= 1) {
            //     foreach (Mod mod in selectedProfile.Mods) {
            //         if (!mod.FileExists()) {
            //             await selectedProfile.RemoveModFromListAsync(mod);

            //             foreach (Mod dependency in mod.Dependencies) {
            //                 if (dependency.FileExists()) {
            //                     File.Delete($"{SettingsManager.Settings.Minecraft_Directory}/mods/{dependency.FileName}");
            //                 }
            //             }
            //         } else {
            //             existingFiles.Remove($"{SettingsManager.Settings.Minecraft_Directory}/mods/{mod.FileName}");
                        
            //             foreach (Mod dependency in mod.Dependencies) {
            //                 if (mod.FileExists()) {
            //                     existingFiles.Remove($"{SettingsManager.Settings.Minecraft_Directory}/mods/{dependency.FileName}");
            //                 }
            //             }   
            //         }
            //     }
            // }

            APIClient client = new APIClient();

            // TODO: Rework to open mod file, grab name/id then use search to find mod.
            // should search both services and allow user choice

            // TODO: Trim duplicate root mods from dependencies

            List<VersionModel> candidates = new List<VersionModel>();

            foreach (string path in existingFiles) {
                logger.Info("Trying to generate a mod from {0}", path);  

                string name = parseFileName(path);
                try {
                    // await Mod.GenerateFromNameAsync(parseFileName(path), client); // Deprecated

                    // I don't know what teh fuck happened here: something something trying to prevent duplicate mods by
                    // checking if any of the grabberd versions were contained in the dependencies of any version.

                    // SearchModel searchRes = await client.SearchAsync(name);

                    // foreach (Hit hit in searchRes.hits) {
                    //     if (name.Contains(hit.title.ToLower()) && hit.versions.Contains(selectedProfile.MinecraftVersion)) {
                    //         // return await ProfileManager.FetchModAsync(client, hit.project_id, Repo.modrinth, false);
                    //         ProjectModel project = await client.GetProjectAsync(hit.project_id);
                    //         VersionModel[] versions = await client.ListVersionsAsync(project);

                    //         foreach (VersionModel version in versions) {
                    //             if (version.game_versions.Contains(selectedProfile.MinecraftVersion)) {
                    //                 candidates.Add(version);
                    //             }
                    //         }
                    //     }
                    // }

                    // throw new Exception("No valid install candidates found!");   
                } catch (Exception e) {
                    return new DbusResponse {
                        Code = -1,
                        Data = e.StackTrace,
                        Message = e.Message,
                        Type = DataType.Error
                    };
                }

                // foreach (VersionModel candidate in candidates) {
                //     candidates.Where((version) => candidate.dependencies.Any((dependency) => dependency.version_id.Equals(version.id)));
                // }
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