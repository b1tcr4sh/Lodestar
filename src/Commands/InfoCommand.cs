using System.Threading.Tasks;
using Mercurius.Modrinth;
using Mercurius.Modrinth.Models;
using Tmds.DBus;

namespace Mercurius.Commands {
    public class InfoCommand : BaseCommand {
        public override string Name { get => "Info"; }
        public override string Description { get => "Gets the details of a mod."; }
        public override string Format { get => "info <Mod Name>"; }
        public override bool TakesArgs { get => true; }
        public override ObjectPath ObjectPath { get => _objectPath; }
        private ObjectPath _objectPath = new ObjectPath("/org/mercurius/command/info");
        public override async Task ExecuteAsync(string[] args) {
            APIClient client = new APIClient();
            string query = string.Join<string>(" ", args);
            SearchModel searchResponse = await client.SearchAsync(query);

            string modTitle = searchResponse.hits[0].title;
            string modId = searchResponse.hits[0].project_id;
            if (!query.Equals(modTitle.ToLower())) {
                modId = CommandExtensions.SelectFromList(searchResponse);
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
    }
}