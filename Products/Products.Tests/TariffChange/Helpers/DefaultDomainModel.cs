namespace Products.Tests.TariffChange.Helpers
{
    using System;
    using System.Collections.Generic;
    using Model.Enums;
    using Model.TariffChange.Customers;
    using Model.TariffChange.Tariffs;

    public class DefaultDomainModel
    {
        public static Customer CustomerForTariffs()
        {
            return new Customer
            {
                FalloutReasons = new List<FalloutReasonResult>()
            };
        }

        public static Customer CustomerForSummary()
        {
            return new Customer
            {
                EmailAddress = "",
                CustomerSelectedTariff = new SelectedTariff
                {
                    Name = "Abc Tariff",
                    EffectiveDate = DateTime.Today,
                    ProjectedAnnualCost = "12",
                    ProjectedMonthlyCost = "1",
                    ProjectedAnnualCostValue = 12,
                    ProjectedMonthlyCostValue = "1",
                    ExitFeePerFuel = "40",
                    ExitFee = 40
                }
            };
        }

        public static CustomerAccount ForEligibility()
        {
            return new CustomerAccount
            {
                SiteDetails = new SiteDetails
                {
                    AccountNumber = "1111111113",
                    HasSingleActiveEnergyServiceAccount = true
                },
                CurrentTariff = new CurrentTariffForFuel
                {
                    Name = "Default Domain Model Tariff Name",
                    BrandCode = "Default Domain Model BrandCode",
                    FuelType = FuelType.Electricity
                }
            };
        }

        public static CustomerAccount ForTariffs()
        {
            return new CustomerAccount
            {
                SiteDetails = new SiteDetails
                {
                    AccountNumber = "1111111113",
                    MeterRegisterCount = 1
                },
                PaymentDetails = new PaymentDetails
                {
                    HasDirectDebitDiscount = true,
                    IsDirectDebit = true,
                    IsPaperless = true
                },
                CurrentTariff = new CurrentTariffForFuel
                {
                    AnnualUsageKwh = 1000,
                    BrandCode = "Default Domain Model BrandCode",
                    EndDate = DateTime.Today.AddDays(1),
                    FuelType = FuelType.Electricity,
                    Name = "Default Domain Model Tariff Name",
                    PeakPercentageOperand = 0.45,
                    ServicePlanId = "ABC123"
                }
            };
        }

        public static CustomerAccount ForWHDAndName(bool hasWHDFlag, string name, string tariffType)
        {
            return new CustomerAccount
            {
                SiteDetails = new SiteDetails
                {
                    AccountNumber = "1111111113",
                    MeterRegisterCount = 1
                },
                PaymentDetails = new PaymentDetails
                {
                    HasDirectDebitDiscount = true,
                    IsDirectDebit = true,
                    IsPaperless = true
                },
                IsWHD = hasWHDFlag,
                CurrentTariff = new CurrentTariffForFuel
                {
                    AnnualUsageKwh = 1000,
                    BrandCode = "Default Domain Model BrandCode",
                    EndDate = DateTime.Today.AddDays(1),
                    FuelType = FuelType.Electricity,
                    Name = name,
                    PeakPercentageOperand = 0.45,
                    ServicePlanId = "ABC123",
                    TariffType = tariffType
                }
            };
        }

        public static CustomerAccount ForSummary()
        {
            return new CustomerAccount
            {
                SiteDetails = new SiteDetails
                {
                    AccountNumber = "1111111113",
                    SiteId = 12345
                },
                PaymentDetails = new PaymentDetails
                {
                    BankAccountName = "Default Domain Model Bank Account Name",
                    BankAccountNumber = "12345678",
                    BankSortCode = "098765",
                    DirectDebitDay = "23",
                    IsDirectDebit = true,
                    IsPaperless = true,
                    IsMonthlyDirectDebit = true
                },
                CurrentTariff = new CurrentTariffForFuel
                {
                    FuelType = FuelType.Electricity,
                    AnnualUsageKwh = 1000
                },
                SelectedTariff = new SelectedTariffForFuel
                {
                    ServicePlanId = "ABCDE",
                    AnnualCostValue = 400,
                    EffectiveDate = DateTime.Today
                }
            };
        }
    }
}