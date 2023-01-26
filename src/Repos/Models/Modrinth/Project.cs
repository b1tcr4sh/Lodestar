namespace Mercurius.Modrinth {
    public struct ProjectModel {
        public string slug { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string[] categories { get; set; } 
        public string client_side { get; set; } 
        public string server_side { get; set; } 
        public string body { get; set; } 
        public string issues_url { get; set; } 
        public string source_url { get; set; } 
        public string wiki_url { get; set; } 
        public string discord_url { get; set; } 
        public donation_url[] donation_urls { get; set; } 
        public string project_type { get; set; }
        int downloads { get; set; }
        public string icon_url { get; set; }
        public string id { get; set; }
        public string team { get; set; }
        public string body_url { get; set; }
        public string moderator_message { get; set; }
        public string published { get; set; }
        public string updated { get; set; }
        public int followers { get; set; }
        public string status { get; set; }
        public license license { get; set; }
        public string[] versions { get; set; }
        public gallery[] gallery { get; set; }
    }

    public struct donation_url {
        public string id { get; set; }
        public string platform { get; set; }
        public string url { get; set; }
    }
    public struct license {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; } 
    }
    public struct gallery {
        public string url { get; set; }
        public bool featured { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string created { get; set; }
    }
}


/*
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
  "body": "A long body describing my project in detail",
  "issues_url": "https://github.com/my_user/my_project/issues",
  "source_url": "https://github.com/my_user/my_project",
  "wiki_url": "https://github.com/my_user/my_project/wiki",
  "discord_url": "https://discord.gg/AaBbCcDd",
  "donation_urls": [
    {
      "id": "patreon",
      "platform": "Patreon",
      "url": "https://www.patreon.com/my_user"
    }
  ],
  "project_type": "mod",
  "downloads": 0,
  "icon_url": "https://cdn.modrinth.com/data/AABBCCDD/b46513nd83hb4792a9a0e1fn28fgi6090c1842639.png",
  "id": "AABBCCDD",
  "team": "MMNNOOPP",
  "body_url": null,
  "moderator_message": null,
  "published": "2019-08-24T14:15:22Z",
  "updated": "2019-08-24T14:15:22Z",
  "followers": 0,
  "status": "approved",
  "license": {
    "id": "lgpl-3",
    "name": "GNU Lesser General Public License v3",
    "url": "https://cdn.modrinth.com/licenses/lgpl-3.txt"
  },
  "versions": [
    "IIJJKKLL",
    "QQRRSSTT"
  ],
  "gallery": [
    {
      "url": "https://cdn.modrinth.com/data/AABBCCDD/images/009b7d8d6e8bf04968a29421117c59b3efe2351a.png",
      "featured": true,
      "title": "My awesome screenshot!",
      "description": "This awesome screenshot shows all of the blocks in my mod!",
      "created": "2019-08-24T14:15:22Z"
    }
  ]
}
*/