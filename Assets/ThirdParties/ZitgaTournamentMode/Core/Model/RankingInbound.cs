using Newtonsoft.Json;

namespace ZitgaTournamentMode
{
    public class RankingInbound
    {
        [JsonProperty("0")] public RankingTournament Ranking { get; set; }
    }
}