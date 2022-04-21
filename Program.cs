using Mercurius.Commands;
using Mercurius.Configuration;
using System.Threading.Tasks;

namespace Mercurius {
    public static class Program {
        public static async Task Main(string[] args) {
            SettingsManager.Init();
            CommandHandler handler = new CommandHandler(args);

            await handler.ExecuteCommandAsync();
        }
    }
}