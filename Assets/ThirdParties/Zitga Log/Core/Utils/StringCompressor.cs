using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace ZitgaLog
{
    public class StringCompressor
    {
        /// <summary>
        /// Compresses the string.
        /// </summary>
        /// <param name="plainText">The text.</param>
        /// <returns></returns>
        public static string CompressString(string plainText)
        {
            byte[] inputData = Encoding.UTF8.GetBytes(plainText);
            using (var memoryStream = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    gZipStream.Write(inputData, 0, inputData.Length);
                }

                var compressedData = memoryStream.ToArray();
                return Convert.ToBase64String(compressedData);
            }
        }

        /// <summary>
        /// Decompresses the string.
        /// </summary>
        /// <param name="compressedText">The compressed text.</param>
        /// <returns></returns>
        public static string DecompressString(string compressedText)
        {
            byte[] inputData = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream(inputData))
            {
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    using (var outputStream = new MemoryStream())
                    {
                        gZipStream.CopyTo(outputStream);

                        var decompressedData = outputStream.ToArray();
                        return Encoding.UTF8.GetString(decompressedData);
                    }
                }
            }
        }
    }
}