namespace Products.Core.Configuration.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using Sections;
    using Sections.API.APIElements;
    using Sections.BroadbandManagement.ProductCodes;
    using Sections.BroadbandManagement.Products;
    using Sections.HomeServices.HomeServicesPDFs;
    using Sections.TariffManagement.TariffGroups;
    using Sections.Tariffs.SmartTariffElements;

    public class ConfigurationSettings : IConfigurationSettings
    {
        private const string RevProjLabel = "ReDevProj.DbConnection";
        private const string TcDataLabel = "tcData.DbConnection";
        private const string CSGatewayProfilesLabel = "CSGatewayProfiles.DbConnection";
        private const string EComAuditLabel = "eComAudit.DbConnection";
        private const string PersonalProjectionLabel = "PersonalProjection.DbConnection";

        public ConfigurationSettings()
        {
            PopulateConfigurationSettings();
        }

        public static IConfigurationSettings Instance { get; set; }

        public HomeServicesSettings HomeServicesSettings { get; set; }

        public Dictionary<string, APISettings> APISettings { get; set; }

        public Dictionary<string, TariffGroup> TariffGroupSettings { get; set; }

        public TariffManagementSettings TariffManagementSettings { get; set; }

        public BroadbandManagementSettings BroadbandManagementSettings { get; set; }

        public ConnectionStringSettings ConnectionStringSettings { get; set; }
		
		public List<string> BlockedEmails { get; set; }

        private void PopulateConfigurationSettings()
        {
            HomeServicesSettings = GetHomeServicesSettings();
            APISettings = GetAPISettings();
            TariffGroupSettings = GetTariffGroupSettings();
            TariffManagementSettings = GetTariffManagementSettings();
            BroadbandManagementSettings = GetBroadbandManagementSettings();
            ConnectionStringSettings = GetConnectionStringSettings();
			BlockedEmails = GetBlockedEmailsList();
            Instance = this;
        }

        private static HomeServicesSettings GetHomeServicesSettings()
        {
            var pdfList = new List<PDFSettings>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (PDFConfigElement pdf in HomeServicesPDFsConfigurationSection.Section.PDFs)
            {
                pdfList.Add(new PDFSettings
                {
                    Key = pdf.Key,
                    AccText = pdf.AccText,
                    DisplayName = pdf.DisplayName,
                    FilePath = pdf.FilePath
                });
            }

            return new HomeServicesSettings
            {
                PDFs = pdfList
            };
        }

        private static Dictionary<string, APISettings> GetAPISettings()
        {
            var apiDict = new Dictionary<string, APISettings>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (APIConfigElement apiElement in APIElementsConfigurationSection.Section.Urls)
            {
                apiDict.Add(apiElement.Name, 
                    new APISettings
                    {
                        Url = apiElement .Url,
                        SubscriptionKey = apiElement.SubscriptionKey,
                        Version = apiElement .Version
                    });
            }

            return apiDict;
        }

        private static Dictionary<string, TariffGroup> GetTariffGroupSettings()
        {
            var dict = new Dictionary<string, TariffGroup>();

            foreach (TariffGroupConfigElement tariffGroup in TariffGroupsConfigurationSection.Section.TariffGroups)
            {
                var group = (TariffGroup)Enum.Parse(typeof(TariffGroup), tariffGroup.Name, true);
                List<string> servicePlanIds = tariffGroup.ServicePlanIds.Split(',').ToList();
                foreach (string servicePlanId in servicePlanIds)
                {
                    dict.Add(servicePlanId, group);
                }
            }

            return dict;
        }

        private static TariffManagementSettings GetTariffManagementSettings()
        {
            return new TariffManagementSettings { SmartTariffSettings = GetSmartTariffSettings() };
        }

        private static SmartTariffSettings GetSmartTariffSettings()
        {
            var idList = new List<string>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (SmartTariffConfigElement servicePlan in SmartTariffElementsConfigurationSection.Section.ServicePlans)
            {
                idList.Add(servicePlan.Id);
            }

            return new SmartTariffSettings
            {
                ServicePlanIds = idList
            };
        }

        private static BroadbandManagementSettings GetBroadbandManagementSettings()
        {
            return new BroadbandManagementSettings { Products = GetBroadbandProducts() };
        }

        private static List<ProductSettings> GetBroadbandProducts()
        {
            var productSettings = new List<ProductSettings>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (BroadbandProductConfigElement product in BroadbandProductsConfigurationSection.Section.Products)
            {
                productSettings.Add(new ProductSettings
                {
                    Code = product.Code,
                    BroadbandType = (BroadbandType)Enum.Parse(typeof(BroadbandType), product.BroadbandType)
                });
            }

            return productSettings;
        }

        private static ConnectionStringSettings GetConnectionStringSettings()
        {
            return new ConnectionStringSettings
            {
                RedevProj = ConfigurationManager.ConnectionStrings[RevProjLabel].ConnectionString,
                TcData = ConfigurationManager.ConnectionStrings[TcDataLabel].ConnectionString,
                CSGatewayProfiles = ConfigurationManager.ConnectionStrings[CSGatewayProfilesLabel].ConnectionString,
                EComAudit = ConfigurationManager.ConnectionStrings[EComAuditLabel].ConnectionString,
                PersonalProjection = ConfigurationManager.ConnectionStrings[PersonalProjectionLabel].ConnectionString
            };
        }
		
		private static List<string> GetBlockedEmailsList()
        {
            string blockedEmails = SystemOverridesConfigurationSection.Section.BlockedEmails.TrimEnd(',');
            return blockedEmails.Split(',').Where(s => s.Length > 0).ToList();
        }
    }
}
