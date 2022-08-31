using Mercurius.Modrinth.Models;

namespace Mercurius.Profiles {
    public class UnknownMod : Mod {
        public string HostURL { get; set; }

        public UnknownMod(VersionModel version, ProjectModel project) : base(version, project) {
            
        }
    }
}