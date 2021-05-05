using Products.Infrastructure;
using Products.Model.Constants;
using Products.Model.HomeServices;
using Products.ServiceWrapper.HomeServicesProductService;
using System;
using System.Threading.Tasks;

namespace Products.Service.HomeServices
{
    public class ProductService : IProductService
    {
        private IHomeServicesProductServiceWrapper _homeServicesProductServiceWrapper;
        private ISessionManager _sessionManager;

        public ProductService(IHomeServicesProductServiceWrapper homeServicesProductServiceWrapper, ISessionManager sessionManager)
        {
            Guard.Against<ArgumentException>(sessionManager == null, $"{nameof(sessionManager)} is null");
            Guard.Against<ArgumentException>(homeServicesProductServiceWrapper == null, $"{nameof(homeServicesProductServiceWrapper)} is null");

            _homeServicesProductServiceWrapper = homeServicesProductServiceWrapper;
            _sessionManager = sessionManager;
        }

        public async Task<bool> GetHomeServicesResidentialProduct(string postcode, string productCode)
        {
            var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            Guard.Against<Exception>(homeServicesCustomer == null, "Home Services customer session object is null");

            var product = await _homeServicesProductServiceWrapper.GetHomeServiceResidentialProduct(productCode, postcode);
            if (product == null)
                return false;

            homeServicesCustomer.AvailableProduct = product;
            return true;
        }

        public async Task<bool> GetHomeServicesLandlordProduct(string postcode, string productCode)
        {
            var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            Guard.Against<Exception>(homeServicesCustomer == null, "Home Services customer session object is null");

            var product = await _homeServicesProductServiceWrapper.GetHomeServiceLandlordProduct(productCode, postcode);
            if (product == null)
                return false;

            homeServicesCustomer.AvailableProduct = product;
            return true;
        }

    }
}
