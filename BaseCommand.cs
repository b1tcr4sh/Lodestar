using System.Threading.Tasks;
using Mercurius.Modrinth;

namespace Mercurius.Commands {
    public abstract class BaseCommand {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string Format { get; }
        public abstract int ArgsQuantity { get; }
        public abstract Task Execute(string[] args);
    }
}