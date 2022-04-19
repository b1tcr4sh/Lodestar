using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mercurius.Modrinth.Models;

namespace Mercurius.Modrinth {
    public class APIClient {
        private HttpClient client = new HttpClient();
        public async Task<SearchResponse> SearchAsync(string query) {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("user-agent", "Mercurius");

            Stream responseStream = await client.GetStreamAsync($@"https://api.modrinth.com/v2/search?{query}");
            SearchResponse deserializedRes = await JsonSerializer.DeserializeAsync<SearchResponse>(responseStream);

            return deserializedRes;
        }
    }
}