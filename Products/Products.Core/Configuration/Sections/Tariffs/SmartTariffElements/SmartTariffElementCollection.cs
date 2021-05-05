namespace Products.Core.Configuration.Sections.Tariffs.SmartTariffElements
{
    using System.Configuration;

    [ConfigurationCollection(typeof(SmartTariffConfigElement))]
    public sealed class SmartTariffElementCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;

        protected override string ElementName => "servicePlan";

        protected override ConfigurationElement CreateNewElement() => new SmartTariffConfigElement();

        protected override object GetElementKey(ConfigurationElement element) => ((SmartTariffConfigElement)element).Id;
    }
}

