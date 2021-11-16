using System.Collections.Generic;
using Newtonsoft.Json;

namespace ZitgaTournamentMode
{
    public class TournamentInfoOutbound
    {
        [JsonProperty("0")] public int StatNerfId { get; set; }
        [JsonProperty("1")] public int StatBuffId { get; set; }
        [JsonProperty("2")] public int StatBanId { get; set; }
        [JsonProperty("3")] public List<int> HeroBuffIds { get; set; }
        [JsonProperty("4")] public int HeroNerfId { get; set; }
        [JsonProperty("5")] public long Season { get; set; }
    }
}