namespace Products.Tests.TariffChange.Summary
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Common.Fakes;
    using Fakes.Services;
    using Helpers;
    using Model.TariffChange;
    using Model.TariffChange.Customers;
    using Model.TariffChange.Tariffs;
    using NUnit.Framework;
    using Should;
    using Web.Areas.TariffChange.Controllers;
    using WebModel.ViewModels.TariffChange;

    public class ConfirmationEmailTests
    {
        [Test]
        public async Task ShouldThrowExceptionOnEmailServiceFailure()
        {
            // Arrange
            var fakeException = new Exception();

            var customer = new Customer
            {
                CustomerSelectedTariff = new SelectedTariff { Name = "Lifetime Guarantee" }
            };
            CustomerAccount customerAccount = DefaultDomainModel.ForSummary();
            customer.AddCustomerAccount(customerAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer
            });

            var fakeEmailManager = new FakeEmailManager(fakeException);
            var fakeLogger = new FakeLogger();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithLogger(fakeLogger)
                .WithEmailManager(fakeEmailManager)
                .Build<SummaryController>();

            // Act
            ActionResult result = await controller.TariffSummary(new TariffSummaryViewModel());

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>()
                .RouteValues["action"].ShouldEqual("ShowConfirmation");

            fakeLogger.LoggedException.ShouldNotBeNull();
            fakeLogger.LoggedException.ShouldEqual(fakeException);
            fakeLogger.ErrorMessage.ShouldNotBeNull();
            fakeLogger.ErrorMessage.ShouldStartWith("Exception occured attempting to send confirmation email");
        }

        [Test]
        public async Task ShouldSendEmail()
        {
            // Arrange
            const string email = "hello@sse.com";
            var customer = new Customer
            {
                EmailAddress = email,
                CustomerSelectedTariff = new SelectedTariff { Name = "Free Lifetime Energy" }
            };
            CustomerAccount customerAccount = DefaultDomainModel.ForSummary();
            customer.AddCustomerAccount(customerAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer
            });

            var fakeEmailManager = new FakeEmailManager();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithEmailManager(fakeEmailManager)
                .Build<SummaryController>();

            // Act
            ActionResult result =await controller.TariffSummary(new TariffSummaryViewModel());

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>().RouteValues["action"].ShouldEqual("ShowConfirmation");

            fakeEmailManager.To.ShouldContain(email);
        }
    }
}