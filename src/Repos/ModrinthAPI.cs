using System.Net;
using System.Text.Json;
using Serilog;

using Mercurius.Profiles;
using Mercurius.API.Models.Modrinth;

namespace Mercurius.API {
    public class ModrinthAPI : Repository {
        public override Remote Source { get; } = Remote.modrinth;
        protected internal ModrinthAPI(string baseUrl, HttpClient client, ILogger logger) : base(baseUrl, client, logger) {
            _objectPath = "/org/mercurius/modrinth";
        }

        public override async Task<Project[]> SearchModAsync(string query, string version, string loader) {
            _logger.Debug($"Searching Labrynth with {query}...");

            SearchModel deserializedRes;

            try {
                Stream responseStream = await _http.GetStreamAsync(_baseUrl + $@"search?query={query}" + $"?facets=[[\"project_type:mod\"],[\"versions:{version}\"],[\"categories:{loader.ToString()}\"]]");
                deserializedRes = await JsonSerializer.DeserializeAsync<SearchModel>(responseStream);
            } catch (Exception e) {
                _logger.Warning(e.Message);
                _logger.Warning(e.StackTrace);
                throw new ApiException("Connection Failed!");
            }

            if (deserializedRes.hits.Length <= 0) {
                _logger.Debug("No results found... Sorry");
                throw new ApiException("No results found");
            }

            List<Project> projects = new List<Project>();
            foreach (Hit hit in deserializedRes.hits) {
                projects.Add(ProjectFromHit(hit, ProjectType.Mod));
            }
            return projects.ToArray<Project>();
        }
        internal override async Task<Project> GetModProjectAsync(string projectId) {
            // _logger.Debug($"Getting Project with ID {projectId}...");
            ProjectModel project = await FetchProjectAsync(projectId);

            return ConvertProject(project, ProjectType.Mod);
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
        internal override async Task<Mod[]> ListModVersionsAsync(string projectId) {
            List<Mod> mods = new List<Mod>();
            VersionModel[] deserializedRes;

            Stream responseStream = await _http.GetStreamAsync(_baseUrl + $@"project/{projectId}/version");
            deserializedRes = await JsonSerializer.DeserializeAsync<VersionModel[]>(responseStream);
            ProjectModel project = await FetchProjectAsync(projectId);

            foreach (VersionModel version in deserializedRes) {
                mods.Add(ModFromVersion(version, project));
            }

            return mods.ToArray<Mod>();
        }
        private async Task<ProjectModel> FetchProjectAsync(string projectId) {
            ProjectModel deserializedRes;

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
        private Mod ModFromVersion(VersionModel version, ProjectModel project) {
            Mod mod = new Mod();
            mod.Title = project.title;
            mod.ProjectId = version.project_id;
            mod.VersionId = version.id;
            mod.MinecraftVersion = version.game_versions[0];
            mod.ModVersion = version.version_number;
            mod.ClientDependency = RequiredBy.unknown;
            mod.DependencyVersions = new List<string>();

            modFile primaryFile = version.files[0];
            foreach (modFile file in version.files) {
                if (file.primary) primaryFile = file;
                break;
            }
            mod.FileName = primaryFile.filename;
            mod.DownloadURL = primaryFile.url;


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

            foreach (Dependency dependency in version.dependencies) {
                mod.DependencyVersions.Append(dependency.version_id);
            }

            return mod;
        }
        private Project ConvertProject(ProjectModel project, ProjectType type) {
            return new Project() {
                Id = project.id,
                Title = project.title,
                Description = project.description,
                PageUrl = project.source_url,
                Slug = project.slug,
                IconUrl = project.icon_url,
                ProjectType = type,
                LastModified = project.updated
            };
        }
        private Project ProjectFromHit(Hit hit, ProjectType type) {
            return new Project() {
                Id = hit.project_id,
                Title = hit.title,
                Description = hit.description,
                Slug = hit.slug,
                PageUrl = string.Empty, // Hits don't have link to page
                IconUrl = hit.icon_url,
                ProjectType = type,
                DownloadCount = hit.downloads,
                LastModified = hit.date_modified
            };
        }
    }
}