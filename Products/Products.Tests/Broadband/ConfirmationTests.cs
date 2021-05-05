namespace Products.Tests.Broadband
{
    using System.Web.Mvc;
    using Helpers;
    using NUnit.Framework;
    using Products.Tests.Common.Fakes;
    using Products.Tests.Common.Helpers;
    using Products.Web.Areas.Broadband.Controllers;
    using Products.WebModel.ViewModels.Broadband;
    using Should;
    using FakeSessionManager = Fakes.Services.FakeSessionManager;

    [TestFixture]
    public class ConfirmationTests
    {
        [Test]
        public void ShouldReturnProductNameInTheViewModelWhenJourneyIsComplete()
        {
            //Arrange
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest())
                .Build();

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithContextManager(fakeContextManager);

            var lineCheckerController = controllerFactory.Build<LineCheckerController>();

            var packageController = controllerFactory.Build<PackagesController>();

            var confirmationController = controllerFactory.Build<ConfirmationController>();

            //Act
            // ReSharper disable once UnusedVariable
            ActionResult lineCheckerActionResult = lineCheckerController.LineChecker("BB_ANY19").Result;
            // ReSharper disable once UnusedVariable
            ActionResult lineCheckerSubmitActionResult = lineCheckerController.Submit(new LineCheckerViewModel { PhoneNumber = "0208 999 8888", PostCode = "PO9 1QH" }).Result;
            // ReSharper disable once UnusedVariable
            ActionResult selectAddressActionResult = lineCheckerController.SelectAddress().Result;
            // ReSharper disable once UnusedVariable
            JsonResult result4 = lineCheckerController.SelectAddress(new SelectAddressViewModel { SelectedAddressId = 1 }).Result;
            packageController.SelectedPackage(new SelectedPackageViewModel(), "FIBRE_ANY19");
            ActionResult result = confirmationController.Confirmation();

            //Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.ViewName.ShouldEqual("~/Areas/Broadband/Views/Confirmation/Confirmation.cshtml");
            var viewModel = viewResult.Model.ShouldBeType<ConfirmationViewModel>();
            viewModel.ProductName.ShouldEqual("Unlimited Fibre");
        }

        [Test]
        public void ShouldClearSessionWhenJourneyIsComplete()
        {
            //Arrange
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest())
                .Build();

            var fakeSessionManager = new FakeSessionManager();

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager);

            controllerFactory.Build<ExistingCustomerController>();

            var lineCheckerController = controllerFactory.Build<LineCheckerController>();

            var packageController = controllerFactory.Build<PackagesController>();

            var confirmationController = controllerFactory.Build<ConfirmationController>();

            //Act
            // ReSharper disable once UnusedVariable
            ActionResult result1 = lineCheckerController.LineChecker("BB_ANY19").Result;

            // ReSharper disable once UnusedVariable
            ActionResult result2 = lineCheckerController.Submit(new LineCheckerViewModel { PhoneNumber = "0208 999 8888", PostCode = "PO9 1QH" }).Result;
            // ReSharper disable once UnusedVariable
            ActionResult result3 = lineCheckerController.SelectAddress().Result;
            // ReSharper disable once UnusedVariable
            ActionResult result4 = lineCheckerController.SelectAddress(new SelectAddressViewModel { SelectedAddressId = 1 }).Result;
            packageController.SelectedPackage(new SelectedPackageViewModel(), "FIBRE_LRO19");
            ActionResult result = confirmationController.Confirmation();

            //Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.ViewName.ShouldEqual("~/Areas/Broadband/Views/Confirmation/Confirmation.cshtml");
            fakeSessionManager.SessionObject.ShouldBeNull();
        }
    }
}
