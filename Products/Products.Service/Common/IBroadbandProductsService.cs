namespace Products.Service.Common
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Products.Model.Broadband;
    using Products.Model.Common;

    public interface IBroadbandProductsService
    {
        Task<List<BroadbandProduct>> GetAvailableBroadbandProducts(BTAddress selectedAddress, string cliNumber);

        Task<List<BTAddress>> GetAddressesForPostcode(string postCode);
    }
}
