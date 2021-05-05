namespace Products.Model.Energy.TariffPdfConfiguration
{
    using System.Configuration;

    public class PdfConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("isBroadband", IsRequired = true, IsKey = false)]
        public bool IsBroadband
        {
            get => (bool)this["isBroadband"];
            set => this["isBroadband"] = value;
        }

        [ConfigurationProperty("isEnergy", IsRequired = true, IsKey = false)]
        public bool IsEnergy
        {
            get => (bool)this["isEnergy"];
            set => this["isEnergy"] = value;
        }

        [ConfigurationProperty("isHomeServices", IsRequired = true, IsKey = false)]
        public bool IsHomeServices
        {
            get => (bool)this["isHomeServices"];
            set => this["isHomeServices"] = value;
        }

        [ConfigurationProperty("filePath", IsRequired = true, IsKey = false)]
        public string FilePath
        {
            get => (string)this["filePath"];
            set => this["filePath"] = value;
        }

        [ConfigurationProperty("accText", IsRequired = false, IsKey = false)]
        public string AccText
        {
            get => (string)this["accText"];
            set => this["accText"] = value;
        }

        [ConfigurationProperty("displayName", IsRequired = false, IsKey = false)]
        public string DisplayName
        {
            get => (string) this["displayName"];
            set => this["displayName"] = value;
        }
    }
}