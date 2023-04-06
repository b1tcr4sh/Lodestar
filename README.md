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

### How Exactly Does it Work?

Lodestar is a local server that uses [DBus](https://www.freedesktop.org/wiki/Software/dbus/) to interface with frontend apps.
>But why a server?  And why does it need to run in the backround?
This is for two primary reasons:
  - DBus provides a interpoperable interface for interacting with the app. This means that your could manage minecraft packages from a UI, CLI, webpage, or whatever else you can dream up.
  - It is accessible over the network.  This is especially useful for server admins, because they can access their package manager on a remote server from a local interface.
>How does it work internally?

  - The server manages 'profiles' which hold information about separate instances of the game.  This can include game version, mod loader, server type, and information about the mods or plugins associated with it. 
  - Profiles are saved as JSON files and are completely decoupled from the current state of the client/server.  If you want to use a profile, it must be synced, which removes unneeded mods/profiles and installs the ones listed in the profile. 

## Documentation 

Check out the [wiki](https://github.com/vividuwu/Lodestar/wiki).
