namespace Products.Tests.HomeServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Fakes;
    using Helpers;
    using Model;
    using Model.Constants;
    using Model.Enums;
    using NUnit.Framework;
    using Products.Model.HomeServices;
    using Products.Tests.Common.Fakes;
    using Products.Tests.Common.Helpers;
    using Products.Web.Areas.HomeServices.Controllers;
    using Products.WebModel.ViewModels.HomeServices;
    using Should;

    public class PostcodeTests
    {
        [Test]
        public async Task ShouldPopulatePostcodeViewModelWithCorrectValuesForResidential()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            // Act
            ActionResult actionResult = await controller.Postcode("BC");
            var viewResult = actionResult.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<PostcodeViewModel>();
            viewModel.Postcode = "PO91QH";
            controller.ValidateViewModel(viewModel);
            await controller.Postcode(viewModel);

            // Assert
            viewModel.CustomerType.ShouldEqual(HomeServicesCustomerType.Residential);
            viewModel.HeaderText.ShouldNotBeEmpty();
            viewModel.ParagraphText.ShouldNotBeEmpty();
            fakeSessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer).IsLandlord.ShouldBeFalse();
        }

        [Test]
        public async Task ShouldPopulatePostcodeViewModelWithCorrectValuesForLandlord()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            // Act
            ActionResult actionResult = await controller.LandlordPostcode("BC");
            var viewResult = actionResult.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<PostcodeViewModel>();
            viewModel.Postcode = "PO91QH";
            controller.ValidateViewModel(viewModel);
            await controller.Postcode(viewModel);

            // Assert
            viewModel.CustomerType.ShouldEqual(HomeServicesCustomerType.Landlord);
            viewModel.HeaderText.ShouldNotBeEmpty();
            viewModel.ParagraphText.ShouldNotBeEmpty();
            fakeSessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer).IsLandlord.ShouldBeTrue();
        }


        [Test]
        [TestCase("DN55 1PT")]
        [TestCase("  DN55 1PT  ")]
        [TestCase(" DN551PT ")]
        public async Task ShouldRedirectToCoverDetailsPageWhenAValidPostcodeIsEntered(string postcode)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            var postcodeViewModel = new PostcodeViewModel { Postcode = postcode, AddressTypes = AddressTypes.Cover };

            // Act
            controller.ValidateViewModel(postcodeViewModel);
            ActionResult result = await controller.Postcode(postcodeViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("CoverDetails");
        }

        [Test]
        [TestCase("", "Please enter your postcode.")]
        [TestCase("PO", "Sorry, we don’t recognise that postcode. Please try entering it again.")]
        public async Task ShouldReturnToEnterPostcodeWhenPostcodeIsMissingOrInvalid(string postcode, string errorMessage)
        {
            // Arrange
            var controller = new ControllerFactory()
                .Build<HomeServicesController>();

            var postcodeViewModel = new PostcodeViewModel { Postcode = postcode };

            // Act
            controller.ValidateViewModel(postcodeViewModel);
            ActionResult result = await controller.Postcode(postcodeViewModel);

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<PostcodeViewModel>();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Keys.Count.ShouldEqual(1);
            controller.ModelState.Values.First().Errors.First().ErrorMessage.ShouldEqual(errorMessage);
        }

        [Test]
        public async Task ShouldStorePostcodeInTheSessionWhenCustomerEnteredAValidPostcode()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer ());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            const string postcode = "PO9 1BH";

            var postcodeViewModel = new PostcodeViewModel { Postcode = postcode, AddressTypes = AddressTypes.Cover };

            // Act
            controller.ValidateViewModel(postcodeViewModel);
            // ReSharper disable once UnusedVariable
            ActionResult result =  await controller.Postcode(postcodeViewModel);

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            customer.CoverPostcode.ShouldEqual(postcode);
        }

        [Test]
        public async Task ShouldRedirectToAreaNotCoveredWhenNorthernIrelandPostcodeSupplied()
        {
            // Arrange
            var controller = new ControllerFactory()
                .Build<HomeServicesController>();

            var viewModel = new PostcodeViewModel
            {
                Postcode = "BT1 4GT"
            };

            // Act
            ActionResult result =  await controller.Postcode(viewModel);

            // Assert            
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("AreaNotCovered");
        }

        [Test]
        public async Task ShouldRedirectToExcludedPostcodeWhenProductIsNotAvailableForPostcodeSupplied()
        {
            // Arrange
            var fakeProductServiceWrapper = new FakeHomeServicesProductServiceWrapper();

            var controller = new ControllerFactory()
                .WithHomeServicesProductServiceWrapper(fakeProductServiceWrapper)
                .Build<HomeServicesController>();

            var viewModel = new PostcodeViewModel
            {
                Postcode = "NG10 1BH",
                ProductCode = "InvalidProductCode",
                AddressTypes  = AddressTypes.Cover
            };

            // Act
            ActionResult result = await controller.Postcode(viewModel);

            // Assert            
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("ExcludedPostcode");
        }

        [Test]
        public async Task ShouldRedirectToGenericFalloutPageWhenHomeServicesProductApiThrowsException()
        {
            var fakeProductServiceWrapper = new FakeHomeServicesProductServiceWrapper { ServiceException = new Exception() };

            var controller = new ControllerFactory()
                .WithHomeServicesProductServiceWrapper(fakeProductServiceWrapper)
                .Build<HomeServicesController>();

            var viewModel = new PostcodeViewModel
            {
                Postcode = "NG10 1BH",
                ProductCode = "BC50"
            };

            // Act
            ActionResult result = await controller.Postcode(viewModel);

            // Assert            
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("UnableToComplete");
        }

        [Test]
        public async Task ShouldStoreProductCodeInTheSessionWhenCustomerEnteredAValidPostcodeAndProductCodePresentInQueryString()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            const string productCode = "BC50";

            var postcodeViewModel = new PostcodeViewModel { Postcode = "PO9 1BH", ProductCode = productCode, AddressTypes = AddressTypes.Cover };

            // Act
            controller.ValidateViewModel(postcodeViewModel);
            // ReSharper disable once UnusedVariable
            ActionResult result = await controller.Postcode(postcodeViewModel);

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            customer.SelectedProductCode.ShouldEqual(productCode);
        }

        [Test]
        public void ShouldPrePopulateBillingAddressPostcodeWhenCustomerRevisitsThePage()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();

            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer
            {
               BillingPostcode =  "TH15NHE",
               IsLandlord = true
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            // Act
            ActionResult result =  controller.LandlordBillingPostcode();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<PostcodeViewModel>();
            viewModel.Postcode.ShouldEqual("TH15NHE");

        }

        [Test]
        public async Task ShouldPrePopulateCoverAddressPostcodeWhenCustomerRevisitsThePage()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();

            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer
            {
                CoverPostcode = "TH15NHE"
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            // Act
            ActionResult result = await controller.Postcode("BC");

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<PostcodeViewModel>();
            viewModel.Postcode.ShouldEqual("TH15NHE");

        }

        [Test]
        public async Task ShouldShowHomeServicesNotAvailablePageWhenCustomerAlertIsActive()
        {

            // Arrange
            var fakeSessionManager = new FakeSessionManager();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithCustomerAlertRepository(new FakeCustomerAlertRepository(new List<CustomerAlertResult> { new CustomerAlertResult
                    {
                        StartTime = null,
                        EndTime = null
                    }
                }))
                .Build<HomeServicesController>();

            // Act
            ActionResult result = await controller.Postcode("BC50");

            // Assert
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult)result).RouteValues["action"].ShouldEqual("HomeServicesUnavailable");

        }
    }
}
