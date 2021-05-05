namespace Products.Core.Configuration.Sections.HomeServices.HomeServicesPDFs
{
    using System.Configuration;

    public class HomeServicesPDFsConfigurationSection : ConfigurationSection
    {
        public const string ConfigPath = "homeservicesManagement/homeServicesPDFs";

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public PDFCollection PDFs => (PDFCollection)base[""];

        public static HomeServicesPDFsConfigurationSection Section { get; } = ConfigurationManager.GetSection(ConfigPath) as HomeServicesPDFsConfigurationSection;
    }
}