using Newtonsoft.Json;

namespace ZitgaSaveLoad
{
    public class SaveMetadata
    {
        [JsonProperty("0")]
        public int SnapshotType { get; set; }

        [JsonProperty("1")]
        public string Description { get; set; }

        [JsonProperty("2")]
        public string GameVersion { get; set; }

        [JsonProperty("3")]
        public string DeviceModel { get; set; }

        [JsonProperty("4")]
        public string DeviceName { get; set; }

        [JsonProperty("5")]
        public string DeviceId { get; set; }

        [JsonProperty("6")]
        public long CreatedTime { get; set; }

        [JsonProperty("7")]
        public string Hash { get; set; }
    }
}