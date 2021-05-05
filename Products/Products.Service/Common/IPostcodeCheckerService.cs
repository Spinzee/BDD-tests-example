namespace Products.Service.Common
{
    public interface IPostcodeCheckerService
    {
        bool IsNorthernIrelandPostcode(string postcode);
        bool IsScottishPostcode(string postcode);
    }
}