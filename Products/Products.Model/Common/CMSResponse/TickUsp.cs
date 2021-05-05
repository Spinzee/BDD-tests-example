namespace Products.Model.Common.CMSResponse
{
    using Newtonsoft.Json;

    public class TickUsp
    {
        [JsonProperty("heading")]
        public string Heading { get; set; }

        [JsonProperty("content")]
        public string Description { get; set; }

        public int DisplayOrder { get; set; }
    }
}
