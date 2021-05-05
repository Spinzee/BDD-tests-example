namespace Products.Model.Energy
{
    using Broadband;
    using Common;
    using Enums;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Core.Enums;
    using Infrastructure.Extensions;

    [Serializable]
    public class EnergyCustomer
    {
        public EnergyCustomer()
        {
            CLIChoice = new CLIChoice();
            UnavailableBundles = new List<string>();
            SelectedExtras = new HashSet<Extra>();
        }

        public Guid? UserId { get; set; }

        public bool IsBundlingJourney { get; set; }

        public bool ShowCli { get; set; }

        public string Postcode { get; set; }

        public FuelType SelectedFuelType { get; set; }

        public PaymentMethod SelectedPaymentMethod { get; set; }

        public BillingPreference SelectedBillingPreference { get; set; }

        public ElectricityMeterType SelectedElectricityMeterType { get; set; }

        public bool? HasSmartMeter { get; set; }

        public bool? ProfileExists { get; set; }

        public PersonalDetails PersonalDetails { get; set; }

        public QasAddress SelectedAddress { get; set; }

        public BTAddress SelectedBTAddress { get; set; }

        public ContactDetails ContactDetails { get; set; }

        public DirectDebitDetails DirectDebitDetails { get; set; }

        public int BankServiceRetryCount { get; set; }

        public Projection Projection { get; set; }

        public bool IsUsageKnown { get; set; }

        public Tariff SelectedTariff { get; set; }

        public string MigrateAffiliateId { get; set; }

        public int EnergyApplicationId { get; set; }

        public string MigrateMemberId { get; set; }

        public string CampaignCode { get; set; }

        public string ChosenProduct { get; set; }

        public string SelectedBroadbandProductCode { get; set; }

        public MeterDetail MeterDetail { get; set; }

        public SmartMeterFrequency SmartMeterFrequency { get; set; }

        public bool ApplyInstallationFee { get; set; }

        public BroadbandProduct SelectedBroadbandProduct { get; set; }

        public CLIChoice CLIChoice { get; set; }

        public bool IsSSECustomerCLI { get; set; }

        public int BroadbandApplicationId { get; set; }

        public List<string> UnavailableBundles { get; set; }

        public HashSet<Extra> SelectedExtras { get; set; }

        public string OnlineAccountPassword { get; set; }

        public void ResetSelectedExtras()
        {
            SelectedExtras = new HashSet<Extra>();
        }

        public bool HasConfirmedNonMatchingBTAddress { get; set; }

        public bool CanUpgradeToFibrePlus { get; set; }

        public Tariff AvailableBundleUpgrade { get; set; }
    }

    public static class EnergyCustomerExtensions
    {
        public static bool HasE7Meter(this EnergyCustomer customer) => customer.SelectedElectricityMeterType == ElectricityMeterType.Economy7;

        public static bool HasStandardMeter(this EnergyCustomer customer) => customer.SelectedElectricityMeterType == ElectricityMeterType.Standard;

        public static bool IsPrePay(this EnergyCustomer customer) => customer.SelectedPaymentMethod == PaymentMethod.PayAsYouGo;

        public static bool HasGas(this EnergyCustomer customer) => customer.SelectedFuelType == FuelType.Dual || customer.SelectedFuelType == FuelType.Gas;

        public static bool HasElectricity(this EnergyCustomer customer) => customer.SelectedFuelType == FuelType.Dual || customer.SelectedFuelType == FuelType.Electricity;

        public static bool IsDualFuel(this EnergyCustomer customer) => customer.SelectedFuelType == FuelType.Dual;

        public static bool IsGasOnly(this EnergyCustomer customer) => customer.SelectedFuelType == FuelType.Gas;

        public static bool IsElectricityOnly(this EnergyCustomer customer) => customer.SelectedFuelType == FuelType.Electricity;

        public static bool IsDirectDebit(this EnergyCustomer customer) => customer.SelectedPaymentMethod == PaymentMethod.MonthlyDirectDebit;

        public static bool IsPaperless(this EnergyCustomer customer) => customer.SelectedBillingPreference == BillingPreference.Paperless;

        public static string FullAddress(this EnergyCustomer customer)
        {
            if (customer.SelectedAddress == null)
            {
                return string.Empty;
            }

            var sb = new System.Text.StringBuilder($"{customer.SelectedAddress.HouseName},");
            sb.Append($" {customer.SelectedAddress.AddressLine1},");

            if (!string.IsNullOrEmpty(customer.SelectedAddress.AddressLine2))
            {
                sb.Append($" {customer.SelectedAddress.AddressLine2},");
            }

            sb.Append($" {customer.SelectedAddress.Town},");

            if (!string.IsNullOrEmpty(customer.SelectedAddress.County))
            {
                sb.Append($" {customer.SelectedAddress.County},");
            }

            sb.Append($" {customer.Postcode}");
            return sb.ToString();
        }

        public static bool IsCAndCJourney(this EnergyCustomer customer)
        {
            if (customer.MeterDetail == null || !(customer.MeterDetail?.MeterInformation?.Any() ?? false))
            {
                return false;
            }

            return customer.SelectedFuelType == FuelType.Gas || customer.GetElectricityMeterInformation() != null;
        }

        public static MeterInformation GetGasMeterInformation(this EnergyCustomer customer)
        {
            return customer.MeterDetail?.MeterInformation?.FirstOrDefault(m => m.FuelType == FuelType.Gas);
        }

        public static MeterInformation GetElectricityMeterInformation(this EnergyCustomer customer)
        {
            return customer.MeterDetail?.MeterInformation?.FirstOrDefault(m => m.FuelType == FuelType.Electricity);
        }

        public static bool IsSseInstaller(this EnergyCustomer customer)
        {
            return customer.IsCAndCJourney() && (customer.MeterDetail?.MeterInformation?.Any(m => m.IsInstallerSSE) ?? false);
        }

        public static bool IsSmartMeterSmets1(this EnergyCustomer customer)
        {
            return customer.IsCAndCJourney() && (customer.MeterDetail?.MeterInformation?.Any(m => m.SmartType == SmartMeterType.Smets1) ?? false);
        }

        public static bool IsSmartMeterSmets2(this EnergyCustomer customer)
        {
            return customer.IsCAndCJourney() && (customer.MeterDetail?.MeterInformation?.Any(m => m.SmartType == SmartMeterType.Smets2) ?? false);
        }

        public static bool IsSmartMeter(this EnergyCustomer customer)
        {
            return customer.IsSmartMeterSmets1() || customer.IsSmartMeterSmets2();
        }

        public static bool IsMeterDetailsPayGo(this EnergyCustomer customer)
        {
            return customer.IsCAndCJourney() && (customer.MeterDetail?.MeterInformation?.Any(m => m.IsPrePay) ?? false);
        }

        public static bool AskSmartMeterFrequency(this EnergyCustomer customer)
        {
            return customer.IsSseInstaller() && customer.IsSmartMeterSmets1() || customer.IsSmartMeterSmets2();
        }

        public static bool SelectedTariffHasExtras(this EnergyCustomer customer) => customer.SelectedTariff.Extras?.Count > 0;

        public static bool HasExtrasSelected(this EnergyCustomer customer)
        {
            return customer.SelectedExtras.Count > 0 && customer.SelectedTariff.IsBundle;
        }

        public static bool IsElectricalWiringSelected(this EnergyCustomer customer) => customer.SelectedExtras.Any(e => e.Type == ExtraType.ElectricalWiring);

        public static double GetTotalSelectedExtrasCost(this EnergyCustomer customer)
        {
            return customer.SelectedExtras.Aggregate<Extra, double>(0, (current, extra) => current + extra.BundlePrice);
        }

        public static bool HasSelectedBroadbandProduct(this EnergyCustomer customer) => customer.SelectedBroadbandProduct != null;

        public static string GetSmartMeterType(this EnergyCustomer customer)
        {
            string result = string.Empty;
            if (customer != null)
            {
                result = customer.IsSmartMeterSmets1() ? SmartMeterType.Smets1.GetDescription() : customer.IsSmartMeterSmets2() ? SmartMeterType.Smets2.GetDescription() : string.Empty;
            }

            return result;
        }
    }
}
