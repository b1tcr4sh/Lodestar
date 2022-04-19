using System.Threading.Tasks;

namespace Mercurius.Commands {
    public abstract class BaseCommand {
        public BaseCommand() {}
        string Alias => this.GetType().Name.ToLower();
        string Description { get; }
        public abstract Task Execute(string[] args);
    }
}