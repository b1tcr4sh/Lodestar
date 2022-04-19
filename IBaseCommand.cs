using System.Threading.Tasks;

namespace Mercurius.Commands {
    public interface IBaseCommand {
        string Alias => this.GetType().Name.ToLower();
        string Description { get; }
        Task Execute(string[] args);
    }
}