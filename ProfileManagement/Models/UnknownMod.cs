using Mercurius.Modrinth.Models;

namespace Mercurius.Profiles {
    public class UnknownMod : Mod {
        public string HostURL { get; set; }

        public UnknownMod(VersionModel version, ProjectModel project, bool isDependency = false) : base(version, project, isDependency = false) {
            
        }
    }
}