namespace Products.ServiceWrapper.HomeServicesProductService
{
    using System.Threading.Tasks;
    using Products.Model.HomeServices;

    public interface IHomeServicesProductServiceWrapper
    {
        Task<ProductGroup> GetHomeServiceResidentialProduct(string productCode, string postcode);

        Task<ProductGroup> GetHomeServiceLandlordProduct(string productCode, string postcode);
    }
}