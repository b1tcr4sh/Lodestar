namespace Mercurius.Profiles {
    public enum Remote {
        modrinth,
        curseforge,
        custom
    }
    public enum ClientType {
        Client, Server
    }
    public enum ModLoader {
        unknown, forge, fabric, quilt, liteloader, rift, modloader
    }
    public enum RequiredBy {
        client, server, mutuak, unknown
    }
    public enum DependencyType {
        required, optional, incompatible, embedded
    }
}