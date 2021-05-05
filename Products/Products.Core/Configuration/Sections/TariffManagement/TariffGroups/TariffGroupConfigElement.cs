namespace Products.Core.Configuration.Sections.TariffManagement.TariffGroups
{
    using System.Configuration;

    public class TariffGroupConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = false)]
        public string Name
        {
            get => (string)this["name"];
            set => this["name"] = value;
        }

        [ConfigurationProperty("servicePlanIds", IsRequired = true, IsKey = false)]
        public string ServicePlanIds
        {
            get => (string)this["servicePlanIds"];
            set => this["servicePlanIds"] = value;
        }
    }
}
