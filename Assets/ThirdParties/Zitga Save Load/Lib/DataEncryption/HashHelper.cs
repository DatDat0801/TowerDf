using System.Security.Cryptography;
using System.Text;

namespace SecurityHelper
{
    public class HashHelper
    {
        const string stringFormat = "x2";

        public static string HashMD5(string input)
        {
            if (input == null)
            {
                return null;
            }

            using (MD5 helper = MD5.Create())
            {
                byte[] crypto = helper.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder hash = new StringBuilder();
                for (int i = 0; i < crypto.Length; i++)
                {
                    hash.Append(crypto[i].ToString(stringFormat));
                }

                return hash.ToString().ToLower();
            }
        }

        public static string HashSHA256(string input)
        {
            if (input == null)
            {
                return null;
            }

            using (SHA256 helper = SHA256.Create())
            {
                byte[] crypto = helper.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder hash = new StringBuilder();
                for (int i = 0; i < crypto.Length; i++)
                {
                    hash.Append(crypto[i].ToString(stringFormat));
                }

                return hash.ToString().ToLower();
            }
        }
    }
}