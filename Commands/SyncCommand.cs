using System;
using System.Threading.Tasks;
using Mercurius.Modrinth;
using Mercurius.Commands;
using Mercurius.Configuration;
using Mercurius.Profiles;

public class SyncCommand : BaseCommand {
    public override string Name { get => "Sync"; }
    public override string Description { get => "Syncronises Mods with Profile"; }
    public override string Format { get => "Sync"; }

    private APIClient client = new APIClient();
    public override async Task Execute(string[] args) {
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
        }
        
        List<string> keepers = existingFiles.Intersect<string>(modPaths).ToList<string>();

        foreach (string mod in keepers) {
            existingFiles.Remove(mod);
        }

        Console.WriteLine("Removing Unrecognized Mod jars...");
        foreach (string file in existingFiles)
            File.Delete(file);

        foreach (Mod mod in ProfileManager.SelectedProfile.Mods) {
            if (File.Exists($"{SettingsManager.Settings.Minecraft_Directory}/mods/{mod.FileName}")) {
                Console.Write("{0}: {1} is already installed, reinstall? (y/N) >", mod.Title, mod.ModVersion);

                if (Console.ReadLine().ToLower().Equals("y"))
                    await client.DownloadVersionAsync(mod);
            } else
                await client.DownloadVersionAsync(mod);
        }
    }
}