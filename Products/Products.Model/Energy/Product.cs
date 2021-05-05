namespace Products.Model.Energy
{
    using Enums;

    using System;
    using Infrastructure.Extensions;

    public class Product
    {
        public double? ProjectedYearlyCost { get; set; }

        public double? ExitFee1 { get; set; }

        public string ServicePlanId { get; set; }

        public string ServicePlanInvoiceDescription { get; set; }

        public string DisplayName => ServicePlanInvoiceDescription.TrimEconomyWording();

        public TariffType TariffType { get; set; }

        public string EndOfTariffDate { get; set; }

        public string PriceGuaranteeDate { get; set; }

        public string RateCodeStandardDescription { get; set; }

        public string LoyaltyBenefits { get; set; }

        public string EndOfTariffDateDescription { get; set; }

        public string PriceGuaranteeDateDescription { get; set; }

        public int? RateCode { get; set; }

        public double? UnitRate1InclVAT { get; set; }

        public double? UnitRate1ExVAT { get; set; }

        public string UnitRate1InvoiceDescription { get; set; }

        public double? UnitRate2InclVAT { get; set; }

        public double? UnitRate2ExVAT { get; set; }

        public string UnitRate2InvoiceDescription { get; set; }

        public double? UnitRate3InclVAT { get; set; }

        public double? UnitRate3ExVAT { get; set; }

        public string UnitRate3InvoiceDescription { get; set; }

        public double? UnitRate4InclVAT { get; set; }

        public double? UnitRate4ExVAT { get; set; }

        public string UnitRate4InvoiceDescription { get; set; }

        public double? StandingCharge { get; set; }

        public double? StandingChargeExVAT { get; set; }

        public double? DirectDebitDiscount { get; set; }

        public bool? IsBundle { get; set; }

        public DateTime EffectiveDate { get; set; }
    }
}
