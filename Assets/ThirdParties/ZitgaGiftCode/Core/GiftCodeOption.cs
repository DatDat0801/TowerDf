using static ZitgaGiftCode.Delegates;

namespace ZitgaGiftCode
{
    public class GiftCodeOption
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public int ApiVersion { get; set; }

        public int GameId { get; set; }

        public string SecretKey { get; set; }

        public GiftCodeCallback CheckGiftCodeCallback { get; set; }

        public GiftCodeCallback ClaimGiftCodeCallback { get; set; }

        public GiftCodeOption(string host, int port)
        {
            Host = host;
            Port = port;
        }
    }
}
