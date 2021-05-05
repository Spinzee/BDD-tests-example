using NUnit.Framework;
using Products.Model.Common;
using Products.Model.Constants;
namespace Products.Tests.HomeServices
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Helpers;
    using Model.Enums;
    using Products.Model.HomeServices;
    using Products.Tests.Common.Fakes;
    using Products.Tests.Common.Fakes.Services;
    using Products.Tests.Common.Helpers;
    using Products.Web.Areas.HomeServices.Controllers;
    using Products.WebModel.ViewModels.HomeServices;
    using Should;

    public class AddressTests
    {
        [TestCase(false, AddressTypes.Cover, "ContactDetails")]
        [TestCase(true,AddressTypes.Cover, "LandlordBillingPostcode")]
        [TestCase(true, AddressTypes.Billing, "ContactDetails")]
        public async Task ShouldRedirectToContactDetailsWhenAddressIsSelected(bool isLandlord,AddressTypes addressType, string actionName)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer { IsLandlord= isLandlord } );

            var fakeServiceWrapper = new FakeQASServiceWrapper();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithQASServiceWrapper(fakeServiceWrapper)
                .Build<HomeServicesController>();

            var viewModel = new SelectAddressViewModel
            {
                SelectedAddressId = "123",
                AddressType = addressType
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = await controller.SelectAddress(viewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual(actionName);
        }

        [Test]
        public async Task ShouldRedirectToContactDetailsWhenAValidAddressIsEnteredManually()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer());

            var fakeServiceWrapper = new FakeQASServiceWrapper();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithQASServiceWrapper(fakeServiceWrapper)
                .Build<HomeServicesController>();

            var viewModel = new SelectAddressViewModel
            {
                IsManual = true,
                PropertyNumber = "123456789012345678901234567890",
                AddressLine1 = "12345678901234567890123456789012345678901234567890",
                Town = "abcdefghijklmnopqrstuvwxyzABCD"
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = await controller.SelectAddress(viewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("ContactDetails");
        }

        [Test]
        public async Task ShouldReturnToSelectAddressWhenAddressIsNotSelected()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer());

            var fakeServiceWrapper = new FakeQASServiceWrapper();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithQASServiceWrapper(fakeServiceWrapper)
                .Build<HomeServicesController>();

            var viewModel = new SelectAddressViewModel();

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = await controller.SelectAddress(viewModel);

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<SelectAddressViewModel>();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Keys.Count.ShouldEqual(1);
            controller.ModelState.Values.First().Errors.First().ErrorMessage.ShouldEqual("Please select your address.");
        }

        [TestCaseSource(nameof(GetInvalidAddresses))]
        public async Task ShouldReturnToEnterAddressManuallyWhenManuallyEnteredAddressIsNotValid(SelectAddressViewModel model, int errorCount, string description)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer());

            var fakeServiceWrapper = new FakeQASServiceWrapper();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithQASServiceWrapper(fakeServiceWrapper)
                .Build<HomeServicesController>();

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = await controller.SelectAddress(model);

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<SelectAddressViewModel>();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Keys.Count.ShouldEqual(errorCount);
        }

        [Test]
        public async Task ShouldSetManuallyEnteredCoverAddressInSession()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            var viewModel = new SelectAddressViewModel
            {
                IsManual = true,
                PropertyNumber = "123456789012345678901234567890",
                AddressLine1 = "12345678901234567890123456789012345678901234567890",
                Town = "abcdefghijklmnopqrstuvwxyzABCD",
                AddressType =  AddressTypes.Cover
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = await controller.SelectAddress(viewModel);

            // Assert
            result.ShouldBeType<RedirectToRouteResult>();
            var customer = fakeSessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            customer.SelectedCoverAddress.HouseName.ShouldEqual(viewModel.PropertyNumber);
            customer.SelectedCoverAddress.AddressLine1.ShouldEqual(viewModel.AddressLine1);
            customer.SelectedCoverAddress.Town.ShouldEqual(viewModel.Town);
            customer.SelectedBillingAddress.ShouldBeNull();
        }

        [Test]
        public async Task ShouldSetManuallyEnteredBillingAddressInSession()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            var viewModel = new SelectAddressViewModel
            {
                IsManual = true,
                PropertyNumber = "123456789012345678901234567890",
                AddressLine1 = "12345678901234567890123456789012345678901234567890",
                Town = "abcdefghijklmnopqrstuvwxyzABCD",
                AddressType = AddressTypes.Billing
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = await controller.SelectAddress(viewModel);

            // Assert
            result.ShouldBeType<RedirectToRouteResult>();
            var customer = fakeSessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            customer.SelectedBillingAddress.HouseName.ShouldEqual(viewModel.PropertyNumber);
            customer.SelectedBillingAddress.AddressLine1.ShouldEqual(viewModel.AddressLine1);
            customer.SelectedBillingAddress.Town.ShouldEqual(viewModel.Town);
            customer.BillingPostcode.ShouldEqual(viewModel.Postcode);
            customer.SelectedCoverAddress.ShouldBeNull();
        }

        [Test]
        public async Task ShouldSetSelectedCoverQASAddressInSession()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            var viewModel = new SelectAddressViewModel
            {
                SelectedAddressId = "123",
                AddressType  = AddressTypes.Cover
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = await controller.SelectAddress(viewModel);

            // Assert
            result.ShouldBeType<RedirectToRouteResult>();
            var customer = fakeSessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            customer.SelectedCoverAddress.HouseName.ShouldEqual("1");
            customer.SelectedCoverAddress.AddressLine1.ShouldEqual("Waterloo Road");
            customer.SelectedCoverAddress.Town.ShouldEqual("Havant");
            customer.SelectedBillingAddress.ShouldBeNull();
        }

        [Test]
        public async Task ShouldSetSelectedBillingQASAddressInSession()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            var viewModel = new SelectAddressViewModel
            {
                SelectedAddressId = "123",
                AddressType = AddressTypes.Billing
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = await controller.SelectAddress(viewModel);

            // Assert
            result.ShouldBeType<RedirectToRouteResult>();
            var customer = fakeSessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            customer.SelectedBillingAddress.HouseName.ShouldEqual("1");
            customer.SelectedBillingAddress.AddressLine1.ShouldEqual("Waterloo Road");
            customer.SelectedBillingAddress.Town.ShouldEqual("Havant");
            customer.SelectedCoverAddress.ShouldBeNull();
        }

       
        [Test]
        public async Task ShouldRedirectToFalloutWhenQASServiceDoesNotReturnAddress()
        {
            // Arrange
            var fakeServiceWrapper = new FakeQASServiceWrapper { ReturnAddressList = false };
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithQASServiceWrapper(fakeServiceWrapper)
                .Build<HomeServicesController>();

            // Act
            ActionResult result = await controller.SelectAddress();

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("UnableToComplete");
        }

        [Test]
        public void ShouldDisplayManualAddressPagetWhenQASEnabledIsFalse()
        {
            // Arrange
            var fakeServiceWrapper = new FakeQASServiceWrapper
            {
                ReturnAddressList = false
            };
            var fakeSessionManager = new FakeSessionManager();
            var fakeconfigManager = new FakeConfigManager();
            fakeconfigManager.AddConfiguration("QASEnabled", "false");
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithQASServiceWrapper(fakeServiceWrapper)
                .WithConfigManager(fakeconfigManager)
                .Build<HomeServicesController>();

            // Act
            ActionResult result = controller.SelectAddress().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();

            var viewmodel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();
            viewmodel.IsManual.ShouldBeTrue();
        }

        [Test]
        public async Task ShouldRedirectToFalloutWhenQASServiceDoesNotReturnAddressForSelectedAddress()
        {
            // Arrange
            var fakeServiceWrapper = new FakeQASServiceWrapper { ReturnAddressByMoniker = false };
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithQASServiceWrapper(fakeServiceWrapper)
                .Build<HomeServicesController>();

            var viewModel = new SelectAddressViewModel
            {
                SelectedAddressId = "123"
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = await controller.SelectAddress(viewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("UnableToComplete");
        }

       
        [Test]
        public async Task ShouldRedirectToFalloutWhenQASServiceThrowsExceptionWhileGettingListOfAddresses()
        {
            // Arrange
            var fakeServiceWrapper = new FakeQASServiceWrapper { ThrowException = true };
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithQASServiceWrapper(fakeServiceWrapper)
                .Build<HomeServicesController>();

            // Act
            ActionResult result = await controller.SelectAddress();

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("UnableToComplete");
        }

        [Test]
        public async Task ShouldRedirectToFalloutWhenQASServiceThrowsExceptionWhileGettingAddressByMoniker()
        {
            // Arrange
            var fakeServiceWrapper = new FakeQASServiceWrapper { ThrowException = true };
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithQASServiceWrapper(fakeServiceWrapper)
                .Build<HomeServicesController>();

            var viewModel = new SelectAddressViewModel
            {
                SelectedAddressId = "123"
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = await controller.SelectAddress(viewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("UnableToComplete");
        }

       
        [Test]
        public async Task ShouldPrePopulateSelectedCoverAddressWhenCustomerRevisitsThePage()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer
            {
                SelectedCoverAddress = new QasAddress  
                {
                    Moniker = "ABC"
                }
            });

            var fakeServiceWrapper = new FakeQASServiceWrapper();
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithQASServiceWrapper(fakeServiceWrapper)
                .Build<HomeServicesController>();
            
            // Act
            ActionResult result = await controller.SelectAddress();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();
            viewModel.IsManual.ShouldBeFalse();
            viewModel.SelectedAddressId.ShouldEqual("ABC");
        }

        [Test]
        public async Task ShouldPrePopulateManuallyEnteredCoverAddressWhenCustomerRevisitsThePage()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var coverAddress = new QasAddress
            {
                HouseName = "1",
                AddressLine1 = "Waterloo Road",
                Town = "Havant",
                AddressLine2 =   "Portsmouth",
                County = "Hampshire"
            };

            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer
            {
                SelectedCoverAddress = coverAddress
            });

            var fakeServiceWrapper = new FakeQASServiceWrapper();
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithQASServiceWrapper(fakeServiceWrapper)
                .Build<HomeServicesController>();

            // Act
            ActionResult result = await controller.SelectAddress();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();
            viewModel.IsManual.ShouldBeTrue();
            viewModel.PropertyNumber.ShouldEqual(coverAddress.HouseName);
            viewModel.AddressLine1.ShouldEqual(coverAddress.AddressLine1);
            viewModel.AddressLine2.ShouldEqual(coverAddress.AddressLine2);
            viewModel.Town.ShouldEqual(coverAddress.Town);
            viewModel.County.ShouldEqual(coverAddress.County);

        }

        
        [Test]
        public async Task ShouldPrePopulateSelectedBillingAddressWhenCustomerRevisitsThePage()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer
            {
                SelectedBillingAddress = new QasAddress
                {
                    Moniker = "ABC"
                },
                IsLandlord = true
            });

            var fakeServiceWrapper = new FakeQASServiceWrapper();
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithQASServiceWrapper(fakeServiceWrapper)
                .Build<HomeServicesController>();

            // Act
            ActionResult result = await controller.SelectBillingAddress();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();
            viewModel.IsManual.ShouldBeFalse();
            viewModel.SelectedAddressId.ShouldEqual("ABC");
        }

        [Test]
        public async Task ShouldPrePopulateManuallyEnteredBillingAddressWhenCustomerRevisitsThePage()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var coverAddress = new QasAddress
            {
                HouseName = "1",
                AddressLine1 = "Waterloo Road",
                Town = "Havant",
                AddressLine2 = "Portsmouth",
                County = "Hampshire"
            };

            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer
            {
                SelectedBillingAddress = coverAddress,
                IsLandlord = true
            });

            var fakeServiceWrapper = new FakeQASServiceWrapper();
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithQASServiceWrapper(fakeServiceWrapper)
                .Build<HomeServicesController>();

            // Act
            ActionResult result = await controller.SelectBillingAddress();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();
            viewModel.IsManual.ShouldBeTrue();
            viewModel.PropertyNumber.ShouldEqual(coverAddress.HouseName);
            viewModel.AddressLine1.ShouldEqual(coverAddress.AddressLine1);
            viewModel.AddressLine2.ShouldEqual(coverAddress.AddressLine2);
            viewModel.Town.ShouldEqual(coverAddress.Town);
            viewModel.County.ShouldEqual(coverAddress.County);
        }

        private static IEnumerable<TestCaseData> GetInvalidAddresses()
        {
            yield return new TestCaseData(new SelectAddressViewModel
            {
                IsManual = true, AddressType  =  AddressTypes.Billing
            }, 3, "3 required errors for mandatory fields.");

            yield return new TestCaseData(new SelectAddressViewModel
            {
                AddressType = AddressTypes.Billing,
                IsManual = true,
                PropertyNumber = "1234567890123456789012345678901",
                AddressLine1 = "123456789012345678901234567890123456789012345678901",
                Town = "abcdefghijklmnopqrstuvwxyzABCDE"
            }, 3, "3 regex errors for mandatory fields.");

            yield return new TestCaseData(new SelectAddressViewModel
            {
                AddressType = AddressTypes.Cover,
                IsManual = true,
                PropertyNumber = "123456789012345678901234567890",
                AddressLine1 = "12345678901234567890123456789012345678901234567890",
                AddressLine2 = "123456789012345678901234567890123456789012345678901",
                Town = "abcdefghijklmnopqrstuvwxyzABCD",
                County = "1234567890123456789012345678901"
            }, 2, "2 regex errors for optional fields.");
        }
    }
}
