namespace Products.Tests.TariffChange.CreatePassword
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Common.Fakes;
    using Common.Helpers;
    using Fakes.Services;
    using Helpers;
    using Model.TariffChange;
    using Model.TariffChange.Customers;
    using NUnit.Framework;
    using Products.Model.TariffChange.Tariffs;
    using Should;
    using Web.Areas.TariffChange.Controllers;
    using WebModel.ViewModels.TariffChange;

    public class ActivationEmailTests
    {
        [Test]
        public async Task ShouldThrowExceptionOnEmailServiceFailure()
        {
            var customer = new Customer
            {
                EmailAddress = "a@a.com",
                CustomerSelectedTariff = new SelectedTariff { Name = "Free Lifetime Energy" },
                Password = "pa55w0rd"
            };
            CustomerAccount customerAccount = DefaultDomainModel.ForSummary();
            customer.AddCustomerAccount(customerAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer
            });
            var fakeEmailManager = new FakeEmailManager(new Exception());
            var fakeLogger = new FakeLogger();
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultTariffChange();
            fakeConfigManager.AddConfiguration("MySseBaseUrl", "aa");
            fakeConfigManager.AddConfiguration("EmailFromAddress", "aa");

            var controller = new ControllerFactory()
                .WithLogger(fakeLogger)
                .WithConfigManager(fakeConfigManager)
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithEmailManager(fakeEmailManager)
                .Build<SummaryController>();

            // Act
            ActionResult result = await controller.TariffSummary(new TariffSummaryViewModel());

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>()
                .RouteValues["action"].ShouldEqual("ShowConfirmation");

            fakeLogger.LoggedException.ShouldNotBeNull();
            fakeLogger.ErrorMessage.ShouldNotBeNull();
            fakeLogger.ErrorMessage.ShouldStartWith($"Exception occured attempting to send confirmation email for customer tariff change, Accounts = 1111111113. Exception of type 'System.Exception' was thrown.");
        }

        [Test]
        public async Task ShouldSendEmail()
        {            
            const string email = "hello@sse.com";
            var customer = new Customer
            {
                EmailAddress = email,
                CustomerSelectedTariff = new SelectedTariff { Name = "Free Lifetime Energy" },
                Password = "pa55w0rd"
            };
            CustomerAccount customerAccount = DefaultDomainModel.ForSummary();
            customer.AddCustomerAccount(customerAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer
            });
            var fakeEmailManager = new FakeEmailManager();
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultTariffChange();
            fakeConfigManager.AddConfiguration("MySseBaseUrl", "aa");
            fakeConfigManager.AddConfiguration("EmailFromAddress", "aa");

            var controller = new ControllerFactory()
                .WithConfigManager(fakeConfigManager)
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithEmailManager(fakeEmailManager)
                .Build<SummaryController>();
            
            // Act
            ActionResult result = await controller.TariffSummary(new TariffSummaryViewModel());

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>()
                .RouteValues["action"].ShouldEqual("ShowConfirmation");

            fakeEmailManager.To.ShouldContain(customer.EmailAddress);
        }
    }
}