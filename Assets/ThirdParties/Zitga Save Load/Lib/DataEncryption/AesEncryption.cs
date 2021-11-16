using System.Text;
using System.Security.Cryptography;
using System;
using System.IO;
using UnityEngine;

namespace SecurityHelper
{
    public class AesEncryption
    {
        private AesManaged aes;

        //--------------------------------------------------------------------------------
        public AesEncryption(string password, string saltKey)
        {
            //10000 iterations is recommended by many security experts
            //But this library focuses on providing cross-platform solution, including low processing-power devices such as mobile
            //So I stay at 100 iterations
            //You can freely increase this value to improve security
            int numberIteration = 100;

            Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(saltKey), numberIteration);

            aes = new AesManaged();
            aes.Key = rfc2898.GetBytes(16);
            aes.IV = rfc2898.GetBytes(16);
        }

        //--------------------------------------------------------------------------------
        public string Encrypt(string plain)
        {
            if (plain != null)
            {
                try
                {
                    string encryptedString = string.Empty;

                    using (MemoryStream aesMemoryStream = new MemoryStream())
                    {
                        using (CryptoStream aesCryptoStream = new CryptoStream(aesMemoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            // Encrypt data
                            byte[] data = Encoding.UTF8.GetBytes(plain);
                            aesCryptoStream.Write(data, 0, data.Length);
                            aesCryptoStream.FlushFinalBlock();

                            // Convert encrypted data to base64 string
                            encryptedString = Convert.ToBase64String(aesMemoryStream.ToArray());
                        }
                    }
                    return encryptedString;
                }
                catch (Exception e)
                {
#if UNITY_EDITOR
                    Debug.LogError(e);
#endif
                    return null;
                }

            }
            else
            {
                return null;
            }
        }

        public string Decrypt(string encrypted)
        {
            if (encrypted != null)
            {
                try
                {
                    string decryptedString = string.Empty;

                    using (MemoryStream aesMemoryStream = new MemoryStream())
                    {
                        using (CryptoStream aesCryptoStream = new CryptoStream(aesMemoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            // Convert from base64 string to bytes and decrypt
                            byte[] data = Convert.FromBase64String(encrypted);
                            aesCryptoStream.Write(data, 0, data.Length);
                            aesCryptoStream.FlushFinalBlock();

                            byte[] decryptBytes = aesMemoryStream.ToArray();

                            // Convert decrypted message to string
                            decryptedString = Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);
                        }
                    }

                    return decryptedString;
                }
                catch (Exception e)
                {
#if UNITY_EDITOR
                    Debug.LogError(e);
#endif
                    return null;
                }

            }
            else
            {
                return null;
            }
        }
    }
}