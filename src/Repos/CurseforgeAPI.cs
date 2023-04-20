using System.Net;
using System.Text.Json;
using NLog;

using Mercurius.Profiles;
using Mercurius.API.Modrinth; // TEMPEPPEPEP AHHAHA

namespace Mercurius.API {
    public class CurseforgeAPI : Repository {
        public override Remote Source { get; } = Remote.curseforge;
        private ILogger _logger;
        protected internal CurseforgeAPI(string baseUrl, HttpClient client) : base(baseUrl, client) {
            _objectPath = "/org/mercurius/curseforge";
        }

        public override async Task<ProjectModel /* Temp */ > GetModProjectAsync(string projectId) {
            _logger.Debug($"Getting Project with ID {projectId}...");
            
            ProjectModel deserializedProject;

            try {
            deserializedProject = await JsonSerializer.DeserializeAsync<ProjectModel>(await _http.GetStreamAsync(_base+ @"/mods/" + projectId));
            } catch (HttpRequestException e) {
                if (e.StatusCode == HttpStatusCode.NotFound) {
                    throw new ProjectInvalidException($"Project ID {projectId} is invalid!");
                } else {
                    throw new Exception($"Failed to connect...?  {e.Message}");
                }
            }

            return deserializedProject;
        }
        public override async Task<Mod> GetModVersionAsync(string versionId) { // Model is a bit different between the two routes :(
            throw new NotImplementedException();
        }
        protected internal async Task<Curseforge.File[]> ListVersionsAsync(string projectId) { // Model is a bit different between the two routes :(
            _logger.Debug($"Getting List of Versions for {projectId}...");

            throw new NotImplementedException();
        }
    }
}