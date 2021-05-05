using Products.Model.Energy;
using Products.ServiceWrapper.EnergyProductService;
using Products.Tests.Energy.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.Tests.Energy.Fakes.Services
{
    public class FakeProductServiceWrapper : IEnergyProductServiceWrapper
    {
        public bool ThrowException { get; set; }

        public ProductsRequest ProductsRequest { get; private set; }

        public bool ReturnSingleDualFuelFakeProduct { get; set; }

        public bool ReturnNoProducts { get; set; }
        
        public string GeoAreaForPostCode { get; set; } = "_H";

        public async Task<List<Product>> GetEnergyProducts(ProductsRequest productsRequest)
        {
            if (ThrowException)
            {
                throw new Exception("EnergyQuotationService Exception");
            }

            if (ReturnNoProducts)
            {
                return new List<Product>();
            }

            ProductsRequest = productsRequest;
             
            List<Product> productList = ReturnSingleDualFuelFakeProduct ? FakeProductsStub.GetSingleDualFuelFakeProduct() : FakeProductsStub.GetFakeProducts(productsRequest.FuelType, productsRequest.MeterType);

            return await Task.FromResult(productList);
        }


        public async Task<string> GetGeoAreaForPostCode(string postCode)
        {
            return await Task.FromResult(GeoAreaForPostCode);
        }

        public async Task<List<Product>> GetOurPricesProducts(string postCode, string tariffStatus, string fuelCategory)
        {
            return await Task.FromResult(FakeProductsStub.GetProductsForOurPrices(tariffStatus, fuelCategory));
        }
    }
}
