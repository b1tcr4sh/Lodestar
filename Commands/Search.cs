using System.Threading;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace Mercurius.Commands {
    public class Search : BaseCommand {
        public override async Task Execute(string query) {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/Json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Mercurius");

            string response = await client.GetStringAsync(@$"https://api.modrinth.com/v2/search?query={query}");
            Console.WriteLine(response);
        }
    }
}