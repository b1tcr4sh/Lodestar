namespace Mercurius.API.Curseforge {
    internal struct Project {
        public Data data;
    }
    internal struct Data {
        public int id;
        public int gameId;
        public string name;
        public string slug;
        public Links links;
        public string summary;
        public int status;
        public int downloadCount;
        public bool isFeatured;
        public string primaryCategoryId;
        public Category[] categories;
        public int classId;
        public Author[] authors;
        public Logo logo { get; set; }
        public Screenshot[] screenshots;
        public int mainFileId;
        public ShortFile[] latestFiles;
        public FileIndex[] latestFileIndexes;
        public string dateCreated;
        public string dateModified;
        public string dateReleased;
        public bool allowModDistribution;
        public int gamePopularityRank;
        public bool isAvailable;
        public int thumbsUpCount;
    }
    internal struct Links {
        string websiteUrl;
        string wikiUrl;
        string issuesUrl;
        string sourceUrl;
    }
    internal struct Category {
        public int id;
        public int gameId;
        public string name;
        public string slug;
        public string url;
        public string iconUrl;
        public string dateModified;
        public bool isClass;
        public int classId;
        public int parentCategoryId;
    }
    internal struct Author {
        public int id;
        public string name;
        public string url;
    }
    internal struct Logo {
        public int id { get; set; }
        public int modId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string thumbnailUrl { get; set; }
        public string url { get; set; }
    }
    internal struct Screenshot {}
    internal struct ShortFile {
        public int id;
        public int gameId;
        public int modId;
        public bool isAvailable;
        public string displayName;
        public string fileName;
        public int releaseType;
        public int fileStatus;
        public Hash[] hashes;
        public string fileDate;
        public int fileLength;
        public int downloadCount;
        public string downloadUrl;
        public string[] gameVersions;
        public GameVersion[] sortableGameVersions;
        public Dependency[] dependencies;
        public int alternateFileId;
        public bool isServerPack;
        public int fileFingerprint;
        public Module[] modules;
    }
    internal struct Hash {
        public string value;
        public int algo;
    }
    internal struct GameVersion {
        public string gamerVersionName;
        public string gameVersionPadded;
        public string gameVersion;
        public string gameVersionReleaseDate;
        public int gameVersionTypeId;
    }
    internal struct Dependency {}
    internal struct Module {
        public string name;
        public string fingerprint;
    }
    internal struct FileIndex {
        public string gameVersion;
        public int fileId;
        public string fileName;
        public int releaseType;
        public int gameVersionTypeId;
        public int modLoader;
    }
}

/*
{
  "data": {
    "id": 0,
    "gameId": 0,
    "name": "string",
    "slug": "string",
    "links": {
      "websiteUrl": "string",
      "wikiUrl": "string",
      "issuesUrl": "string",
      "sourceUrl": "string"
    },
    "summary": "string",
    "status": 1,
    "downloadCount": 0,
    "isFeatured": true,
    "primaryCategoryId": 0,
    "categories": [
      {
        "id": 0,
        "gameId": 0,
        "name": "string",
        "slug": "string",
        "url": "string",
        "iconUrl": "string",
        "dateModified": "2019-08-24T14:15:22Z",
        "isClass": true,
        "classId": 0,
        "parentCategoryId": 0,
        "displayIndex": 0
      }
    ],
    "classId": 0,
    "authors": [
      {
        "id": 0,
        "name": "string",
        "url": "string"
      }
    ],
    "logo": {
      "id": 0,
      "modId": 0,
      "title": "string",
      "description": "string",
      "thumbnailUrl": "string",
      "url": "string"
    },
    "screenshots": [
      {
        "id": 0,
        "modId": 0,
        "title": "string",
        "description": "string",
        "thumbnailUrl": "string",
        "url": "string"
      }
    ],
    "mainFileId": 0,
    "latestFiles": [
      {
        "id": 0,
        "gameId": 0,
        "modId": 0,
        "isAvailable": true,
        "displayName": "string",
        "fileName": "string",
        "releaseType": 1,
        "fileStatus": 1,
        "hashes": [
          {
            "value": "string",
            "algo": 1
          }
        ],
        "fileDate": "2019-08-24T14:15:22Z",
        "fileLength": 0,
        "downloadCount": 0,
        "downloadUrl": "string",
        "gameVersions": [
          "string"
        ],
        "sortableGameVersions": [
          {
            "gameVersionName": "string",
            "gameVersionPadded": "string",
            "gameVersion": "string",
            "gameVersionReleaseDate": "2019-08-24T14:15:22Z",
            "gameVersionTypeId": 0
          }
        ],
        "dependencies": [
          {
            "modId": 0,
            "relationType": 1
          }
        ],
        "exposeAsAlternative": true,
        "parentProjectFileId": 0,
        "alternateFileId": 0,
        "isServerPack": true,
        "serverPackFileId": 0,
        "fileFingerprint": 0,
        "modules": [
          {
            "name": "string",
            "fingerprint": 0
          }
        ]
      }
    ],
    "latestFilesIndexes": [
      {
        "gameVersion": "string",
        "fileId": 0,
        "filename": "string",
        "releaseType": 1,
        "gameVersionTypeId": 0,
        "modLoader": 0
      }
    ],
    "dateCreated": "2019-08-24T14:15:22Z",
    "dateModified": "2019-08-24T14:15:22Z",
    "dateReleased": "2019-08-24T14:15:22Z",
    "allowModDistribution": true,
    "gamePopularityRank": 0,
    "isAvailable": true,
    "thumbsUpCount": 0
  }
}
*/