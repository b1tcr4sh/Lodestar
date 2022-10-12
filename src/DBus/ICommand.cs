using Mercurius;
using Tmds.DBus;

namespace Mercurius.DBus {
    [DBusInterface("org.mercurius.command")]
    public interface ICommand : IDBusObject {
        Task<object> GetAsync(string prop);
        Task ExecuteAsync(string[] args);
    }
}