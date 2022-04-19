namespace Mercurius.Modrinth.Models {
    public struct SearchResponse {
        public Hits[] hits { get; set; };
        public int offset;
        public int limit;
        public int total_hits;
    }
    public struct Hits {
        public string slug;
        public string title;
        public string description;
        public string[] categories;
        public string client_side;
        public string server_side;
        public string project_type;
        public int downloads;
        public string icon_url;
        public string project_id;
        public string author;
        public string[] versions;
        public int follows;
        public string date_created;
        public string date_modified;
        public string latest_version;
        public string license;
        public string[] gallery;
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