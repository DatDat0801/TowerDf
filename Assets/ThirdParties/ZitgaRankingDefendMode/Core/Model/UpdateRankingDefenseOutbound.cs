using SecurityHelper;
using UnityEngine;
using ZitgaSaveLoad;

namespace ZitgaRankingDefendMode
{
    public class UpdateRankingDefenseOutbound
    {
        public AuthProvider Provider { get; private set; }

        public string Token { get; private set; }

        public string Data { get; set; }

        public UpdateRankingDefenseOutbound(AuthProvider provider, string token)
        {
            Provider = provider;
            Token = token;
        }

        public void SetData(string data)
        {
            Data = StringCompressor.CompressString(data);
        }

        public string CreateHash(string secret)
        {
            Debug.Log("Provider: " + (int)Provider);
            Debug.Log("Token: " + Token);
            Debug.Log("Data: " + Data);
            Debug.Log("Secret: " + secret);

            return HashHelper.HashSHA256((int)Provider + Token + Data + secret);
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Token))
            {
                return false;
            }

            if (string.IsNullOrEmpty(Data))
            {
                return false;
            }

            return true;
        }
    }
}