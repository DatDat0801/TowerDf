using BestHTTP;

namespace ZitgaLog
{
    public class LogOption
    {
        public string Host { get; private set; }

        public int Port { get; private set; }

        public int ApiVersion { get; set; }

        public string GameVersion { get; set; }

        public string SecretKey { get; set; }

        public OnRequestFinishedDelegate OnTransactionLogFinished { get; set; }

        public LogOption(string host, int port)
        {
            Host = host;
            Port = port;
        }
    }
}