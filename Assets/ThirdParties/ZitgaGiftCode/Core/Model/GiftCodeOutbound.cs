using SecurityHelper;
using System;

namespace ZitgaGiftCode
{
    public class GiftCodeOutbound
    {
        public AuthProvider Provider { get; private set; }

        /// <summary>
        /// id which uniquely identify an user
        /// it's can be Google Play Service user id, Apple id...
        /// </summary>
        public string Token { get; private set; }

        public string Code { get; set; }

        public long CreateTime { get; private set; }

        public GiftCodeOutbound(AuthProvider provider, string token)
        {
            Provider = provider;
            Token = token;

            CreateTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        public string GetSignature(int gameId)
        {
            Code= Code.Trim().ToUpper();
            return (int)Provider + Token + Code + gameId + CreateTime;
        }

        public string CreateHash(string secret, int gameId)
        {
            string signature = GetSignature(gameId);
            return HashHelper.HashSHA256(signature + secret);
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Token))
            {
                return false;
            }

            if (string.IsNullOrEmpty(Code))
            {
                return false;
            }

            return true;
        }
    }
}