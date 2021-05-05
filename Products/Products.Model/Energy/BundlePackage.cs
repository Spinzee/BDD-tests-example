namespace Products.Model.Energy
{
    using System;
    using System.Collections.Generic;
    using Core.Enums;
    using HomeServices;
    using Infrastructure;

    public class BundlePackage
    {
        private int ContractLength { get; }

        public BundlePackage(
            string code
            , double monthlyDiscountedCost
            , double monthlyOriginalCost
            , BundlePackageType bundlePackageType
            , IEnumerable<TariffTickUsp> tickUsps)
        {
            Guard.Against<ArgumentNullException>(code == null, $"{nameof(code)} is null");
            Guard.Against<ArgumentException>(monthlyDiscountedCost < 0.0, $"{nameof(monthlyDiscountedCost)} less than 0.0");
            Guard.Against<ArgumentException>(monthlyOriginalCost <= 0.0, $"{nameof(monthlyOriginalCost)} is less than 0.0");
            Guard.Against<ArgumentException>(monthlyDiscountedCost > monthlyOriginalCost
                , $"Bundle broadband discounted monthly cost({monthlyDiscountedCost}) is more than {monthlyOriginalCost}." +
                  "This makes bundle 'Yearly Savings' become a negative value, i.e. there is no discount/savings on the bundle.");

            ProductCode = code;
            MonthlyDiscountedCost = monthlyDiscountedCost;
            MonthlyOriginalCost = monthlyOriginalCost;
            TickUsps = tickUsps ?? new List<TariffTickUsp>();
            ContractLength = bundlePackageType == BundlePackageType.FixAndFibre ? 18 : 12;
            ProjectedYearlyCost = monthlyDiscountedCost * 12;
            YearlySavings = Math.Round(MonthlyOriginalCost * ContractLength - monthlyDiscountedCost * ContractLength, 2);
            MonthlySavings = Math.Round(MonthlyOriginalCost - MonthlyDiscountedCost, 2);
            MonthlySavingsPercentage = Math.Floor(MonthlySavings / MonthlyOriginalCost * 100.00);
            MonthlyOriginalCostInPounds = MonthlyOriginalCost.ToString("C").Split('.')[0];
            BundlePackageType = bundlePackageType;
        }

        public BundlePackageType BundlePackageType { get; }

        public string ProductCode { get; }

        public double MonthlyDiscountedCost { get; }

        public double MonthlyOriginalCost { get; }

        public IEnumerable<TariffTickUsp> TickUsps { get; }

        public double ProjectedYearlyCost { get; }

        public double YearlySavings { get; }

        public double MonthlySavings { get; }

        public double MonthlySavingsPercentage { get; }

        public string MonthlyOriginalCostInPounds { get; }

        public ProductGroup HesMoreInformation { get; set; }
    }
}