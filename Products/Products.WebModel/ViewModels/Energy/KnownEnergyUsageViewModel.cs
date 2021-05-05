namespace Products.WebModel.ViewModels.Energy
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Attributes;
    using Core;
    using Model.Enums;
    using Products.WebModel.Resources.Common;
    using Products.WebModel.Resources.Energy;

    public class KnownEnergyUsageViewModel
    {
        [AriaDescription(ResourceType = typeof(EnergyUsage_Resources), Name = nameof(EnergyUsage_Resources.ElectricityUsageAriaDescription))]
        [RequiredIf("ShowStandardElectricityUsage", true, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = nameof(Form_Resources.ElectricityUsageRequiredError))]
        [RegularExpression(RegularExpressionConstants.MeterReading, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = nameof(Form_Resources.ElectricityUsageRegexError))]
        [Range(0, 999999, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = nameof(Form_Resources.ElectricityUsageRegexError))]
        [Display(ResourceType = typeof(Form_Resources), Name = nameof(Form_Resources.ElectricityUsageTitle))]
        [UIHint("Numbers")]
        public int? StandardElectricityUsage { get; set; }

        [AriaDescription(ResourceType = typeof(EnergyUsage_Resources), Name = nameof(EnergyUsage_Resources.GasUsageAriaDescription))]
        [RequiredIf("ShowStandardGasUsage", true, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = nameof(Form_Resources.GasUsageRequiredError))]
        [RegularExpression(RegularExpressionConstants.MeterReading, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = nameof(Form_Resources.GasUsageRegexError))]
        [Range(0, 999999, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = nameof(Form_Resources.GasUsageRegexError))]
        [Display(ResourceType = typeof(Form_Resources), Name = nameof(Form_Resources.GasUsageTitle))]
        [UIHint("Numbers")]
        public int? StandardGasUsage { get; set; }

        [AriaDescription(ResourceType = typeof(EnergyUsage_Resources), Name = nameof(EnergyUsage_Resources.Economy7ElecricityDayUsageAriaDescription))]
        [RequiredIf("ShowEconomy7Usage", true, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = nameof(Form_Resources.Economy7ElecricityDayUsageRequiredError))]
        [RegularExpression(RegularExpressionConstants.MeterReading, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = nameof(Form_Resources.Economy7ElecricityDayUsageRegexError))]
        [Range(0, 999999, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = nameof(Form_Resources.Economy7ElecricityDayUsageRegexError))]
        [Display(ResourceType = typeof(Form_Resources), Name = nameof(Form_Resources.Economy7ElecricityDayUsageTitle))]
        [UIHint("Numbers")]
        public int? Economy7ElectricityDayUsage { get; set; }

        [AriaDescription(ResourceType = typeof(EnergyUsage_Resources), Name = nameof(EnergyUsage_Resources.Economy7ElectricityNightUsageAriaDescription))]
        [RequiredIf("ShowEconomy7Usage", true, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = nameof(Form_Resources.Economy7ElectricityNightUsageRequiredError))]
        [RegularExpression(RegularExpressionConstants.MeterReading, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = nameof(Form_Resources.Economy7ElectricityNightUsageRegexError))]
        [Range(0, 999999, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = nameof(Form_Resources.Economy7ElectricityNightUsageRegexError))]
        [Display(ResourceType = typeof(Form_Resources), Name = nameof(Form_Resources.Economy7ElectricityNightUsageTitle))]
        [UIHint("Numbers")]
        public int? Economy7ElectricityNightUsage { get; set; }

        public UsageFrequency Frequency { get; set; } = UsageFrequency.Annual;

        [HiddenInput]
        public FuelType SelectedFuelType { get; set; }

        [HiddenInput]
        public ElectricityMeterType SelectedElectricityMeterType { get; set; }

        public bool ShowStandardElectricityUsage => SelectedFuelType != FuelType.Gas && SelectedElectricityMeterType != ElectricityMeterType.Economy7;

        public bool ShowStandardGasUsage => SelectedFuelType != FuelType.Electricity;

        public bool ShowEconomy7Usage => SelectedFuelType != FuelType.Gas && SelectedElectricityMeterType == ElectricityMeterType.Economy7;
    }
}
