using System.Runtime.InteropServices;

namespace Mercurius.Profiles {
    [StructLayout(LayoutKind.Sequential)]
    public struct Project {
        public string Title { get; set; }
        public string Id { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string PageUrl { get; set; }
        public int DownloadCount { get; set; }
        public ProjectType ProjectType { get; set;}
        public string LastModified { get; set; }
        public string IconUrl { get; set; }
    }
    public enum ProjectType {
        Mod, Plugin, ResourcePack
    }
}