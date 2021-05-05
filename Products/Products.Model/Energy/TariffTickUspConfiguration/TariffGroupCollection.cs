using System.Configuration;

namespace Products.Model.Energy.TariffTickUspConfiguration
{
    public sealed class TariffGroupCollection : ConfigurationElementCollection
    {
        public TariffGroupCollection()
        {
            var details = (TariffGroupConfigElement)CreateNewElement();
            if (details.ServicePlanIds != "")
            {
                BaseAdd(details);
            }
        }

        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;
        protected override ConfigurationElement CreateNewElement() => new TariffGroupConfigElement();
        protected override object GetElementKey(ConfigurationElement element) => ((TariffGroupConfigElement)element).ServicePlanIds;
        protected override string ElementName => "tariffGroup";
    }
}