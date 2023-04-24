using System.Net;
using System.Text.Json;
using Serilog;

using Mercurius.Profiles;
using Mercurius.API.Modrinth; // just for nowsies
using Mercurius.API.Curseforge;

namespace Mercurius.API {
    public class CurseforgeAPI : Repository {
        public override Remote Source { get; } = Remote.curseforge;
        private ILogger _logger;
        protected internal CurseforgeAPI(string baseUrl, HttpClient client, ILogger logger) : base(baseUrl, client) {
            _objectPath = "/org/mercurius/curseforge";
            _logger = logger;
        }

        public override async Task<Mod[]> SearchModAsync(string query, string version, string loader) {
            throw new NotImplementedException();
        }

        internal override async Task<ProjectModel /* Temp */ > GetModProjectAsync(string projectId) {
            _logger.Debug($"Getting Project with ID {projectId}...");
            
            ProjectModel deserializedProject;

            try {
            deserializedProject = await JsonSerializer.DeserializeAsync<ProjectModel>(await _http.GetStreamAsync(_baseUrl+ @"/mods/" + projectId));
            } catch (HttpRequestException e) {
                if (e.StatusCode == HttpStatusCode.NotFound) {
                    throw new ProjectInvalidException($"Project ID {projectId} is invalid!");
                } else {
                    throw new Exception($"Failed to connect...?  {e.Message}");
                }
            }

            return deserializedProject;
        }
        internal override async Task<Mod> GetModVersionAsync(string versionId) { // Model is a bit different between the two routes :(
            throw new NotImplementedException();
        }
        internal override async Task<Mod[]> ListModVersionsAsync(string projectId) { // Model is a bit different between the two routes :(
            throw new NotImplementedException();
        }
    }
}