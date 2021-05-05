namespace Products.Tests.TariffChange.AccountEligibility
{
    using System;
    using System.Configuration;
    using Common.Helpers;
    using Fakes.Models;
    using Fakes.Services;
    using Helpers;
    using Model.TariffChange;
    using Model.TariffChange.Customers;
    using NUnit.Framework;
    using Should;
    using Web.Areas.TariffChange.Controllers;

    public class RenewalTests
    {
        [TestCase(59, true, "If tariff end date is 59 days in future, customer is a renewal")]
        [TestCase(60, true, "If tariff end date is 60 days in future, customer is a renewal")]
        [TestCase(61, false, "If tariff end date is 61 days in future, customer is not a renewal")]
        public void RenewalCustomerShouldNotProceedPastEligibilityChecks(int daysToTariffEndDate, bool isRenewal,
            string description)
        {
            var fakeAerData = new FakeAERData
            {
                EndDate = DateTime.Today.AddDays(daysToTariffEndDate)
            };
            var fakeAnnualEnergyReviewServiceWrapper =
                new FakeAnnualEnergyReviewServiceWrapper(fakeAerData.CustomerAccountNumber, new[] { fakeAerData });

            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.SiteDetails.AccountNumber = fakeAerData.CustomerAccountNumber;
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                CustomerAccount = customerAccount
            });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .Build<AccountEligibilityController>();

            string allowRenewals = ConfigurationManager.AppSettings["AllowRenewals"];

            if (!string.IsNullOrEmpty(allowRenewals) && allowRenewals == "false")
            {
                TestHelper.GetResultUrlString(controller.CheckEligibility())
                    .ShouldEqual(isRenewal ? "FalloutRenewal" : "AvailableTariffs");
            }
        }

        [Test]
        public void RenewalCustomerFallOutPageShouldClearSessionData()
        {
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = new Customer()
            });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<AccountEligibilityController>();

            controller.FalloutRenewal();
            fakeInMemoryTariffChangeSessionService.GetJourneyDetails().ShouldBeNull();
        }
    }
}