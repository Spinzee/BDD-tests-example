namespace Products.Core.Configuration.Sections.Tariffs.SmartTariffElements
{
    using System.Configuration;

    public class SmartTariffConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("id", IsRequired = true, IsKey = false)]
        public string Id
        {
            get => (string)this["id"];
            set => this["id"] = value;
        }
    }
}
