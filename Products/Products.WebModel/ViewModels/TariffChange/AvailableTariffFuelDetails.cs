namespace Products.WebModel.ViewModels.TariffChange
{
    using Infrastructure.Extensions;

    public class TariffInformationLabel
    {
        public string AnnualCost { get; set; }

        public string MonthlyCost { get; set; }

        public double AnnualCostValue { get; set; }

        public string Supplier { get; set; }

        public string TariffName { get; set; }

        public string DisplayName => TariffName.TrimEconomyWording();

        public string TariffType { get; set; }

        public string PaymentMethod { get; set; }

        public string UnitRate1 { get; set; }

        public string UnitRate2 { get; set; }

        public string DayOrStandardLabel { get; set; }

        public string NightOrOffPeakLabel { get; set; }

        public string StandingCharge { get; set; }

        public string TariffEndsOn { get; set; }

        public string PriceGuaranteedUntil { get; set; }

        public string ExitFees { get; set; }

        public string DiscountsAndAdditionalCharges { get; set; }

        public string AdditionalProductsAndServicesIncluded { get; set; }

        public string TCR { get; set; }

        public string ServicePlanId { get; set; }
    }
}