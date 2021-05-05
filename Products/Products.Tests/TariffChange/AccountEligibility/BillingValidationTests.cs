namespace Products.Tests.TariffChange.AccountEligibility
{
    using System.Collections.Generic;
    using Common.Fakes;
    using Common.Helpers;
    using Fakes.Models;
    using Fakes.Services;
    using Helpers;
    using Model.TariffChange;
    using Model.TariffChange.Customers;
    using NUnit.Framework;
    using ServiceWrapper.AnnualEnergyReviewService;
    using Should;
    using Web.Areas.TariffChange.Controllers;

    public class BillingValidationTests
    {
        [TestCase("N", true, "001", consumptionDetailsTypeConsumptionRuleDescription.lc31a, "Customer with no billing exception is eligible")]
        [TestCase("Y", false, "001", consumptionDetailsTypeConsumptionRuleDescription.eac_or_aq, "Existing Customer with billing exception is ineligible")]
        [TestCase("Y", false, "000", consumptionDetailsTypeConsumptionRuleDescription.lc31a, "Existing Customer with billing exception is ineligible")]
        [TestCase("Y", true, "000", consumptionDetailsTypeConsumptionRuleDescription.eac_or_aq, "New Customer with billing exception is eligible")]
        public void AccountIneligibleIfBillIsInException(string billInException, bool isValid, string lastBillSendDays, consumptionDetailsTypeConsumptionRuleDescription annualConsumptionTypeDescription, string description)
        {
            // Arrange
            // ReSharper disable once UseObjectOrCollectionInitializer
            var fakeAerData = new FakeAERData
            {
                ConsumptionRuleDescription = annualConsumptionTypeDescription
            };
            fakeAerData.CustomerAccountVariables["CAException"] = billInException;
            fakeAerData.CustomerAccountVariables["LastBillSendDays"] = lastBillSendDays;

            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(fakeAerData.CustomerAccountNumber, new[] { fakeAerData });

            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.SiteDetails.AccountNumber = fakeAerData.CustomerAccountNumber;
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                CustomerAccount = customerAccount
            });

            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AtlasAnnouncementDate", "30/04/2018 9:00");

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<AccountEligibilityController>();

            // Act + Assert
            TestHelper.GetResultUrlString(controller.CheckEligibility())
                .ShouldEqual(isValid ? "AvailableTariffs" : "CustomerAccountIneligible");
            if (isValid)
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.FalloutReasons.ShouldBeEmpty();
            }
            else
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.ShouldBeNull();
            }
        }

        private static IEnumerable<TestCaseData> LastBillDates()
        {
            yield return new TestCaseData(211, true, "When Last bill date is < 212 days in past account is eligible");
            yield return new TestCaseData(212, true, "When Last bill date is 212 days in past account is eligible");
            yield return new TestCaseData(213, false, "When Last bill date is > 212 days in past account is ineligible");
        }

        [Test, TestCaseSource(nameof(LastBillDates))]
        public void AccountIneligibleWhenLastBillDateGreaterThan212DaysInPast(int daysSinceLastBill, bool isValid, string description)
        {
            // Arrange
            // ReSharper disable once UseObjectOrCollectionInitializer
            var fakeAerData = new FakeAERData();
            fakeAerData.CustomerAccountVariables["LastBillSendDays"] = daysSinceLastBill.ToString();
            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(fakeAerData.CustomerAccountNumber, new[] { fakeAerData });

            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.SiteDetails.AccountNumber = fakeAerData.CustomerAccountNumber;
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                CustomerAccount = customerAccount
            });
            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AtlasAnnouncementDate", "30/04/2018 9:00");

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<AccountEligibilityController>();

            // Act + Assert
            TestHelper.GetResultUrlString(controller.CheckEligibility())
                .ShouldEqual(isValid ? "AvailableTariffs" : "CustomerAccountIneligible");
            if (isValid)
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.FalloutReasons.ShouldBeEmpty();
            }
            else
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.ShouldBeNull();
            }
        }
    }
}