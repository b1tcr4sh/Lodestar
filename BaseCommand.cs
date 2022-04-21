using System.Threading.Tasks;
using Mercurius.Modrinth;

namespace Mercurius.Commands {
    public abstract class BaseCommand {
        public abstract string Name { get; set; }
        public abstract string Description { get; set; }
        public abstract Task Execute(string[] args, APIClient client);
    }
}