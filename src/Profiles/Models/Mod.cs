using Mercurius.Configuration;
using System.Runtime.InteropServices;

using Mercurius.API.Modrinth;

namespace Mercurius.Profiles {
    [StructLayout(LayoutKind.Sequential)]
    public struct Mod {
        public string Title { get; set; } = String.Empty;
        public string FileName { get; set; } = String.Empty;
        public string DownloadURL { get; set; } = String.Empty;
        public string ProjectId { get; set; } = String.Empty;
        public string VersionId { get; set; } = String.Empty;
        public string MinecraftVersion { get; set; } = String.Empty;
        public string ModVersion { get; set; } = String.Empty;
        public Remote Repo { get; set; } = Remote.custom;
        public ModLoader[] Loaders { get; set; } = new ModLoader[] { ModLoader.unknown };
        public Dictionary<string, Remote> DependencyVersions { get; set; } = new Dictionary<string, Remote>();
        public ClientDependency ClientDependency { get; set; } = ClientDependency.Unknown;

        internal Mod(VersionModel version, ProjectModel project) {
            Title = project.title;
            ProjectId = version.project_id;
            VersionId = version.id;
            MinecraftVersion = version.game_versions[0];
            ModVersion = version.version_number;
            DownloadURL = version.files.Where<modFile>((file) => file.primary).ToArray<modFile>()[0].url;
            DependencyVersions = new Dictionary<string, Remote>();
            ClientDependency = ClientDependency.Unknown;

            modFile primaryFile = version.files[0];
            foreach (modFile file in version.files) {
                if (file.primary) primaryFile = file;
            }
            FileName = primaryFile.filename;


            string serverSideDependency = project.server_side;
            string clientSideDependence = project.client_side;

            switch (clientSideDependence) {
                case "required":
                    if (serverSideDependency.Equals("required") || serverSideDependency.Equals("optional"))
                        ClientDependency = ClientDependency.Mutual;
                    else if (serverSideDependency.Equals("unsupported"))
                        ClientDependency = ClientDependency.Client;
                    break;
                case "optional":
                    if (serverSideDependency.Equals("required"))
                        ClientDependency = ClientDependency.Mutual;
                    else if (serverSideDependency.Equals("optinoal") || serverSideDependency.Equals("unsupported"))
                        ClientDependency = ClientDependency.Client;
                    break;
                case "unsupported":
                    if (serverSideDependency.Equals("required") || serverSideDependency.Equals("optional"))
                        ClientDependency = ClientDependency.Server;
                    break;
                default:
                    ClientDependency = ClientDependency.Unknown;
                    break;
            }

            List<ModLoader> loaders = new List<ModLoader>();

            foreach (string loader in version.loaders) {
                ModLoader parsed;

                if (!Enum.TryParse<ModLoader>(loader, out parsed)) {
                    throw new ProfileException("Invalid mod loader given... ?");
                }

                loaders.Add(parsed);
            }  
            Loaders = loaders.ToArray<ModLoader>();
        }

        internal void AddDependency(string id) {
            DependencyVersions.Add(id, Repo);
        }

        internal bool CheckFileExists() {
            return File.Exists($"{SettingsManager.Settings.Minecraft_Directory}/mods/{FileName}"); 
        }
    }
}