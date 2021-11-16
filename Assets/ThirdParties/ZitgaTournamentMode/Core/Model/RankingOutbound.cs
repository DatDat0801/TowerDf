using System.Collections.Generic;
using Newtonsoft.Json;

namespace ZitgaTournamentMode
{
    public class RankingOutbound
    {
        [JsonProperty("0")] public List<RankingTournament> ListRankingSeasonData { get; set; }

        [JsonProperty("1")] public RankingTournament MyRankingSeasonData { get; set; }

        [JsonProperty("2")] public List<RankingTournament> ListRankingTopPlayerData { get; set; }

        [JsonProperty("3")] public RankingTournament MyRankingTopPlayerData { get; set; }

        [JsonProperty("4")] public long season { get; set; }
    }
}