using System.Collections.Generic;
using Newtonsoft.Json;

namespace ZitgaRankingDefendMode
{
    public class RankingDefenseInbound
    {
        [JsonProperty("0")] public RankingDefense RankingData { get; set; }
    }

    public class RankingDefenseOutbound
    {
        [JsonProperty("0")] public List<RankingDefense> ListRankingData { get; set; }
        [JsonProperty("1")] public RankingDefense RankingData { get; set; }
    }

    public class RankingDefense
    {
        [JsonProperty("0")] public int Avatar { get; set; }

        [JsonProperty("1")] public int AvatarFrame { get; set; }

        [JsonProperty("2")] public string Country { get; set; }

        [JsonProperty("3")] public string Name { get; set; }

        [JsonProperty("4")] public List<HeroUseDefend> ListOfHeroes { get; set; }

        [JsonProperty("5")] public int DefensivePoint { get; set; }

        [JsonProperty("6")] public int WaveCleared { get; set; }

        [JsonProperty("20")] public int Rank { get; set; }
    }

    public class HeroUseDefend
    {
        [JsonProperty("0")] public int HeroId { get; set; }

        [JsonProperty("1")] public int HeroLevel { get; set; }
    }
}