namespace Products.Tests.Common.Helpers
{
    using Fakes;

    public static class FakeConfigManagerFactory
    {
        public static FakeConfigManager DefaultHomeServices()
        {
            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("encryption", "encryptionKeys", "HomeServicesSignupEncryptPublicKey", "a1247d2a-c978-4db6-acbe-935f5219d21a");
            fakeConfigManager.AddConfiguration("homeservicesManagement", "homeServicesPDFs", "BOBC", "BOBCPDFURL");
            fakeConfigManager.AddConfiguration("homeservicesManagement", "homeServicesPDFs", "BOHC", "BOHCPDFURL");
            fakeConfigManager.AddConfiguration("homeservicesManagement", "homeServicesPDFs", "BC", "BCPDFURL");
            fakeConfigManager.AddConfiguration("homeservicesManagement", "homeServicesPDFs", "BC50", "BC50PDFURL");
            fakeConfigManager.AddConfiguration("homeservicesManagement", "homeServicesPDFs", "HC", "HCPDFURL");
            fakeConfigManager.AddConfiguration("homeservicesManagement", "homeServicesPDFs", "HC50", "HC50PDFURL");
            fakeConfigManager.AddConfiguration("homeservicesManagement", "homeServicesPDFs", "LANDHC", "LANDHCPDFURL");
            fakeConfigManager.AddConfiguration("homeservicesManagement", "homeServicesPDFs", "LANDBC", "LANDBCPDFURL");
            fakeConfigManager.AddConfiguration("homeservicesManagement", "homeServicesPDFs", "EC", "ECPDFURL");
            fakeConfigManager.AddConfiguration("homeservicesManagement", "homeServicesPDFs", "Breakdown", "BreakdownPDFURL");
            fakeConfigManager.AddConfiguration("homeservicesManagement", "homeServicesPDFs", "Policy", "PolicyPDFURL");
            fakeConfigManager.AddConfiguration("HomeServicesHubUrl", "https://sse.co.uk/home-services");
            fakeConfigManager.AddConfiguration("HomeServicesCustomerAlertName", "NoHES");

            return fakeConfigManager;
        }

        public static FakeConfigManager DefaultBroadband()
        {
            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("SpeculativeCode", "WWQTCI");
            fakeConfigManager.AddConfiguration("AffiliateCampaignCode", "WADXCI");
            fakeConfigManager.AddConfiguration("EmailBaseUrl", "devandtest.externalmail@sse.com");
            fakeConfigManager.AddConfiguration("FibrePlusSpeedCap", "76000");
            fakeConfigManager.AddConfiguration("Surcharge", "5");
            fakeConfigManager.AddConfiguration("ConnectionFee", "50");
            fakeConfigManager.AddConfiguration("InstallationFee", "60");
            fakeConfigManager.AddConfiguration("encryption", "encryptionKeys", "BroadbandSignupEncryptPublicKey", "ae519629-4fb0-431a-b49d-2a9d5c09eaf8");
            fakeConfigManager.AddConfiguration("encryption", "encryptionKeys", "EnergySignupEncryptPublicKey", "ae519629-4fb0-431a-b49d-2a9d5c09eaf8");
            fakeConfigManager.AddConfiguration("encryption", "encryptionKeys", "HomeServicesSignupEncryptPublicKey", "a1247d2a-c978-4db6-acbe-935f5219d21a");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productCodes", "UB19", "ADSL");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productCodes", "UF19", "Fibre");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productCodes", "UFP19", "FibrePlus");
            fakeConfigManager.AddConfiguration("CustomerAlertName", "BroadbandCustomerAlertName");
            fakeConfigManager.AddConfiguration("BroadbandCustomerAlertName", "BroadbandCustomerAlertName");
            fakeConfigManager.AddConfiguration("MembershipEmailTo", "devandtest.externalmail@sse.com");
            fakeConfigManager.AddConfiguration("EnergyProjectionApiSubscriptionKey", "23242342342343242");

            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "FF3_ANY18", "FixAndFibreV3");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "FF3_AP18", "FixAndFibreV3");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "FF3_LR18", "FixAndFibreV3");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "FF3_EAW18", "FixAndFibreV3");

            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "FFP_LR18", "FixAndFibrePlus");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "FFP_ANY18", "FixAndFibrePlus");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "FFP_AP18", "FixAndFibrePlus");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "FFP_EAW18", "FixAndFibrePlus");

            
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "FIBRE_EAW18_CH", "CompleteHomeTest");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "FIBRE18_LR_CH", "CompleteHomeTest");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "FIBRE_AP18_CH", "CompleteHomeTest");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "FIBRE_ANY18_CH", "CompleteHomeTest");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "BB_EAW18_FF", "NotAvailableOnline");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "BB_ANY18_FF", "NotAvailableOnline");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "BB_AP18_FF", "NotAvailableOnline");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "BB18_LR_FF", "NotAvailableOnline");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "ADSLFF3_LR18", "NotAvailableOnline");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "ADSLFF3_AP18", "NotAvailableOnline");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "ADSLFF3_ANY18", "NotAvailableOnline");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "ADSLFF3_EAW18", "NotAvailableOnline");
            fakeConfigManager.AddConfiguration("broadbandManagement", "availableTariffPdfs", "FixAndFibreV3", "PDF A.pdf|PDF B.pdf");
            fakeConfigManager.AddConfiguration("bundleManagement", "BroadbandUpgradesNotToBeDisplayedInBasket", "FF3_LR18", "Line Rental Included");
            return fakeConfigManager;
        }

        public static FakeConfigManager DefaultBundling()
        {
            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("SpeculativeCode", "WWQTCI");
            fakeConfigManager.AddConfiguration("AffiliateCampaignCode", "WADXCI");
            fakeConfigManager.AddConfiguration("EmailBaseUrl", "devandtest.externalmail@sse.com");
            fakeConfigManager.AddConfiguration("FibrePlusSpeedCap", "76000");
            fakeConfigManager.AddConfiguration("Surcharge", "5");
            fakeConfigManager.AddConfiguration("ConnectionFee", "50");
            fakeConfigManager.AddConfiguration("InstallationFee", "60");
            fakeConfigManager.AddConfiguration("encryption", "encryptionKeys", "BroadbandSignupEncryptPublicKey", "ae519629-4fb0-431a-b49d-2a9d5c09eaf8");
            fakeConfigManager.AddConfiguration("encryption", "encryptionKeys", "EnergySignupEncryptPublicKey", "ae519629-4fb0-431a-b49d-2a9d5c09eaf8");
            fakeConfigManager.AddConfiguration("encryption", "encryptionKeys", "HomeServicesSignupEncryptPublicKey", "a1247d2a-c978-4db6-acbe-935f5219d21a");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productCodes", "UB19", "ADSL");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productCodes", "UF19", "Fibre");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productCodes", "UFP19", "FibrePlus");
            fakeConfigManager.AddConfiguration("CustomerAlertName", "BroadbandCustomerAlertName");
            fakeConfigManager.AddConfiguration("BroadbandCustomerAlertName", "BroadbandCustomerAlertName");
            fakeConfigManager.AddConfiguration("MembershipEmailTo", "devandtest.externalmail@sse.com");
            fakeConfigManager.AddConfiguration("EnergyProjectionApiSubscriptionKey", "23242342342343242");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "FF3_ANY18", "FixAndFibreV3");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "FF3_AP18", "FixAndFibreV3");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "FF3_LR18", "FixAndFibreV3");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "FF3_EAW18", "FixAndFibreV3");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "BB_EAW18_FF", "NotAvailableOnline");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "BB_ANY18_FF", "NotAvailableOnline");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "BB_AP18_FF", "NotAvailableOnline");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "BB18_LR_FF", "NotAvailableOnline");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "ADSLFF3_LR18", "NotAvailableOnline");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "ADSLFF3_AP18", "NotAvailableOnline");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "ADSLFF3_ANY18", "NotAvailableOnline");
            fakeConfigManager.AddConfiguration("broadbandManagement", "productGroup", "ADSLFF3_EAW18", "NotAvailableOnline");
            fakeConfigManager.AddConfiguration("BundlingApiSubscriptionKey", "cad3d3ae743f414187dc3b7477a24ec7");
            fakeConfigManager.AddConfiguration("BundlingApiAddress", "https://api.dev.digitalsse.cloud/dev/bundlingstub/");

            return fakeConfigManager;
        }

        public static FakeConfigManager DefaultTariffChange()
        {
            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AllowEconomyMultiRateMeters", "false");
            fakeConfigManager.AddConfiguration("CustomerAlertName", "CustomerAlertName");
            fakeConfigManager.AddConfiguration("VerificationLink", "VerificationLink");
            fakeConfigManager.AddConfiguration("PrivateKey", "PrivateKey");
            return fakeConfigManager;
        }
    }
}