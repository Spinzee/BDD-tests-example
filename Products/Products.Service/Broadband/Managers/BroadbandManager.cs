namespace Products.Service.Broadband.Managers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Core;
    using Core.Configuration.Settings;
    using Infrastructure;
    using Products.Model.Broadband;
    using Products.WebModel.Resources.Broadband;
    using Products.WebModel.ViewModels.Common;

    public class BroadbandManager : IBroadbandManager
    {
        private readonly IConfigManager _configManager;
        private readonly IConfigurationSettings _configurationSettings;

        private const string SectionGroupPath = "broadbandManagement";
        private const string AdditionalPricesSection = "additionalPrices";
        private const string SurchargeKey = "Surcharge";
        private const string ConnectionFeeKey = "ConnectionFee";
        private const string InstallationFeeKey = "InstallationFee";
        ////private const string productCodesSection = "productCodes";
        private const string EquipmentChargeKey = "EquipmentCharge";
        private const string EarlyTerminationChargeBroadbandKey = "EarlyTerminationChargeBroadband";
        private const string EarlyTerminationChargeFibreAndFibrePlusKey = "EarlyTerminationChargeFibreAndFibrePlus";
        private const string TerminationChargeBroadbandKey = "TerminationChargeBroadband";
        private const string TerminationChargeFibreAndFibrePlusKey = "TerminationChargeFibreAndFibrePlus";
        private const string ProductGroupSection = "productGroup";
        private const string EarlyExitFeeFixAndFibreKey = "EarlyExitFeeFixAndFibre";

        public BroadbandManager(IConfigManager configManager, IConfigurationSettings configurationSettings)
        {
            Guard.Against<ArgumentException>(configManager == null, $"{nameof(configManager)} is null");
            Guard.Against<ArgumentException>(configurationSettings == null, $"{nameof(configurationSettings)} is null");
            _configManager = configManager;
            _configurationSettings = configurationSettings;
        }

        public BroadbandType GetBroadbandType(string broadbandCode)
        {
            var type = (BroadbandType) 0;
            ProductSettings product = _configurationSettings.BroadbandManagementSettings.Products.FirstOrDefault(pc => pc.Code == broadbandCode);
            
            if (product != null)
            {
                type = product.BroadbandType;
            }
            
            return type;
        }

        public double GetSurcharge()
        {
            double.TryParse(_configManager.GetValueForKeyFromSection(SectionGroupPath, AdditionalPricesSection, SurchargeKey), out double surcharge);
            return surcharge;
        }

        public double GetConnectionFee()
        {
            double.TryParse(_configManager.GetValueForKeyFromSection(SectionGroupPath, AdditionalPricesSection, ConnectionFeeKey), out double connectionFee);
            return connectionFee;
        }

        public double GetInstallationFee()
        {
            double.TryParse(_configManager.GetValueForKeyFromSection(SectionGroupPath, AdditionalPricesSection, InstallationFeeKey), out double installationFee);
            return installationFee;
        }

        public double GetEquipmentChargeFee()
        {
            double.TryParse(_configManager.GetValueForKeyFromSection(SectionGroupPath, AdditionalPricesSection, EquipmentChargeKey), out double equipmentCharge);
            return equipmentCharge;
        }

        public decimal GetEarlyTerminationChargeBroadbandFee()
        {
            decimal.TryParse(_configManager.GetValueForKeyFromSection(SectionGroupPath, AdditionalPricesSection, EarlyTerminationChargeBroadbandKey), out decimal earlyTerminationBroadbandCharge);
            return earlyTerminationBroadbandCharge;
        }

        public decimal GetEarlyExitFeeFixAndFibre()
        {
            decimal.TryParse(_configManager.GetValueForKeyFromSection(SectionGroupPath, AdditionalPricesSection, EarlyExitFeeFixAndFibreKey), out decimal earlyExitFeeFixAndFibre);
            return earlyExitFeeFixAndFibre;
        }

        public decimal GetEarlyTerminationChargeFibreAndFibrePlusFee()
        {
            decimal.TryParse(_configManager.GetValueForKeyFromSection(SectionGroupPath, AdditionalPricesSection, EarlyTerminationChargeFibreAndFibrePlusKey), out decimal earlyTerminationFibreFibrePlusCharge);

            return earlyTerminationFibreFibrePlusCharge;
        }

        public decimal GetTerminationChargeBroadbandFee()
        {
            decimal.TryParse(_configManager.GetValueForKeyFromSection(SectionGroupPath, AdditionalPricesSection, TerminationChargeBroadbandKey), out decimal terminationChargeBroadband);
            return terminationChargeBroadband;
        }

        public decimal GetTerminationChargeFibreAndFibrePlusFee()
        {
            decimal.TryParse(_configManager.GetValueForKeyFromSection(SectionGroupPath, AdditionalPricesSection, TerminationChargeFibreAndFibrePlusKey), out decimal terminationFibreFibrePlusCharge);
            return terminationFibreFibrePlusCharge;
        }

        public BroadbandProductGroup BroadbandProductGroup(string productCode)
        {
            Enum.TryParse(_configManager.GetValueForKeyFromSection(SectionGroupPath, ProductGroupSection, productCode), out BroadbandProductGroup group);
            return group;
        }

        public List<TermsAndConditionsPdfLink> GetTermsAndConditionPdfWithLinks(BroadbandProductGroup productGroup)
        {
            return GetPdfLinks(productGroup.ToString())?.Select(link => new TermsAndConditionsPdfLink
            {
                DisplayName = Path.GetFileNameWithoutExtension(link),
                Link = link,
                Title = string.Format(Common_Resources.PdfAltText, Path.GetFileNameWithoutExtension(link))

            }).ToList() ?? new List<TermsAndConditionsPdfLink>();
        }

        // ReSharper disable once ReturnTypeCanBeEnumerable.Local
        private List<string> GetPdfLinks(string broadbandProductGroupName)
        {
            string pdfLink = _configManager.GetValueForKeyFromSection(SectionGroupPath, "availableTariffPdfs", broadbandProductGroupName);

            if (!string.IsNullOrEmpty(pdfLink))
            {
                return pdfLink.Split('|').ToList();
            }

            return new List<string>();
        }
    }
}