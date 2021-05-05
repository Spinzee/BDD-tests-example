using System.Threading.Tasks;

namespace Products.Service.HomeServices
{
    public interface IProductService
    {
        Task<bool> GetHomeServicesResidentialProduct(string postcode, string productCode);
        Task<bool> GetHomeServicesLandlordProduct(string postcode, string productCode);
    }
}
