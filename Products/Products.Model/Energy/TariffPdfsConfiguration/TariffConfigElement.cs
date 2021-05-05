namespace Products.Model.Energy.TariffPdfsConfiguration
{
    using System.Configuration;

    public class TariffConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get => (string) this["name"];
            set => this["name"] = value;
        }

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public PdfCollection Pdfs => (PdfCollection) base[""];
    }
}