namespace Products.Web.Helpers
{
    public class WebClientHelper
    {
        private static WebClientHelper _instance;

        private WebClientHelper() { }

        public static WebClientHelper Instance => _instance ?? (_instance = new WebClientHelper());

        public string BaseUrl { get; set; }
        public string Version { get; set; }
    }
}