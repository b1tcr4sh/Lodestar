using System;

namespace Mercurius.Configuration {
    public class Configuration {

        private PlatformID Platform = Environment.OSVersion.Platform;
        public string Minecraft_Mod_Path { get; private set; }

        public Configuration() {
            if (Platform == PlatformID.Win32NT) Minecraft_Mod_Path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/AppData/Roaming/.minecraft/mods";
                else if (Platform == PlatformID.Unix) Minecraft_Mod_Path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.Minecraft/mods";
        }
    }
}