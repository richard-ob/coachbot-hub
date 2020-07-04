using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CoachBot.Tools
{
    public static class EncryptionTools
    {
        public static string Encrypt(string plainText, byte[] key, byte[] iv)
        {
            byte[] encrypted;

            using (var aesAlgo = Aes.Create())
            {
                aesAlgo.Key = key;
                aesAlgo.IV = iv;

                var encryptor = aesAlgo.CreateEncryptor(aesAlgo.Key, aesAlgo.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }

                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return Encoding.UTF8.GetString(encrypted);
        }

        public static string Decrypt(string text, byte[] key, byte[] iv)
        {
            var cipherText = Encoding.UTF8.GetBytes(text);

            string result;
            using (var aesAlgo = Aes.Create())
            {
                aesAlgo.Key = key;
                aesAlgo.IV = iv;

                var decryptor = aesAlgo.CreateDecryptor(aesAlgo.Key, aesAlgo.IV);
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            result = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return result;
        }

        public static byte[] GetRandomData(int bits)
        {
            var result = new byte[bits / 8];
            RandomNumberGenerator.Create().GetBytes(result);
            return result;
        }
    }
}
