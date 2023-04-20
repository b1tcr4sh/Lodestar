namespace Mercurius.Profiles {
    public enum Remote {
        modrinth,
        curseforge,
        custom
    }
    public enum ClientType {
        ClientSide, ServerSide
    }
    public enum ModLoader {
        unknown, forge, fabric, quilt, liteloader, rift, modloader
    }
    public enum ClientDependency {
        Client, Server, Mutual, Unknown
    }
}