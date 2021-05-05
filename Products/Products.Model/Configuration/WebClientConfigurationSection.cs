namespace Products.Model.Configuration
{
    using System.Configuration;

    public class WebClientConfigurationSection : ConfigurationSection
    {
        public const string ConfigPath = "webClientConfiguration";
        private const string BaseUrlProperty = "baseUrl";
        private const string VersionProperty = "version";

        public static WebClientConfigurationSection Section { get; } = ConfigurationManager.GetSection(ConfigPath) as WebClientConfigurationSection;

        [ConfigurationProperty(BaseUrlProperty, IsRequired = true)]
        public string BaseUrl => (string)this[BaseUrlProperty];

        [ConfigurationProperty(VersionProperty, IsRequired = true)]
        public string Version => (string)this[VersionProperty];
    }
}
