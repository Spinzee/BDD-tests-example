namespace Products.Tests.Common.Fakes
{
    using System.Collections.Generic;
    using Core;
    using Core.Configuration.Settings;
    using ConfigurationSettings = Core.Configuration.Settings.ConfigurationSettings;
    using ConnectionStringSettings = Core.Configuration.Settings.ConnectionStringSettings;

    public class FakeConfigurationSettings : IConfigurationSettings
    {
        public FakeConfigurationSettings()
        {
            PopulateConfigurationSettings();
        }

        public static IConfigurationSettings Instance { get; set; }

        public HomeServicesSettings HomeServicesSettings { get; set; }

        public Dictionary<string, TariffGroup> TariffGroupSettings { get; set; }

        public Dictionary<string, APISettings> APISettings { get; set; }

        public TariffManagementSettings TariffManagementSettings { get; set; }

        public BroadbandManagementSettings BroadbandManagementSettings { get; set; }

        public ConnectionStringSettings ConnectionStringSettings { get; set; }
		
		public List<string> BlockedEmails { get; set; }

        private void PopulateConfigurationSettings()
        {
            HomeServicesSettings = GetHomeServicesSettings();
            TariffGroupSettings = GetTariffGroupSettings();
            APISettings = GetAPISettings();
            TariffManagementSettings = GetTariffManagementSettings();
            BroadbandManagementSettings = GetBroadbandManagementSettings();
            ConnectionStringSettings = GetConnectionStringSettings();
			BlockedEmails = GetBlockedEmails();
            Instance = this;
            ConfigurationSettings.Instance = this;
        }

        private static Dictionary<string, APISettings> GetAPISettings()
        {
            return new Dictionary<string, APISettings>
            {
                { "CMSAPI", new APISettings { Url = "http://blah", SubscriptionKey = "bd22fe9712774867a2a8ececd88a4f41", Version = "V1" } },
                { "HomeServicesAPI", new APISettings {  Url = "http://blah", SubscriptionKey = "bd22fe9712774867a2a8ececd88a4f42", Version = "V1" } }
            };
        }

        private static HomeServicesSettings GetHomeServicesSettings()
        {
            return new HomeServicesSettings
            {
                PDFs = new List<PDFSettings>
                {
                    new PDFSettings { Key = "Breakdown", FilePath ="BreakdownPDFURL", DisplayName ="SSE Boiler Breakdown Cover Policy Booklet", AccText = "SSE Boiler Breakdown Cover Policy Booklet" },
                    new PDFSettings { Key = "Policy", FilePath ="PolicyPDFURL", DisplayName ="SSE Home Services Policy Booklet", AccText = "SSE Home Services Policy Booklet" },
                    new PDFSettings { Key = "BOBC" , FilePath ="BOBCPDFURL", DisplayName ="SSE Boiler Breakdown Cover Insurance Information Document", AccText = "SSE Boiler Breakdown Cover Insurance Information Document" },
                    new PDFSettings { Key = "BOHC" , FilePath ="BOHCPDFURL", DisplayName ="SSE Heating Breakdown Cover Insurance Information Document", AccText = "SSE Heating Breakdown Cover Insurance Information Document" },
                    new PDFSettings { Key = "BC", FilePath ="BCPDFURL", DisplayName ="Energy - Terms and Conditions - 18 Month Fix and Fibre", AccText = "acc Text" },
                    new PDFSettings { Key = "BC50", FilePath ="BC50PDFURL", DisplayName ="Energy - Terms and Conditions - 18 Month Fix and Fibre", AccText = "acc Text" },
                    new PDFSettings { Key = "HC", FilePath ="HCPDFURL", DisplayName ="Energy - Terms and Conditions - 18 Month Fix and Fibre", AccText = "acc Text" },
                    new PDFSettings { Key = "HC50", FilePath ="HC50PDFURL", DisplayName ="Energy - Terms and Conditions - 18 Month Fix and Fibre", AccText = "acc Text" },
                    new PDFSettings { Key = "LANDHC", FilePath ="LANDHCPDFURL", DisplayName ="Energy - Terms and Conditions - 18 Month Fix and Fibre", AccText = "acc Text" },
                    new PDFSettings { Key = "LANDBC", FilePath ="LANDBCPDFURL", DisplayName ="Energy - Terms and Conditions - 18 Month Fix and Fibre", AccText = "acc Text" },
                    new PDFSettings { Key = "EC" , FilePath ="ECPDFURL", DisplayName ="SSE Electrical Wiring Cover Insurance product information document", AccText = "SSE Electrical Wiring Cover Insurance product information document" }
                }
            };
        }

        private static TariffManagementSettings GetTariffManagementSettings()
        {
            return new TariffManagementSettings { SmartTariffSettings = new SmartTariffSettings { ServicePlanIds = new List<string>()}};
        }

        private static Dictionary<string, TariffGroup> GetTariffGroupSettings()
        {
            var retVal = new Dictionary<string, TariffGroup>
            {
                { "MG098", TariffGroup.FixAndProtect },
                { "ME697", TariffGroup.FixAndProtect },
                { "MG101", TariffGroup.FixAndFibre },
                { "ME724", TariffGroup.FixAndFibre }
            };

            return retVal;
        }

        private static BroadbandManagementSettings GetBroadbandManagementSettings()
        {
            var products = new List<ProductSettings>
            {
                new ProductSettings { Code = "UB19", BroadbandType = BroadbandType.ADSL },
                new ProductSettings { Code = "UF19", BroadbandType = BroadbandType.Fibre },
                new ProductSettings { Code = "UFP19", BroadbandType = BroadbandType.FibrePlus }
            };

            return new BroadbandManagementSettings { Products = products };
        }

        private static ConnectionStringSettings GetConnectionStringSettings()
        {
            return new ConnectionStringSettings
            {
                RedevProj = "RedevProj",
                TcData = "TcData",
                CSGatewayProfiles = "CSGatewayProfiles",
                EComAudit = "EComAudit",
                PersonalProjection = "PersonalProjection"
            };
        }
		
		private static List<string> GetBlockedEmails()
        {
            return new List<string>
            {
                "devvy.dev@devdev.org",
                "testy.test@testy.gov"
            };
        }
    }
}