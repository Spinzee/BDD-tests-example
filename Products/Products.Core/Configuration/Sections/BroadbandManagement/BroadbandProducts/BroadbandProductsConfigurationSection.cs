namespace Products.Core.Configuration.Sections.BroadbandManagement.ProductCodes
{
    using System.Configuration;

    public class BroadbandProductsConfigurationSection : ConfigurationSection
    {
        public const string ConfigPath = "broadbandManagement/products";

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public BroadbandProductsCollection Products => (BroadbandProductsCollection)base[""];

        public static BroadbandProductsConfigurationSection Section { get; } = ConfigurationManager.GetSection(ConfigPath) as BroadbandProductsConfigurationSection;
    }
}
