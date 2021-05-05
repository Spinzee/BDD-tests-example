using Products.Infrastructure;
using Products.Model.Constants;
using Sse.Ecommerce.Encryption;
using System;

namespace Products.Service.Security
{
    public class CryptographyService : ICryptographyService
    {
        private readonly IConfigManager _configManager;

        public CryptographyService(IConfigManager configManager)
        {
            Guard.Against<Exception>(configManager == null, "Config Manager is null");
            _configManager = configManager;
        }

        public string EncryptBroadbandValue(string valueToEncrypt)
        {
            var signupEncryptPublicKeyData = _configManager.GetValueForKeyFromSection(ConfigSectionGroups.Encryption, "encryptionKeys", "BroadbandSignupEncryptPublicKey");
            return Encrypt(valueToEncrypt, signupEncryptPublicKeyData);
        }

        public string EncryptHomeServicesValue(string valueToEncrypt)
        {
            var signupEncryptPublicKeyData = _configManager.GetValueForKeyFromSection(ConfigSectionGroups.Encryption, "encryptionKeys", "HomeServicesSignupEncryptPublicKey");
            return Encrypt(valueToEncrypt, signupEncryptPublicKeyData);
        }

        public string EncryptEnergyValue(string valueToEncrypt)
        {
            var signupEncryptPublicKeyData = _configManager.GetValueForKeyFromSection(ConfigSectionGroups.Encryption, "encryptionKeys", "EnergySignupEncryptPublicKey");
            return Encrypt(valueToEncrypt, signupEncryptPublicKeyData);
        }

        private string Encrypt(string valueToEncrypt, string encryptionKey)
        {
            return Convert.ToBase64String(TripleDES.Encrypt(valueToEncrypt, encryptionKey));
        }
    }
}