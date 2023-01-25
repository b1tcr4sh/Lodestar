<div align="center">
  <h1>Lodestar</h1>
 
  <p>Modular and hackable package manager daemon for Minecraft ✨</p>  
  <br>
  <br>
  <br>

</div>

Lodestar is a package manager daemon written for developers with customizeablility and modularity as the main focus.  If you're a user looking for an app to use for their Minecraft mods, this isn't exactly for you.  You can try out the first-party CLI [Stargazer](https://github.com/vividuwu/stargazer) or one of the other user-made frontends.  These could take the form of anything, whether that be a CLI, UI app, mod from within the game, or even a website.

<br>

## Features

✅: Supported\
🚧 : Being Implemented\
🔮 : Planned for Future

| Repository | Mod Support | Resource Pack Support | Plugin Support |
| --- | --- | --- | --- |
| [Modrinth](https://modrinth.org) | ✅ | 🔮 | 🔮 |
| [Curseforge](https://www.curseforge.com/minecraft/mc-mods) | 🚧 | 🔮 | 🔮 |

### Specifications

- Mod management
  - Creates and manages "profiles" to store collections of compatible mod metadate for specific modloaders and game versions
    - Profiles stored as JSON files and can be share between users
   - Automatic resolving and installing of mod Jars
   - Dependency management
   - Compatibility checks
 - DBus
  - Can connect to DBus system bus or host peer bus (Configurable in settings.json)
  - Exposes profiles as DBus objects
    - Remote management of profiles and mods over DBus api.


## Installation

The daemon can be installed by one of the install scripts, or manually depending on your situation.  Lodestar is also a dependency of any of the frontends, and should be installed alongside.

## Documentation 

Developer docs are currently being made using the [Github wiki](https://github.com/vividuwu/Lodestar/wiki), but will probably be implemented on a github.io page (if ever I get around to it).
