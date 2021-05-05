namespace Products.Core.Configuration.Sections.API.APIElements
{
    using System.Configuration;

    public class APIConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = false)]
        public string Name
        {
            get => (string)this["name"];
            set => this["name"] = value;
        }

        [ConfigurationProperty("url", IsRequired = true, IsKey = false)]
        public string Url
        {
            get => (string)this["url"];
            set => this["url"] = value;
        }

        [ConfigurationProperty("subscriptionKey", IsRequired = true, IsKey = false)]
        public string SubscriptionKey
        {
            get => (string)this["subscriptionKey"];
            set => this["subscriptionKey"] = value;
        }

        [ConfigurationProperty("version", IsRequired = false, DefaultValue = "", IsKey = false)]
        public string Version
        {
            get => (string)this["version"];
            set => this["version"] = value;
        }
    }
}
