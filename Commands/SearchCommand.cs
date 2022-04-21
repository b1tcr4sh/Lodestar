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
        public override async Task Execute(string[] args, APIClient client) {
            SearchModel searchResults = await client.SearchAsync(args[0]);

            Console.WriteLine($"Found {searchResults.total_hits} results:\n");
            Console.WriteLine("{0, 10} {1, 10} {2, 10}", "Project Title", "Latest Version", "Downloads");
            foreach (Hit result in searchResults.hits) {
                Console.WriteLine("{0, 10} {1, 10} {2, 10}", result.title, result.latest_version, result.downloads);
            }
        }
    }
}