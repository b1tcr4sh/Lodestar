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
        // public ClientDependency ClientDependency { get; set; }

        public Mod(VersionModel version, ProjectModel project, bool isDependency = false) {
            Title = project.title;
            ModrinthProjectId = version.project_id;
            VersionId = version.id;
            MinecraftVersion = version.game_versions[0];
            ModVersion = version.version_number;
            FileName = version.files[0].filename;

            if (isDependency) {
                Dependency = true;
                DependencyOf.Add(version.name);
            } else {
                Dependency = false;
                DependencyOf = null;
            }
        }
    }
    public enum ClientDependency {
        ClientSideRequired, serverSideRequired, ClientServerDependent, Unknown
    }
}