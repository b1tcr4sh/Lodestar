using System.Threading;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace Mercurius.Commands {
    public class Search : BaseCommand {
        public string Alias { get; private set; }
        public string Description { get; private set; }
        public override async Task Execute(string[] args) {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/Json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Mercurius");

            string response = await client.GetStringAsync(@$"https://api.modrinth.com/v2/search?query={args[0]}");
            Console.WriteLine(response);
        }
    }
}