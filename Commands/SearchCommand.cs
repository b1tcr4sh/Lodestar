using System.Threading.Tasks;
using Mercurius.Modrinth;
using Mercurius.Modrinth.Models;

namespace Mercurius.Commands {
    public class SearchCommand : BaseCommand {
        public override string Name { get; set; }
        public override string Description { get; set; }
        public SearchCommand() {
            Name = "Search";
            Description = "Gets top 10 results for query from Labrynth.";
        }
        public override async Task Execute(string[] args) {
            APIClient client = new APIClient();
            SearchModel searchResults;

            searchResults = await client.SearchAsync(string.Join(" ", args));

            Console.WriteLine($"Found {searchResults.total_hits} results, displaying 10:\n");
            Console.WriteLine("{0, -30} {1, -20} {2, 15}", "Project Title", "Latest Minecraft Version", "Downloads");
            foreach (Hit result in searchResults.hits) {
                Console.WriteLine("{0, -30} {1, -20} {2, 15}", result.title, result.latest_version, result.downloads);
            }
        }
    }
}