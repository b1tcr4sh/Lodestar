using Mercurius.Modrinth.Models;
using Mercurius.Configuration;
using Mercurius.Modrinth;

namespace Mercurius.Profiles {
    public class Mod {
        public string Title { get; set; }
        public string FileName { get; set; }
        public string DownloadURL { get; set; }
        public string ProjectId { get; set; }
        public string VersionId { get; set; }
        public string MinecraftVersion { get; set; }
        public string ModVersion { get; set; }
        public List<Mod> Dependencies { get; set; }
        public ClientDependency ClientDependency { get; set; }

        public Mod() {}
        internal Mod(VersionModel version, ProjectModel project) {
            Title = project.title;
            ProjectId = version.project_id;
            VersionId = version.id;
            MinecraftVersion = version.game_versions[0];
            ModVersion = version.version_number;
            DownloadURL = version.files.Where<file>((file) => file.primary).ToArray<file>()[0].url;

            file primaryFile = version.files[0];
            foreach (file file in version.files) {
                if (file.primary) primaryFile = file;
            }
            FileName = primaryFile.filename;
            Dependencies = new List<Mod>();


            string serverSideDependency = project.server_side;
            string clientSideDependence = project.client_side;

            switch (clientSideDependence) {
                case "required":
                    if (serverSideDependency.Equals("required") || serverSideDependency.Equals("optional"))
                        ClientDependency = ClientDependency.ClientServerDependent;
                    else if (serverSideDependency.Equals("unsupported"))
                        ClientDependency = ClientDependency.ClientSide;
                    break;
                case "optional":
                    if (serverSideDependency.Equals("required"))
                        ClientDependency = ClientDependency.ClientServerDependent;
                    else if (serverSideDependency.Equals("optinoal") || serverSideDependency.Equals("unsupported"))
                        ClientDependency = ClientDependency.ClientSide;
                    break;
                case "unsupported":
                    if (serverSideDependency.Equals("required") || serverSideDependency.Equals("optional"))
                        ClientDependency = ClientDependency.ServerSide;
                    break;
                default:
                    ClientDependency = ClientDependency.Unknown;
                    break;
            }
        }

        public void AddDependency(Mod dependency) {
            // Mod dependency = new Mod(version, project);

            Dependencies.Add(dependency);
        }

        public bool FileExists() {
            return File.Exists($"{SettingsManager.Settings.Minecraft_Directory}/mods/{FileName}");
        }

        public static async Task<Mod> GenerateFromNameAsync(string name, APIClient client) {
            SearchModel searchRes = await client.SearchAsync(name);

            List<Hit> candidates = new List<Hit>();

            foreach (Hit hit in searchRes.hits) {
                if (name.Contains(hit.title.ToLower()) && hit.versions.Contains(ProfileManager.SelectedProfile.MinecraftVersion)) {
                    return await ProfileManager.AddModAsync(client, hit.project_id, Repo.modrinth, false);
                }
            }
            throw new Exception("No valid install candidates found!");
        }
    }

    public enum ClientDependency {
        ClientSide, ServerSide, ClientServerDependent, Unknown
    }
}