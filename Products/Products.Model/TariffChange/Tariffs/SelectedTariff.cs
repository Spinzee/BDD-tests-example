namespace Products.Model.TariffChange.Tariffs
{
    using System;
    using System.Collections.Generic;
    using Core;
    using Infrastructure.Extensions;

    public class SelectedTariff
    {
        public string Name { get; set; }

        public string DisplayName => Name.TrimEconomyWording();

        public string Tagline { get; set; }

        public string ExitFeePerFuel { get; set; }

        public double ExitFee { get; set; }

        public string AdditionalProductsIncluded { get; set; }

        public string ProjectedMonthlyCost { get; set; }

        public string ProjectedAnnualCost { get; set; }

        public double ProjectedAnnualCostValue { get; set; }

        public string ProjectedMonthlyCostValue { get; set; }

        public DateTime EffectiveDate { get; set; }

        public List<string> TermsAndConditionsPdfLink { get; set; } = new List<string>();

        public bool IsFollowOnTariff { get; set; }

        public TariffGroup TariffGroup { get; set; }

        public bool IsSmartTariff  { get; set; }
    }
}