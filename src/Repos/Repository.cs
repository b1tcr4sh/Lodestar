using Mercurius.Profiles;
using Mercurius.API.Modrinth;
using Mercurius.Configuration;

namespace Mercurius.API {
    public abstract class Repository {
        internal HttpClient _http;
        internal string _base;
        public Repository(string baseUrl, HttpClient client) {
            _base = baseUrl;
            _http = client;
        }
        abstract public Task<Mod> GetModAsync(string id);
        // abstract public Task</*plugin*/> GetPluginAsync(string id);
        // abstract public Task</*resource pack*/> GetResourcePackAsync(string id);
        public async Task<bool> DownlodMod(Mod mod) {
            if (mod.DownloadURL is null) {
                throw new ArgumentNullException("Mod values are null!");
            }

                Stream readStream;
                Stream writeStream;
            try {
                readStream = await _http.GetStreamAsync(mod.DownloadURL);
                writeStream = File.Open(@$"{SettingsManager.Settings.Minecraft_Directory}/mods/{mod.FileName}", FileMode.Create);
            } catch (HttpRequestException e) {
                // logger warn
                return false;
            }

            try {
                await readStream.CopyToAsync(writeStream);
            } catch (Exception e) {
                // logger warn message
                return false;
            }
            readStream.Close();
            writeStream.Close();

            //TODO Report download progress
            //TODO Check download SHA256

            return true;
        }
        // public async Task</*plugin*/> DownloadPluginAsync(plugin) {

        // }
        // public async Task</*resourcePack8?> DownloadResourcePackAsync(pack) {

        // }
    }
}