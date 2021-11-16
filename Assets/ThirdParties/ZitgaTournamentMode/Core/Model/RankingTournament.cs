using System.Collections.Generic;
using EW2;
using Newtonsoft.Json;
using ZitgaRankingDefendMode;

namespace ZitgaTournamentMode
{
    public class RankingTournament
    {
        [JsonProperty("0")] public int Avatar { get; set; }

        [JsonProperty("1")] public int AvatarFrame { get; set; }

        [JsonProperty("2")] public string Country { get; set; }

        [JsonProperty("3")] public string Name { get; set; }

        [JsonProperty("4")] public List<HeroUseDefend> ListOfHeroes { get; set; }

        [JsonProperty("6")] public int WaveCleared { get; set; }

        [JsonProperty("7")] public long Season { get; set; }

        [JsonProperty("8")] public int CreepCleared { get; set; }

        [JsonProperty("50")] public int RankSeason { get; set; }

        [JsonProperty("30")] public int RankTopPlayer { get; set; }

        [JsonProperty("51")] public int RewardStatus { get; set; }
        
        public bool IsShowMore { get; set; }

        public static RankingTournament CreateRankingEmpty()
        {
            var ranking = new RankingTournament();
            ranking.Avatar = UserData.Instance.AccountData.avatarId;
            ranking.Name = UserData.Instance.AccountData.name;
            ranking.ListOfHeroes = new List<HeroUseDefend>();
            foreach (var hero in UserData.Instance.UserHeroData.GetSelectedHeroItems())
            {
                ranking.ListOfHeroes.Add(new HeroUseDefend() {HeroId = hero.heroId, HeroLevel = hero.level});
            }

            ranking.RankSeason = -1;
            ranking.RankTopPlayer = -1;
            
            return ranking;
        }
    }
}