namespace Products.Model.Energy.TariffPdfsConfiguration
{
    using System.Configuration;

    public class TariffPdfsSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public TariffCollection Tariffs => (TariffCollection) base[""];
    }
}