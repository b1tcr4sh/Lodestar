using Mercurius.Configuration;
using System.Net.Http.Headers;
using NLog;

namespace Mercurius.API {
    public static class APIManager {
        public static ModrinthAPI Modrinth { get; private set; }
        public static CurseforgeAPI Curseforge { get {
            if (CurseforgeAuthAvailable()) {
                return Curseforge;
            }
                throw new ConfigurationException("No Curseforge API key is set!");
            
        } private set {} }

        public static void Init() {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("x-api-key", SettingsManager.Settings.Cureforge_Api_Key);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("user-agent", "Mercurius");

            Modrinth = new ModrinthAPI(client);

            if (!CurseforgeAuthAvailable()) {
                LogManager.GetCurrentClassLogger().Warn("Curseforge API is not set! Curseforge requests will not authenticate!");
                return;
            } 

            Curseforge = new CurseforgeAPI(client);
        }
        private static bool CurseforgeAuthAvailable() {
            if (SettingsManager.Settings.Cureforge_Api_Key.Equals(string.Empty) || SettingsManager.Settings.Cureforge_Api_Key is null) {
                return false;
            } else {
                return true;
            }
        }
    }
}