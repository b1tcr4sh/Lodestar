namespace Mercurius.Modrinth {
    public struct SearchModel {
        public Hit[] hits { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public int total_hits { get; set; }
    }
    public struct Hit {
        public string slug { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string[] categories { get; set; }
        public string client_side { get; set; }
        public string server_side { get; set; }
        public string project_type { get; set; }
        public int downloads { get; set; }
        public string icon_url { get; set; }
        public string project_id { get; set; }
        public string author { get; set; }
        public string[] versions { get; set; }
        public int follows { get; set; }
        public string date_created { get; set; }
        public string date_modified { get; set; }
        public string latest_version { get; set; }
        public string license { get; set; }
        public string[] gallery { get; set; }
    }
}

/*
{
  "hits": [
    {
      "slug": "my_project",
      "title": "My Project",
      "description": "A short description",
      "categories": [
        "technology",
        "adventure",
        "fabric"
      ],
      "client_side": "required",
      "server_side": "optional",
      "project_type": "mod",
      "downloads": 0,
      "icon_url": "https://cdn.modrinth.com/data/AABBCCDD/b46513nd83hb4792a9a0e1fn28fgi6090c1842639.png",
      "project_id": "AABBCCDD",
      "author": "my_user",
      "versions": [
        "1.8",
        "1.8.9"
      ],
      "follows": 0,
      "date_created": "2019-08-24T14:15:22Z",
      "date_modified": "2019-08-24T14:15:22Z",
      "latest_version": "1.8.9",
      "license": "mit",
      "gallery": [
        "https://cdn.modrinth.com/data/AABBCCDD/images/009b7d8d6e8bf04968a29421117c59b3efe2351a.png",
        "https://cdn.modrinth.com/data/AABBCCDD/images/c21776867afb6046fdc3c21dbcf5cc50ae27a236.png"
      ]
    }
  ],
  "offset": 0,
  "limit": 10,
  "total_hits": 10
}
*/