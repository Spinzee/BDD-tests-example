namespace Products.Core.Configuration.Sections.BroadbandManagement.ProductCodes
{
    using System.Configuration;
    using Products;

    public class BroadbandProductsCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;

        protected override string ElementName => "product";

        protected override ConfigurationElement CreateNewElement() => new BroadbandProductConfigElement();

        protected override object GetElementKey(ConfigurationElement element) => ((BroadbandProductConfigElement)element).Code;
    }
}
