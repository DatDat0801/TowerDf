namespace ZitgaTournamentMode
{
    public class Delegates
    {
        public delegate void OnUpdate(int logicCode);

        public delegate void OnGet(int logicCode, RankingOutbound rankingData);
        
        public delegate void OnGetInfo(int logicCode, TournamentInfoOutbound tournamentInfo);
    }
}