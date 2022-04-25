using Mercurius.Commands;
using Mercurius.Configuration;
using Mercurius.Profiles;
using System.Threading.Tasks;

namespace Mercurius {
    public static class Program {
        public static async Task Main(string[] args) {
            SettingsManager.Init();
            ProfileManager.InitializeDirectory(@"./Profiles");
            CommandHandler handler = new CommandHandler(args);

            await handler.ExecuteCommandAsync();
        }
    }
}