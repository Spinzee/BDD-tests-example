namespace Products.ServiceWrapper
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using Infrastructure;
    using Products.Core.Configuration.Settings;

    public class BaseAPIWrapper
    {
        protected Dictionary<string, List<string>> _headers;
        protected string _apiUrl;

        protected BaseAPIWrapper(IConfigurationSettings settings, string api)
        {
            Guard.Against<ArgumentException>(settings == null, $"{nameof(settings)} is null");

            // ReSharper disable once PossibleNullReferenceException
            APISettings apiSettings = settings.APISettings[api];

            // ReSharper disable once PossibleNullReferenceException
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(apiSettings.SubscriptionKey), $"{nameof(apiSettings.SubscriptionKey)} is null or empty");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(apiSettings.Url), $"{nameof(apiSettings.Url)} is null or empty");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(apiSettings.Version), $"{nameof(apiSettings.Version)} is null or empty");

            _headers = new Dictionary<string, List<string>>
            {
                { "Ocp-Apim-Subscription-Key", new List<string> { apiSettings.SubscriptionKey } },
                { "Api-Version", new List<string> { apiSettings.Version } }
            };

            _apiUrl = apiSettings.Url;
        }

        protected HttpClient GetHttpClient()
        {
            var client = new HttpClient { BaseAddress = new Uri(_apiUrl) };
            return AddHeadersToHttpClient(client);
        }

        private HttpClient AddHeadersToHttpClient(HttpClient httpClient)
        {
            foreach (var header in _headers)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            return httpClient;
        }
    }
}
