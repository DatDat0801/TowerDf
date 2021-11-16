using ZitgaSaveLoad;

namespace ZitgaUtils
{
    public class GetEventDataInbound
    {
        public AuthProvider Provider { get; private set; }

        public string Token { get; private set; }

        public void SetData(AuthProvider provider, string token)
        {
            Provider = provider;
            Token = token;
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Token))
            {
                return false;
            }

            return true;
        }
    }
}