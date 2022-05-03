# Mercurius
#### An easy to use, feature-rich package manager for Minecraft mods.

## How to Use:
### Installation   
  ***In the future, this will be handled with an install script***
  
  (Not currently working) The app is separated into a deamon and an executeable, and so should be installed so that the executeable is accessible through the "mercurius" command, and the daemon process runs on startup.
### Profiles:
  Profiles are json files (denoted by the [name].profile.json file naming scheme) which represent individual instances or "modpacks", containing mods for a specific Minecraft version and mod loader.
  
  Profiles are stored at the path specified in the *settings.json* (`./Profiles/` by default).
  They can be managed through the `profile` command, where users can create, remove, and import profiles and profile prefabs.
### Commands: 
  **Currently Supported:**
  - help - Displays help message and about information.
  - search [query] - Searches Modrinth's API with the provided query.
  - view [mod name] - Shows specific information about the mod entered.
  - install [mod name] - Installs the specified mod to the mod directory (denoted in *settings.json*) and adds it to the selected profile.

## Features:
### Currently Supported:
  - Compatibility with Windows and Linux.
  - Searching, viewing, and installing mods from [Modrinth's](https://modrinth.com) web API.
  - Dependency management.
  - Under-the-hood compatibility checks for mod versions, Minecraft versions, and supported mod loaders ((Forge)[https://forums.minecraftforge.net/] and (Fabric)[https://fabricmc.net].
  - Profiles designed to be easily shareable with other users.
  - Rudimentary profile management.
    - Creating profiles, loading, and selecting profiles as individually contained instances.
### Under Development:
  - Extended profile management commands.
  - Importing profiles made by other users as "modpacks".
  - Checks for server-side and client-side mod compatibility.
  - Server-side profiles which an be created on a server and check for server-side mod compatibility.   
   - Generate server-side profiles to accompany client-side profiles.
  - Installing a profile's respective mod loader.
  - Profile prefabs/templates which can be customized. (E.g. Creating all Fabric profiles with Fabric-API as a default dependency.) 
  - Install scripts.
  - Integration with [CurseForge's](https://www.curseforge.com/) web API.
  - Ability to add "unknown" mods (those not hosted on either API) which can be custom URLs and will present appropriate warnings about unknown sources.
  - Caching mod files in a separate folder (To prevent having to reinstall all mods upon selecting a new profile). 
