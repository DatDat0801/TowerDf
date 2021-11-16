using static ZitgaUtils.Delegates;

namespace ZitgaUtils
{
    public class ZitgaUtilsOption
    {
        public string Host { get; private set; }

        public int Port { get; private set; }

        public int ApiVersion { get; set; }

        public string SecretKey { get; set; }

        public OnGetLocationInfo GetLocationInfoCallback { get; set; }
        
        public OnGetEventData GetEventDataCallback { get; set; }

        public ZitgaUtilsOption(string host, int port)
        {
            Host = host;
            Port = port;
        }
    }
}