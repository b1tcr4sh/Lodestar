namespace Mercurius.Profiles {
    public class Profile {
        // Make a reference to it's respective json file, with methods to update, delete, etc.

        public string Name { get; set; }
        public string MinecraftVersion { get; set; }
        public ClientType ClientType { get; set; }
        public bool ContainsUnknownMods = false;
        public Mod[] Mods { get; set; }
        public UnknownMod[] UnknownMods = null;
    }
    public enum ClientType {
        ClientSide, ServerSide
    }
}