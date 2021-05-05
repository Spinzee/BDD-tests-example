namespace Products.Model.Energy.TariffTickUspConfiguration
{
    using System.Configuration;
    public class TariffTickUspSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public TariffGroupCollection TariffGroups => (TariffGroupCollection)base[""];
    }
}