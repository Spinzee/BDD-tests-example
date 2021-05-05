namespace Products.Tests.TariffChange.Fakes.Models
{
    using Model.Enums;

    public class FakeTariffData
    {
        public FuelType FuelType { get; set; } = FuelType.Gas;

        public string Name { get; set; } = "SSE 1 Year Fixed";

        public string Type { get; set; } = "Fixed";

        public double ExitFee { get; set; } = 25.00;

        public string MeterCategory { get; set; } = "B";

        public int RateCode { get; set; } = 4;

        public string PaymentMethod { get; set; } = "Direct Debit";

        public double UnitRate1ExclVat { get; set; } = 10.00;

        public double UnitRate1InclVat { get; set; } = 10.50;

        public double UnitRate2ExclVat { get; set; } = 5.00;

        public double UnitRate2InclVat { get; set; } = 5.25;

        public double StandingChargeExclVat { get; set; } = 20.00;

        public double StandingChargeInclVat { get; set; } = 21.00;

        public double TCR { get; set; } = 12.34;

        public string TariffEndDescription { get; set; } = "This tariff ends in 1 year.";

        public string PriceGuaranteeDescription { get; set; } = "This price is guaranteed for 1 year.";

        public string RateCodeStandardDescription { get; set; } = "These prices include an annual paperless billing discount of £6";

        public string LoyaltyItems { get; set; } = "Includes a £25 Amazon gift card per fuel";

        public string UnitRate1SPCOBillingDesc { get; set; } = "Day";

        public string UnitRate2SPCOBillingDesc { get; set; } = "Night";

        public string ServicePlanId { get; set; } = "ABC123";
    }
}