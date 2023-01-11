namespace Mercurius.Configuration {
    public class SettingsFile {

        public string Minecraft_Directory { get; set; }
        public string Profile_Directory { get; set; }
        public string Server_Mod_Directory { get; set; }
        public bool Mod_Caching { get; set; }
        public bool Dbus_System_Bus { get; set; }
    }
}