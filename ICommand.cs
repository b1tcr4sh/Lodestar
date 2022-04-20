using System.Threading.Tasks;
using Mercurius.Modrinth;

namespace Mercurius {
    public interface ICommand {
        string Name { get; set; }
        string Description { get; set; }
        Task Execute(string[] args, APIClient client);
    }
}