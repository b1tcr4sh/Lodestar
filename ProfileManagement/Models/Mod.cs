using Mercurius.Modrinth.Models;

namespace Mercurius.Profiles {
    public class Mod {
        public string Title { get; set; }
        public string FileName { get; set; }
        public string ModrinthProjectId { get; set; }
        public string VersionId { get; set; }
        public string MinecraftVersion { get; set; }
        public string ModVersion { get; set; }
        public bool Dependency { get; set; }
        public List<string> DependencyOf { get; set; }
        public ClientDependency ClientDependency { get; set; }

        public Mod(VersionModel version, ProjectModel project, bool isDependency = false) {
            Title = project.title;
            ModrinthProjectId = version.project_id;
            VersionId = version.id;
            MinecraftVersion = version.game_versions[0];
            ModVersion = version.version_number;

            file primaryFile = version.files[0];
            foreach (file file in version.files) {
                if (file.primary) primaryFile = file;
            }
            FileName = primaryFile.filename;

            if (isDependency) {
                Dependency = true;
                DependencyOf.Add(version.name);
            } else {
                Dependency = false;
                DependencyOf = null;
            }

            string serverSideDependency = project.server_side;
            string clientSideDependence = project.client_side;

            switch (clientSideDependence) {
                case "required":
                    if (clientSideDependence.Equals("required"))
                        ClientDependency = ClientDependency.ClientServerDependent;
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
    }
    public enum ClientDependency {
        ClientSide, ServerSide, ClientServerDependent, Unknown
    }
}