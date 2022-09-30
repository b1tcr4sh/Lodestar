using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mercurius;

namespace Mercurius.Configuration {
    public static class SettingsManager {
        public static SettingsFile Settings { get; private set; }
        private static string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        private static string programDirectory = $"{userPath}/.mercurius";

        public static async void Init() {
            if (!Directory.Exists(programDirectory)) {
                MCSLogger.logger.Debug("Creating program folder at {0}", userPath);
                Directory.CreateDirectory(programDirectory);
            }

            if (!File.Exists($"{programDirectory}/settings.json")) {
                await CreateConfigFile();
            } else {
                try {
                    using FileStream settings = File.OpenRead($"{programDirectory}/settings.json");
                    Settings = JsonSerializer.Deserialize<SettingsFile>(settings);
                    Console.WriteLine(Settings.Minecraft_Directory);
                    MCSLogger.logger.Debug("Loaded configuration at {0}/{1}", programDirectory, "settings.json");
                } catch (JsonException e) {
                    MCSLogger.logger.Fatal("Error loading config file... ?");
                    MCSLogger.logger.Trace(e.Message);
                    Environment.Exit(1);
                }
            }
        }
        private static async Task CreateConfigFile() {
            string minecraftDirectory;

            PlatformID platform = Environment.OSVersion.Platform;
            if (platform == PlatformID.Win32NT) minecraftDirectory = $@"{userPath}/AppData/Roaming/.minecraft/";
                else if (platform == PlatformID.Unix) minecraftDirectory = $@"{userPath}/.minecraft/";
                else minecraftDirectory = string.Empty;

            SettingsFile config = new SettingsFile {
                Minecraft_Directory = minecraftDirectory,
                Profile_Directory = $"{programDirectory}/Profiles/",
                Server_Mod_Directory = string.Empty
            };

            string contents = JsonSerializer.Serialize<SettingsFile>(config, new JsonSerializerOptions { WriteIndented = true });

            using FileStream file = new FileStream($"{programDirectory}/settings.json", FileMode.CreateNew, FileAccess.Write);
            await file.WriteAsync(Encoding.ASCII.GetBytes(contents));
            file.Close();
            
            Settings = config;

            MCSLogger.logger.Info($"Created a new configuration file at {programDirectory}/settings.json");
        }
    }
}