namespace Products.Model.TariffChange.Tariffs
{
    public class TariffForFuel
    {
        public string PaymentMethod { get; set; }
        public double UnitRate1ExclVat { get; set; }
        public double UnitRate1InclVat { get; set; }
        public double UnitRate2ExclVat { get; set; }
        public double UnitRate2InclVat { get; set; }
        public double StandingChargeExclVat { get; set; }
        public double StandingChargeInclVat { get; set; }
        public string DayOrStandardLebel { get; set; }
        public string NightOrOffPeakLebel { get; set; }
        public double TCR { get; set; }
        public string TariffEndDescription { get; set; }
        public string PriceGuaranteeDescription { get; set; }
        public double ExitFee { get; set; }
        public string RateCodeStandardDescription { get; set; }
        public string AdditionalProductsIncluded { get; set; }
        public string ServicePlanId { get; set; }
    }
}