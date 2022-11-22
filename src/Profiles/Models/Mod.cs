using Mercurius.Modrinth.Models;
using Mercurius.Configuration;
using Mercurius.Modrinth;
using Mercurius.DBus;
using Tmds.DBus;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

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
        public IEnumerable<string> DependencyVersions { get; set; } = new String[0];
        public ClientDependency ClientDependency { get; set; } = ClientDependency.Unknown;

        internal Mod(VersionModel version, ProjectModel project) {
            Title = project.title;
            ProjectId = version.project_id;
            VersionId = version.id;
            MinecraftVersion = version.game_versions[0];
            ModVersion = version.version_number;
            DownloadURL = version.files.Where<modFile>((file) => file.primary).ToArray<modFile>()[0].url;
            DependencyVersions = new List<string>();
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

        internal void AddDependency(string id) {
            DependencyVersions = DependencyVersions.Append(id);
        }

        internal bool CheckFileExists() {
            return File.Exists($"{SettingsManager.Settings.Minecraft_Directory}/mods/{FileName}"); 
        }
    }

    public enum ClientDependency {
        ClientSide, ServerSide, ClientServerDependent, Unknown
    }
}