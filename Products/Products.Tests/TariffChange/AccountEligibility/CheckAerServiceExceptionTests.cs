namespace Products.Tests.TariffChange.AccountEligibility
{
    using System;
    using Common.Fakes;
    using Common.Helpers;
    using Fakes.Services;
    using Helpers;
    using Model.TariffChange;
    using Model.TariffChange.Customers;
    using NUnit.Framework;
    using Should;
    using Web.Areas.TariffChange.Controllers;

    public class CheckAerServiceExceptionTests
    {
        [Test]
        public void ShouldThrowExceptionWithErrorWhenAnnualEnergyReviewServiceCheckAerMethodFails()
        {
            // Arrange
            var checkAerException = new Exception();
            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(checkAerException);
            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.SiteDetails.AccountNumber = "1111111113";
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                CustomerAccount = customerAccount
            });

            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultTariffChange();
            fakeConfigManager.AddConfiguration("RedirectionUrl", "url");

            var controller = new ControllerFactory()
                .WithConfigManager(fakeConfigManager)
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .Build<AccountEligibilityController>();

            // Act/Assert
            var ex = Assert.Throws<Exception>(() => controller.CheckEligibility());
            ex.Message.ShouldNotBeNull();
            ex.Message.ShouldEqual($"Exception occured, Account = {customerAccount.SiteDetails.AccountNumber}, checking account eligibility.");
        }
    }
}