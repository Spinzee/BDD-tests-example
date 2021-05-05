namespace Products.ServiceWrapper.BroadbandProductsService
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Configuration.Settings;
    using Extensions;
    using Infrastructure;
    using Model.Common;
    using Products.Model.Broadband;

    public class BroadbandProductsServiceWrapper : BaseAPIWrapper, IBroadbandProductsServiceWrapper, IDisposable
    {
        private readonly HttpClient _client;

        public BroadbandProductsServiceWrapper(IConfigurationSettings settings) : base(settings, "BroadbandAPI")
        {
            _client = GetHttpClient();
        }

        public async Task<List<BTAddress>> GetAddressesForPostcode(string postcode)
        {
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(postcode), $"{nameof(postcode)} is null or empty");

            BroadbandProductAddress[] response = await _client.GetResponseAsync<BroadbandProductAddress[]>($"addresses?postcode={postcode}");
            return BroadbandProductsServiceMapper.ToAddresses(response);
        }

        public async Task<List<Model.Broadband.Tariff>> GetAllTariffs(string brandId)
        {
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(brandId), $"{nameof(brandId)} is null or empty");

            ProductsResponse response = await _client.GetResponseAsync<ProductsResponse>($"product/find?brandId={brandId}").ConfigureAwait(false);
            return BroadbandProductsServiceMapper.ToTariffs(response.BroadbandProducts.Broadband.Brand.Tariffs);
        }

        public async Task<BroadbandTariffsForAddress> GetAvailableTariffs(string brandId, BTAddress address, string cli)
        {
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(brandId), $"{nameof(brandId)} is null or empty");
            Guard.Against<ArgumentException>(address == null, "address is null");

            string queryParameters = GetQueryParameters(address, brandId, cli);
            ProductsResponse response = await _client.GetResponseAsync<ProductsResponse>($"product/find?{queryParameters}").ConfigureAwait(false);
            return BroadbandProductsServiceMapper.ToAvailableTariffs(response);
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        private static string GetQueryParameters(BTAddress address, string brandId, string cli)
        {
            var queryParameters = new StringBuilder($"brandId={brandId}");

            if (!string.IsNullOrEmpty(address.Postcode))
            {
                queryParameters.AppendFormat("&postCode={0}", Uri.EscapeDataString(address.Postcode));
            }

            if (!string.IsNullOrEmpty(address.PremiseName))
            {
                queryParameters.AppendFormat("&premiseName={0}", Uri.EscapeDataString(address.PremiseName));
            }

            if (!string.IsNullOrEmpty(address.SubPremises))
            {
                queryParameters.AppendFormat("&subPremises={0}", Uri.EscapeDataString(address.SubPremises));
            }

            if (!string.IsNullOrEmpty(address.ThoroughfareName))
            {
                queryParameters.AppendFormat("&thoroughfareName={0}", Uri.EscapeDataString(address.ThoroughfareName));
            }

            if (!string.IsNullOrEmpty(address.ThoroughfareNumber))
            {
                queryParameters.AppendFormat("&thoroughfareNumber={0}", Uri.EscapeDataString(address.ThoroughfareNumber));
            }

            if (!string.IsNullOrEmpty(cli))
            {
                queryParameters.AppendFormat("&phoneNumber={0}", cli);
            }

            if (!string.IsNullOrEmpty(address.UPRN))
            {
                queryParameters.AppendFormat("&uprn={0}", address.UPRN);
            }

            if (!string.IsNullOrEmpty(address.DistrictId))
            {
                queryParameters.AppendFormat("&districtId={0}", address.DistrictId);
            }

            return queryParameters.ToString();
        }
    }
}