namespace Products.Model.Common.CMSResponse
{
    using System;
    using Newtonsoft.Json;

    public class PublishDetails
    {
        [JsonProperty("environment")]
        public string Environment { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("time")]
        public DateTime Time { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }
    }
}
