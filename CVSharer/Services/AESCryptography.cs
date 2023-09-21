using System.Text;
using System;
using System.Security.Cryptography;

namespace CVSharer.Services
{
    public class AESCryptography
    {
        private static byte[] IV = { 4, 6, 2, 7, 6, 0, 9, 4, 2, 3, 5, 9, 5, 3, 9, 1 };

        public static string Encrypt(string plaintText, string password)
        {
            byte[] key = Encoding.UTF8.GetBytes(password);

            //Create AES class
            AesManaged aes = new AesManaged();
            aes.Key = key;
            aes.IV = IV;

            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);

            byte[] input = Encoding.UTF8.GetBytes(plaintText);
            cryptoStream.Write(input, 0, input.Length);
            cryptoStream.FlushFinalBlock();

            byte[] encrypted = memoryStream.ToArray();

            return Convert.ToBase64String(encrypted);
        }

        public static string Decrypt(string encrypted, string password)
        {
            byte[] key = Encoding.UTF8.GetBytes(password);

            //Create AES class
            AesManaged aes = new AesManaged();
            aes.Key = key;
            aes.IV = IV;

            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write);

            byte[] input = Convert.FromBase64String(encrypted);
            cryptoStream.Write(input, 0, input.Length);
            cryptoStream.FlushFinalBlock();

            byte[] decrypted = memoryStream.ToArray();

            return UTF8Encoding.UTF8.GetString(decrypted, 0, decrypted.Length);
        }
    }
}
