namespace Products.Tests.TariffChange.Summary
{
    using System;
    using Fakes.Services;
    using Helpers;
    using Model.TariffChange;
    using Model.TariffChange.Customers;
    using Model.TariffChange.Tariffs;
    using NUnit.Framework;
    using Should;
    using Web.Areas.TariffChange.Controllers;
    using WebModel.ViewModels.TariffChange;

    public class PersonalProjectionServiceExceptionTests
    {
        [Test]
        public void AddPersonalProjectionShouldThrowArgumentExceptionWhenServiceFails()
        {
            // Arrange
            var customer = new Customer
            {
                CustomerSelectedTariff = new SelectedTariff()
            };
            CustomerAccount customerAccount = DefaultDomainModel.ForSummary();
            customer.AddCustomerAccount(customerAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer
            });

            var fakePersonalProjectionServiceWrapper = new FakePersonalProjectionServiceWrapper(new Exception());

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithPersonalProjectionServiceWrapper(fakePersonalProjectionServiceWrapper)
                .Build<SummaryController>();

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await controller.TariffSummary(new TariffSummaryViewModel()));
            ex.Message.ShouldNotBeNull();
            ex.Message.ShouldStartWith("Exception occured attempting to call personal projection service");
        }
    }
}