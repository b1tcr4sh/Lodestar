using System.Threading.Tasks;
using System.Reflection;
using Mercurius.Commands;

namespace Mercurius {
    public static class Program {
        public static async Task Main(string[] args) {
            CommandHandler.RegisterCommands();
            CommandHandler.HandleCommand(args[0], ShiftArrayDown(args));
        }
        private static string[] ShiftArrayDown(string[] array) {
            List<string> list = array.ToList<string>();

            list.RemoveAt(0);
            return list.ToArray<string>();
        }
    }
}