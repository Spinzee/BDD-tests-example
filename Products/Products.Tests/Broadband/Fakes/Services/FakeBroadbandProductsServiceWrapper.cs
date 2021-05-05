namespace Products.Tests.Broadband.Fakes.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Products.Model.Broadband;
    using Products.Model.Common;
    using ServiceWrapper.BroadbandProductsService;
    using Model;
    using Tariff = Products.Model.Broadband.Tariff;

    public class FakeBroadbandProductsServiceWrapper : IBroadbandProductsServiceWrapper
    {
        public List<BTAddress> Addresses { get; set; }

        public AddressResult AddressResult { get; set; }

        public BroadbandProductsResult BroadbandProductsResult { get; set; }

        public FakeBroadbandProductsData.FakeLineSpeed FakeLineSpeed { get; set; }

        public int WebServiceCalled { get; set; }

        public FakeBroadbandProductsServiceWrapper()
        {
            WebServiceCalled = 0;            
        }

        public void SetAddressResult(string subPremises, string premiseName, string throughFareNumber, string throughFareName)
        {
            Addresses = new List<BTAddress> { new BTAddress { SubPremises = subPremises, PremiseName = premiseName, ThoroughfareNumber = throughFareNumber, ThoroughfareName = throughFareName} };
        }

        public async Task<List<BTAddress>> GetAddressesForPostcode(string postcode)
        {
            await Task.Delay(1);
            if (Addresses?.Count > 0)
                return Addresses;            
            return FakeBroadbandProductsData.GetAddresses(AddressResult);
        }

        public async Task<List<Tariff>> GetAllTariffs(string brandId)
        {
            await Task.Delay(1);
            WebServiceCalled++;

            // ReSharper disable once SuggestVarOrType_Elsewhere
            var tariffs = FakeBroadbandProductsData.GetAllTariffs();
            return BroadbandProductsServiceMapper.ToTariffs(tariffs);
        }
        
        public async Task<BroadbandTariffsForAddress> GetAvailableTariffs(string brandId, BTAddress address, string cli)
        {
            await Task.Delay(1);
            WebServiceCalled++;
            ProductsResponse response = FakeBroadbandProductsData.GetBroadbandProducts(BroadbandProductsResult, FakeLineSpeed);
            return BroadbandProductsServiceMapper.ToAvailableTariffs(response);
        }
    }
}
