namespace Products.Tests.HomeServices
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Helpers;
    using Model.Constants;
    using NUnit.Framework;
    using Products.Model.HomeServices;
    using Products.Tests.Common.Fakes;
    using Products.Web.Areas.HomeServices.Controllers;
    using Products.WebModel.ViewModels.HomeServices;
    using Should;

    public class ConfirmationTests
    {
        [Test]
        public void ConfirmationViewModelShouldBePopulatedWithCorrectValues()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer
            {
                AvailableProduct = FakeHomeServicesProductStub.GetFakeProducts("BOBC"),
                SelectedExtraCodes = new List<string> { "EC" },
                SelectedProductCode = "BOBC"
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            // Act
            ActionResult result = controller.Confirmation();

            // Assert
            var confirmationViewResult = result.ShouldBeType<ViewResult>();
            var confirmationViewModel = confirmationViewResult.Model.ShouldBeType<ConfirmationViewModel>();

            confirmationViewModel.ProductName.ShouldEqual("Boiler cover");
            confirmationViewModel.ProductExtras.FirstOrDefault().ShouldEqual("Extra1");
        }

        [Test]
        public void ShouldPopulateDataLayerOnConfirmationPage()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer
            {
                AvailableProduct = FakeHomeServicesProductStub.GetFakeProducts("BOBC"),
                SelectedExtraCodes = new List<string> { "EC" },
                SelectedProductCode = "BOBC"
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            // Act
            ActionResult result = controller.Confirmation();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<ConfirmationViewModel>();
            viewModel.DataLayer.ShouldNotBeNull();
            viewModel.DataLayer.ShouldNotBeEmpty();
        }
    }
}
