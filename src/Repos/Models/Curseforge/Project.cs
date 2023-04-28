namespace Mercurius.API.Models.Curseforge {
    internal struct ProjectModel {
        public CurseforgeProject data { get; set; }
    }
    internal struct CurseforgeProject {
        public int id { get; set; }
        public int gameId { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public Links links { get; set; }
        public string summary { get; set; }
        public int status { get; set; }
        public int downloadCount { get; set; }
        public bool isFeatured { get; set; }
        public string primaryCategoryId { get; set; }
        public Category[] categories { get; set; }
        public int classId { get; set; }
        public Author[] authors{ get; set; } 
        public Logo logo { get; set; }
        public Screenshot[] screenshots { get; set; }
        public int mainFileId { get; set; }
        public ShortFile[] latestFiles { get; set; }
        public FileIndex[] latestFileIndexes { get; set; }
        public string dateCreated { get; set; }
        public string dateModified { get; set; }
        public string dateReleased { get; set; }
        public bool allowModDistribution { get; set; }
        public int gamePopularityRank { get; set; }
        public bool isAvailable { get; set; }
        public int thumbsUpCount { get; set; }
    }
    internal struct Links {
        public string websiteUrl { get; set; }
        public string wikiUrl { get; set; }
        public string issuesUrl { get; set; }
        public string sourceUrl { get; set; }
    }
    internal struct Category {
        public int id { get; set; }
        public int gameId { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string url { get; set; }
        public string iconUrl { get; set; }
        public string dateModified { get; set; }
        public bool isClass { get; set; }
        public int classId { get; set; }
        public int parentCategoryId { get; set; }
    }
    internal struct Author {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
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
        public int id { get; set; }
        public int gameId { get; set; }
        public int modId { get; set; }
        public bool isAvailable { get; set; }
        public string displayName { get; set; }
        public string fileName { get; set; }
        public int releaseType { get; set; }
        public int fileStatus { get; set; }
        public Hash[] hashes { get; set; }
        public string fileDate { get; set; }
        public int fileLength { get; set; }
        public int downloadCount { get; set; }
        public string downloadUrl { get; set; }
        public string[] gameVersions { get; set; }
        public GameVersion[] sortableGameVersions { get; set; }
        public Dependency[] dependencies { get; set; }
        public int alternateFileId { get; set; }
        public bool isServerPack { get; set; }
        public int fileFingerprint { get; set; }
        public Module[] modules { get; set; }
    }
    internal struct Hash {
        public string value { get; set; }
        public int algo { get; set; }
    }
    internal struct GameVersion {
        public string gamerVersionName { get; set; }
        public string gameVersionPadded { get; set; }
        public string gameVersion { get; set; }
        public string gameVersionReleaseDate { get; set; }
        public int gameVersionTypeId { get; set; }
    }
    internal struct Dependency {}
    internal struct Module {
        public string name { get; set; }
        public string fingerprint { get; set; }
    }
    internal struct FileIndex {
        public string gameVersion { get; set; }
        public int fileId { get; set; }
        public string fileName { get; set; }
        public int releaseType { get; set; }
        public int gameVersionTypeId { get; set; }
        public int modLoader { get; set; }
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