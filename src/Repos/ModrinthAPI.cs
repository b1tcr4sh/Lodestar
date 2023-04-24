using System.Net;
using System.Text.Json;
using NLog;

using Mercurius.Profiles;
using Mercurius.API.Modrinth;

namespace Mercurius.API {
    public class ModrinthAPI : Repository {
        public override Remote Source { get; } = Remote.modrinth;
        private ILogger _logger;
        protected internal ModrinthAPI(string baseUrl, HttpClient client) : base(baseUrl, client) {
            _objectPath = "/org/mercurius/modrinth";
        }

        public override async Task<Mod[] /* Unified search result model array */> SearchModAsync(string query, string version, string loader) {
            throw new NotImplementedException();
            _logger.Debug($"Querying Labrynth with {query}...");

            SearchModel deserializedRes;

            try {
                Stream responseStream = await _http.GetStreamAsync(_baseUrl + $@"search?query={query}");
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

            // return deserializedRes;
        }
        internal override async Task<ProjectModel> GetModProjectAsync(string projectId) {
            throw new NotImplementedException(); // Need to create some kind of unified project model
            _logger.Debug($"Getting Project with ID {projectId}...");

            ProjectModel deserializedRes = new ProjectModel();

            try {
                Stream responseStream = await _http.GetStreamAsync(_baseUrl + $@"project/{projectId}");
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
        internal override async Task<Mod> GetModVersionAsync(string versionId) {
            if (versionId is null) {
                throw new ArgumentNullException();
            }

            _logger.Debug($"Getting Project Version with ID {versionId}...");

            VersionModel version;
            ProjectModel project;

            try {
                Stream versionRes = await _http.GetStreamAsync(_baseUrl + $@"version/{versionId}");
                version = await JsonSerializer.DeserializeAsync<VersionModel>(versionRes);
                Stream projectRes = await _http.GetStreamAsync(_baseUrl + $@"project/{version.project_id}");
                project = await JsonSerializer.DeserializeAsync<ProjectModel>(projectRes);
            } catch (HttpRequestException e) {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound) {
                    throw new VersionInvalidException("Invalid version id");
                } else {
                    throw new Exception($"failed to connect: {e.StatusCode}");
                }
            }

            return ModFromVersion(version, project);
        }
        internal override async Task<Mod[]> ListModVersionsAsync(string id) {
            List<Mod> mods = new List<Mod>();
            VersionModel[] deserializedRes;

            Stream responseStream = await _http.GetStreamAsync(_baseUrl + $@"project/{id}/version");
            deserializedRes = await JsonSerializer.DeserializeAsync<VersionModel[]>(responseStream);
           
            foreach (VersionModel version in deserializedRes) {
                ProjectModel project = await GetModProjectAsync(version.project_id);
                mods.Add(ModFromVersion(version, project));
            }

            return mods.ToArray<Mod>();
        }
        public Mod ModFromVersion(VersionModel version, ProjectModel project) {
            Mod mod = new Mod();
            mod.Title = project.title;
            mod.ProjectId = version.project_id;
            mod.VersionId = version.id;
            mod.MinecraftVersion = version.game_versions[0];
            mod.ModVersion = version.version_number;
            mod.DownloadURL = version.files.Where<modFile>((file) => file.primary).ToArray<modFile>()[0].url;
            mod.DependencyVersions = new Dictionary<string, Remote>();
            mod.ClientDependency = RequiredBy.unknown;

            modFile primaryFile = version.files[0];
            foreach (modFile file in version.files) {
                if (file.primary) primaryFile = file;
            }
            mod.FileName = primaryFile.filename;


            string serverSideDependency = project.server_side;
            string clientSideDependence = project.client_side;

            switch (clientSideDependence) {
                case "required":
                    if (serverSideDependency.Equals("required") || serverSideDependency.Equals("optional"))
                        mod.ClientDependency = RequiredBy.mutuak;
                    else if (serverSideDependency.Equals("unsupported"))
                        mod.ClientDependency = RequiredBy.client;
                    break;
                case "optional":
                    if (serverSideDependency.Equals("required") || serverSideDependency.Equals("optional"))
                        mod.ClientDependency = RequiredBy.mutuak;
                    else if (serverSideDependency.Equals("unsupported"))
                        mod.ClientDependency = RequiredBy.client;
                    break;
                case "unsupported":
                    if (serverSideDependency.Equals("required") || serverSideDependency.Equals("optional"))
                        mod.ClientDependency = RequiredBy.server;
                    break;
                default:
                    mod.ClientDependency = RequiredBy.unknown;
                    break;
            }

            List<ModLoader> loaders = new List<ModLoader>();
            foreach (string loader in version.loaders) {
                ModLoader parsed;

                if (!Enum.TryParse<ModLoader>(loader, out parsed)) {
                    throw new ProfileException("Invalid mod loader given... ?");
                }

                loaders.Add(parsed);
            }  
            mod.Loaders = loaders.ToArray<ModLoader>();

            return mod;
        }
    }
}