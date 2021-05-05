namespace Products.Model.Common.CMSResponse
{
    using Newtonsoft.Json;

    public class DocumentUrl
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
