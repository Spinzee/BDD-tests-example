namespace Products.Core.Configuration.Sections.TariffManagement.TariffGroups
{
    using System.Configuration;

    public class TariffGroupsConfigurationSection : ConfigurationSection
    {
        public const string ConfigPath = "tariffManagement/tariffGroups";

        public static TariffGroupsConfigurationSection Section { get; } = ConfigurationManager.GetSection(ConfigPath) as TariffGroupsConfigurationSection;

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public TariffGroupCollection TariffGroups => (TariffGroupCollection)base[""];
    }
}
