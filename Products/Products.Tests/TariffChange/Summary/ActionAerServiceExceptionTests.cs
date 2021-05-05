namespace Products.Tests.TariffChange.Summary
{
    using System;
    using System.Threading.Tasks;
    using Fakes.Services;
    using Helpers;
    using Model.TariffChange;
    using Model.TariffChange.Customers;
    using NUnit.Framework;
    using Products.Tests.Common.Fakes;
    using Should;
    using Web.Areas.TariffChange.Controllers;
    using WebModel.ViewModels.TariffChange;

    public class ActionAERServiceExceptionTests
    {
        [Test]
        public void ShouldThrowExceptionWithErrorWhenAnnualEnergyReviewServiceActionAerMethodFails()
        {
            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(new Exception());
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = new Customer()
            });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .Build<SummaryController>();

            var ex = Assert.ThrowsAsync<Exception>(async () => await controller.TariffSummary(new TariffSummaryViewModel { IsTermsAndConditionsChecked = true }));
            ex.Message.ShouldNotBeNull();
            ex.Message.ShouldStartWith("Exception occured attempting to call actionAER method for customer tariff change, Accounts = ");
        }
    }
}