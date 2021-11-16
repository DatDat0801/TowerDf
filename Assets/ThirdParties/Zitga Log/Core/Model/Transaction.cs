using Newtonsoft.Json;

namespace ZitgaLog
{
    public class Transaction
    {
        [JsonProperty("0")]
        public int AuthProvider { get; set; }

        [JsonProperty("1")]
        public string AuthToken { get; set; }

        [JsonProperty("2")]
        public string TransactionId { get; set; }

        [JsonProperty("3")]
        public string ProductMetadata { get; set; }

        [JsonProperty("4")]
        public string ProductDefinition { get; set; }

        [JsonProperty("5")]
        public string Receipt { get; set; }

        [JsonProperty("6")]
        public string GameVersion { get; set; }

        [JsonProperty("7")]
        public string DeviceModel { get; set; }

        [JsonProperty("8")]
        public string DeviceName { get; set; }

        [JsonProperty("9")]
        public string DeviceId { get; set; }
    }
}