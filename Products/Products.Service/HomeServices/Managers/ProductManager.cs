namespace Products.Service.HomeServices.Managers
{
    using System.Configuration;
    using Core.Configuration.Sections.HomeServices.HomeServicesPDFs;

    public class ProductManager
    {
        private const string SectionGroupPath = "homeservicesManagement";
        private readonly HomeServicesPDFsConfigurationSection _pdfsSection = (HomeServicesPDFsConfigurationSection)ConfigurationManager.GetSection($"{SectionGroupPath}/homeServicesPDFs");
    }
}
