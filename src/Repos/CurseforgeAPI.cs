using System.Threading.Tasks;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mercurius.Configuration;
using Mercurius.API.Curseforge;
using NLog;

namespace Mercurius.API {
    public class CurseforgeAPI {
        private HttpClient _client;
        private const string BaseUrl = @"https://api.curseforge.com/v1/";
        private ILogger logger;
        internal CurseforgeAPI(HttpClient client) {
            logger = LogManager.GetCurrentClassLogger();

            _client = client;
        }

        public async Task<Project> GetProjectAsync(string projectId) {
            logger.Debug($"Getting Project with ID {projectId}...");
            
            Project deserializedProject;

            try {
            deserializedProject = await JsonSerializer.DeserializeAsync<Project>(await _client.GetStreamAsync(BaseUrl+ @"/mods/" + projectId));
            } catch (HttpRequestException e) {
                if (e.StatusCode == HttpStatusCode.NotFound) {
                    throw new ProjectInvalidException($"Project ID {projectId} is invalid!");
                } else {
                    throw new Exception($"Failed to connect...?  {e.Message}");
                }
            }

            return deserializedProject;
        }
        public async Task<Curseforge.File[]> ListVersionsAsync(string projectId) { // Model is a bit different between the two routes :(
            logger.Debug($"Getting List of Versions for {projectId}...");

            throw new NotImplementedException();
        }
        public async Task <Curseforge.File> GetVersionAsync(string versionId) { // Model is a bit different between the two routes :(
            throw new NotImplementedException();
        }
    }
}