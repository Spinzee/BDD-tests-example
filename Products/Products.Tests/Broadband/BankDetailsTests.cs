namespace Products.Tests.Broadband
{
    using System;
    using System.ServiceModel;
    using System.Web.Mvc;
    using Helpers;
    using NUnit.Framework;
    using Products.Model.Broadband;
    using Products.Tests.Common.Fakes;
    using Products.Tests.Common.Helpers;
    using Products.Web.Areas.Broadband.Controllers;
    using Products.WebModel.ViewModels.Broadband;
    using ServiceWrapper.BankDetailsService;
    using Should;
    using FakeSessionManager = Fakes.Services.FakeSessionManager;

    [TestFixture]
    public class BankDetailsTests
    {
        [TestCase("99999!", false, 1, "If sort code is not valid, do not call bank details service, display in line error and remain on bank details view")]
        [TestCase("ABC999", false, 1, "If sort code is not valid, do not call bank details service, display in line error and remain on bank details view")]
        [TestCase("", false, 1, "If sort code is not valid, do not call bank details service, display in line error and remain on bank details view")]
        [TestCase(null, false, 1, "If sort code is not valid, do not call bank details service, display in line error and remain on bank details view")]
        [TestCase("1234", false, 1, "If sort code is not valid, do not call bank details service, display in line error and remain on bank details view")]
        [TestCase("1234567", false, 1, "If sort code is not valid, do not call bank details service, display in line error and remain on bank details view")]
        [TestCase("10-20-30", false, 1, "Invalid sort code, stay on Bank details page view")]
        [TestCase("11/11/11", false, 1, "Invalid sort code, stay on Bank details page view")]
        [TestCase("11_11_11", false, 1, "Invalid sort code, stay on Bank details page view")]
        [TestCase("11:11:11", false, 1, "Invalid sort code, stay on Bank details page view")]
        [TestCase("7777777", false, 1, "Invalid sort code, stay on Bank details page view")]
        [TestCase("45678", false, 1, "Invalid sort code, stay on Bank details page view")]
        [TestCase("10001000", false, 1, "invalid sort code, stay on Bank details page view")]
        public void ShouldNotCallBankDetailsServiceAndStayOnBankDetailsPageWhenSortCodeIsInvalid(string sortCode, bool bankServiceCalled, int numberOfInlineErrors, string description)
        {
            // Arrange
            getBankDetailsResponse response = CreateValidBankDetailResponse();
            var fakeBankDetailsService = new FakeBankDetailsService(response);
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory()
                .WithBankDetailsServiceWrapper(fakeBankDetailsService)
                .WithSessionManager(fakeSessionManager)
                .Build<BankDetailsController>();

            var model = new BankDetailsViewModel
            {
                AccountNumber = "12345678",
                AccountHolder = "Mr Test",
                // random sortcode segments 
                SortCodeSegmentOne = "11",
                SortCodeSegmentTwo = "11",
                SortCodeSegmentThree = "11",
                SortCode = sortCode,
                IsAuthorisedChecked = true
            };

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = controller.Submit(model);

            // Assert
            result.ShouldNotBeNull();
            var view = result.ShouldBeType<ViewResult>();
            view.ViewName.ShouldEqual("~/Areas/Broadband/Views/BankDetails/BankDetails.cshtml");
            fakeBankDetailsService.ServiceCalled.ShouldEqual(bankServiceCalled);
            controller.ModelState.Keys.Count.ShouldEqual(numberOfInlineErrors);
        }

        [TestCase("050200", true, 0, "Summary", "If sort code is valid, call bank details service, then go to summary page")]
        [TestCase("102030", true, 0, "Summary", "Valid sort code, go to Summary view")]
        [TestCase("111111", true, 0, "Summary", "Valid sort code, go to Summary view")]
        [TestCase("999999", true, 0, "Summary", "Valid sort code, go to Summary view")]
        public void ShouldCallBankDetailsServiceAndGoToSummaryPageWhenSortCodeIsValid(string sortCode, bool bankServiceCalled, int numberOfInlineErrors, string viewName, string description)
        {
            // Arrange
            getBankDetailsResponse response = CreateValidBankDetailResponse();

            var fakeBankDetailsService = new FakeBankDetailsService(response);
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory()
                .WithBankDetailsServiceWrapper(fakeBankDetailsService)
                .WithSessionManager(fakeSessionManager)
                .Build<BankDetailsController>();

            var model = new BankDetailsViewModel
            {
                AccountNumber = "12345678",
                AccountHolder = "Mr Test",
                // random sortcode segments 
                SortCodeSegmentOne = "11",
                SortCodeSegmentTwo = "11",
                SortCodeSegmentThree = "11",
                SortCode = sortCode,
                IsAuthorisedChecked = true
            };

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = controller.Submit(model);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult)result).RouteValues["action"].ShouldEqual(viewName);
            fakeBankDetailsService.ServiceCalled.ShouldEqual(bankServiceCalled);
            controller.ModelState.Keys.Count.ShouldEqual(numberOfInlineErrors);
        }

        [TestCase("1234567!", false, 1, "If account number is not valid, do not call bank details service, display in line error and remain on bank details view")]
        [TestCase("1234ABCD", false, 1, "If account number is not valid, do not call bank details service, display in line error and remain on bank details view")]
        [TestCase("", false, 1, "If account number is not valid, do not call bank details service, display in line error and remain on bank details view")]
        [TestCase(null, false, 1, "If account number is not valid, do not call bank details service, display in line error and remain on bank details view")]
        [TestCase("1234", false, 1, "If account number is not valid, do not call bank details service, display in line error and remain on bank details view")]
        [TestCase("123456789", false, 1, "If account number is not valid, do not call bank details service, display in line error and remain on bank details view")]
        public void ShouldNotCallBankDetailsServiceWhenAccountNumberIsInvalid(string accountNumber, bool bankServiceCalled, int numberOfInlineErrors, string description)
        {
            // Arrange
            getBankDetailsResponse response = CreateValidBankDetailResponse();
            var fakeBankDetailsService = new FakeBankDetailsService(response);
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory()
                .WithBankDetailsServiceWrapper(fakeBankDetailsService)
                .WithSessionManager(fakeSessionManager)
                .Build<BankDetailsController>();

            var model = new BankDetailsViewModel
            {
                AccountNumber = accountNumber,
                AccountHolder = "Mr Test",
                // random sortcode segments 
                SortCodeSegmentOne = "11",
                SortCodeSegmentTwo = "11",
                SortCodeSegmentThree = "11",
                SortCode = "050200",
                IsAuthorisedChecked = true
            };

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = controller.Submit(model);

            // Assert
            result.ShouldNotBeNull();
            var view = result.ShouldBeType<ViewResult>();
            view.ViewName.ShouldEqual("~/Areas/Broadband/Views/BankDetails/BankDetails.cshtml");
            fakeBankDetailsService.ServiceCalled.ShouldEqual(bankServiceCalled);
            controller.ModelState.Keys.Count.ShouldEqual(numberOfInlineErrors);
        }

        [TestCase("12345678", true, 0, "Summary", "If account number is valid, call bank details service, and go to summary view")]
        public void ShouldOnlyCallBankDetailsServiceIfAccountNumberIsEightNumericalValues(string accountNumber, bool bankServiceCalled, int numberOfInlineErrors, string viewName, string description)
        {
            // Arrange
            getBankDetailsResponse response = CreateValidBankDetailResponse();
            var fakeBankDetailsService = new FakeBankDetailsService(response);
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory()
                .WithBankDetailsServiceWrapper(fakeBankDetailsService)
                .WithSessionManager(fakeSessionManager)
                .Build<BankDetailsController>();

            var model = new BankDetailsViewModel
            {
                AccountNumber = accountNumber,
                AccountHolder = "Mr Test",
                // random sortcode segments 
                SortCodeSegmentOne = "11",
                SortCodeSegmentTwo = "11",
                SortCodeSegmentThree = "11",
                SortCode = "050200",
                IsAuthorisedChecked = true
            };

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = controller.Submit(model);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult)result).RouteValues["action"].ShouldEqual(viewName);
            fakeBankDetailsService.ServiceCalled.ShouldEqual(bankServiceCalled);
            controller.ModelState.Keys.Count.ShouldEqual(numberOfInlineErrors);
        }

        [TestCase("THISNAMEHASNINETEEN", false, 1, "If account name is longer than 18 characters, do not call bank details service, display in line error and remain on bank details view")]
        [TestCase("1234ABCD", false, 1, "If account name contains numbers, do not call bank details service, display in line error and remain on bank details view")]
        [TestCase("", false, 1, "If account name is empty, do not call bank details service, display in line error and remain on bank details view")]
        [TestCase(null, false, 1, "If account name is null, do not call bank details service, display in line error and remain on bank details view")]
        [TestCase("Mr Te@T", false, 1, "If account name is not valid, do not call bank details service, display in line error and remain on bank details view")]
        [TestCase("1", false, 1, "If account name is not valid, do not call bank details service, display in line error and remain on bank details view")]
        [TestCase("Mr Te_T", false, 1, "If account name is not valid, do not call bank details service, display in line error and remain on bank details view")]
        public void ShouldNotCallBankDetailsServiceWhenAccountHolderNameIsInvalid(string accountHolderName, bool bankServiceCalled, int numberOfInlineErrors, string description)
        {
            // Arrange
            getBankDetailsResponse response = CreateValidBankDetailResponse();
            var fakeBankDetailsService = new FakeBankDetailsService(response);
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory()
                .WithBankDetailsServiceWrapper(fakeBankDetailsService)
                .WithSessionManager(fakeSessionManager)
                .Build<BankDetailsController>();

            var model = new BankDetailsViewModel
            {
                AccountNumber = "12345678",
                AccountHolder = accountHolderName,
                // random sortcode segments 
                SortCodeSegmentOne = "11",
                SortCodeSegmentTwo = "11",
                SortCodeSegmentThree = "11",
                SortCode = "050200",
                IsAuthorisedChecked = true
            };

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = controller.Submit(model);

            // Assert
            result.ShouldNotBeNull();
            var view = result.ShouldBeType<ViewResult>();
            view.ViewName.ShouldEqual("~/Areas/Broadband/Views/BankDetails/BankDetails.cshtml");
            fakeBankDetailsService.ServiceCalled.ShouldEqual(bankServiceCalled);
            controller.ModelState.Keys.Count.ShouldEqual(numberOfInlineErrors);
        }

        [TestCase("Mr Test Test", true, 0, "Summary", "If account name is valid, call bank details service, and go to summary view")]
        [TestCase("Mr Test's T-est", true, 0, "Summary", "If account name has ' or - this is valid, call bank details service, and go to summary page")]
        [TestCase("A", true, 0, "Summary", "Single character account name, call bank details service, and go to summary page")]
        [TestCase("THISNAMEISEIGHTEEN", true, 0, "Summary", "Eighteen character account name, call bank details service, and go to summary page")]
        public void ShouldOnlyCallBankDetailsServiceIfAccountHolderNameIsNoMoreThan18Characters(string accountHolderName, bool bankServiceCalled, int numberOfInlineErrors, string viewName, string description)
        {
            // Arrange
            getBankDetailsResponse response = CreateValidBankDetailResponse();
            var fakeBankDetailsService = new FakeBankDetailsService(response);
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory()
                .WithBankDetailsServiceWrapper(fakeBankDetailsService)
                .WithSessionManager(fakeSessionManager)
                .Build<BankDetailsController>();

            var model = new BankDetailsViewModel
            {
                AccountNumber = "12345678",
                AccountHolder = accountHolderName,
                // random sortcode segments 
                SortCodeSegmentOne = "11",
                SortCodeSegmentTwo = "11",
                SortCodeSegmentThree = "11",
                SortCode = "050200",
                IsAuthorisedChecked = true
            };

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = controller.Submit(model);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult)result).RouteValues["action"].ShouldEqual(viewName);
            fakeBankDetailsService.ServiceCalled.ShouldEqual(bankServiceCalled);
            controller.ModelState.Keys.Count.ShouldEqual(numberOfInlineErrors);
        }

        [Test]
        public void ShouldDisplaySummaryPageWhenIsAuthorisedConsentIsTicked()
        {
            // Arrange
            getBankDetailsResponse response = CreateValidBankDetailResponse();
            var fakeBankDetailsService = new FakeBankDetailsService(response);
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory()
                .WithBankDetailsServiceWrapper(fakeBankDetailsService)
                .WithSessionManager(fakeSessionManager)
                .Build<BankDetailsController>();

            var model = new BankDetailsViewModel
            {
                AccountNumber = "12345678",
                AccountHolder = "Test Test",
                // random sortcode segments 
                SortCodeSegmentOne = "11",
                SortCodeSegmentTwo = "11",
                SortCodeSegmentThree = "11",
                SortCode = "050200",
                IsAuthorisedChecked = true
            };

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = controller.Submit(model);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult)result).RouteValues["action"].ShouldEqual("Summary");
        }

        [Test]
        public void ShouldStayOnBankDetailsPageWhenIsAuthorisedConsentIsNotTicked()
        {
            // Arrange
            getBankDetailsResponse response = CreateValidBankDetailResponse();
            var fakeBankDetailsService = new FakeBankDetailsService(response);
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory()
                .WithBankDetailsServiceWrapper(fakeBankDetailsService)
                .WithSessionManager(fakeSessionManager)
                .Build<BankDetailsController>();

            var model = new BankDetailsViewModel
            {
                AccountNumber = "12345678",
                AccountHolder = "Test Test",
                // random sortcode segments 
                SortCodeSegmentOne = "11",
                SortCodeSegmentTwo = "11",
                SortCodeSegmentThree = "11",
                SortCode = "050200",
                IsAuthorisedChecked = false
            };

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = controller.Submit(model);

            // Assert
            result.ShouldNotBeNull();
            var view = result.ShouldBeType<ViewResult>();
            view.ViewName.ShouldEqual("~/Areas/Broadband/Views/BankDetails/BankDetails.cshtml");
            fakeBankDetailsService.ServiceCalled.ShouldEqual(false);
            controller.ModelState.Keys.Count.ShouldEqual(1);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void ShouldDisplayBankDetailsPageAgainIfRetryCountIsLessThanOrEqualToThree(int attempt)
        {
            // Arrange
            var fakeBankDetailsService = new FakeBankDetailsService
            {
                Exception = new FaultException<invalidRequestFaultType>(new invalidRequestFaultType()),
                ThrowException = true
            };
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory()
                .WithBankDetailsServiceWrapper(fakeBankDetailsService)
                .WithSessionManager(fakeSessionManager)
                .Build<BankDetailsController>();

            var model = new BankDetailsViewModel
            {
                AccountNumber = "12345678",
                AccountHolder = "Test",
                SortCode = "000000"
            };

            // Act
            ActionResult result = null;
            for (int i = 0; i < attempt; i++)
            {
                result = controller.Submit(model);
            }

            // Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.ViewName.ShouldEqual("~/Areas/Broadband/Views/BankDetails/BankDetails.cshtml");
            var viewModel = viewResult.Model.ShouldBeType<BankDetailsViewModel>();
            viewModel.IsRetry.ShouldEqual(true);
        }

        [Test]
        public void ShouldGoToFalloutPageIfRetryCountIsMoreThanThree()
        {
            // Arrange
            var fakeBankDetailsService = new FakeBankDetailsService
            {
                Exception = new FaultException<invalidRequestFaultType>(new invalidRequestFaultType()),
                ThrowException = true
            };
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory()
                .WithBankDetailsServiceWrapper(fakeBankDetailsService)
                .WithSessionManager(fakeSessionManager)
                .Build<BankDetailsController>();

            var model = new BankDetailsViewModel
            {
                AccountNumber = "12345678",
                AccountHolder = "Test",
                SortCode = "000000"
            };

            // Act
            controller.Submit(model);
            controller.Submit(model);
            controller.Submit(model);
            ActionResult result = controller.Submit(model); // 4th attempt

            // Assert
            result.ShouldNotBeNull();
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("Fallout");
        }

        [TestCase("Summary", true, true, false, "The Big Bank", "If valid sortcode and account number and bank address returned, go to Summary page")]        
        public void ShouldDisplaySummaryPageIfSortCodeAndAccountNumberIsValid(string viewName, bool sortCodeValid, bool sortCodeAccountNumberValid, bool corporateAccountValid, string bankName, string description)
        {
            // Arrange
            var response = new getBankDetailsResponse
            {
                sortCodeAccountNumberValid = sortCodeAccountNumberValid,
                sortCodeValid = sortCodeValid,
                corporateAccountValid = corporateAccountValid,
                bankName = bankName
            };

            var fakeBankDetailsService = new FakeBankDetailsService(response);
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory()
                .WithBankDetailsServiceWrapper(fakeBankDetailsService)
                .WithSessionManager(fakeSessionManager)
                .Build<BankDetailsController>();

            var model = new BankDetailsViewModel
            {
                AccountNumber = "12345678",
                AccountHolder = "Mr Test",
                SortCode = "050200"
            };

            // Act
            ActionResult result = controller.Submit(model);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult)result).RouteValues["action"].ShouldEqual(viewName);
        }

        [Test]
        public void IfBankDetailsServiceFailsShouldGoToTechFaultAndLogError()
        {
            // Arrange
            var fakeBankDetailsService = new FakeBankDetailsService { ThrowException = true };
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory()
                .WithBankDetailsServiceWrapper(fakeBankDetailsService)
                .WithSessionManager(fakeSessionManager)
                .Build<BankDetailsController>();

            var model = new BankDetailsViewModel
            {
                AccountNumber = "12345678",
                AccountHolder = "Mr Test",
                SortCode = "050200",
                YourPriceViewModel = new YourPriceViewModel()
            };

            // Assert
            var ex = Assert.Throws<Exception>(() => controller.Submit(model));
            ex.Message.ShouldNotBeNull();
            ex.Message.ShouldEqual("Exception occured while calling Bank Details Service");
        }

        [TestCase(true, "If SSE Customer display connection fee with strike through")]
        [TestCase(false, "If not SSE customer display connection fee")]
        public void ShouldDisplayConnectionFeeWithStrikeThroughForSSECustomer(bool isSSECustomer, string description)
        {
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest())
                .Build();

            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = isSSECustomer } }
            };

            fakeSessionManager.SetListOfProducts();

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager);

            var packageController = controllerFactory.Build<PackagesController>();

            var controller = controllerFactory.Build<BankDetailsController>();

            packageController.SelectedPackage(new SelectedPackageViewModel(), "ABC");
            ActionResult result = controller.BankDetails();

            var view = result.ShouldBeType<ViewResult>();
            var viewModel = view.ViewData.Model.ShouldBeType<BankDetailsViewModel>();
            viewModel.YourPriceViewModel.IsExistingCustomer.ShouldEqual(isSSECustomer);
        }

        [TestCase(null, "Step 4 of 5", "no cli was provided, final step count should be 5")]
        [TestCase("12345789", "Step 4 of 5", "cli was provided, final step count should be 4")]
        public void ShouldDisplayCorrectStepCountOnBankDetailsPage(string cli, string stepCount, string description)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };

            ControllerFactory controllerFactory = new ControllerFactory().WithSessionManager(fakeSessionManager);

            var lineCheckerController = controllerFactory.Build<LineCheckerController>();

            var controller = controllerFactory.Build<BankDetailsController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO9 1QH",
                PhoneNumber = cli
            };

            // Act
            // ReSharper disable once UnusedVariable
            ActionResult result1 = lineCheckerController.Submit(lineCheckerViewModel).Result;
            ActionResult result = controller.BankDetails();

            // Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            (viewResult.ViewBag.StepCounter as string).ShouldEqual(stepCount);
        }

        [TestCase(null, "Step 4 of 5", "no cli was provided, final step count should be 5")]
        [TestCase("12345789", "Step 4 of 5", "cli was provided, final step count should be 4")]
        public void ShouldDisplayCorrectStepCountOnBankDetailsPageIfModelIsNotValid(string cli, string stepCount, string description)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };

            ControllerFactory controllerFactory = new ControllerFactory().WithSessionManager(fakeSessionManager);

            var lineCheckerController = controllerFactory.Build<LineCheckerController>();

            var controller = controllerFactory.Build<BankDetailsController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO9 1QH",
                PhoneNumber = cli
            };

            var model = new BankDetailsViewModel();

            // Act
            // ReSharper disable once UnusedVariable
            ActionResult result1 = lineCheckerController.Submit(lineCheckerViewModel).Result;
            controller.ValidateViewModel(model);
            ActionResult result = controller.Submit(model);

            // Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            (viewResult.ViewBag.StepCounter as string).ShouldEqual(stepCount);
        }

        private static getBankDetailsResponse CreateValidBankDetailResponse()
        {
            return new getBankDetailsResponse
            {
                sortCodeAccountNumberValid = true,
                sortCodeValid = true,
                corporateAccountValid = false,
                bankName = "Test Bank",
                bankFormattedAddress = new bankFormattedAddressType
                {
                    bankAddressLine1 = "TestAddressLine1",
                    bankAddressLine2 = "TestAddressLine2",
                    bankAddressLine3 = "TestAddressLine3",
                    bankAddressLine4 = "TestAddressLine4",
                    bankPostcode = "PO9 1QH"
                }
            };
        }
    }
}