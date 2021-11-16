using System;
using UnityEngine;

namespace SecurityHelper
{
    public class DataEncryption
    {
        public enum HASH_MODE
        {
            MODE_0_NO_HASH = 0,

            MODE_1_MD5 = 1,

            MODE_2_SHA256 = 2,
        }

        char[] delimiters = new char[] { '.' };
        char delimiter = '.';

        AesEncryption aesEncryption;

        public HASH_MODE encryptionHashMode { get; set; }
        public HASH_MODE decryptionHashMode { get; set; }

        //--------------------------------------------------------------------------------
        public DataEncryption(string password, string saltKey)
        {
            aesEncryption = new AesEncryption(password, saltKey);

            encryptionHashMode = HASH_MODE.MODE_1_MD5;
            decryptionHashMode = HASH_MODE.MODE_1_MD5;
        }

        //--------------------------------------------------------------------------------
        public string Encrypt(string plain)
        {
            string encrypted = aesEncryption.Encrypt(plain);

            switch (encryptionHashMode)
            {
                case HASH_MODE.MODE_1_MD5:
                    encrypted = HashHelper.HashMD5(plain) + delimiter + encrypted;
                    break;

                case HASH_MODE.MODE_2_SHA256:
                    encrypted = HashHelper.HashSHA256(plain) + delimiter + encrypted;
                    break;
            }

            return encrypted;
        }

        public string Decrypt(string encrypted)
        {
            if (decryptionHashMode != HASH_MODE.MODE_0_NO_HASH)
            {
                string[] parts = encrypted.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2)
                {
                    string plain = aesEncryption.Decrypt(parts[1]);
                    string hash = parts[0];

                    if (Hash(plain, decryptionHashMode) == hash)
                    {
                        return plain;
                    }
                    else
                    {
#if UNITY_EDITOR
                        Debug.LogError("Hash failed");
#endif
                        return null;
                    }
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogError("Invalid data");
#endif
                    return null;
                }
            }
            else
            {
                return aesEncryption.Decrypt(encrypted);
            }
        }

        public string Hash(string plain, HASH_MODE hashMode)
        {
            switch (hashMode)
            {
                case HASH_MODE.MODE_1_MD5:
                    return HashHelper.HashMD5(plain);

                case HASH_MODE.MODE_2_SHA256:
                    return HashHelper.HashSHA256(plain);

                default:
                    return string.Empty;
            }
        }
    }
}