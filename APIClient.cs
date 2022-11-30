using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mercurius.Modrinth.Models;
using Mercurius.Configuration;
using Mercurius.Profiles;
using NLog;

namespace Mercurius.Modrinth {
    public class APIClient {
        private HttpClient client;
        private const string BaseUrl = @"https://api.modrinth.com/v2/";
        private ILogger logger;
        public APIClient() {
            logger = LogManager.GetCurrentClassLogger();

            client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("user-agent", "Mercurius");
        }
        public async Task<SearchModel> SearchAsync(string query) {
            logger.Debug($"Querying Labrynth with {query}...");

            SearchModel deserializedRes;

            try {
                Stream responseStream = await client.GetStreamAsync(BaseUrl + $@"search?query={query}");
                deserializedRes = await JsonSerializer.DeserializeAsync<SearchModel>(responseStream);
            } catch (Exception e) {
                logger.Warn(e.Message);
                logger.Trace(e.StackTrace);
                throw new ApiException("Connection Failed!");
            }

            if (deserializedRes.hits.Length <= 0) {
                logger.Debug("No results found... Sorry");
                throw new ApiException("No results found");
            }

            return deserializedRes;
        }
        public async Task<ProjectModel> GetProjectAsync(string projectId) {
            logger.Debug($"Getting Project with ID {projectId}...");

            ProjectModel deserializedRes;

            Stream responseStream = await client.GetStreamAsync(BaseUrl + $@"project/{projectId}");
            deserializedRes = await JsonSerializer.DeserializeAsync<ProjectModel>(responseStream);
            

            return deserializedRes;
        }
        public async Task<VersionModel> GetVersionInfoAsync(string versionId) {
            logger.Debug($"Getting Project Version with ID {versionId}...");

            VersionModel deserializedRes;

            Stream responseStream = await client.GetStreamAsync(BaseUrl + $@"version/{versionId}");
            deserializedRes = await JsonSerializer.DeserializeAsync<VersionModel>(responseStream);


            return deserializedRes;
        }
        public async Task<VersionModel[]> ListVersionsAsync(ProjectModel project) {
            logger.Debug($"Getting List of Versions for {project.title}...");

            VersionModel[] deserializedRes;

            Stream responseStream = await client.GetStreamAsync(BaseUrl + $@"project/{project.id}/version");
            deserializedRes = await JsonSerializer.DeserializeAsync<VersionModel[]>(responseStream);
           
            return deserializedRes;
        }
        public async Task DownloadVersionAsync(Mod mod) {
            if (mod.Title is null) {
                throw new ArgumentNullException("Mod values are null!");
            }

            logger.Debug($"Starting Download of {mod.Title}: version {mod.ModVersion}");

            HttpResponseMessage response;

            response = await client.GetAsync(mod.DownloadURL, HttpCompletionOption.ResponseContentRead);

            using Stream readStream = await response.Content.ReadAsStreamAsync();
            using Stream writeStream = File.Open(@$"{SettingsManager.Settings.Minecraft_Directory}/mods/{mod.FileName}", FileMode.Create);

            await readStream.CopyToAsync(writeStream);
            readStream.Close();
            writeStream.Close();

            logger.Debug($"Completed Download of {mod.Title}: version {mod.ModVersion}");

            //TODO Report download progress
            //TODO Check download SHA256
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
}