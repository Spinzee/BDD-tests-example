namespace Products.Tests.Broadband
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Common.Fakes;
    using Common.Helpers;
    using Fakes.Services;
    using Helpers;
    using Model;
    using NUnit.Framework;
    using Products.Model;
    using Products.Model.Broadband;
    using Products.Model.Common;
    using Should;
    using Web.Areas.Broadband.Controllers;
    using WebModel.ViewModels.Broadband;
    using FakeSessionManager = Fakes.Services.FakeSessionManager;

    [TestFixture]
    public class LineCheckerControllerTests
    {
        [TestCase("123456")]
        [TestCase("ABCDEF")]
        [TestCase("2!!&*(&(")]
        [TestCase("PO9%!")]
        public async Task WhenAnInvalidPostcodeIsSuppliedThenRemainOnLineCheckerPage(string postCode)
        {
            // Arrange
            var controller = new ControllerFactory().Build<LineCheckerController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = postCode,
                PhoneNumber = "0123456789"
            };

            // Act
            controller.ValidateViewModel(lineCheckerViewModel);
            ActionResult result = await controller.Submit(lineCheckerViewModel);
            //Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.ViewName.ShouldEqual("~/Areas/Broadband/Views/LineChecker/LineChecker.cshtml");
        }

        [TestCase("", "", "1", "Test Road", "1 Test Road", "If no premise or sub premise name, then display thoroughfare number and road name")]
        [TestCase("Flat 1", "", "1", "Test Road", "Flat 1, 1 Test Road", "If no premise name, then display sub premise then display thoroughfare number and road name")]
        [TestCase("", "Test House", "1", "Test Road", "Test House, 1 Test Road", "If no sub premise name, display premise name and road name")]
        [TestCase("Flat 1", "Test House", "1", "Test Road", "Flat 1, Test House, 1 Test Road", "Display sub and premise name, thoroughfare number and name")]
        [TestCase("Flat 1", "", "", "", "Flat 1", "If only sub premise name then display in drop down")]
        [TestCase("", "Test House", "", "", "Test House", "If only premise name then display in drop down")]
        [TestCase("", "", "1", "", "1", "If only thoroughfare number then display in dropdown")]
        [TestCase("", "", "", "Test Road", "Test Road", "If only thoroughfare name then display in dropdown")]
        public async Task ShouldDisplayAddressWithCorrectlyFormattedAddressLine1(string subPremises, string premiseName, string thoroughfareNumber, string thoroughfareName, string expectedFormattedLine, string description)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper();
            fakeBroadbandProductsServiceWrapper.SetAddressResult(subPremises, premiseName, thoroughfareNumber, thoroughfareName);
            var controller = new ControllerFactory().WithSessionManager(fakeSessionManager)
                .WithBroadbandProductsServiceWrapper(fakeBroadbandProductsServiceWrapper)
                .Build<LineCheckerController>();

            // Act
            ActionResult result = await controller.SelectAddress();
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();

            // Assert
            // ReSharper disable once PossibleNullReferenceException
            viewModel.Addresses.FirstOrDefault().FormattedAddressLine1.ShouldContain(expectedFormattedLine);
        }

        [TestCase(null)]
        public async Task BackChevronViewModelRouteValuesShouldBeNullWhenNoProductCode(string productCode)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { SelectedProductCode = productCode } }
            };

            var controller = new ControllerFactory()
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses })
                .WithSessionManager(fakeSessionManager)
                .Build<LineCheckerController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO91BH",
                PhoneNumber = null
            };

            // Act
            await controller.Submit(lineCheckerViewModel);
            ActionResult resultSelectAddressGet = await controller.SelectAddress();
            var viewResult = resultSelectAddressGet.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();

            //Assert
            viewModel.BackChevronViewModel.RouteValues.ShouldBeNull();
        }

        [TestCase("FIBRE_EAW18")]
        [TestCase("FF3_EAW18")]
        public async Task BackChevronViewModelRouteValuesShouldBePopulatedWithProductCode(string productCode)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { SelectedProductCode = productCode } }
            };

            var controller = new ControllerFactory()
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses })
                .WithSessionManager(fakeSessionManager)
                .Build<LineCheckerController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO91BH",
                PhoneNumber = null,
                ProductCode = productCode
            };

            // Act
            await controller.Submit(lineCheckerViewModel);
            ActionResult resultSelectAddressGet = await controller.SelectAddress();
            var viewResult = resultSelectAddressGet.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();

            //Assert
            viewModel.BackChevronViewModel.RouteValues.ToString().ShouldContain(productCode);
        }

        [TestCase("FF3_ANY18", BroadbandProductGroup.FixAndFibreV3)]
        [TestCase("FF3_AP18", BroadbandProductGroup.FixAndFibreV3)]
        [TestCase("FF3_LR18", BroadbandProductGroup.FixAndFibreV3)]
        [TestCase("FF3_EAW18", BroadbandProductGroup.FixAndFibreV3)]
        [TestCase("FIBRE_ANY18", BroadbandProductGroup.None)]
        public async Task ShouldReturnProductNameInTheViewModelWhenJourneyIsComplete(string productCode, BroadbandProductGroup expectedBroadbandProductGroup)
        {
            // Arrange
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest())
                .Build();

            var fakeSessionManager = new FakeSessionManager();

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager);

            var viewModel = new LineCheckerViewModel
            {
                PostCode = "PO91BH",
                PhoneNumber = null,
                ProductCode = productCode
            };

            var lineCheckerController = controllerFactory.Build<LineCheckerController>();

            // Act
            await lineCheckerController.Submit(viewModel);
            var broadbandJourneyDetails = fakeSessionManager.GetSessionDetails<BroadbandJourneyDetails>("broadband_journey");
            broadbandJourneyDetails.Customer.SelectedProductGroup.ShouldEqual(expectedBroadbandProductGroup);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task ShouldSetInstallRequiredInSession(bool installRequired)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { InstallLine = installRequired })
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses })
                .Build<LineCheckerController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO91BH",
                PhoneNumber = "1234567"
            };

            // Act
            await controller.Submit(lineCheckerViewModel);
            await controller.SelectAddress();
            await controller.SelectAddress(new SelectAddressViewModel { SelectedAddressId = 1 });

            // Assert
            var broadbandJourneyDetails = fakeSessionManager.SessionObject.ShouldBeType<BroadbandJourneyDetails>();
            broadbandJourneyDetails.Customer.ApplyInstallationFee.ShouldEqual(installRequired);
        }

        [Test]
        public async Task ShouldShowBroadbandNotAvailablePageWhenBroadbandCustomerAlertIsActive()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithCustomerAlertRepository(new FakeCustomerAlertRepository(new List<CustomerAlertResult>
                {
                    new CustomerAlertResult
                    {
                        StartTime = null,
                        EndTime = null
                    }
                }))
                .Build<LineCheckerController>();

            // Act
            ActionResult resultExistingCustomer = await controller.LineChecker();

            // Assert
            resultExistingCustomer.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) resultExistingCustomer).RouteValues["action"].ShouldEqual("BroadbandUnavailable");
        }

        [TestCase("Test")]
        [TestCase("12345678901234567890")]
        [TestCase("07092022982")]
        [TestCase("12345")]
        [TestCase("03123456789")]
        [TestCase("04123456789")]
        [TestCase("05123456789")]
        [TestCase("06123456789")]
        [TestCase("07123456789")]
        [TestCase("08123456789")]
        [TestCase("09123456789")]
        [TestCase("11123456789")]
        [TestCase("+442392370922")]
        [TestCase("(023) 92 370922")]
        [TestCase("07975780980")]
        [TestCase("02        9")]
        [TestCase("02    11119")]
        [TestCase("02     1119")]
        public async Task WhenAnInvalidPhoneNumberIsSuppliedThenRemainOnLineCheckerPage(string phoneNumber)
        {
            // Arrange
            var controller = new ControllerFactory().Build<LineCheckerController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO9 1QH",
                PhoneNumber = phoneNumber
            };

            // Act
            controller.ValidateViewModel(lineCheckerViewModel);
            ActionResult result = await controller.Submit(lineCheckerViewModel);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<ViewResult>();
            ((ViewResult) result).ViewName.ShouldEqual("~/Areas/Broadband/Views/LineChecker/LineChecker.cshtml");
        }

        [TestCase("02392370922", "02392370922")]
        [TestCase("023 92 370922", "02392370922")]
        [TestCase("023 92370922", "02392370922")]
        [TestCase("023 92 370 922", "02392370922")]
        [TestCase("0208 1223 123", "02081223123")]
        [TestCase("0208 122 3123", "02081223123")]
        [TestCase("01256 123 123", "01256123123")]
        [TestCase("01256 123123", "01256123123")]
        public async Task WhenAValidPostcodeAndPhoneNumberAreSuppliedStoreThemInSessionAndCallSelectAddress(string phoneNumber, string formattedPhoneNumber)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<LineCheckerController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "P091BH",
                PhoneNumber = phoneNumber
            };

            // Act
            controller.ValidateViewModel(lineCheckerViewModel);
            ActionResult resultLineCheckerController = await controller.Submit(lineCheckerViewModel);

            //Assert
            resultLineCheckerController.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) resultLineCheckerController).RouteValues["action"].ShouldEqual("SelectAddress");
            var broadbandJourneyDetails = fakeSessionManager.GetSessionDetails<BroadbandJourneyDetails>("broadband_journey");
            broadbandJourneyDetails.Customer.PostcodeEntered.ShouldEqual(lineCheckerViewModel.PostCode);
            broadbandJourneyDetails.Customer.CliNumber.ShouldEqual(formattedPhoneNumber);
        }

        [Test]
        public async Task WhenBroadbandProductsServiceThroughExceptionShowFallout()
        {
            // Scenario: LineSuitable = false
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory()
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper { BroadbandProductsResult = BroadbandProductsResult.Exception })
                .WithSessionManager(fakeSessionManager)
                .Build<LineCheckerController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO91BH",
                PhoneNumber = null
            };

            // Act
            ActionResult resultLineCheckerController = await controller.Submit(lineCheckerViewModel);

            ActionResult resultSelectAddressGet = await controller.SelectAddress();
            var viewResult = resultSelectAddressGet.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();

            // ReSharper disable once PossibleNullReferenceException
            viewModel.SelectedAddressId = viewModel.Addresses.FirstOrDefault().Id;

            JsonResult resultSelectAddressPost = await controller.SelectAddress(viewModel);

            // Assert
            resultLineCheckerController.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) resultLineCheckerController).RouteValues["action"].ShouldEqual("SelectAddress");

            resultSelectAddressGet.ShouldNotBeNull();
            resultSelectAddressGet.ShouldBeType<ViewResult>();
            viewResult.ViewName.ShouldEqual("~/Areas/Broadband/Views/LineChecker/SelectAddress.cshtml");

            resultSelectAddressPost.ShouldNotBeNull();
            var actualObject = resultSelectAddressPost.ShouldBeType<JsonResult>();
            PropertyInfo property = actualObject.Data.GetType().GetProperty("Status");
            object propertyValue = property?.GetValue(actualObject.Data);
            propertyValue.ShouldEqual("CannotCompleteOnline");
        }

        [Test, Ignore("OPEN REACH HACK")]
        // ReSharper disable once IdentifierTypo
        public async Task WhennewBtLineAvailabilityServiceThrowsAnExceptionShouldShowFallout()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory()
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses })
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { NewBTLineAvailabilityServiceException = true })
                .WithSessionManager(fakeSessionManager)
                .Build<LineCheckerController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO91BH",
                PhoneNumber = null
            };

            // Act
            await controller.Submit(lineCheckerViewModel);

            ActionResult resultSelectAddressGet = await controller.SelectAddress();
            var viewResult = resultSelectAddressGet.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();

            // ReSharper disable once PossibleNullReferenceException
            viewModel.SelectedAddressId = viewModel.Addresses.FirstOrDefault().Id;
            JsonResult resultSelectAddressPost = await controller.SelectAddress(viewModel);

            // Assert            
            var actualObject = resultSelectAddressPost.ShouldBeType<JsonResult>();
            PropertyInfo property = actualObject.Data.GetType().GetProperty("Status");
            object propertyValue = property?.GetValue(actualObject.Data);
            propertyValue.ShouldEqual("CannotCompleteOnline");
        }

        [Test]
        public async Task WhenNoAddressIsSelectedThenRemainOnSelectAddressPage()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory()
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses })
                .WithSessionManager(fakeSessionManager)
                .Build<LineCheckerController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO91BH",
                PhoneNumber = null
            };

            // Act
            await controller.Submit(lineCheckerViewModel);
            ActionResult resultSelectAddressGet = await controller.SelectAddress();
            var viewResultAddressGet = resultSelectAddressGet.ShouldBeType<ViewResult>();
            var viewModel = viewResultAddressGet.Model.ShouldBeType<SelectAddressViewModel>();

            controller.ValidateViewModel(viewModel);
            JsonResult resultSelectAddressPost = await controller.SelectAddress(viewModel);

            // Assert
            resultSelectAddressPost.ShouldNotBeNull();
            var actualObject = resultSelectAddressPost.ShouldBeType<JsonResult>();
            PropertyInfo property = actualObject.Data.GetType().GetProperty("Status");
            object propertyValue = property?.GetValue(actualObject.Data);
            propertyValue.ShouldEqual("SelectAddress");
        }

        [Test]
        public async Task WhenNoBroadbandProductsAreReturnedShowFallout()
        {
            // Scenario: LineSuitable = false
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory()
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.NoThoroughfareOnly, BroadbandProductsResult = BroadbandProductsResult.LineUnsuitable })
                .WithSessionManager(fakeSessionManager)
                .Build<LineCheckerController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO91BH",
                PhoneNumber = null
            };

            // Act
            ActionResult resultLineCheckerController = await controller.Submit(lineCheckerViewModel);

            ActionResult resultSelectAddressGet = await controller.SelectAddress();

            var viewResult = resultSelectAddressGet.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();

            // ReSharper disable once PossibleNullReferenceException
            viewModel.SelectedAddressId = viewModel.Addresses.FirstOrDefault().Id;

            JsonResult resultSelectAddressPost = await controller.SelectAddress(viewModel);

            //Assert
            resultLineCheckerController.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) resultLineCheckerController).RouteValues["action"].ShouldEqual("SelectAddress");

            resultSelectAddressGet.ShouldNotBeNull();
            resultSelectAddressGet.ShouldBeType<ViewResult>();
            viewResult.ViewName.ShouldEqual("~/Areas/Broadband/Views/LineChecker/SelectAddress.cshtml");

            resultSelectAddressPost.ShouldNotBeNull();
            var actualObject = resultSelectAddressPost.ShouldBeType<JsonResult>();
            PropertyInfo property = actualObject.Data.GetType().GetProperty("Status");
            object propertyValue = property?.GetValue(actualObject.Data);
            propertyValue.ShouldEqual("CannotCompleteOnline");
        }

        [Test]
        public async Task WhenNorthernIrelandPostcodeSuppliedShowFallout()
        {
            // Arrange
            var controller = new ControllerFactory()
                .Build<LineCheckerController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "BT1 4GT",
                PhoneNumber = null
            };

            // Act
            ActionResult result = await controller.Submit(lineCheckerViewModel);

            // Assert            
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("AreaNotCovered");
        }

        [Test]
        public async Task WhenOpenReachFalloutFlagIsTrueThenShowFallout()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory()
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses })
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = true })
                .WithSessionManager(fakeSessionManager)
                .Build<LineCheckerController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO91BH",
                PhoneNumber = null
            };

            // Act
            ActionResult resultLineCheckerController = await controller.Submit(lineCheckerViewModel);

            ActionResult resultSelectAddressGet = await controller.SelectAddress();

            var viewResult = resultSelectAddressGet.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();
            // ReSharper disable once PossibleNullReferenceException
            viewModel.SelectedAddressId = viewModel.Addresses.FirstOrDefault().Id;
            JsonResult resultSelectAddressPost = await controller.SelectAddress(viewModel);

            // Assert
            resultLineCheckerController.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) resultLineCheckerController).RouteValues["action"].ShouldEqual("SelectAddress");

            resultSelectAddressGet.ShouldNotBeNull();
            resultSelectAddressGet.ShouldBeType<ViewResult>();
            viewResult.ViewName.ShouldEqual("~/Areas/Broadband/Views/LineChecker/SelectAddress.cshtml");

            resultSelectAddressPost.ShouldNotBeNull();
            var actualObject = resultSelectAddressPost.ShouldBeType<JsonResult>();
            PropertyInfo property = actualObject.Data.GetType().GetProperty("Status");
            object propertyValue = property?.GetValue(actualObject.Data);
            propertyValue.ShouldEqual("CannotCompleteOnline");
        }

        [TestCase("PO9 1BH")]
        [TestCase("PO91BH")]
        [TestCase("  PO91BH  ")]
        [TestCase("  PO9 1BH  ")]
        public async Task WhenPostcodeSuppliedIsFoundShowListOfAddresses(string postCode)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses })
                .Build<LineCheckerController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = postCode,
                PhoneNumber = null
            };

            List<BTAddress> listOfTestAddresses = FakeBroadbandProductsData.GetAddresses(AddressResult.AllAddresses);

            // Act
            ActionResult resultLineCheckerController = await controller.Submit(lineCheckerViewModel);
            ActionResult resultSelectAddress = await controller.SelectAddress();
            var viewResult = resultSelectAddress.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();

            // Assert
            resultLineCheckerController.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) resultLineCheckerController).RouteValues["action"].ShouldEqual("SelectAddress");
            resultSelectAddress.ShouldNotBeNull();
            resultSelectAddress.ShouldBeType<ViewResult>();
            viewResult.ViewName.ShouldEqual("~/Areas/Broadband/Views/LineChecker/SelectAddress.cshtml");
            viewModel.Addresses.Count.ShouldEqual(listOfTestAddresses.Count);
        }

        [TestCase(AddressResult.Exception, "CannotCompleteOnline")]
        [TestCase(AddressResult.NoAddressFound, "CannotCompleteOnline")]
        public async Task WhenPostcodeSuppliedIsNotFoundShowFallout(AddressResult addressResult, string expectedAction)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };

            var controller = new ControllerFactory()
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper { AddressResult = addressResult })
                .WithSessionManager(fakeSessionManager)
                .Build<LineCheckerController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO91BH",
                PhoneNumber = null
            };

            // Act
            // ReSharper disable once UnusedVariable
            ActionResult result = await controller.Submit(lineCheckerViewModel);
            ActionResult resultSelectAddress = await controller.SelectAddress();

            // Assert            
            resultSelectAddress.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) resultSelectAddress).RouteValues["action"].ShouldEqual(expectedAction);
        }

        [Test]
        public async Task WhenSelectedAddressHasThoroughfareNumberShowSelectedPackagesAndSelectedAddressIsStoredInSession()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses })
                .Build<LineCheckerController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO91BH",
                PhoneNumber = "1234567"
            };

            // Act
            ActionResult resultLineCheckerController = await controller.Submit(lineCheckerViewModel);
            ActionResult resultSelectAddressGet = await controller.SelectAddress();

            var viewResult = resultSelectAddressGet.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();

            // ReSharper disable once PossibleNullReferenceException
            viewModel.SelectedAddressId = viewModel.Addresses.FirstOrDefault().Id;
            JsonResult resultSelectAddressPost = await controller.SelectAddress(viewModel);

            //Assert
            resultLineCheckerController.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) resultLineCheckerController).RouteValues["action"].ShouldEqual("SelectAddress");

            resultSelectAddressGet.ShouldNotBeNull();
            resultSelectAddressGet.ShouldBeType<ViewResult>();
            viewResult.ViewName.ShouldEqual("~/Areas/Broadband/Views/LineChecker/SelectAddress.cshtml");

            resultSelectAddressPost.ShouldNotBeNull();
            var actualObject = resultSelectAddressPost.ShouldBeType<JsonResult>();
            PropertyInfo property = actualObject.Data.GetType().GetProperty("Status");
            object propertyValue = property?.GetValue(actualObject.Data);
            propertyValue.ShouldEqual("AvailablePackages");

            var broadbandJourneyDetails = fakeSessionManager.GetSessionDetails<BroadbandJourneyDetails>("broadband_journey");

            broadbandJourneyDetails.Customer.SelectedAddress.ShouldNotBeNull();
            broadbandJourneyDetails.Customer.PostcodeEntered.ShouldNotBeNull();
            broadbandJourneyDetails.Customer.SelectedAddress.Id.ShouldEqual(viewModel.SelectedAddressId);
        }
    }
}