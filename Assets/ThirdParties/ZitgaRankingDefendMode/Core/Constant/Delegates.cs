namespace ZitgaRankingDefendMode
{
    public class Delegates
    {
        public delegate void OnUpdate(int logicCode);

        public delegate void OnGet(int logicCode, RankingDefenseOutbound rankingData);
    }
}