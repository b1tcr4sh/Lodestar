using Mercurius.Modrinth;
using Mercurius.Modrinth.Models;
using Mercurius.Commands;
using System.Threading.Tasks;

namespace Mercurius {
    public static class Program {
        private static APIClient client = new APIClient();

        public static async Task Main(string[] args) {
            CommandHandler handler = new CommandHandler(args);

            

            // await ParseArgs(args);
        }
        private static async Task ParseArgs(string[] args) {
            
            switch (args[0])
            {
                case "search":
                    await client.SearchAsync(args[1]);
                    break;
                case "view":
                    await client.GetProjectAsync(args[1]);
                    break;
                case "versions":
                    ProjectModel project = await client.GetProjectAsync(args[1]);
                    await client.ListVersionsAsync(project);
                    break;
                case "install":
                    SearchModel search = await client.SearchAsync(args[1]);
                    ProjectModel projectAHH = await client.GetProjectAsync(search.hits[0].project_id); 
                    VersionModel[] versions = await client.ListVersionsAsync(projectAHH);
                    VersionModel version = await client.GetVersionInfoAsync(versions[0].id);
                    await client.DownloadVersionAsync(version);
                    break;
                default: 
                    Console.WriteLine($"Command {args[0]} not found...   ?");
                    break;
            }
        }
    }
}