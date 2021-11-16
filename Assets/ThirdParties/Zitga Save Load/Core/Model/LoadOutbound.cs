using SecurityHelper;
using System;

namespace ZitgaSaveLoad
{
    public class LoadOutbound
    {
        public AuthProvider Provider { get; private set; }

        /// <summary>
        /// id which unique identify user
        /// it's can be Google Play Service user id, Apple id...
        /// </summary>
        public string Token { get; private set; }

        public long CreatedTime { get; private set; }

        public LoadOutbound(AuthProvider provider, string token)
        {
            Provider = provider;
            Token = token;

            CreatedTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        public string GetSignature(string gameVersion)
        {
            return gameVersion + CreatedTime;
        }

        public string CreateHash(string secret, string gameVersion)
        {
            string signature = GetSignature(gameVersion);
            return HashHelper.HashSHA256((int)Provider + Token + signature + secret);
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