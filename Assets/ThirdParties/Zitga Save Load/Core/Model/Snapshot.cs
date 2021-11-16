using Newtonsoft.Json;

namespace ZitgaSaveLoad
{
    public class Snapshot
    {
        [JsonProperty("1")]
        public long PlayerId { get; set; }

        [JsonProperty("2")]
        public string Data { get; set; }
    }
}