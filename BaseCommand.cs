namespace Mercurius.Commands {
    public class BaseCommand {
        public string Alias { get; private set; }
        public string Description { get; private set; }
        public virtual void Execute() {}
    }
}