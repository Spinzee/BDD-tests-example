namespace Products.Core.Configuration.Sections.TariffManagement.TariffGroups
{
    using System.Configuration;

    [ConfigurationCollection(typeof(TariffGroupConfigElement))]
    public sealed class TariffGroupCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;

        protected override string ElementName => "tariffGroup";

        protected override ConfigurationElement CreateNewElement() => new TariffGroupConfigElement();

        protected override object GetElementKey(ConfigurationElement element) => ((TariffGroupConfigElement)element).Name;
    }
}

