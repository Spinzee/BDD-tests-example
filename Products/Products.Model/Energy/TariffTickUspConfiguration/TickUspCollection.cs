using System.Configuration;

namespace Products.Model.Energy.TariffTickUspConfiguration
{
    public class TickUspCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;
        protected override ConfigurationElement CreateNewElement() => new TickUspConfigElement();
        protected override object GetElementKey(ConfigurationElement element) => ((TickUspConfigElement)element).Header;
        protected override string ElementName => "tickUsp";
    }
}