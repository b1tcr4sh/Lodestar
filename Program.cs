using Mercurius.Modrinth;
using Mercurius.Modrinth.Models;
using System.Threading.Tasks;

namespace Mercurius {
    public static class Program {
        private static APIClient client = new APIClient();

        public static async Task Main(string[] args) {
            // await ParseArgs(args);
            ProjectModel project = await client.GetProjectAsync("AANobbMI");
            await client.ListVersionsAsync(project);
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
                default: 
                    Console.WriteLine($"Command {args[0]} not found...   ?");
                    break;
            }
        }
    }
}