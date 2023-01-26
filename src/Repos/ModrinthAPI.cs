using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mercurius.Configuration;
using Mercurius.Profiles;
using NLog;

namespace Mercurius.Modrinth {
    public class ModrinthAPI {
        private HttpClient client;
        private const string BaseUrl = @"https://api.modrinth.com/v2/";
        private ILogger logger;
        public ModrinthAPI() {
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

            ProjectModel deserializedRes = new ProjectModel();

            try {
                Stream responseStream = await client.GetStreamAsync(BaseUrl + $@"project/{projectId}");
                deserializedRes = await JsonSerializer.DeserializeAsync<ProjectModel>(responseStream);
            } catch (HttpRequestException e) {
                if (e.StatusCode == HttpStatusCode.NotFound) {
                    throw new ProjectInvalidException($"Project ID {projectId} is invalid!");
                } else {
                    throw new Exception($"Failed to connect...?  {e.Message}");
                }
            }
            

            return deserializedRes;
        }
        public async Task<VersionModel> GetVersionInfoAsync(string versionId) {
            if (versionId is null) {
                throw new ArgumentNullException();
            }

            logger.Debug($"Getting Project Version with ID {versionId}...");

            VersionModel deserializedRes = new VersionModel();

            try {
                Stream responseStream = await client.GetStreamAsync(BaseUrl + $@"version/{versionId}");
                deserializedRes = await JsonSerializer.DeserializeAsync<VersionModel>(responseStream);
            } catch (HttpRequestException e) {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound) {
                    throw new VersionInvalidException("Invalid version id");
                } else {
                    throw new Exception($"failed to connect: {e.StatusCode}");
                }
            }


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