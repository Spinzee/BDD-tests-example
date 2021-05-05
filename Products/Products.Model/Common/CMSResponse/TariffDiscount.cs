namespace Products.Model.Common.CMSResponse
{
    using Newtonsoft.Json;

    public class TariffDiscount
    {
        [JsonProperty("service_type")]
        public string ServiceType { get; set; }

        [JsonProperty("discount")]
        public string Discount { get; set; }
    }
}
