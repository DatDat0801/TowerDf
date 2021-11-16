using Newtonsoft.Json;

namespace ZitgaUtils
{
    public class LocationInbound
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("0")]
        public string SenderAddress { get; set; }

        [JsonProperty("1")]
        public int ContinentCode { get; set; }

        [JsonProperty("2")]
        public string ContinentName { get; set; }

        [JsonProperty("3")]
        public int CountryCode { get; set; }

        [JsonProperty("4")]
        public string CountryName { get; set; }

        [JsonProperty("5")]
        public int CityCode { get; set; }

        [JsonProperty("6")]
        public string CityName { get; set; }

        [JsonProperty("7")]
        public long CurrentTimeInMillisecond { get; set; }

        public LocationInbound()
        {
            this.Code = LogicCode.FAIL;
        }
    }
}