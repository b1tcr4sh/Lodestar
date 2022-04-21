using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mercurius.Configuration {
    public static class SettingsManager {
        public static Configuration Settings { get; private set; }

        public static async void Init() {
            if (!File.Exists("./settings.json")) {
                await CreateConfigFile();
            } else {
                try {
                    FileStream settings = File.OpenRead("./settings.json");
                    Settings = await JsonSerializer.DeserializeAsync<Configuration>(settings);
                } catch {
                    Console.WriteLine("Error loading config file... ?");
                    Environment.Exit(0);
                }
            }
        }
        private static async Task CreateConfigFile() {
            string minecraftDirectory;
            PlatformID platform = Environment.OSVersion.Platform;
            if (platform == PlatformID.Win32NT) minecraftDirectory = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/AppData/Roaming/.minecraft/";
                else if (platform == PlatformID.Unix) minecraftDirectory = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.Minecraft/";
                else minecraftDirectory = string.Empty;

            Configuration config = new Configuration {
                Minecraft_Directory = minecraftDirectory
            };

            string contents = JsonSerializer.Serialize<Configuration>(config);

            using FileStream file = new FileStream("./settings.json", FileMode.CreateNew, FileAccess.Write);
            await file.WriteAsync(Encoding.ASCII.GetBytes(contents));
            file.Close();
            
            Settings = config;

            Console.WriteLine($"Created a new configuration file at {Environment.ProcessPath}/settings.json");
        }
    }
}