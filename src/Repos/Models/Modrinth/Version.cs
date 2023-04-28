namespace Mercurius.API.Models.Modrinth {
    public struct VersionModel {
        public string name { get; set; }
        public string version_number { get; set; }
        public string changelog { get; set; }
        public Dependency[] dependencies { get; set; }
        public string[] game_versions { get; set; }
        public string version_type { get; set; }
        public string[] loaders { get; set; }
        public bool featured { get; set; }
        public string id { get; set; }
        public string project_id { get; set; }
        public string author_id { get; set; }
        public string date_published { get; set; }
        public int downloads { get; set; }
        public string changelog_url { get; set; }
        public modFile[] files { get; set; } 
    }
    public struct Dependency {
        public string version_id { get; set; }
        public string project_id { get; set; }
        public string dependency_type { get; set; }
    }
    public struct modFile {
        public hash hashes { get; set; }
        public string url { get; set; }
        public string filename { get; set; }
        public bool primary { get; set; }
    }
    public struct hash {
        public string sha512 { get; set; }
        public string sha1 { get; set; }
    }
}

/*
{
  "name": "Version 1.0.0",
  "version_number": "1.0.0",
  "changelog": "List of changes in this version: ...",
  "dependencies": [
    {
      "version_id": "IIJJKKLL",
      "project_id": "QQRRSSTT",
      "dependency_type": "required"
    }
  ],
  "game_versions": [
    "1.16.5",
    "1.17.1"
  ],
  "version_type": "release",
  "loaders": [
    "fabric",
    "forge"
  ],
  "featured": true,
  "id": "IIJJKKLL",
  "project_id": "AABBCCDD",
  "author_id": "EEFFGGHH",
  "date_published": "2019-08-24T14:15:22Z",
  "downloads": 0,
  "changelog_url": null,
  "files": [
    {
      "hashes": {
        "sha512": "93ecf5fe02914fb53d94aa3d28c1fb562e23985f8e4d48b9038422798618761fe208a31ca9b723667a4e05de0d91a3f86bcd8d018f6a686c39550e21b198d96f",
        "sha1": "c84dd4b3580c02b79958a0590afd5783d80ef504"
      },
      "url": "https://cdn.modrinth.com/data/AABBCCDD/versions/1.0.0/my_file.jar",
      "filename": "my_file.jar",
      "primary": false
    }
  ]
}
*/