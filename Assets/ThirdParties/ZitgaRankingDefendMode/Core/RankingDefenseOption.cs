namespace ZitgaRankingDefendMode
{
    public class RankingDefenseOption
    {
        public string Host { get; private set; }

        public int Port { get; private set; }

        public int ApiVersion { get; set; }

        public string SecretKey { get; set; }

        public Delegates.OnUpdate UpdateRankingDefenseCallback { get; set; }

        public Delegates.OnGet GetRankingDefenseCallback { get; set; }

        public RankingDefenseOption(string host, int port)
        {
            Host = host;
            Port = port;
        }
    }
}