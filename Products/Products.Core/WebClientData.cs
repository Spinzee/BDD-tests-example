namespace Products.Core
{
    public class WebClientData
    {
        public WebClientData(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public string BaseUrl { get; }
    }
}
