namespace Products.Model.Energy.TariffPdfsConfiguration
{
    using System.Configuration;

    public sealed class TariffCollection : ConfigurationElementCollection
    {
        public TariffCollection()
        {
            var tariffConfigElement = (TariffConfigElement) CreateNewElement();
            if (!string.IsNullOrEmpty(tariffConfigElement.Name))
            {
                BaseAdd(tariffConfigElement);
            }
        }

        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;
        protected override string ElementName => "tariff";
        protected override ConfigurationElement CreateNewElement() => new TariffConfigElement();
        protected override object GetElementKey(ConfigurationElement element) => ((TariffConfigElement) element).Name;
    }
}