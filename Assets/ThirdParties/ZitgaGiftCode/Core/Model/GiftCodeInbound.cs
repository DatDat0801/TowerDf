using Newtonsoft.Json;

namespace ZitgaGiftCode
{
    public class GiftCodeInbound
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }
    }
}