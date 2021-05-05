namespace Products.ServiceWrapper.Extensions
{
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public static class HttpClientExtension
    {
        public static async Task<T> GetResponseAsync<T>(this HttpClient httpClient, string relativeUri)
        {
            using (HttpResponseMessage response = await httpClient.GetAsync(relativeUri).ConfigureAwait(false))
            using (HttpContent content = response.Content)
            {
                string message = await content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpException($"Status Code: {response.StatusCode}, Message: {message}");
                }

                return DeserializeJson<T>(message);
            }
        }

        public static async Task<T> PostResponseAsync<T, TV>(this HttpClient httpClient, string relativeUri, TV messageBody) where TV : class
        {
            var serializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            string json = JsonConvert.SerializeObject(messageBody, serializerSettings);

            var jasonContent = new StringContent(json, Encoding.UTF8, "application/json");

            using (HttpResponseMessage response = await httpClient.PostAsync(relativeUri, jasonContent).ConfigureAwait(false))
            using (HttpContent content = response.Content)
            {
                string message = await content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpException($"Status Code: {response.StatusCode}, Message: {message}");
                }

                return DeserializeJson<T>(message);
            }
        }


        private static T DeserializeJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}