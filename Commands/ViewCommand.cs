using System.Threading.Tasks;
using Mercurius.Modrinth;
using Mercurius.Modrinth.Models;

namespace Mercurius.Commands {
    public class ViewCommand : BaseCommand {
        public override string Name { get => "View"; }
        public override string Description { get => "Gets the details of a mod."; }
        public override string Format { get => "view [Mod Name]"; }
        public override async Task Execute(string[] args) {
            APIClient client = new APIClient();
            string query = string.Join<string>(" ", args);
            SearchModel searchResponse = await client.SearchAsync(query);

            string modTitle = searchResponse.hits[0].title;
            string modId = searchResponse.hits[0].project_id;
            if (!query.Equals(modTitle)) {
                modId = SelectFromList(searchResponse);
            }

            ProjectModel project = await client.GetProjectAsync(modId);
            VersionModel[] versions = await client.ListVersionsAsync(project);

            int listLength = 5;
            if (versions.Length < 5) listLength = versions.Length;
            Console.Write($"\n\n{project.title}\n{project.description}\n\nCategories: ");

            foreach (string category in project.categories) {
                Console.Write(category + ", ");
            }
            Console.WriteLine($"\nProject URL: {project.source_url}\nServer Side: {project.server_side}\nClient Side: {project.client_side}\nStatus: {project.status}\nLast Updated: {project.updated}\n");
            Console.WriteLine($"Top {listLength} Project Versions:");

            for (int i = 0; i < listLength; i++) {
                VersionModel version = versions[i];

                Console.WriteLine("\n     " + version.name);
                Console.Write($"       Date Published: {version.date_published}\n       Supported Minecraft Versions: ");

                foreach (string mcVersion in version.game_versions) {
                    Console.Write(mcVersion + ", ");
                } 
                Console.WriteLine("\n       Supported Mod Loaders:");
                foreach (string loader in version.loaders) {
                    Console.Write("         " + loader);
                }
                Console.WriteLine();
            }
        }
        private string SelectFromList(SearchModel response) {
            Console.WriteLine($"Found {response.total_hits} results, displaying 10:\n");
            Console.WriteLine("{0, -30} {1, -20} {2, 15}", "Project Title", "Latest Minecraft Version", "Downloads");
            for (int i = 0; i < response.hits.Length; i++) {
                Hit result = response.hits[i];

                Console.WriteLine("{0, -2} {1, -30} {2, -20} {3, 15}", i + 1, result.title, result.latest_version, result.downloads);
            }

            Console.Write("Select which mod to view by number > ");
            int selection = Convert.ToInt32(Console.ReadLine());

            return response.hits[selection - 1].project_id;
        }
    }
}