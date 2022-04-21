using Mercurius.Commands;
using System.Threading.Tasks;

namespace Mercurius {
    public static class Program {
        public static async Task Main(string[] args) {
            CommandHandler handler = new CommandHandler(args);

            await handler.ExecuteCommandAsync();
        }
    }
}