using Mercurius.Modrinth;
using Mercurius.Modrinth.Models;
using System.Threading.Tasks;

namespace Mercurius {
    public static class Program {
        public static async Task Main(string[] args) {
            await ParseArgs(args);
        }
        private static async Task ParseArgs(string[] args) {
            APIClient client = new APIClient();
            
            switch (args[0])
            {
                case "search":
                    SearchResponse response = await client.SearchAsync(args[1]);

                    Console.WriteLine(response);
                    break;
                
            }
        }
    }
}