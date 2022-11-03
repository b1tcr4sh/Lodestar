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

    [DBusInterface("org.mercurius.mod")]
    public interface IMod : IDBusObject {
        public string Title { get; set; }
        public string FileName { get; set; }
        public string DownloadURL { get; set; }
        public string ProjectId { get; set; }
        public string VersionId { get; set; }
        public string MinecraftVersion { get; set; }
        public string ModVersion { get; set; }
        public List<Mod> Dependencies { get; set; }
        public ClientDependency ClientDependency { get; set; }
    }
}