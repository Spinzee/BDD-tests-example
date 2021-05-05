namespace Products.Core.Configuration.Sections.BroadbandManagement.Products
{
    using System.Configuration;

    public class BroadbandProductConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("code", IsRequired = true, IsKey = false)]
        public string Code
        {
            get => (string)this["code"];
            set => this["code"] = value;
        }

        [ConfigurationProperty("broadbandType", IsRequired = true, IsKey = false)]
        public string BroadbandType
        {
            get => (string)this["broadbandType"];
            set => this["broadbandType"] = value;
        }
    }
}
