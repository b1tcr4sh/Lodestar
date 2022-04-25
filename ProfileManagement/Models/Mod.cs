namespace Mercurius.Profiles {
    public class Mod {
        public string Title { get; set; }
        public string ModrinthProjectId { get; set; }
        public string MinecraftVersion { get; set; }
        public string ModVersion { get; set; }
        public bool ServerSideSupported { get; set; }
        public bool ClientSideSupported { get; set; }
        public Mod[] Dependencies { get; set; }

    }
}