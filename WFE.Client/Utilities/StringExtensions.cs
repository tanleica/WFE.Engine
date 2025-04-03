using System.Text;
using System.Security.Cryptography;

namespace WFE.Client.Utilities
{
    public static class StringExtensions
    {

        private static readonly byte[] Key = Encoding.UTF8.GetBytes("ItIsSecuredToUseConnectionString"); // Must be 32 bytes for AES-256
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("WhenInProduction"); // Must be 16 bytes for AES

        public static string CoreDecrypt(this string str)
        {
            using Aes aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;

            using MemoryStream memoryStream = new(Convert.FromBase64String(str));
            using CryptoStream cryptoStream = new(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader streamReader = new(cryptoStream);
            return streamReader.ReadToEnd();
        }
    }
}