
namespace Products.ServiceWrapper.CAndCService
{
    using Infrastructure;
    using Model.Common;
    using Model.Energy;
    using Sse.Retail.CAndC.Client;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web;

    public class CAndCServiceWrapper : ICAndCServiceWrapper, IDisposable
    {
        private readonly CAndC _client;
        private readonly Dictionary<string, List<string>> headers;

        public CAndCServiceWrapper(IConfigManager configManager)
        {
            Guard.Against<ArgumentNullException>(configManager == null, "configManager is null");

            var subscriptionKey = configManager.GetAppSetting("CAndCApiSubscriptionKey");
            var apiAddress = configManager.GetAppSetting("CAndCApiAddress");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(subscriptionKey), "apiKey is null");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(apiAddress), "baseAddress is null");

            _client = new CAndC(new Uri(apiAddress));

            headers = new Dictionary<string, List<string>>();
            headers.Add("Ocp-Apim-Subscription-Key", new List<string> { subscriptionKey });
        }


        public async Task<MeterDetail> GetMeterDetail(string postcode, QasAddress customerAddress)
        {
            var response = await _client.GetMeterDetailsWithHttpMessagesAsync(postcode, customerAddress.HouseName,
                customerAddress.AddressLine1, customerAddress.AddressLine2, customerAddress.Town, customerAddress.County,
                headers).ConfigureAwait(false);

            if (response.Response.StatusCode != HttpStatusCode.OK)
                throw new HttpException($"Request URL: {response.Response.RequestMessage.RequestUri}, {await response.Response.Content.ReadAsStringAsync()}");

            return CAndCMapper.ToMeterDetails(response.Body);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}