using Mercurius;   
using Mercurius.Profiles;
using Mercurius.Configuration;
using Tmds.DBus;

namespace Mercurius.DBus {
    [DBusInterface("org.mercurius.profile")]
    public interface IProfile : IDBusObject {
        public string Name { get; set; }
        public string MinecraftVersion { get; set; }
        public bool ServerSide { get; set; }
        public string Loader { get; set; }
        public List<Mod> Mods { get; set; }

        public string Path { get; }
    
    }
}