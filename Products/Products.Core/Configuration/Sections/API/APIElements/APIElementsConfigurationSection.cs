namespace Products.Core.Configuration.Sections.API.APIElements
{
    using System.Configuration;

    public class APIElementsConfigurationSection : ConfigurationSection
    {
        public const string ConfigPath = "apiConfiguration/apiElements";

        public static APIElementsConfigurationSection Section { get; } = ConfigurationManager.GetSection(ConfigPath) as APIElementsConfigurationSection;

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public APIElementCollection  Urls => (APIElementCollection)base[""];
    }
}
