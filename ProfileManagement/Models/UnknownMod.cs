using Mercurius.Modrinth.Models;

namespace Mercurius.Profiles {
    public class UnknownMod : Mod {
        public string HostURL { get; set; }

        public UnknownMod(VersionModel version, bool isDependency = false) : base(version, isDependency = false) {
            
        }
    }
}