namespace Products.Model.Common.CMSResponse
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Entry
    {
        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("summary_sub_header")]
        public string SummarySubHeader { get; set; }

        [JsonProperty("key_points")]
        public List<TickUsp> KeyPoints { get; set; }

        [JsonProperty("documents")]
        public List<Document> Documents { get; set; }

        //[JsonProperty("discounts")]
        //public List<TariffDiscount> Discounts { get; set; }
        //[JsonProperty("terms_conditions")]
        //public string TermsConditions { get; set; }
        //[JsonProperty("tags")]
        //public List<object> Tags { get; set; }
        //[JsonProperty("uid")]
        //public string UId { get; set; }
        //[JsonProperty("created_by")]
        //public string CreatedBy { get; set; }
        //[JsonProperty("updated_by")]
        //public string UpdatedBy { get; set; }
        //[JsonProperty("created_at")]
        //public DateTime CreatedAt { get; set; }
        //[JsonProperty("updated_at")]
        //public DateTime UpdatedAt { get; set; }
        //[JsonProperty("ACL")]
        //public ACL ACL { get; set; }
        //[JsonProperty("_version")]
        //public int Version { get; set; }
        //[JsonProperty("publish_details")]
        //public PublishDetails PublishDetails { get; set; }
    }
}
