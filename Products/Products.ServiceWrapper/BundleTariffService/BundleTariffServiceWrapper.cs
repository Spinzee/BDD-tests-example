namespace Products.ServiceWrapper.BundleTariffService
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web;
    using Core.Configuration.Settings;
    using Microsoft.Rest;
    using Model.Energy;
    using Sse.Retail.Bundling.Client;
    using Sse.Retail.Bundling.Client.Models;

    public class BundleTariffServiceWrapper : BaseAPIWrapper, IBundleTariffServiceWrapper
    {
        private readonly Bundling _client;

        public BundleTariffServiceWrapper(IConfigurationSettings settings) : base(settings, "BundlingAPI") 
        {
            _client = new Bundling(new Uri(_apiUrl));
        }

        public async Task<List<Bundle>> GetDualMultiRateElectricBundles(BundleRequest request)
        {
            HttpOperationResponse<IList<BundleResponse>> response = await _client.GetDualMultiRateElectricBundlesWithHttpMessagesAsync(
                request.PostCode
                , request.StandardGasKwh
                , request.Economy7ElectricityDayKwh
                , request.Economy7ElectricityNightKwh
                , _headers).ConfigureAwait(false);

            if (response.Response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpException($"Request URL: {response.Response.RequestMessage.RequestUri}, {await response.Response.Content.ReadAsStringAsync()}");
            }

            return BundleTariffMapper.ToBundle(response.Body);
        }

        public async Task<List<Bundle>> GetDualSingleRateElectricBundles(BundleRequest request)
        {
            HttpOperationResponse<IList<BundleResponse>> response = await _client.GetDualSingleRateElectricBundlesWithHttpMessagesAsync(
                request.PostCode
                , request.StandardGasKwh
                , request.StandardElectricityKwh
                , _headers).ConfigureAwait(false);

            if (response.Response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpException($"Request URL: {response.Response.RequestMessage.RequestUri}, {await response.Response.Content.ReadAsStringAsync()}");
            }

            return BundleTariffMapper.ToBundle(response.Body);
        }

        public async Task<List<Bundle>> GetSingleRateElectricBundles(BundleRequest request)
        {
            HttpOperationResponse<IList<BundleResponse>> response = await _client.GetSingleRateElectricBundlesWithHttpMessagesAsync(
                request.PostCode
                , request.StandardElectricityKwh
                , _headers).ConfigureAwait(false);

            if (response.Response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpException($"Request URL: {response.Response.RequestMessage.RequestUri}, {await response.Response.Content.ReadAsStringAsync()}");
            }

            return BundleTariffMapper.ToBundle(response.Body);
        }

        public async Task<List<Bundle>> GetMultiRateElectricBundles(BundleRequest request)
        {
            HttpOperationResponse<IList<BundleResponse>> response = await _client.GetMultiRateElectricBundlesWithHttpMessagesAsync(
                request.PostCode
                , request.Economy7ElectricityDayKwh
                , request.Economy7ElectricityNightKwh
                , _headers).ConfigureAwait(false);

            if (response.Response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpException($"Request URL: {response.Response.RequestMessage.RequestUri}, {await response.Response.Content.ReadAsStringAsync()}");
            }

            return BundleTariffMapper.ToBundle(response.Body);
        }

        public async Task<List<Bundle>> GetSingleRateGasBundles(BundleRequest request)
        {
            HttpOperationResponse<IList<BundleResponse>> response = await _client.GetSingleRateGasBundlesWithHttpMessagesAsync(
                request.PostCode
                , request.StandardGasKwh
                , _headers).ConfigureAwait(false);

            if (response.Response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpException($"Request URL: {response.Response.RequestMessage.RequestUri}, {await response.Response.Content.ReadAsStringAsync()}");
            }

            return BundleTariffMapper.ToBundle(response.Body);
        }
    }
}