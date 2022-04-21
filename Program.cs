using Mercurius.Modrinth;
using Mercurius.Commands;
using System.Threading.Tasks;

namespace Mercurius {
    public static class Program {
        private static APIClient client = new APIClient();

        public static async Task Main(string[] args) {
            CommandHandler handler = new CommandHandler(args);

            await handler.ExecuteCommandAsync();

        }
    }
}