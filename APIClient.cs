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
        public APIClient() {
            client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("user-agent", "Mercurius");
        }
        public async Task<SearchModel> SearchAsync(string query) {
            Console.WriteLine($@"https://api.modrinth.com/v2/search?query={query}");

            Stream responseStream = await client.GetStreamAsync($@"https://api.modrinth.com/v2/search?query={query}");
            SearchModel deserializedRes = await JsonSerializer.DeserializeAsync<SearchModel>(responseStream);

            return deserializedRes;
        }
        public async Task<ProjectModel> ViewAsync(string query) {
            SearchModel search = await SearchAsync(query);

            Stream responseStream = await client.GetStreamAsync($@"https://api.modrinth.com/v2/project/{search.hits[0].project_id}");
            ProjectModel deserializedRes = await JsonSerializer.DeserializeAsync<ProjectModel>(responseStream);

            return deserializedRes;
        }
    }
}