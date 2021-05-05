namespace Products.ServiceWrapper.EnergyProjectionService
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using Infrastructure;
    using Microsoft.Rest;
    using Model.Energy;
    using Sse.Projection.Client;
    using Sse.Projection.Client.Models;

    public class EnergyProjectionServiceWrapper : IEnergyProjectionServiceWrapper, IDisposable
    {
        private readonly ICacheService _cacheService;
        private readonly EnergyUsageProjection _client;
        private readonly Dictionary<string, List<string>> _headers;

        public EnergyProjectionServiceWrapper(IConfigManager configManager, ICacheService cacheService)
        {
            Guard.Against<ArgumentNullException>(configManager == null, "configManager is null");
            Guard.Against<ArgumentNullException>(cacheService == null, "cacheService is null");

            // ReSharper disable once PossibleNullReferenceException
            string subscriptionKey = configManager.GetAppSetting("EnergyProjectionApiSubscriptionKey");
            string apiAddress = configManager.GetAppSetting("EnergyProjectionApiAddress");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(subscriptionKey), "apiKey is null");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(apiAddress), "baseAddress is null");

            _cacheService = cacheService;
            // ReSharper disable once AssignNullToNotNullAttribute
            _client = new EnergyUsageProjection(new Uri(apiAddress));

            _headers = new Dictionary<string, List<string>> { { "Ocp-Apim-Subscription-Key", new List<string> { subscriptionKey } } };
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public async Task<List<EnergyMultiplier>> GetMultipliers()
        {
            return await _cacheService.Get("multipliers", async () =>
            {
                HttpOperationResponse<IList<Multiplier>> response = await _client.GetMultipliersWithHttpMessagesAsync(_headers).ConfigureAwait(false);

                if (response.Response.StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpException(
                        $"Request URL: {response.Response.RequestMessage.RequestUri}, {await response.Response.Content.ReadAsStringAsync()}");
                }

                return EnergyProjectionMapper.ToEnergyMultiplierList(response.Body);
            });
        }

        public async Task<CumulativeEnergyMultiplier> GetCumulativeEnergyMultiplier(IEnumerable<EnergyMultiplier> selectedMultipliers)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            string key = GetCumulativeMultiplierKey(selectedMultipliers);

            return await _cacheService.Get(key, async () =>
            {
                // ReSharper disable once PossibleMultipleEnumeration
                IList<Multiplier> selectedAPIMultipliers = EnergyProjectionMapper.ToAPIMultiplierList(selectedMultipliers);
                HttpOperationResponse<Multiplier> response =
                    await _client.HandleGetCumulativeWithHttpMessagesAsync(selectedAPIMultipliers, _headers).ConfigureAwait(false);

                if (response.Response.StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpException(
                        $"Request URL: {response.Response.RequestMessage.RequestUri}, {await response.Response.Content.ReadAsStringAsync()}");
                }

                return EnergyProjectionMapper.ToCumulativeEnergyMultiplier(response.Body);
            });
        }


        public async Task<Projection> GetProjection(CumulativeEnergyMultiplier cumulativeEnergyMultiplier, string postCode)
        {
            HttpOperationResponse<Usage> response = await _client.GetProjectionWithHttpMessagesAsync(
                postCode,
                // ReSharper disable once PossibleInvalidOperationException
                cumulativeEnergyMultiplier.MultiplierElec.Value,
                // ReSharper disable once PossibleInvalidOperationException
                cumulativeEnergyMultiplier.MultiplierGas.Value,
                _headers).ConfigureAwait(false);

            if (response.Response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpException($"Request URL: {response.Response.RequestMessage.RequestUri}, {await response.Response.Content.ReadAsStringAsync()}");
            }

            return EnergyProjectionMapper.ToProjection(response.Body);
        }

        private string GetCumulativeMultiplierKey(IEnumerable<EnergyMultiplier> selectedMultipliers)
        {
            var builder = new StringBuilder();

            foreach (EnergyMultiplier obj in selectedMultipliers)
            {
                builder.AppendFormat("{0}_{1}_", obj.MultiplierType, obj.Value);
            }

            return builder.ToString();
        }
    }
}