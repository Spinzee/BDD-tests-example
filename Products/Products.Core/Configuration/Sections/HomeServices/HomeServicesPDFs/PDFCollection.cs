namespace Products.Core.Configuration.Sections.HomeServices.HomeServicesPDFs
{
    using System.Configuration;

    public class PDFCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;

        protected override string ElementName => "pdf";

        protected override ConfigurationElement CreateNewElement() => new PDFConfigElement();

        protected override object GetElementKey(ConfigurationElement element) => ((PDFConfigElement)element).Key;
    }
}
