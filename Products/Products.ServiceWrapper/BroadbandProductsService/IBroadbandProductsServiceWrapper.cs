namespace Products.ServiceWrapper.BroadbandProductsService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Products.Model.Broadband;
    using Products.Model.Common;

    public interface IBroadbandProductsServiceWrapper
    {
        Task<List<BTAddress>> GetAddressesForPostcode(string postcode);

        Task<List<Model.Broadband.Tariff>> GetAllTariffs(string brandId);

        Task<BroadbandTariffsForAddress> GetAvailableTariffs(string brandId, BTAddress address, string cli);
    }
}