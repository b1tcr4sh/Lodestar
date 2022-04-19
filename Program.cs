using Mercurius.Modrinth;
using System.Threading.Tasks;

namespace Mercurius {
    public static class Program {
        public static void Main(string[] args) {
            
        }
        private static async Task ParseArgs(string[] args) {
            APIClient client = new APIClient();
            
            switch (args[0])
            {
                case "search":
                    await client.SearchAsync(args[1]);
                    break;
                
            }
        }
    }
}