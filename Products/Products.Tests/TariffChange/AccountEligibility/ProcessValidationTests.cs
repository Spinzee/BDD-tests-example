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
    using Should;
    using Web.Areas.TariffChange.Controllers;

    public class ProcessValidationTests
    {
        [TestCase("00", true, "No existing tariff change is eligible")]
        [TestCase("01", false, "Existing tariff change is ineligible")]
        public void IneligibleIfExistingTariffChangeInProgress(string tariffChangesInProgress, bool isValid, string description)
        {
            // Arrange
            // ReSharper disable once UseObjectOrCollectionInitializer
            var fakeAerData = new FakeAERData();
            fakeAerData.CustomerAccountVariables["AERPendingTriggers"] = tariffChangesInProgress;
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