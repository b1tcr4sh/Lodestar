using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mercurius.Modrinth.Models;

namespace Mercurius.Modrinth {
    public class APIClient {
        private HttpClient client;
        private const string BaseUrl = @"https://api.modrinth.com/v2/";
        public APIClient() {
            client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("user-agent", "Mercurius");
        }
        public async Task<SearchModel> SearchAsync(string query) {
            Console.WriteLine($"Querying Labrynth with {query}");

            Stream responseStream = await client.GetStreamAsync(BaseUrl + $@"search?query={query}");
            SearchModel deserializedRes = await JsonSerializer.DeserializeAsync<SearchModel>(responseStream);

            return deserializedRes;
        }
        public async Task<ProjectModel> GetProjectAsync(string projectId) {
            Console.WriteLine($"Getting Project with ID {projectId}");

            Stream responseStream = await client.GetStreamAsync(BaseUrl + $@"project/{projectId}");
            ProjectModel deserializedRes = await JsonSerializer.DeserializeAsync<ProjectModel>(responseStream);

            return deserializedRes;
        }
        public async Task<VersionModel> GetVersionInfoAsync(string versionId) {
            Console.WriteLine($"Getting Project Version with ID {versionId}");

            Stream responseStream = await client.GetStreamAsync(BaseUrl + $@"version/{versionId}");
            VersionModel deserializedRes = await JsonSerializer.DeserializeAsync<VersionModel>(responseStream);

            return deserializedRes;
        }
        public async Task<VersionModel[]> ListVersionsAsync(ProjectModel project) {
            Console.WriteLine($"Getting List of Versions for {project.title}");

            Stream responseStream = await client.GetStreamAsync(BaseUrl + $@"project/{project.id}/version");
            VersionModel[] deserializedRes = await JsonSerializer.DeserializeAsync<VersionModel[]>(responseStream);

            return deserializedRes;
        }
        public async Task DownloadVersionAsync(VersionModel version) {
            Console.WriteLine($"Starting Download of {version.name}: version {version.version_number}");

            using HttpResponseMessage response = await client.GetAsync(version.files[0].url, HttpCompletionOption.ResponseContentRead);
            using Stream readStream = await response.Content.ReadAsStreamAsync();
            using Stream writeStream = File.Open(@$"./{version.files[0].filename}", FileMode.Create);
            await readStream.CopyToAsync(writeStream);
        }
    }
}