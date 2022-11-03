using Mercurius.Modrinth.Models;
using Mercurius.Configuration;
using Mercurius.Modrinth;
using Mercurius.DBus;
using Tmds.DBus;

namespace Mercurius.Profiles {
    public struct Mod {
        public string Title { get; private set; }
        public string FileName { get; private set; }
        public string DownloadURL { get; private set; }
        public string ProjectId { get; private set; }
        public string VersionId { get; private set; }
        public string MinecraftVersion { get; private set; }
        public string ModVersion { get; private set; }
        public List<Mod> Dependencies { get; private set; }
        public ClientDependency ClientDependency { get; private set; }

        public ObjectPath ObjectPath { get => _objectPath; }
        private ObjectPath _objectPath;

        internal Mod(VersionModel version, ProjectModel project) : this() {
            Title = project.title;
            ProjectId = version.project_id;
            VersionId = version.id;
            MinecraftVersion = version.game_versions[0];
            ModVersion = version.version_number;
            DownloadURL = version.files.Where<file>((file) => file.primary).ToArray<file>()[0].url;
            _objectPath = new ObjectPath($"/org/mercurius/mod/{Title}");
            Dependencies = new List<Mod>();

            file primaryFile = version.files[0];
            foreach (file file in version.files) {
                if (file.primary) primaryFile = file;
            }
            FileName = primaryFile.filename;


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

        internal void AddDependency(Mod dependency) {
            // Mod dependency = new Mod(version, project);

            Dependencies.Add(dependency);
        }

        internal bool FileExists() {
            return File.Exists($"{SettingsManager.Settings.Minecraft_Directory}/mods/{FileName}");
        }

        internal static async Task<Mod> GenerateFromNameAsync(string name, APIClient client) {
            Profile selectedProfile = await ProfileManager.GetSelectedProfileAsync();

            SearchModel searchRes = await client.SearchAsync(name);

            List<Hit> candidates = new List<Hit>();

            foreach (Hit hit in searchRes.hits) {
                if (name.Contains(hit.title.ToLower()) && hit.versions.Contains(selectedProfile.MinecraftVersion)) {
                    return await ProfileManager.FetchModAsync(client, hit.project_id, Repo.modrinth, false);
                }
            }
            throw new Exception("No valid install candidates found!");
        }
    }

    public enum ClientDependency {
        ClientSide, ServerSide, ClientServerDependent, Unknown
    }
}