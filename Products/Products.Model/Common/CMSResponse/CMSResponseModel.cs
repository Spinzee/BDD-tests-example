namespace Products.Model.Common.CMSResponse
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class CMSResponseModel
    {
        [JsonProperty("entries")]
        public List<Entry> Entries { get; set; }
    }
}
