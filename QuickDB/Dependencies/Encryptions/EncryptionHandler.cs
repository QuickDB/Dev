using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using QuickDB.Core.Models;
using QuickDB.Dependencies.Contracts;


namespace QuickDB.Dependencies.Encryptions
{
    public class EncryptionHandler : AbstractEncryptionHandler
    {

        public EncryptionHandler(ISecurityStrings securityStrings)
        {
            if (securityStrings == null) throw new ArgumentNullException("securityStrings");
            SecurityStrings = securityStrings;
        }

        protected override sealed ISecurityStrings SecurityStrings { get; set; }

        public override string EnCrypt(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            var keyBytes = new Rfc2898DeriveBytes(SecurityStrings.PasswordHash, Encoding.ASCII.GetBytes(SecurityStrings.SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(SecurityStrings.VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        public override string DeCrypt(string encryptedText)
        {
            var cipherTextBytes = Convert.FromBase64String(encryptedText);
            var keyBytes = new Rfc2898DeriveBytes(SecurityStrings.PasswordHash, Encoding.ASCII.GetBytes(SecurityStrings.SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(SecurityStrings.VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            var plainTextBytes = new byte[cipherTextBytes.Length];

            var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
    }
}