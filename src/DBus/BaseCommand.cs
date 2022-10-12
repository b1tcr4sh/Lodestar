using System.Threading.Tasks;
using System.Reflection;
using Mercurius.Modrinth;
using Tmds.DBus;

namespace Mercurius.DBus {
    public abstract class BaseCommand : ICommand {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string Format { get; }
        public abstract bool TakesArgs { get; }
        public abstract ObjectPath ObjectPath { get; }

        public Task<object> GetAsync(string prop) {
            // Not exposed like it's supposed to be?
            return Task.FromResult(typeof(BaseCommand).GetProperty(prop).GetValue(this));
        }
        public abstract Task ExecuteAsync(string[] args);
    }
}