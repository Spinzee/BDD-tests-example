namespace Products.Tests.HomeServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Helpers;
    using Model.Constants;
    using NUnit.Framework;
    using Products.Model.HomeServices;
    using Products.Tests.Common.Fakes;
    using Products.Tests.Common.Helpers;
    using Products.Web.Areas.HomeServices.Controllers;
    using Products.WebModel.ViewModels.HomeServices;
    using ServiceWrapper.BankDetailsService;
    using Should;

    public class BankDetailsTests
    {
        [Test]
        public void ShouldRedirectToSummaryPageWhenValidBankDetailsAreEntered()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            var viewModel = new BankDetailsViewModel
            {
                IsAuthorisedChecked = true,
                AccountHolder = "Test",
                AccountNumber = "12345678",
                SortCode = "102030",
                SortCodeSegmentOne = "10",
                SortCodeSegmentTwo = "20",
                SortCodeSegmentThree = "30",
                DirectDebitDate = "1"
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = controller.BankDetails(viewModel);
            var customer = fakeSessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);

            // Assert
            customer.BankServiceRetryCount.ShouldEqual(0);
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("Summary");
        }

        [TestCaseSource(nameof(GetInvalidBankDetailsViewModel))]
        public void ShouldReturnBankDetailsViewWhenDirectDebitDetailsAreInvalid(BankDetailsViewModel model, string errorMessage)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer
            {
                AvailableProduct = FakeHomeServicesProductStub.GetFakeProducts("BOBC"),
                SelectedExtraCodes = new List<string>() { "EC" },
                SelectedProductCode = "BOBC"
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = controller.BankDetails(model);

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<BankDetailsViewModel>();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Keys.Count.ShouldEqual(1);
            controller.ModelState.Values.First().Errors.First().ErrorMessage.ShouldEqual(errorMessage);
        }

        [Test]
        public void ShouldStoreBankDetailsInSession()
        {
            // Arrange
            var fakeBankDetailsService = new FakeBankDetailsService();
            getBankDetailsResponse bankDetails = fakeBankDetailsService.GetBankDetailsResponse;

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            var bankDetailsViewModel = new BankDetailsViewModel
            {
                IsAuthorisedChecked = true,
                AccountHolder = "Mr Test",
                AccountNumber = "12345678",
                SortCode = "102030",
                SortCodeSegmentOne = "10",
                SortCodeSegmentTwo = "20",
                SortCodeSegmentThree = "30",
                DirectDebitDate = "1"
            };

            // Act
            controller.ValidateViewModel(bankDetailsViewModel);
            controller.BankDetails(bankDetailsViewModel);

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            customer.DirectDebitDetails.AccountName.ShouldEqual(bankDetailsViewModel.AccountHolder);
            customer.DirectDebitDetails.AccountNumber.ShouldEqual(bankDetailsViewModel.AccountNumber);
            customer.DirectDebitDetails.SortCode.ShouldEqual(bankDetailsViewModel.SortCode);
            customer.DirectDebitDetails.DirectDebitPaymentDate.ToString().ShouldEqual(bankDetailsViewModel.DirectDebitDate);

            customer.DirectDebitDetails.BankName.ShouldEqual(bankDetails.bankName);
            customer.DirectDebitDetails.BankAddressLine1.ShouldEqual(bankDetails.bankFormattedAddress.bankAddressLine1);
            customer.DirectDebitDetails.BankAddressLine2.ShouldEqual(bankDetails.bankFormattedAddress.bankAddressLine2);
            customer.DirectDebitDetails.BankAddressLine3.ShouldEqual(bankDetails.bankFormattedAddress.bankAddressLine3);
            customer.DirectDebitDetails.Postcode.ShouldEqual(bankDetails.bankFormattedAddress.bankPostcode);
        }

        [Test]
        public void IfBankDetailsServiceFailsShouldGoToTechFaultAndLogError()
        {
            // Arrange
            var fakeBankDetailsService = new FakeBankDetailsService { ThrowException = true };
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBankDetailsServiceWrapper(fakeBankDetailsService)
                .Build<HomeServicesController>();

            var model = new BankDetailsViewModel
            {
                AccountNumber = "12345678",
                AccountHolder = "Mr Test",
                SortCode = "050200"
            };

            // Assert
            var ex = Assert.Throws<Exception>(() => controller.BankDetails(model));
            ex.Message.ShouldNotBeNull();
            ex.Message.ShouldEqual("Exception occured while calling Bank Details Service");
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void ShouldDisplayBankDetailsPageAgainIfRetryCountIsLessThanOrEqualToThree(int retryCount)
        {
            // Arrange
            var fakeBankDetailsService = new FakeBankDetailsService(null);

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer
            {
                BankServiceRetryCount = retryCount,
                AvailableProduct = FakeHomeServicesProductStub.GetFakeProducts("BOBC"),
                SelectedExtraCodes = new List<string>() { "EC" },
                SelectedProductCode = "BOBC"
            });
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBankDetailsServiceWrapper(fakeBankDetailsService)
                .Build<HomeServicesController>();

            var model = new BankDetailsViewModel
            {
                AccountNumber = "12345678",
                AccountHolder = "Test",
                SortCode = "000000"
            };

            // Act
            ActionResult result = controller.BankDetails(model);

            // Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<BankDetailsViewModel>();
            viewModel.IsRetry.ShouldEqual(true);
        }

        [Test]
        public void ShouldGoToFalloutPageIfRetryCountIsMoreThanThree()
        {
            // Arrange
            var fakeBankDetailsService = new FakeBankDetailsService(null);

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer
            {
                BankServiceRetryCount = 3
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBankDetailsServiceWrapper(fakeBankDetailsService)
                .Build<HomeServicesController>();

            var model = new BankDetailsViewModel
            {
                AccountNumber = "12345678",
                AccountHolder = "Test",
                SortCode = "000000"
            };

            // Act
            ActionResult result = controller.BankDetails(model);

            // Assert
            result.ShouldNotBeNull();
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("InvalidBankDetails");
        }

        private static IEnumerable<TestCaseData> GetInvalidBankDetailsViewModel()
        {
            yield return new TestCaseData(new BankDetailsViewModel
            {
                IsAuthorisedChecked = false,
                AccountHolder = "Test",
                AccountNumber = "12345678",
                SortCode = "102030",
                SortCodeSegmentOne = "10",
                SortCodeSegmentTwo = "20",
                SortCodeSegmentThree = "30",
                DirectDebitDate = "1"
            }, "Please select this box to confirm this statement is correct or contact us to complete your order.");

            yield return new TestCaseData(new BankDetailsViewModel
            {
                IsAuthorisedChecked = true,
                AccountNumber = "12345678",
                SortCode = "102030",
                SortCodeSegmentOne = "10",
                SortCodeSegmentTwo = "20",
                SortCodeSegmentThree = "30",
                DirectDebitDate = "1"
            }, "Please enter your account holder name.");

            yield return new TestCaseData(new BankDetailsViewModel
            {
                IsAuthorisedChecked = true,
                AccountHolder = "LongAccountHolderName",
                AccountNumber = "12345678",
                SortCode = "102030",
                SortCodeSegmentOne = "10",
                SortCodeSegmentTwo = "20",
                SortCodeSegmentThree = "30",
                DirectDebitDate = "1"
            }, "The account holder name can only include letters, hyphens, and apostrophes, and must be less than 18 characters long.");

            yield return new TestCaseData(new BankDetailsViewModel
            {
                IsAuthorisedChecked = true,
                AccountHolder = "Test",
                AccountNumber = "12345678",
                SortCodeSegmentOne = "10",
                SortCodeSegmentTwo = "20",
                SortCodeSegmentThree = "30",
                DirectDebitDate = "1"
            }, "Please enter your sort code.");

            yield return new TestCaseData(new BankDetailsViewModel
            {
                IsAuthorisedChecked = true,
                AccountHolder = "Test",
                AccountNumber = "12345678",
                SortCode = "10203012",
                SortCodeSegmentOne = "10",
                SortCodeSegmentTwo = "20",
                SortCodeSegmentThree = "30",
                DirectDebitDate = "1"
            }, "The sort code must only contain numbers, in three sets of two digits.");

            yield return new TestCaseData(new BankDetailsViewModel
            {
                IsAuthorisedChecked = true,
                AccountHolder = "Test",
                SortCode = "102030",
                SortCodeSegmentOne = "10",
                SortCodeSegmentTwo = "20",
                SortCodeSegmentThree = "30",
                DirectDebitDate = "1"
            }, "Please enter your account number.");

            yield return new TestCaseData(new BankDetailsViewModel
            {
                IsAuthorisedChecked = true,
                AccountHolder = "Test",
                AccountNumber = "1234567812",
                SortCode = "102030",
                SortCodeSegmentOne = "10",
                SortCodeSegmentTwo = "20",
                SortCodeSegmentThree = "30",
                DirectDebitDate = "1"
            }, "The account number must contain eight digits. For seven digit account numbers, add a zero before the first number. For account numbers over eight digits, ask your bank which digits to use.");

            yield return new TestCaseData(new BankDetailsViewModel
            {
                IsAuthorisedChecked = true,
                AccountHolder = "Test",
                AccountNumber = "12345678",
                SortCode = "102030",
                SortCodeSegmentOne = "10",
                SortCodeSegmentTwo = "20",
                SortCodeSegmentThree = "30"
            }, "Please enter a payment date.");

            yield return new TestCaseData(new BankDetailsViewModel
            {
                IsAuthorisedChecked = true,
                AccountHolder = "Test",
                AccountNumber = "12345678",
                SortCode = "102030",
                SortCodeSegmentOne = "10",
                SortCodeSegmentTwo = "20",
                SortCodeSegmentThree = "30",
                DirectDebitDate = "31"
            }, "Please enter a payment date of up to two digits, from 1 to 28.");

        }

    }
}
