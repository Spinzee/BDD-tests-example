namespace Products.ServiceWrapper.EnergyProductService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Products.Model.Energy;
    
    public interface IEnergyProductServiceWrapper
    {
        Task<List<Product>> GetEnergyProducts(ProductsRequest eligibility);

        Task<string> GetGeoAreaForPostCode(string postCode);

        Task<List<Product>> GetOurPricesProducts(string postCode, string tariffStatus, string fuelCategory);
    }
}