using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mercurius.Configuration;
using Mercurius.Profiles;
using Mercurius.API.Modrinth;
using NLog;

namespace Mercurius.API {
    public class ModrinthAPI : Repository {
        public override Remote Source { get; } = Remote.modrinth;
        private ILogger _logger;
        protected internal ModrinthAPI(string baseUrl, HttpClient client) : base(baseUrl, client) {
            _objectPath = "/org/mercurius/modrinth";
        }
        protected internal async Task<SearchModel> SearchAsync(string query) {
            _logger.Debug($"Querying Labrynth with {query}...");

            SearchModel deserializedRes;

            try {
                Stream responseStream = await _http.GetStreamAsync(_base + $@"search?query={query}");
                deserializedRes = await JsonSerializer.DeserializeAsync<SearchModel>(responseStream);
            } catch (Exception e) {
                _logger.Warn(e.Message);
                _logger.Trace(e.StackTrace);
                throw new ApiException("Connection Failed!");
            }

            if (deserializedRes.hits.Length <= 0) {
                _logger.Debug("No results found... Sorry");
                throw new ApiException("No results found");
            }

            return deserializedRes;
        }
        public override async Task<ProjectModel> GetModProjectAsync(string projectId) {
            _logger.Debug($"Getting Project with ID {projectId}...");

            ProjectModel deserializedRes = new ProjectModel();

            try {
                Stream responseStream = await _http.GetStreamAsync(_base + $@"project/{projectId}");
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
        public override async Task<Mod> GetModVersionAsync(string versionId) {
            if (versionId is null) {
                throw new ArgumentNullException();
            }

            _logger.Debug($"Getting Project Version with ID {versionId}...");

            VersionModel version;
            ProjectModel project;

            try {
                Stream versionRes = await _http.GetStreamAsync(_base + $@"version/{versionId}");
                version = await JsonSerializer.DeserializeAsync<VersionModel>(versionRes);
                Stream projectRes = await _http.GetStreamAsync(_base + $@"project/{version.project_id}");
                project = await JsonSerializer.DeserializeAsync<ProjectModel>(projectRes);
            } catch (HttpRequestException e) {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound) {
                    throw new VersionInvalidException("Invalid version id");
                } else {
                    throw new Exception($"failed to connect: {e.StatusCode}");
                }
            }

            return new Mod(version, project);
        }
        protected internal async Task<VersionModel[]> ListVersionsAsync(string id) {
            VersionModel[] deserializedRes;

            Stream responseStream = await _http.GetStreamAsync(_base + $@"project/{id}/version");
            deserializedRes = await JsonSerializer.DeserializeAsync<VersionModel[]>(responseStream);
           
            return deserializedRes;
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