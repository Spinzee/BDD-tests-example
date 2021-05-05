namespace Products.Model.HomeServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;

    [Serializable]
    public class HomeServicesCustomer
    {
        public string CoverPostcode { get; set; }

        public PersonalDetails PersonalDetails { get; set; }

        public QasAddress SelectedCoverAddress { get; set; }

        public ContactDetails ContactDetails { get; set; }

        public DirectDebitDetails DirectDebitDetails { get; set; }

        public int BankServiceRetryCount { get; set; }

        public ProductGroup AvailableProduct { get; set; }

        public string SelectedProductCode { get; set; }

        public List<string> SelectedExtraCodes { get; set; } = new List<string>();

        public bool IsLandlord { get; set; }

        public QasAddress SelectedBillingAddress { get; set; }

        public string BillingPostcode { get; set; }

        public List<int> ApplicationIds { get; set; } = new List<int>();

        public string MigrateAffiliateId { get; set; }

        public string MigrateMemberid { get; set; }

        public string CampaignCode { get; set; }
    }

    // ReSharper disable once UnusedMember.Global
    public static class HomeServicesCustomerExtensions
    {
        public static bool HasExcesses(this HomeServicesCustomer customer)
        {
            List<double> excessesValues = customer.GetExcesses();
            return excessesValues?.Count > 1 || excessesValues?.Count == 1 && excessesValues.First() > 0;
        }

        public static List<ProductExtra> GetSelectedExtras(this HomeServicesCustomer customer)
        {
            return customer.AvailableProduct?.Extras?.Where(extra => customer.SelectedExtraCodes.Contains(extra.ProductCode)).ToList() ??
                   new List<ProductExtra>();
        }

        public static List<double> GetExcesses(this HomeServicesCustomer customer)
        {
            return customer.AvailableProduct?.Products?.Select(e => e.Excess).ToList() ?? new List<double>();
        }

        public static Product GetSelectedProduct(this HomeServicesCustomer customer)
        {
            return customer.AvailableProduct?.Products?.FirstOrDefault(p => string.Equals(p.ProductCode, customer.SelectedProductCode, StringComparison.CurrentCultureIgnoreCase));
        }

        public static double GetMonthlyProductCost(this HomeServicesCustomer customer)
        {
            return customer.GetSelectedProduct()?.MonthlyCost ?? 0;
        }

        public static double GetTotalMonthlyCostWithExtras(this HomeServicesCustomer customer)
        {
            double monthlyProductCost = GetMonthlyProductCost(customer);
            double extrasTotalCost = customer.GetSelectedExtras()?.Select(e => e.Cost).Sum() ?? 0;
            return monthlyProductCost + extrasTotalCost;
        }

        public static double GetYearlyProductCost(this HomeServicesCustomer customer)
        {
            double monthlyProductCost = GetMonthlyProductCost(customer);
            double yearlyProductCost = monthlyProductCost * 12;

            if (customer.GetSelectedProduct().OfferSummary != null)
            {
                yearlyProductCost -= customer.GetSelectedProduct().OfferSummary.Amount;
            }

            return yearlyProductCost;
        }

        public static string GetFormattedTotalYearlyProductCost(this HomeServicesCustomer customer)
        {
            return GetYearlyProductCost(customer).ToString("C");
        }

        public static string GetFormattedTotalYearlyCostWithExtras(this HomeServicesCustomer customer)
        {
            double totalProductCost = GetYearlyProductCost(customer);
            double totalExtrasCost = GetYearlyExtrasCost(customer);
            return (totalProductCost + totalExtrasCost).ToString("C");
        }

        private static double GetYearlyExtrasCost(this HomeServicesCustomer customer)
        {
            return (customer.GetSelectedExtras()?.Select(e => e.Cost).Sum() ?? 0) * 12;
        }

        public static bool IsProductExtrasAvailable(this HomeServicesCustomer customer)
        {
            return customer.AvailableProduct?.Extras?.Any() ?? false;
        }
    }
}