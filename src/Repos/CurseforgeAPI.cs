using System.Net;
using System.Text.Json;
using Serilog;

using Mercurius.Profiles;
using Mercurius.API.Modrinth; // just for nowsies
using Mercurius.API.Curseforge;
using Mercurius.Configuration;

namespace Mercurius.API {
    public class CurseforgeAPI : Repository {
        public override Remote Source { get; } = Remote.curseforge;
        protected internal CurseforgeAPI(string baseUrl, HttpClient client, ILogger logger) : base(baseUrl, client, logger) {
            _objectPath = "/org/mercurius/curseforge";
            _http.DefaultRequestHeaders.Add("x-api-key", SettingsManager.Settings.Cureforge_Api_Key);
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

        private async Task<Project> GetProjectAsync(string id) {
            HttpResponseMessage res;
            try {
                res = await _http.GetAsync(_baseUrl + $"mods/{id}");
            } catch (HttpRequestException e) {
                throw new Exception(e.Message);
                // Map to result
            }
            return await JsonSerializer.DeserializeAsync<Project>((await res.Content.ReadAsStreamAsync()));
        }  
        private async Task<CurseforgeVersion> GetVersionAsync(string versionId, string projectId) {
            HttpResponseMessage res;
            try {
                res = await _http.GetAsync(_baseUrl + $"mods/{projectId}/files/{versionId}");
            } catch (HttpRequestException e) {
                throw new Exception(e.Message);
                // Map to result
            }
            return await JsonSerializer.DeserializeAsync<CurseforgeVersion>((await res.Content.ReadAsStreamAsync()));
        }
        private async Task<CurseforgeVersionList> GetVersionListAsync(string id) {
            HttpResponseMessage res;
            try {
                res = await _http.GetAsync(_baseUrl + $"mods/{id}/files");
            } catch (HttpRequestException e) {
                throw new Exception(e.Message);
                // Map to result
            }
            return await JsonSerializer.DeserializeAsync<CurseforgeVersionList>((await res.Content.ReadAsStreamAsync()));
        }
        private Mod ModFromVersion(CurseforgeVersion version) {
            List<string> dependencies = new List<string>();
            foreach (FileDependency dep in version.data.dependencies) {
                dependencies.Add(dep.modId.ToString());
            }
            
            Mod mod = new Mod() {
                Title = version.data.displayName,
                FileName = version.data.fileName,
                DownloadURL = version.data.downloadUrl,
                Repo = Remote.curseforge,
                ProjectId = version.data.modId.ToString(),
                VersionId = version.data.id.ToString(),
                DependencyVersions = dependencies,

            };

            return mod; // Mod doesn't seem to include a loader ??
        }
    }
    [System.Serializable]
    public class CurseforgeException : System.Exception
    {
        public CurseforgeException() { }
        public CurseforgeException(string message) : base(message) { }
        public CurseforgeException(string message, System.Exception inner) : base(message, inner) { }
        protected CurseforgeException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}