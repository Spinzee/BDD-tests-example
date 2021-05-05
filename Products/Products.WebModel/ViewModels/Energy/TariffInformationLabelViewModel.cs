using Products.Model.Enums;
using System.Collections.Generic;

namespace Products.WebModel.ViewModels.Energy
{
    public class TariffInformationLabelViewModel
    {
        public string Supplier { get; set; }
        public string TariffName { get; set; }
        public string TariffType { get; set; }
        public string PaymentMethod { get; set; }
        public string UnitRate { get; set; }
        public string UnitRatePeak { get; set; }
        public string UnitRateOffPeak { get; set; }
        public string StandingCharge { get; set; }
        public string TariffEndsOn { get; set; }
        public string PriceGuaranteedUntil { get; set; }
        public string ExitFee { get; set; }
        public string Discount { get; set; }
        public string AdditionalProducts { get; set; }
        public FuelType FuelType { get; set; }


        public string ServicePlanId { get; set; }
        public string RateCode { get; set; }
        public string RateCodeDescription { get; set; }

        public List<UnitRateViewModel> UnitRates { get; set; }

        public string StandingChargeValue { get; set; }
        public string StandingChargeValueExVAT { get; set; }

        public string ModalId => $"{ServicePlanId}_{RateCode}";
        public bool? IsBundle { get; set; }
    }

    public class UnitRateViewModel
    {
        public string UnitRateInclVAT { get; set; }
        public string UnitRateExVAT { get; set; }
        public string UnitRateLabel { get; set; }

    }
}