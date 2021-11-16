namespace ZitgaTournamentMode
{
    public class TournamentOption
    {
        public string Host { get; private set; }

        public int Port { get; private set; }

        public int ApiVersion { get; set; }

        public string SecretKey { get; set; }

        public Delegates.OnUpdate UpdateRankingTournamentCallback { get; set; }

        public Delegates.OnGet GetRankingTournamentCallback { get; set; }
        
        public Delegates.OnGetInfo GetTournamentInfoCallback { get; set; }

        public TournamentOption(string host, int port)
        {
            Host = host;
            Port = port;
        }
    }
}