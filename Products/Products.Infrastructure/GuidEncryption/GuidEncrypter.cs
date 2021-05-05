using System;

namespace Products.Infrastructure.GuidEncryption
{
    public interface IGuidEncrypter
    {
        string Encrypt(Guid plainGuid);
        Guid Decrypt(string encryptedGuid);
    }

    public class GuidEncrypter : IGuidEncrypter
    {
        private const int FirstGuidBracket = 0;
        private const int LastGuidBracket = 37;
        private const int FirstGuidHyphen = 9;
        private const int SecoundGuidHyphen = 14;
        private const int ThirdGuidHyphen = 19;
        private const int ForthGuidHyphen = 24;

        private readonly Encrypter _encrypt;

        public GuidEncrypter()
        {
            _encrypt = new Encrypter();
        }

        public string Encrypt(Guid plainGuid)
        {
            WrappingString secureGuidFormat = new WrappingString(plainGuid.ToString("D"));
            secureGuidFormat.Insert(FirstGuidBracket, '1');
            secureGuidFormat.Insert(LastGuidBracket, '1');
            secureGuidFormat.Replace('-', '0');

            return _encrypt.Encrypt(secureGuidFormat.ToString());
        }

        public Guid Decrypt(string encryptedGuid)
        {
            try
            {
                string decryptedMessage = _encrypt.Decrypt(encryptedGuid).ToUpper();
                if (IsEncryptedFormatValid(decryptedMessage))
                {
                    WrappingString decryptedGuid = new WrappingString(decryptedMessage);
                    decryptedGuid.Replace('1', '{', FirstGuidBracket, 1);
                    decryptedGuid.Replace('1', '}', LastGuidBracket, 1);
                    decryptedGuid.Replace('0', '-', FirstGuidHyphen, 1);
                    decryptedGuid.Replace('0', '-', SecoundGuidHyphen, 1);
                    decryptedGuid.Replace('0', '-', ThirdGuidHyphen, 1);
                    decryptedGuid.Replace('0', '-', ForthGuidHyphen, 1);

                    return new Guid(decryptedGuid.ToString());
                }

                throw new Exception("Invalid Guid Format");
            }
            catch (Exception)
            {
                throw new Exception("GUID Decryption Failed");
            }
        }

        private bool IsEncryptedFormatValid(string encryptedGuid)
        {
            char[] encryptedGuidArray = encryptedGuid.ToCharArray();
            return encryptedGuid.StartsWith("1")
                   && encryptedGuid.EndsWith("1")
                   && encryptedGuidArray[FirstGuidHyphen] == '0'
                   && encryptedGuidArray[SecoundGuidHyphen] == '0'
                   && encryptedGuidArray[ThirdGuidHyphen] == '0'
                   && encryptedGuidArray[ForthGuidHyphen] == '0';
        }
    }
}
