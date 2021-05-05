namespace Products.Infrastructure
{
    using System;
    using System.Security.Cryptography;

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
            string result = string.Empty;
            if (!string.IsNullOrEmpty(password))
            {
                byte[] salt = GenerateRandomSaltValue();
                string randomIterationCharacter = GetRandomIterationCharacter();
                string iterationText = $"Iteration{randomIterationCharacter}";
                var iterationCount = (IterationCount)Enum.Parse(typeof(IterationCount), iterationText);
                byte[] hash = GetPBKDF2Bytes(password, salt, iterationCount);
                result = string.Concat(randomIterationCharacter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
            }

            return result;
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
    }
}
