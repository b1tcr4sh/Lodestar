using Tmds.DBus;
using Serilog;

using Mercurius.Profiles;
using Mercurius.Configuration;

namespace Mercurius.API {
    public abstract class Repository : IDisposable, IRepository {
        protected HttpClient _http;
        protected string _baseUrl;
        protected ObjectPath _objectPath; 
        public ObjectPath ObjectPath { get => _objectPath; }
        public abstract Remote Source { get; } 
        protected ILogger _logger { get; set; }
        public Repository(string baseUrl, HttpClient client, ILogger logger) {
            _baseUrl = baseUrl;
            _http = client;
            _logger = logger;
        }
        public void Dispose() {}
        abstract public Task<Project[]> SearchModAsync(string query, string version, string loader);

        abstract internal Task<Project> GetModProjectAsync(string id);
        abstract internal Task<Mod> GetModVersionAsync(string id);
        abstract internal Task<Mod[]> ListModVersionsAsync(string id);
        // abstract public Task</*plugin*/> GetPluginVersionAsync(string id);
        // abstract public Task</*plugin*/> GetPluginProjectAsync(string id);
        // abstract public Task</*resource pack*/> GetResourcePackAsync(string id);
        protected internal async Task<bool> DownlodModAsync(Mod mod) {
            if (mod.DownloadURL is null) {
                throw new ArgumentNullException("Mod values are null!");
            }

            bool success = await DownloadAsync(mod.DownloadURL, @$"{SettingsManager.Settings.Minecraft_Directory}/mods/", mod.FileName);
            // TODO VerifyHash(mod.hash or whatever);

            return success;
        }
        // abstract protected bool VerifyHash(string hash);
        // public async Task</*plugin*/> DownloadPluginAsync(plugin) {

        // }
        // public async Task</*resourcePack8?> DownloadResourcePackAsync(pack) {

        // }
        private async Task<bool> DownloadAsync(string url, string basePath, string filename) {
            Stream readStream;
            Stream writeStream;
            try {
                readStream = await _http.GetStreamAsync(url);
                writeStream = File.Open(@$"{basePath}{filename}", FileMode.Create);
            } catch (HttpRequestException e) {
                _logger.Warning("Download failed: " + e.StatusCode);
                return false;
            }

            try {
                await readStream.CopyToAsync(writeStream);
            } catch (Exception e) {
                _logger.Warning(e.Message);
                return false;
            }
            readStream.Close();
            writeStream.Close();

            //TODO Report download progress
            return true;
        }
    }
    public class ApiException : Exception {
        public ApiException() { }
        public ApiException(string message) : base(message) { }
        public ApiException(string message, System.Exception inner) : base(message, inner) { }
        protected ApiException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    [System.Serializable]
    public class VersionInvalidException : System.Exception
    {
        public VersionInvalidException() { }
        public VersionInvalidException(string message) : base(message) { }
        public VersionInvalidException(string message, System.Exception inner) : base(message, inner) { }
        protected VersionInvalidException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [System.Serializable]
    public class ProjectInvalidException : System.Exception
    {
        public ProjectInvalidException() { }
        public ProjectInvalidException(string message) : base(message) { }
        public ProjectInvalidException(string message, System.Exception inner) : base(message, inner) { }
        protected ProjectInvalidException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}