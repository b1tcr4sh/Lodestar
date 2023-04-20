using Mercurius.Profiles;

namespace Mercurius.API {
    public class APIs {
        private IDictionary<Remote, Repository> _repos = new Dictionary<Remote, Repository>(); 

        public void Add(Repository repo) {
            _repos.Add(repo.Source, repo);
        }
        public Repository Get(Remote identifier) {
            Repository found;
            bool result = _repos.TryGetValue(identifier, out found);

            if (!result) {
                throw new KeyNotFoundException("That repo does not exist!!");
            }
            return found; 
        }
    }    
}