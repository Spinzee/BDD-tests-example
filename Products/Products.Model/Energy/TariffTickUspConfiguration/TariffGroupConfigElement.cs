using System.Configuration;

namespace Products.Model.Energy.TariffTickUspConfiguration
{
    public class TariffGroupConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("servicePlanIds", IsRequired = true, IsKey = true)]
        public string ServicePlanIds
        {
            get => (string)this["servicePlanIds"];
            set => this["servicePlanIds"] = value;
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get => (string)this["name"];
            set => this["name"] = value;
        }

        [ConfigurationProperty("tickUsps", IsDefaultCollection = false)]
        public TickUspCollection TickUsps => (TickUspCollection)base["tickUsps"];
    }
}