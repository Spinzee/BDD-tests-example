namespace Products.ServiceWrapper.HomeServicesProductService
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web;
    using Core.Configuration.Settings;
    using HomeServices;
    using Microsoft.Rest;
    using Sse.Retail.Homeservices.Client;
    using Sse.Retail.Homeservices.Client.Models;
    using ProductGroup = Model.HomeServices.ProductGroup;

    public class HomeServicesProductServiceWrapper : BaseAPIWrapper, IHomeServicesProductServiceWrapper
    {
        private readonly Homeservices _client;

        public HomeServicesProductServiceWrapper(IConfigurationSettings settings):base(settings, "HomeServicesAPI")
        {
            _client = new Homeservices(new Uri(_apiUrl));
        }

        public async Task<ProductGroup> GetHomeServiceResidentialProduct(string productCode, string postcode)
        {
            HttpOperationResponse<ProductResult> response = await _client.GetDomesticProductWithHttpMessagesAsync(productCode, postcode, _headers).ConfigureAwait(false);

            if (response.Response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpException($"Request URL: {response.Response.RequestMessage.RequestUri}, {await response.Response.Content.ReadAsStringAsync()}");
            }

            return HomeServicesProductsMapper.ToHomeServiceProduct(response.Body);
        }

        public async Task<ProductGroup> GetHomeServiceLandlordProduct(string productCode, string postcode)
        {
            HttpOperationResponse<ProductResult> response = await _client.GetLandlordProductWithHttpMessagesAsync(productCode, postcode, _headers).ConfigureAwait(false);

            if (response.Response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpException($"Request URL: {response.Response.RequestMessage.RequestUri}, {await response.Response.Content.ReadAsStringAsync()}");
            }

            return HomeServicesProductsMapper.ToHomeServiceProduct(response.Body);
        }
    }
}