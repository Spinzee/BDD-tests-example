namespace Products.Model.TariffChange.Customers
{
    using System;
    using Tariffs;

    public class CustomerAccount
    {
        public SiteDetails SiteDetails { get; set; }

        public PaymentDetails PaymentDetails { get; set; }

        public CurrentTariffForFuel CurrentTariff { get; set; }

        public SelectedTariffForFuel SelectedTariff { get; set; }

        public bool IsBillingException { get; set; }

        public DateTime LastBilledDate { get; set; }

        public string FollowOnTariffServicePlanID { get; set; }

        public string ConsumptionRuleDescription { get; set; }

        public bool IsSmart { get; set; }

        public bool IsSmartEligible { get; set; }

        public bool IsWHD { get; set; }
    }
}