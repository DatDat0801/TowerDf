using Newtonsoft.Json;

namespace ZitgaSaveLoad
{
    public class SaveLoadInbound
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("1")]
        public long PlayerId { get; set; }

        [JsonProperty("2")]
        public string Data { get; set; }
    }
}