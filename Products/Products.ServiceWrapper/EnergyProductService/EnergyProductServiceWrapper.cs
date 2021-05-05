namespace Products.ServiceWrapper.EnergyProductService
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web;
    using Core.Configuration.Settings;
    using Products.Model.Energy;
    using Sse.Retail.Energy.Products.Client;
    using Product = Model.Energy.Product;

    public class EnergyProductServiceWrapper : BaseAPIWrapper, IEnergyProductServiceWrapper
    {
        private readonly EnergyProductsApi _client;

        public EnergyProductServiceWrapper(IConfigurationSettings settings) : base(settings, "EnergyProductsAPI")
        {
            _client = new EnergyProductsApi(new Uri(_apiUrl));
        }

        public async Task<List<Product>> GetEnergyProducts(ProductsRequest productsRequest)
        {
            var response = await _client.GetProductsWithHttpMessagesAsync(
                productsRequest.PostCode, productsRequest.MeterType, productsRequest.AccountType,
                productsRequest.BillingPreference, productsRequest.PaymentType, productsRequest.FuelType,
                productsRequest.StandardGasKwh, productsRequest.StandardElectricityKwh,
                productsRequest.Economy7ElectricityDayKwh, productsRequest.Economy7ElectricityNightKwh, _headers).ConfigureAwait(false);

            if (response.Response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpException($"Request URL: {response.Response.RequestMessage.RequestUri}, {await response.Response.Content.ReadAsStringAsync()}");
            }

            return EnergyProductMapper.ToProducts(response.Body);
        }

        public async Task<string> GetGeoAreaForPostCode(string postCode)
        {
            var response = await _client.GetGeoAreaWithHttpMessagesAsync(postCode, _headers).ConfigureAwait(false);

            if (response.Response.StatusCode != HttpStatusCode.OK && response.Response.StatusCode != HttpStatusCode.NotFound)
            {
                throw new HttpException($"Request URL: {response.Response.RequestMessage.RequestUri}, {await response.Response.Content.ReadAsStringAsync()}");
            }

            return response.Body;
        }

        public async Task<List<Product>> GetOurPricesProducts(string postCode, string tariffStatus, string fuelCategory)
        {
            var response = await _client.GetEnergyProductsWithHttpMessagesAsync(postCode, tariffStatus, fuelCategory, _headers).ConfigureAwait(false);

            if (response.Response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpException($"Request URL: {response.Response.RequestMessage.RequestUri}, {await response.Response.Content.ReadAsStringAsync()}");
            }

            return EnergyProductMapper.ToProducts(response.Body);
        }
    }
}
