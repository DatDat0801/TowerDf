using static ZitgaSaveLoad.Delegates;

namespace ZitgaSaveLoad
{
    public class SaveLoadOption
    {
        public string Host { get; private set; }

        public int Port { get; private set; }

        public int ApiVersion { get; set; }

        public string GameVersion { get; set; }

        public string SecretKey { get; set; }

        public OnSave SaveCallback { get; set; }

        public OnLoad LoadCallback { get; set; }

        public SaveLoadOption(string host, int port)
        {
            Host = host;
            Port = port;
        }
    }
}