namespace Mercurius.API.Models.Curseforge {
    internal struct VersionModel {
        public CurseforgeVersion data { get; set; }
    }
    internal struct VersionModelList {
        public CurseforgeVersion[] data { get; set; }
    }
    internal struct CurseforgeVersion {
        public int id { get; set; }
        public int gameId { get; set; }
        public int modId { get; set; }
        public bool isAvailable { get; set; }
        public string displayName { get; set; }
        public string fileName { get; set; }
        public ReleaseType releaseType { get; set; }
        public FileStatus fileStatus { get; set; }
        public FileHash[] fileHash { get; set; }
        public string fileDate { get; set; }
        public long fileLength { get; set; }
        public long downloadCount  { get; set; }
        public string downloadUrl { get; set; }
        public string[] gameVersions { get; set; }
        public SortableGameVersion[] sortableGameVersions { get; set; }
        public FileDependency[] dependencies { get; set; }
        public bool exposeAsAlternative { get; set; }
        public int parentProjectFileId { get; set; }
        public int alternateFileId { get; set; }
        public bool isServerPack { get; set; }
        public int serverPackFileId { get; set; }
        public bool isEarlyAccessContent { get; set; }
        public string earlyAccessEndDate { get; set; }
        public long fileFingerprint { get; set; }
        public FileModule[] modules { get; set; }
    }
    internal struct FileHash {
        public string value { get; set; }
        public HashAlgo algo { get; set; }
    }
    internal struct SortableGameVersion {
        public string gameVersionName { get; set; }
        public string gameVersionPadded { get; set; }
        public string gameVersion { get; set; }
        public string gameVersionReleaseDate { get; set; }
        public int gameVersionTypeId { get; set; } 	
    }
    internal struct FileDependency {
        public int modId { get; set; }
        public FileRelationType relationType { get; set; }
    }
    internal struct FileModule {
        public string name { get; set; }
        public long fingerprint { get; set; }
    }
    internal enum ReleaseType {
        Release = 1, Beta = 2, Alpha = 3
    }
    internal enum FileStatus {
        Processing = 1,
        ChangesRequired = 2,
        UnderReview = 3,
        Approved = 4,
        Rejected = 5,
        MalwareDetected = 6,
        Deleted = 7,
        Archived = 8,
        Testing = 9,
        Released = 10,
        ReadyForReview = 11,
        Deprecated = 12,
        Baking = 13,
        AwaitingPublishing = 14,
        FailedPublishing = 15
    }
    internal enum HashAlgo {
        Sha1 = 1, Md5 = 2
    }
    internal enum FileRelationType {
        EmbeddedLibrary = 1,
        OptionalDependency = 2,
        RequiredDependency = 3,
        Tool = 4,
        Incompatible = 5,
        Include = 6
    }
}
/*
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
  "isEarlyAccessContent": true,
  "earlyAccessEndDate": "2019-08-24T14:15:22Z",
  "fileFingerprint": 0,
  "modules": [
    {
      "name": "string",
      "fingerprint": 0
    }
  ]
}
*/