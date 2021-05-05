namespace Products.Core.Configuration.Sections.API.APIElements
{
    using System.Configuration;

    [ConfigurationCollection(typeof(APIConfigElement))]
    public sealed class APIElementCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;

        protected override string ElementName => "apiElement";

        protected override ConfigurationElement CreateNewElement() => new APIConfigElement();

        protected override object GetElementKey(ConfigurationElement element) => ((APIConfigElement)element).Name;
    }
}

