namespace Products.Model.TariffChange.Customers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Enums;
    using Model.Enums;
    using Tariffs;

    public class Customer
    {
        public CustomerAccount ElectricityAccount { get; set; }

        public CustomerAccount GasAccount { get; set; }

        public string EmailAddress { get; set; }

        public string HolderName { get; set; }

        public List<FalloutReasonResult> FalloutReasons { get; set; }

        public SelectedTariff CustomerSelectedTariff { get; set; }

        public SelectedTariff FollowOnTariff { get; set; }

        public TariffCalculationMethod TariffCalculationMethod { get; set; }

        public string CountOfTariffs { get; set; }

        public Customer Clone => (Customer)MemberwiseClone();

        public string Password { get; set; }
    }

    // Extension Methods for customer class
    public static class CustomerExtensions
    {
        public static bool HasElectricityAccount(this Customer customer) => customer.ElectricityAccount != null;

        public static bool HasGasAccount(this Customer customer) => customer.GasAccount != null;

        public static bool IsDualFuel(this Customer customer) => customer.HasElectricityAccount() && customer.HasGasAccount();

        public static List<CustomerAccount> IterableAccounts(this Customer customer) => new[] { customer.ElectricityAccount, customer.GasAccount }.Where(ca => ca != null).ToList();

        public static List<string> CustomerAccountNumbers(this Customer customer) => customer.IterableAccounts().Select(ca => ca.SiteDetails.AccountNumber).ToList();

        public static void AddCustomerAccount(this Customer customer, CustomerAccount customerAccount)
        {
            if (customerAccount.CurrentTariff.FuelType == FuelType.Electricity)
            {
                customer.ElectricityAccount = customerAccount;
            }
            else if (customerAccount.CurrentTariff.FuelType == FuelType.Gas)
            {
                customer.GasAccount = customerAccount;
            }
        }

        public static void SetSummaryDetails(this Customer customer)
        {
            if (customer.HasElectricityAccount() && customer.ElectricityAccount.PaymentDetails.IsDirectDebit)
            {
                customer.ElectricityAccount.PaymentDetails.DirectDebitAmount = Math.Ceiling(customer.ElectricityAccount.SelectedTariff.AnnualCostValue / 12);
            }
            if (customer.HasGasAccount() && customer.GasAccount.PaymentDetails.IsDirectDebit)
            {
                customer.GasAccount.PaymentDetails.DirectDebitAmount = Math.Ceiling(customer.GasAccount.SelectedTariff.AnnualCostValue / 12);
            }
        }

        public static FuelType GetCustomerFuelType(this Customer customer) => customer.IsDualFuel() ? FuelType.Dual : customer.IterableAccounts()[0].CurrentTariff.FuelType;

        public static ConfirmationEmailParameters GetConfirmationEmailParameters(this Customer customer) =>
            new ConfirmationEmailParameters
            {
                AccountHolderName = customer.HolderName,
                TariffName = customer.CustomerSelectedTariff.DisplayName,
                EmailAddress = customer.EmailAddress,
                EffectiveDate = customer.CustomerSelectedTariff.EffectiveDate,
                IncludeBroadbandRedeemContent = customer.CustomerSelectedTariff.TariffGroup == TariffGroup.FixAndFibre
            };

        public static PersonalProjectionDetails GetPersonalProjectionDetails(this Customer customer) =>
            new PersonalProjectionDetails
            {
                SiteId = customer.IterableAccounts().Count > 0 ? customer.IterableAccounts()[0].SiteDetails.SiteId : 0,
                GasUsage = customer.HasGasAccount() ? customer.GasAccount.CurrentTariff.AnnualUsageKwh : 0,
                ElectricityUsage = customer.HasElectricityAccount() ? customer.ElectricityAccount.CurrentTariff.AnnualUsageKwh : 0,
                GasSpend = customer.HasGasAccount() ? customer.GasAccount.SelectedTariff.AnnualCostValue : 0,
                ElectricitySpend = customer.HasElectricityAccount() ? customer.ElectricityAccount.SelectedTariff.AnnualCostValue : 0
            };

        public static bool IsCustomerInRenewalPeriod(this Customer customer)
        {
            int? daysRemaining = customer.NumberOfDaysToTariffExpire();
            return (daysRemaining.HasValue && daysRemaining >= 0 && daysRemaining <= 56);
        }

        public static int? NumberOfDaysToTariffExpire(this Customer customer)
        {
            CurrentTariffForFuel currentTariff = customer.GetCurrentTariff();

            int? calcDate = null;

            if (currentTariff != null)
            {
                calcDate = currentTariff.EndDate == DateTime.MinValue || currentTariff.EndDate == DateTime.MaxValue ? (int?)null : (currentTariff.EndDate - DateTime.Today).Days;
            }

            return calcDate;
        }

        public static CurrentTariffForFuel GetCurrentTariff(this Customer customer)
        {
            return customer.ElectricityAccount?.CurrentTariff ?? customer.GasAccount?.CurrentTariff;
        }
    }
}