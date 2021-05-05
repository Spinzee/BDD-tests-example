namespace Products.Model.Common.CMSResponse
{
    using Newtonsoft.Json;

    public class Document
    {
        [JsonProperty("document_type")]
        public string DocumentType { get; set; }

        [JsonProperty("document_display_name")]
        public string DocumentDisplayName { get; set; }

        [JsonProperty("document_alt_text")]
        public string DocumentAltText { get; set; }

        [JsonProperty("document")]
        public DocumentUrl DocumentUrl { get; set; }
    }
}
