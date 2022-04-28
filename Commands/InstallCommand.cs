using System;
using System.Threading.Tasks;
using Mercurius.Modrinth;
using Mercurius.Modrinth.Models;
using Mercurius.Profiles;
using Mercurius.Configuration;

namespace Mercurius.Commands {
    public class InstallCommand : BaseCommand {
        public override string Name { get => "Install"; }
        public override string Description { get => "Installs a mod and its dependencies."; }
        public override string Format { get => "install [Mod Name] {--dry-run | -d}"; }
        public override async Task Execute(string[] args) {
            if (args.Length < 1) throw new ArgumentException("Insuffcient Arguments Provided.");
            APIClient client = new APIClient();

            if (ProfileManager.SelectedProfile == null) {
                Console.WriteLine("No profile is currently selected for install... ? (Select or create one)");
                return;
            }

            string query = string.Join(" ", args);

            string id;
            SearchModel search = await client.SearchAsync(string.Join<string>(" ", args));
            if (!query.ToLower().Equals(search.hits[0].title.ToLower())) {
                id = SelectFromList(search);
            } else id = search.hits[0].project_id;

            ProjectModel project = await client.GetProjectAsync(id);
            VersionModel[] versions = await client.ListVersionsAsync(project);

            VersionModel[] viableVersions = versions.Where<VersionModel>((version) => version.game_versions[0].Equals(ProfileManager.SelectedProfile.MinecraftVersion)).ToArray<VersionModel>();
            VersionModel version = await client.GetVersionInfoAsync(viableVersions[0].id);

            // TODO: Update profile with appropriate mod info
            // TODO: Some flag for a dry-run to update profile without installing anything
            // TODO: Checks for client/server side, install respective mods.

            await client.DownloadVersionAsync(version);
            if (version.dependencies.Length >= 1) {
                foreach (dependencies dependency in version.dependencies) {
                    VersionModel dependencyVersion = await client.GetVersionInfoAsync(dependency.version_id);
                    
                    Console.WriteLine($"Installing dependency: {dependencyVersion.name}");
                    await client.DownloadVersionAsync(dependencyVersion);
                }
            }            
        }
        private string SelectFromList(SearchModel response) {
            Console.WriteLine($"Found {response.total_hits} results, displaying 10:\n");
            Console.WriteLine("{0, -30} {1, -20} {2, 15}", "Project Title", "Latest Minecraft Version", "Downloads");
            for (int i = 0; i < response.hits.Length; i++) {
                Hit result = response.hits[i];

                Console.WriteLine("{0, -2} {1, -30} {2, -20} {3, 15}", i + 1, result.title, result.latest_version, result.downloads);
            }

            Console.Write("Select which mod to view by number > ");
            int selection = Convert.ToInt32(Console.ReadLine());

            return response.hits[selection - 1].project_id;
        }
    }
}