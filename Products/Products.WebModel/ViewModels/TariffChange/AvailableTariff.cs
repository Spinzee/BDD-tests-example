namespace Products.WebModel.ViewModels.TariffChange
{
    using System;
    using System.Collections.Generic;
    using Core;
    using Infrastructure.Extensions;

    public class AvailableTariff : IComparable<AvailableTariff>
    {
        public string Name { get; set; }

        public string DisplayName => Name.TrimEconomyWording();

        public string HtmlSafeName => DisplayName.GetHTMLSafeName();

        public string Tagline { get; set; }

        public List<string> TermsAndConditionsPdfLinks { get; set; } = new List<string>();

        public string ExitFeePerFuel { get; set; }

        public double ExitFee { get; set; }

        public string AdditionalTariffCardText { get; set; }

        public double ProjectedAnnualCostValue { get; set; }

        public string ProjectedMonthlyCostValue { get; set; }

        public string ProjectedMonthlyCost { get; set; }

        public string ProjectedAnnualCost { get; set; }

        public bool IsDualFuel => ElectricityDetails != null && GasDetails != null;

        public TariffInformationLabel ElectricityDetails { get; set; }

        public TariffInformationLabel GasDetails { get; set; }

        public int CompareTo(AvailableTariff other)
        {
            if (ProjectedAnnualCostValue > other.ProjectedAnnualCostValue)
            {
                return 1;
            }

            if (ProjectedAnnualCostValue < other.ProjectedAnnualCostValue)
            {
                return -1;
            }

            return 0;
        }
        public TariffGroup TariffGroup { get; set; }

        public bool IsFollowOnTariff { get; set; }

        public bool IsSmartTariff { get; set; }

        public bool ShowWHDText { get; set; }

        public Dictionary<string, string> TickUsps { get; set; }

        public bool ShowTickUsps => TickUsps != null && TickUsps.Count > 0;

        public bool ShowAdditionalTariffCardText => !string.IsNullOrEmpty(AdditionalTariffCardText) && TariffGroup != TariffGroup.None;
    }
}