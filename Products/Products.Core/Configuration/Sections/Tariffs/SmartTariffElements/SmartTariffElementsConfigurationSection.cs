namespace Products.Core.Configuration.Sections.Tariffs.SmartTariffElements

{
    using System.Configuration;

    public class SmartTariffElementsConfigurationSection : ConfigurationSection
    {
        public const string ConfigPath = "tariffManagement/smartTariffs";

        public static SmartTariffElementsConfigurationSection Section { get; } = ConfigurationManager.GetSection(ConfigPath) as SmartTariffElementsConfigurationSection;

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public SmartTariffElementCollection  ServicePlans=> (SmartTariffElementCollection)base[""];
    }
}
