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

    public class AllowCollectionDayValidatorTests
    {
        [TestCase(null, true, "If Collection day is null, the customer is eligible")]
        [TestCase(20, true, "If Collection day less or equal to 28 the customer is eligible")]
        [TestCase(28, true, "If Collection day less or equal to 28 the customer is eligible")]
        [TestCase(29, false, "If Collection day more than 28 the customer is ineligible")]
        public void AccountIsInEligibleIfCollectionDayMoreThan28Days(int? collectionDay, bool isEligible, string description)
        {
            // Arrange
            var fakeAerData = new FakeAERData
            {
                CollectionDay = collectionDay.ToString()
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

            // Act/Assert
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