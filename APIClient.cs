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
            Stream responseStream = await client.GetStreamAsync(BaseUrl + $@"search?query={query}");
            SearchModel deserializedRes = await JsonSerializer.DeserializeAsync<SearchModel>(responseStream);

            return deserializedRes;
        }
        public async Task<ProjectModel> GetProjectAsync(string projectId) {
            Stream responseStream = await client.GetStreamAsync(BaseUrl + $@"project/{projectId}");
            ProjectModel deserializedRes = await JsonSerializer.DeserializeAsync<ProjectModel>(responseStream);

            return deserializedRes;
        }
        public async Task<VersionModel> GetVersionInfoAsync(string versionId) {
            Stream responseStream = await client.GetStreamAsync(BaseUrl + $@"version/{versionId}");
            VersionModel deserializedRes = await JsonSerializer.DeserializeAsync<VersionModel>(responseStream);

            return deserializedRes;
        }
        public async Task<VersionModel[]> ListVersionsAsync(ProjectModel project) {
            Console.WriteLine(project.id);

            Stream responseStream = await client.GetStreamAsync(BaseUrl + $@"project/{project.id}/version");
            VersionModel[] deserializedRes = await JsonSerializer.DeserializeAsync<VersionModel[]>(responseStream);

            return deserializedRes;
        }
    }
}