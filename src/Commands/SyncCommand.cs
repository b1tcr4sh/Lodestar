using System;
using System.Threading.Tasks;
using Mercurius;
using Mercurius.Modrinth;
using Mercurius.Commands;
using Mercurius.Configuration;
using Mercurius.Profiles;
using Tmds.DBus;

public class SyncCommand : BaseCommand {
    public override string Name { get => "Sync"; }
    public override string Description { get => "Syncronises Mods with Profile"; }
    public override string Format { get => "Sync"; }
    public override bool TakesArgs { get => false; }
    public override ObjectPath ObjectPath { get => _objectPath; }
    private ObjectPath _objectPath = new ObjectPath("/org/mercurius/command/sync");

    private APIClient client = new APIClient();
    private List<Mod> installQueue = new List<Mod>();
    public override async Task ExecuteAsync(string[] args) {
        if (ProfileManager.SelectedProfile is null) {
            Console.WriteLine("No Profile is Selected... ? (Create or Select One)");
            return;
        }

        await SyncModsFiles();
    }

    private async Task SyncModsFiles() {
        List<string> existingFiles = Directory.GetFiles($"{SettingsManager.Settings.Minecraft_Directory}/mods/").ToList<string>();
        List<string> modPaths = new List<string>();

        foreach (Mod mod in ProfileManager.SelectedProfile.Mods) {
            modPaths.Add($"{SettingsManager.Settings.Minecraft_Directory}/mods/{mod.FileName}");

            foreach (Mod dependency in mod.Dependencies) {
                modPaths.Add($"{SettingsManager.Settings.Minecraft_Directory}/mods/{dependency.FileName}");
            }
        }
        
        if (modPaths.Count <= 0 && existingFiles.Count > 0) {            
            await GenerateModsFromFiles(existingFiles);
        }

        List<string> keepers = existingFiles.Intersect<string>(modPaths).ToList<string>();

        foreach (string mod in keepers) {
            existingFiles.Remove(mod);
        }

        if (existingFiles.Count <= 0) {
            Console.WriteLine("There are no Residiual Mod jars to Remove");
        } else {
            Console.WriteLine("Removing Residual Mod jars...");
            foreach (string file in existingFiles)
                File.Delete(file);
        }

        if (ProfileManager.SelectedProfile.Mods.Count <= 0) {
            Console.WriteLine("There is nothing to do...");
            return;
        }
        List<Mod> preQueue = new List<Mod>();
        preQueue.AddRange(ProfileManager.SelectedProfile.Mods);
        foreach (Mod mod in ProfileManager.SelectedProfile.Mods) {
            preQueue.AddRange(mod.Dependencies);
        }
        

        // Queue mods for install
        foreach (Mod mod in preQueue) {
            if (File.Exists($"{SettingsManager.Settings.Minecraft_Directory}/mods/{mod.FileName}")) {
                Console.Write("{0}: {1} is already installed, reinstall? (y/N) > ", mod.Title, mod.ModVersion);

                if (Console.ReadLine().ToLower().Equals("y")) {
                    installQueue.Add(mod);
                }
                    
            } else
                installQueue.Add(mod);
        }
            await Install();

            //TODO Resolve dependencies for mods in profile
    }
    private async Task<bool> Install() {
        if (installQueue.Count < 1) {
            Console.WriteLine("There is nothing to do...");
            return false;
        }

        Console.WriteLine("Mods Queued for Install: ");
        foreach (Mod modToInstall in installQueue) {
            Console.WriteLine("- {0}", modToInstall.Title);
        }

        Console.Write("\nContinue with Operation? (Y/n) > ");

        if (Console.ReadLine().ToLower().Equals("n")) {
            Console.WriteLine("Aborting...");
            return false;
        }
        
        foreach (Mod mod in installQueue) {
            await client.DownloadVersionAsync(mod);
        }
        return true;
    }
    private async Task GenerateModsFromFiles(List<string> filePaths) {
        Console.WriteLine("\nThere are no mods in the current profile, but files in mods directory.  Generate mods from jars?\n**WARNING** This feature is still a heavy WIP and may require manual profile editing to clean up dependencies.");
        Console.Write("(y/N) > ");

        if (!Console.ReadLine().ToLower().Equals("y")) return;

        APIClient client = new APIClient();

        foreach (string path in filePaths) {
            string query = path.Substring(path.LastIndexOf("/") + 1);

            Console.WriteLine("Generating mod for {0}", query);

            await ProfileManager.AddModAsync(client, query, false);
        }

    }
}