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

    public class PredictAndFixValidationTests
    {
        [Test]
        [TestCase("1 year Predict And Fix v14")]
        [TestCase("1 year Predict & Fix v14")]
        [TestCase("2 year Predict And Fix v14")]
        [TestCase("2 year Predict & Fix v14")]
        [TestCase("Predict And Fix v14")]
        [TestCase("Predict & Fix v14")]
        [TestCase("Predict And Fix")]
        [TestCase("Predict & Fix")]
        public void ShouldRedirectToFalloutIfTariffNameIsPredictAndFix(string tariffName)
        {
            // Arrange
            var fakeAerData = new FakeAERData
            {
                CustomerAccountVariables = { ["RegisterCount"] = "1" }
            };

            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(fakeAerData.CustomerAccountNumber, new[] { fakeAerData });


            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.CurrentTariff.Name = tariffName;
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
            TestHelper.GetResultUrlString(controller.CheckEligibility()).ShouldEqual("CustomerAccountIneligible");
        }

        [Test]
        public void ShouldNotRedirectToFalloutIfTariffNameIsNotPredictAndFix()
        {
            // Arrange
            var fakeAerData = new FakeAERData
            {
                CustomerAccountVariables = { ["RegisterCount"] = "1" }
            };

            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(fakeAerData.CustomerAccountNumber, new[] { fakeAerData });


            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.CurrentTariff.Name = "1 Year Fix V15";
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
            TestHelper.GetResultUrlString(controller.CheckEligibility()).ShouldEqual("AvailableTariffs");
        }
    }
}