using Mercurius;
using Tmds.DBus;

namespace Mercurius {
    [DBusInterface("org.mercurius.command")]
    public interface ICommand : IDBusObject {
        Task<object> GetAsync(string prop);
        Task ExecuteAsync(string[] args);
    }
}