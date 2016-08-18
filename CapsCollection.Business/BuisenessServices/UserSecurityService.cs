using System;
using System.Security.Cryptography;
using CapsCollection.Business.BuisenessServices.Interfaces;

namespace CapsCollection.Business.BuisenessServices
{
    public class UserSecurityService : IUserSecurityService
    {
        public string GenerateSalt()
        {
            var saltBytes = new byte[16];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(saltBytes);

            return Convert.ToBase64String(saltBytes);
        }

        public string CalculatePasswordHash(string password, string salt)
        {
            if (String.IsNullOrEmpty(password) || String.IsNullOrEmpty(salt))
            {
                throw new ArgumentNullException();
            }

            var saltBytes = Convert.FromBase64String(salt);
            var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000);
            byte[] hashBytes = pbkdf2.GetBytes(20);

            return Convert.ToBase64String(hashBytes);
        }

        public bool CheckPassword(string hashString, string saltString, string passwordToCheck)
        {
            if (String.IsNullOrEmpty(hashString) || String.IsNullOrEmpty(saltString) || String.IsNullOrEmpty(passwordToCheck))
            {
                throw new ArgumentNullException();
            }

            byte[] hashBytes = Convert.FromBase64String(hashString);
            byte[] saltBytes = Convert.FromBase64String(saltString);

            return CheckPassword(hashBytes, saltBytes, passwordToCheck);
        }

        public bool CheckPassword(byte[] hashBytes, byte[] saltBytes, string passwordToCheck)
        {
            if (hashBytes.Length == 0 || saltBytes.Length == 0 || String.IsNullOrEmpty(passwordToCheck))
            {
                throw new ArgumentNullException();
            }

            byte[] hashBytesWithSalt = new byte[36];
            Array.Copy(saltBytes, 0, hashBytesWithSalt, 0, 16);
            Array.Copy(hashBytes, 0, hashBytesWithSalt, 16, 20);

            var pbkdf2 = new Rfc2898DeriveBytes(passwordToCheck, saltBytes, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (hashBytesWithSalt[i + 16] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
