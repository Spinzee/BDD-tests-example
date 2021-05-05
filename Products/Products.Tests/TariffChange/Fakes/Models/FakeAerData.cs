namespace Products.Tests.TariffChange.Fakes.Models
{
    using System;
    using System.Collections.Generic;
    using ServiceWrapper.AnnualEnergyReviewService;

    public class FakeAERData
    {
        public string CustomerAccountNumber { get; set; } = "1111111113";

        public string CustomerName { get; set; }

        public Dictionary<string, string> CustomerAccountVariables { get; set; } = new Dictionary<string, string>
        {
            { "LastBillSendDays", "212" },
            { "RegisterCount", "01" },
            { "GreenDealSite", "N" },
            { "CHPAccount", "N" },
            { "M&SBrand", "N" },
            { "MixedBrands", "N" },
            { "SameSite", "Y" },
            { "PPSpecialInt", "N" },
            { "CAException", "N" },
            { "AERPendingTriggers", "00" },
            { "PaymentMethod", "QC" },
            { "TariffChangeDays", "025" }
        };

        public paymentMethodTypePaymentMethodCode PaymentMethodCode { get; set; } = paymentMethodTypePaymentMethodCode.QC;
        public bool HasDirectDebitDiscount { get; set; } = true;
        public bool IsPaperless { get; set; } = true;
        public double AnnualCost { get; set; } = 120;
        public double AnnualUsageKwh { get; set; } = 1000;

        public consumptionDetailsTypeConsumptionRuleDescription ConsumptionRuleDescription { get; set; } =
            consumptionDetailsTypeConsumptionRuleDescription.lc31a;

        public DateTime EndDate { get; set; } = DateTime.Today.AddDays(100);
        public string CollectionDay { get; set; }
        public string FollowOnServicePlanId { get; set; }
        public string ServicePlanId { get; } = "ABC123";
    }
}