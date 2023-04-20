using Tmds.DBus;

using Mercurius.Profiles;
using Mercurius.API.Modrinth;
using Mercurius.Configuration;

namespace Mercurius.API {
    public abstract class Repository : IDisposable, IRepository {
        protected HttpClient _http;
        protected string _baseUrl;
        protected ObjectPath _objectPath; 
        public ObjectPath ObjectPath { get => _objectPath; }
        public abstract Remote Source { get; } 
        public Repository(string baseUrl, HttpClient client) {
            _baseUrl = baseUrl;
            _http = client;
        }
        public void Dispose() {}
        abstract public Task<Mod[]> SearchModAsync(string query, string version, string loader);

        abstract internal Task<ProjectModel> GetModProjectAsync(string id);
        abstract internal Task<Mod> GetModVersionAsync(string id);
        abstract internal Task<Mod[]> ListModVersionsAsync(string id);
        // abstract public Task</*plugin*/> GetPluginAsync(string id);
        // abstract public Task</*resource pack*/> GetResourcePackAsync(string id);
        protected internal async Task<bool> DownlodModAsync(Mod mod) {
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