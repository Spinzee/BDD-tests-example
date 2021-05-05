using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Infrastructure;

namespace Services
{
    internal enum IterationCount
    {
        IterationA = 28746,
        IterationB = 26981,
        IterationC = 29165,
        IterationD = 25009,
        IterationE = 27312,
        IterationF = 25874,
        IterationG = 28251,
        IterationH = 29739,
        IterationI = 26892,
        IterationJ = 27624
    }
    public class PasswordService : IPasswordService
    {
        private const int SaltByteSize = 32;
        private const int HashByteSize = 32;
        public const int SaltStringPosition = 1;
        public const int SaltAndHashStringLength = 44;
        public const int Pbkdf2StringPosition = 45;

        public string HashPasswordPBKDF2(string password)
        {
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(password), "password");

            //Slowing down the algorithm(usually by iteration) make the attacker's job much harder.
            //Password hashes also add a salt value to each hash to make it unique so that an attacker can not attack multiple hashes at the same time.
            //With regular cryptographic hash functions(e.g.MD5, SHA256), an attacker can guess billions of passwords per second.
            //With PBKDF2, bcrypt, or scrypt, the attacker can only make a few thousand guesses per second(or less, depending on the configuration).

            byte[] salt = GenerateRandomSaltValue();
            string randomIterationCharacter = GetRandomIterationCharacter();
            string iterationText = $"Iteration{randomIterationCharacter}";
            var iterationCount = (IterationCount)Enum.Parse(typeof(IterationCount), iterationText);
            byte[] hash = GetPBKDF2Bytes(password, salt, iterationCount);
            var result = string.Concat(randomIterationCharacter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));

            return result;
        }

        public bool AuthenticatePBKDF2(string enteredPassword, string accountHashedPassword)
        {
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(enteredPassword), "enteredPassword");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(accountHashedPassword), "profileHashedPassword");

            string iterationCharacter = accountHashedPassword.Substring(0, 1);
            string iterationText = $"Iteration{iterationCharacter}";
            var iterationCount = (IterationCount)Enum.Parse(typeof(IterationCount), iterationText);
            byte[] saltByteArray = Convert.FromBase64String(accountHashedPassword.Substring(SaltStringPosition, SaltAndHashStringLength));
            byte[] hashByteArray = Convert.FromBase64String(accountHashedPassword.Substring(Pbkdf2StringPosition, SaltAndHashStringLength));
            byte[] testHashArray = GetPBKDF2Bytes(enteredPassword, saltByteArray, iterationCount);
            bool isPasswordValid = CompareHashes(hashByteArray, testHashArray);

            return isPasswordValid;
        }

        private static byte[] GenerateRandomSaltValue()
        {
            var salt = new byte[SaltByteSize];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(salt);
            return salt;
        }

        private static string GetRandomIterationCharacter()
        {
            var rnd = new Random();
            int number = Math.Abs(rnd.Next(1, 11));
            return char.ConvertFromUtf32(64 + number);
        }

        private static byte[] GetPBKDF2Bytes(string password, byte[] salt, IterationCount iterationCount)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt) { IterationCount = (int)iterationCount };
            return pbkdf2.GetBytes(HashByteSize);
        }

        private static bool CompareHashes(IReadOnlyList<byte> hashA, IReadOnlyList<byte> hashB)
        {
            if (hashA.Count == hashB.Count)
            {
                var i = 0;
                while (i < hashA.Count && (hashA[i] == hashB[i]))
                {
                    i++;
                }

                if (i == hashA.Count)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
