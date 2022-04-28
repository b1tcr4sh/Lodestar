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
        public override string Format { get => "install [Mod Name]"; }
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

            // TODO: Some flag for a dry-run to update profile without installing anything (add command??)
            // TODO: Checks for client/server side compatibility;
            // TODO: Check for loader, and make sure mod is compatible.

            // TODO: Differentiate between the installation and adding of mods to the profile.  Even if mods is alrady in profile, it can still be installed if
            // currently not.  Should have 'add' command, which adds a mod to the profile, then install should blindly install.
            if (ProfileManager.SelectedProfile.Mods.Contains(new Mod(version))) {
                Console.WriteLine($"Mod {version.name} already installed");
            }

            List<Mod> mods = new List<Mod>();

            await client.DownloadVersionAsync(version);
            mods.Add(new Mod(version));

            if (version.dependencies.Length >= 1) {
                foreach (Dependency dependency in version.dependencies) {
                    VersionModel dependencyVersion = await client.GetVersionInfoAsync(dependency.version_id);
                    
                    mods.Add(new Mod(dependencyVersion, true));
                    Console.WriteLine($"Installing dependency: {dependencyVersion.name}");
                    await client.DownloadVersionAsync(dependencyVersion);
                }
            } 
            Console.WriteLine("Updating Profile...");
            await ProfileManager.SelectedProfile.UpdateModListAsync(mods);
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