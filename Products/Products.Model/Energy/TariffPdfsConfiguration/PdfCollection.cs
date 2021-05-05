namespace Products.Model.Energy.TariffPdfsConfiguration
{
    using System.Configuration;
    using TariffPdfConfiguration;

    public class PdfCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;
        protected override string ElementName => "pdf";
        protected override ConfigurationElement CreateNewElement() => new PdfConfigElement();
        protected override object GetElementKey(ConfigurationElement element) => ((PdfConfigElement) element).FilePath;
    }
}