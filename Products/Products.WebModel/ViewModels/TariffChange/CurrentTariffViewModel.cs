namespace Products.WebModel.ViewModels.TariffChange
{
    using Infrastructure.Extensions;

    public class CurrentTariffViewModel
    {
        public string Name { get; set; }

        public string DisplayName => Name.TrimEconomyWording();

        public string ExpirationMessage { get; set; }

        public string MonthlyCost { get; set; }

        public string AnnualCost { get; set; }

        public string ElectricityAnnualCost { get; set; }

        public string ElectricityMonthlyCost { get; set; }

        public string GasAnnualCost { get; set; }

        public string GasMonthlyCost { get; set; }

        public double ElectricityAnnualUsage { get; set; }

        public double ElectricityAnnualUsageDayOrStandard { get; set; }

        public double ElectricityAnnualUsageNightOrOffPeak { get; set; }

        public string DayOrStandardLabel { get; set; }

        public string NightOrOffPeakLabel { get; set; }

        public double GasAnnualUsage { get; set; }

        public bool IsStandardTariff => Name.ToLower().Contains("standard");

        public bool Has365OrMoreDaysRemaining { get; set; }

        public string IntroParagraph1 { get; set; }

        public bool ShowIntroParagraph2 => !string.IsNullOrEmpty(IntroParagraph2);

        public string IntroParagraph2 { get; set; }

        public bool ShowIntroParagraph3 => !string.IsNullOrEmpty(IntroParagraph3);

        public string IntroParagraph3 { get; set; }

        public string MonthlyCostHeader { get; set; }

        public string AnnualCostHeader { get; set; }

        public TariffInformationLabel ElectricityDetails { get; set; }

        public TariffInformationLabel GasDetails { get; set; }

        public string HtmlSafeName => DisplayName.GetHTMLSafeName();

        public bool ShowWHDText { get; set; }

        public bool ShowFixAndControlExitFee { get; set; }
    }
}