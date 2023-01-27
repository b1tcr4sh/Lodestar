namespace Mercurius.Configuration {
    public class SettingsFile {

        public string Minecraft_Directory { get; set; }
        public string Profile_Directory { get; set; }
        public string Server_Mod_Directory { get; set; }
        public bool Mod_Caching { get; set; }
        public bool Dbus_System_Bus { get; set; }
        public string Cureforge_Api_Key { get; set; }
    }
    [System.Serializable]
    public class ConfigurationException : System.Exception
    {
        public ConfigurationException() { }
        public ConfigurationException(string message) : base(message) { }
        public ConfigurationException(string message, System.Exception inner) : base(message, inner) { }
        protected ConfigurationException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}