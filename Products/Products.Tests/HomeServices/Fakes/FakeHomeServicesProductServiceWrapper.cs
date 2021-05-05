using System;
using System.Threading.Tasks;
using Products.Model.HomeServices;
using Products.ServiceWrapper.HomeServicesProductService;
using Products.Tests.HomeServices.Helpers;

namespace Products.Tests.HomeServices.Fakes
{
    public  class FakeHomeServicesProductServiceWrapper : IHomeServicesProductServiceWrapper
    {
        public Exception ServiceException { get; set; }

        public async Task<ProductGroup> GetHomeServiceLandlordProduct(string productCode, string postcode)
        {
            await Task.Delay(1);
            return new ProductGroup();
        }

        public async Task<ProductGroup> GetHomeServiceResidentialProduct(string productCode, string postcode)
        {
            await Task.Delay(1);
            if (ServiceException != null)
                throw ServiceException;

            return FakeHomeServicesProductStub.GetFakeProducts(productCode);
        }
    }
}