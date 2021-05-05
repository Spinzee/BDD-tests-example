namespace Products.Model.Energy.TariffTickUspConfiguration
{
    using System.Configuration;

    public class TickUspConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("header", IsRequired = true, IsKey = true)]
        public string Header
        {
            get => (string) this["header"];
            set => this["header"] = value;
        }

        [ConfigurationProperty("description", IsRequired = true)]
        public string Description
        {
            get => (string) this["description"];
            set => this["description"] = value;
        }

        [ConfigurationProperty("displayOrder", IsRequired = true, DefaultValue = 1)]
        public int DisplayOrder
        {
            get => (int) this["displayOrder"];
            set => this["displayOrder"] = value;
        }
    }
}