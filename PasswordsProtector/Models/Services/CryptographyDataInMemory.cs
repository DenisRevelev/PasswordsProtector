using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PasswordsProtector.Models.Services
{
    public class CryptographyDataInMemory
    {
        private static byte[] s_additionalEntropy = { 1, 2, 3, 4, 5, 6 };

        public static string EncryptDataInMemory(string encryptText)
        {

            byte[] dataAr = Encoding.Unicode.GetBytes(encryptText);
            byte[] encryptDataAr = ProtectedData.Protect(dataAr, s_additionalEntropy, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptDataAr);
        }

        public static string DecryptDataInMemory(string decryptText)
        {
            byte[] dataAr = Convert.FromBase64String(decryptText);
            byte[] decryptDataAr = ProtectedData.Unprotect(dataAr, s_additionalEntropy, DataProtectionScope.CurrentUser);
            return Encoding.Unicode.GetString(decryptDataAr);
        }
    }
}
