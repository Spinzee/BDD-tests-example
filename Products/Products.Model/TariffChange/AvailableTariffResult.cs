using Products.Model.TariffChange.Tariffs;

namespace Products.Model.TariffChange
{
    public class AvailableTariffResult
    {
        public int RowId { get; set; }

        public string ServicePlanID { get; set; }

        public string GeographicZone { get; set; }

        public string BrandCode { get; set; }

        public string TariffType { get; set; }

        public string MeterCategory { get; set; }

        public string EndOfTariffDate { get; set; }

        public string PriceGuaranteeDate { get; set; }

        public string TariffEndDateDescription { get; set; }

        public string PriceGuaranteeDateDescription { get; set; }

        public double ExitFee1 { get; set; }

        public double ExitFee2 { get; set; }

        public int RateCode { get; set; }

        public string RateCodeName { get; set; }

        public string PaymentMethod { get; set; }

        public string RateCodeStandardDescription { get; set; }

        public double TCR { get; set; }

        public double StandingChargeExcVat { get; set; }

        public double StandingChargeInclVAT { get; set; }

        public double UnitRate1ExcVAT { get; set; }

        public double UnitRate1InclVAT { get; set; }

        public string UnitRate1SPCOBillingDesc { get; set; }

        public double UnitRate2ExcVAT { get; set; }

        public double UnitRate2InclVAT { get; set; }

        public string UnitRate2SPCOBillingDesc { get; set; }

        public double UnitRate3ExcVAT { get; set; }

        public double UnitRate3InclVAT { get; set; }

        public string UnitRate3SPCOBillingDesc { get; set; }

        public double UnitRate4ExcVAT { get; set; }

        public double UnitRate4InclVAT { get; set; }

        public string UnitRate4SPCOBillingDesc { get; set; }

        public string ServicePlanInvoiceDescription { get; set; }


        public string LoyaltySchemeUnits { get; set; }

        public string BusinessUseCode { get; set; }

        public string TariffUniqueId { get; set; }

        public Tariff GasResult => new Tariff
        {
            Name = ServicePlanInvoiceDescription,
            Type = TariffType,
            GasDetails = new TariffForFuel
            {
                PaymentMethod = PaymentMethod,
                UnitRate1ExclVat = UnitRate1ExcVAT,
                UnitRate2ExclVat = UnitRate2ExcVAT,
                UnitRate1InclVat = UnitRate1InclVAT,
                UnitRate2InclVat = UnitRate2InclVAT,
                StandingChargeExclVat = StandingChargeExcVat,
                StandingChargeInclVat = StandingChargeInclVAT,
                DayOrStandardLebel = UnitRate1SPCOBillingDesc,
                NightOrOffPeakLebel = UnitRate2SPCOBillingDesc,
                TCR = TCR,
                TariffEndDescription = TariffEndDateDescription,
                PriceGuaranteeDescription = PriceGuaranteeDateDescription,
                ExitFee = ExitFee1,
                RateCodeStandardDescription = RateCodeStandardDescription,
                AdditionalProductsIncluded = LoyaltySchemeUnits,
                ServicePlanId = ServicePlanID
            }
        };

        public Tariff ElectricityResult => new Tariff
        {
            Name = ServicePlanInvoiceDescription,
            Type = TariffType,
            ElectricityDetails = new TariffForFuel
            {
                PaymentMethod = PaymentMethod,
                UnitRate1ExclVat = UnitRate1ExcVAT,
                UnitRate2ExclVat = UnitRate2ExcVAT,
                UnitRate1InclVat = UnitRate1InclVAT,
                UnitRate2InclVat = UnitRate2InclVAT,
                StandingChargeExclVat = StandingChargeExcVat,
                StandingChargeInclVat = StandingChargeInclVAT,
                DayOrStandardLebel = UnitRate1SPCOBillingDesc,
                NightOrOffPeakLebel = UnitRate2SPCOBillingDesc,
                TCR = TCR,
                TariffEndDescription = TariffEndDateDescription,
                PriceGuaranteeDescription = PriceGuaranteeDateDescription,
                ExitFee = ExitFee1,
                RateCodeStandardDescription = RateCodeStandardDescription,
                AdditionalProductsIncluded = LoyaltySchemeUnits,
                ServicePlanId = ServicePlanID
            }
        };
    }
}
