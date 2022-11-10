using Mercurius.Modrinth.Models;
using Mercurius.Configuration;
using Mercurius.Modrinth;
using Mercurius.DBus;
using Tmds.DBus;
using System.Runtime.InteropServices;

namespace Mercurius.Profiles {
    public class Mod {
        public string Title { get; private set; }
        public string FileName { get; private set; }
        public string DownloadURL { get; private set; }
        public string ProjectId { get; private set; }
        public string VersionId { get; private set; }
        public string MinecraftVersion { get; private set; }
        public string ModVersion { get; private set; }
        public List<string> DependencyVersions { get; private set; }
        public ClientDependency ClientDependency { get; private set; }

        public ObjectPath ObjectPath { get => _objectPath; }
        private ObjectPath _objectPath;

        internal Mod(VersionModel version, ProjectModel project) {
            Title = project.title;
            ProjectId = version.project_id;
            VersionId = version.id;
            MinecraftVersion = version.game_versions[0];
            ModVersion = version.version_number;
            DownloadURL = version.files.Where<file>((file) => file.primary).ToArray<file>()[0].url;
            _objectPath = new ObjectPath($"/org/mercurius/mod/{Title}");
            DependencyVersions = new List<string>();

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

        internal void AddDependency(string id) {
            DependencyVersions.Add(id);
        }

        internal bool FileExists() {
            return File.Exists($"{SettingsManager.Settings.Minecraft_Directory}/mods/{FileName}");
        }
    }

    public enum ClientDependency {
        ClientSide, ServerSide, ClientServerDependent, Unknown
    }

    [DBusInterface("org.mercurius.mod")]
    public interface IMod {
        public string Title { get; }
        public string FileName { get; }
        public string DownloadURL { get; }
        public string ProjectId { get; }
        public string VersionId { get; }
        public string MinecraftVersion { get; }
        public string ModVersion { get; }
        public List<Mod> Dependencies { get; }
        public ClientDependency ClientDependency { get; }
    }
}