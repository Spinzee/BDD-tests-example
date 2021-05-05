namespace Products.Model.Energy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Broadband;
    using Core;
    using Core.Enums;
    using Enums;

    public class Tariff
    {
        public Tariff()
        {
            Extras = new List<Extra>();
        }

        public string DisplayName { get; set; }

        public string DisplayNameSuffix { get; set; }

        public Product ElectricityProduct { get; set; }

        public Product GasProduct { get; set; }

        public BundlePackage BundlePackage { get; set; }

        public string TariffId { get; set; }

        public TariffGroup TariffGroup { get; set; }

        public IEnumerable<TariffTickUsp> EnergyTickUsps { get; set; }

        public bool IsBundle { get; set; }

        public bool IsSmartTariff { get; set; }

        public BundlePackageType BundlePackageType => BundlePackage?.BundlePackageType ?? BundlePackageType.None;

        public List<Extra> Extras { get; set; }

        public bool IsUpgrade { get; set; }
        public string BackendTariffName { get; set; }
    }

    public static class TariffExtensions
    {
        public static FuelType GetFuelType(this Tariff tariff)
        {
            if (tariff.ElectricityProduct != null && tariff.GasProduct != null)
            {
                return FuelType.Dual;
            }

            if (tariff.ElectricityProduct == null && tariff.GasProduct != null)
            {
                return FuelType.Gas;
            }

            return FuelType.Electricity;
        }

        public static double GetProjectedCombinedYearlyCost(this Tariff tariff, TalkProduct talkProduct = null)
        {
            double elecCost = tariff.ElectricityProduct?.ProjectedYearlyCost ?? 0;
            double gasCost = tariff.GasProduct?.ProjectedYearlyCost ?? 0;
            double broadbandCost = tariff.BundlePackage?.ProjectedYearlyCost ?? 0;
            double talkCost = talkProduct?.GetMonthlyTalkCost() * 12 ?? 0.0;
            return elecCost + gasCost + broadbandCost + talkCost;
        }

        public static double GetProjectedYearlyEnergyCost(this Tariff tariff)
        {
            double? elecCost = tariff.GetProjectedYearlyElectricityCost();
            double? gasCost = tariff.GetProjectedYearlyGasCost();
            return (elecCost == null ? 0 : Convert.ToDouble(elecCost)) + (gasCost == null ? 0 : Convert.ToDouble(gasCost));
        }

        public static double GetProjectedMonthlyEnergyCost(this Tariff tariff)
        {
            double? elecCost = tariff.GetProjectedMonthlyElectricityCost();
            double? gasCost = tariff.GetProjectedMonthlyGasCost();
            return (elecCost == null ? 0 : Convert.ToDouble(elecCost)) + (gasCost == null ? 0 : Convert.ToDouble(gasCost));
        }

        public static double GetProjectedMonthlyCost(this Tariff tariff, TalkProduct talkProduct = null) => tariff.GetProjectedCombinedYearlyCost(talkProduct) / 12;

        public static double? GetProjectedYearlyElectricityCost(this Tariff tariff) => tariff.ElectricityProduct?.ProjectedYearlyCost;

        public static double? GetProjectedMonthlyElectricityCost(this Tariff tariff) => tariff.GetProjectedYearlyElectricityCost() / 12;

        public static double? GetProjectedYearlyGasCost(this Tariff tariff) => tariff.GasProduct?.ProjectedYearlyCost;

        public static double? GetProjectedMonthlyGasCost(this Tariff tariff) => tariff.GetProjectedYearlyGasCost() / 12;

        public static double? GetProjectedMonthlyBundleCost(this Tariff tariff) => tariff.BundlePackage?.ProjectedYearlyCost / 12;

        public static bool IsFixedTariff(this Tariff tariff) => tariff.ElectricityProduct?.TariffType == TariffType.Fixed || tariff.GasProduct?.TariffType == TariffType.Fixed;

        public static bool IsStandardTariff(this Tariff tariff) => tariff.ElectricityProduct?.TariffType == TariffType.Evergreen || tariff.GasProduct?.TariffType == TariffType.Evergreen;

        public static double GetDirectDebitDiscountForElectricity(this Tariff tariff) => tariff.ElectricityProduct?.DirectDebitDiscount ?? 0;

        public static double GetDirectDebitDiscountForGas(this Tariff tariff) => tariff.GasProduct?.DirectDebitDiscount ?? 0;

        public static double GetTotalDirectDebitDiscount(this Tariff tariff) => tariff.GetDirectDebitDiscountForElectricity() + tariff.GetDirectDebitDiscountForGas();

        public static bool IsBroadbandBundle(this Tariff tariff) => tariff.BundlePackage?.BundlePackageType == BundlePackageType.FixAndFibre;

        public static bool IsHesBundle(this Tariff tariff) => tariff.BundlePackage?.BundlePackageType == BundlePackageType.FixAndProtect;

        public static bool HasExtras(this Tariff tariff) => null != tariff.Extras && tariff.Extras.Any();
    }
}