namespace Products.Tests.TariffChange.AccountEligibility
{
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

    public class LC31AAnnualConsumptionTests
    {
        [TestCase(consumptionDetailsTypeConsumptionRuleDescription.lc31a, false, 190, "If Annual Consumption Type is lc31a then account is not renewal")]
        [TestCase(consumptionDetailsTypeConsumptionRuleDescription.eac_or_aq, true, 25, "If Annual Consumption Type is not lc31a and recent SPC change/lapse account falls out to renewals")]
        public void StandardTariffWithoutLC31AAnnualConsumptionForecastRedirectsToRenewals(consumptionDetailsTypeConsumptionRuleDescription annualConsumptionTypeDescription, bool isRenewal, int tariffChangeDays, string description)
        {
            // Arrange
            var fakeAerData = new FakeAERData
            {
                ConsumptionRuleDescription = annualConsumptionTypeDescription,
                CustomerAccountVariables = { ["TariffChangeDays"] = tariffChangeDays.ToString() }
            };

            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(fakeAerData.CustomerAccountNumber, new[] { fakeAerData });

            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.SiteDetails.AccountNumber = fakeAerData.CustomerAccountNumber;
            customerAccount.CurrentTariff.Name = "Standard";
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

            TestHelper.GetResultUrlString(controller.CheckEligibility()).ShouldEqual(isRenewal ? "CustomerAccountIneligible" : "AvailableTariffs");
            if (isRenewal)
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.ShouldBeNull();
            }
            else
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.FalloutReasons.ShouldBeEmpty();
            }
        }
        [TestCase(consumptionDetailsTypeConsumptionRuleDescription.lc31a, true, "If Billed and Annual Consumption Type is lc31a then account is Eligible")]
        [TestCase(consumptionDetailsTypeConsumptionRuleDescription.eac_or_aq, false, "If Billed and Annual Consumption Type is not lc31a then account is Ineligible")]
        [TestCase(consumptionDetailsTypeConsumptionRuleDescription.previoussaforecast, true, "If Billed and Annual Consumption Type is previous forecast then account is Eligible")]
        public void BilledAccountIsEligibleIfLC31AAnnualConsumptionForecast(consumptionDetailsTypeConsumptionRuleDescription annualConsumptionTypeDescription, bool isEligible, string description)
        {
            // Arrange
            var fakeAerData = new FakeAERData
            {
                ConsumptionRuleDescription = annualConsumptionTypeDescription
            };

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
            TestHelper.GetResultUrlString(controller.CheckEligibility()).ShouldEqual(isEligible ? "AvailableTariffs" : "CustomerAccountIneligible");
            if (isEligible)
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