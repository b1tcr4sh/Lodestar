using System.Threading.Tasks;
using Mercurius.Modrinth;
using Tmds.DBus;

namespace Mercurius.Commands {
    public abstract class BaseCommand : ICommand {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string Format { get; }
        public abstract bool TakesArgs { get; }
        public abstract ObjectPath ObjectPath { get; }

        public Task<object> GetAsync(string prop) {
            // Make return the property that's passed in
            return Task.FromResult((new object[0])[0]);
        }
        public abstract Task ExecuteAsync(string[] args);
    }
}