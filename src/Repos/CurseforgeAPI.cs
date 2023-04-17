using System.Threading.Tasks;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mercurius.Configuration;
using Mercurius.API.Curseforge;
using Mercurius.Profiles;
using NLog;

namespace Mercurius.API {
    public class CurseforgeAPI : Repository { 
        private ILogger _logger;
        protected internal CurseforgeAPI(string baseUrl, HttpClient client) : base(baseUrl, client) {
            _objectPath = "/org/mercurius/curseforge";
        }

        protected internal async Task<Project> GetProjectAsync(string projectId) {
            _logger.Debug($"Getting Project with ID {projectId}...");
            
            Project deserializedProject;

            try {
            deserializedProject = await JsonSerializer.DeserializeAsync<Project>(await _http.GetStreamAsync(_base+ @"/mods/" + projectId));
            } catch (HttpRequestException e) {
                if (e.StatusCode == HttpStatusCode.NotFound) {
                    throw new ProjectInvalidException($"Project ID {projectId} is invalid!");
                } else {
                    throw new Exception($"Failed to connect...?  {e.Message}");
                }
            }

            return deserializedProject;
        }
        protected internal async Task<Curseforge.File[]> ListVersionsAsync(string projectId) { // Model is a bit different between the two routes :(
            _logger.Debug($"Getting List of Versions for {projectId}...");

            throw new NotImplementedException();
        }
        protected internal override async Task<Mod> GetModAsync(string versionId) { // Model is a bit different between the two routes :(
            throw new NotImplementedException();
        }
    }
}