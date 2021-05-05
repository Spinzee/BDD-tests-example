namespace Products.Core.Configuration.Sections.HomeServices.HomeServicesPDFs
{
    using System.Configuration;

    public class PDFConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("key", IsRequired = true, IsKey = false)]
        public string Key
        {
            get => (string)this["key"];
            set => this["key"] = value;
        }

        [ConfigurationProperty("filePath", IsRequired = true, IsKey = false)]
        public string FilePath
        {
            get => (string)this["filePath"];
            set => this["filePath"] = value;
        }

        [ConfigurationProperty("accText", IsRequired = true, IsKey = false)]
        public string AccText
        {
            get => (string)this["accText"];
            set => this["accText"] = value;
        }

        [ConfigurationProperty("displayName", IsRequired = true, IsKey = false)]
        public string DisplayName
        {
            get => (string)this["displayName"];
            set => this["displayName"] = value;
        }
    }
}
