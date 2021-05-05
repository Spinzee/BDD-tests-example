namespace Products.Model.TariffChange.Tariffs
{
    using System;
    using Infrastructure.Extensions;
    using Model.Enums;

    public class CurrentTariffForFuel
    {
        public string Name { get; set; }

        public string DisplayName => Name.TrimEconomyWording();

        public string BrandCode { get; set; }

        public FuelType FuelType { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsRenewal => EndDate > DateTime.Today && EndDate <= DateTime.Today.AddDays(60);

        public double AnnualUsageKwh { get; set; }

        public double AnnualCost { get; set; }

        public double PeakPercentageOperand { get; set; }

        public double OffPeakPercentageOperand => Math.Round(1 - PeakPercentageOperand, 4);

        public string ServicePlanId { get; set; }

        public string TariffType { get; set; }
    }
}