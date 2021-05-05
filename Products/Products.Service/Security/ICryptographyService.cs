namespace Products.Service.Security
{
    public interface ICryptographyService
    {
        string EncryptBroadbandValue(string valueToEncrypt);
        string EncryptHomeServicesValue(string valueToEncrypt);
        string EncryptEnergyValue(string valueToEncrypt);
    }
}