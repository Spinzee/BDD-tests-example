namespace Products.Tests.Broadband
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Common.Fakes;
    using Common.Helpers;
    using Fakes.Services;
    using Helpers;
    using Model;
    using NUnit.Framework;
    using Products.Model.Broadband;
    using ServiceWrapper.BankDetailsService;
    using Should;
    using Web.Areas.Broadband.Controllers;
    using WebModel.ViewModels.Broadband;
    using WebModel.ViewModels.Common;
    using FakeSessionManager = Fakes.Services.FakeSessionManager;

    [TestFixture]
    public class SummaryTests
    {
        [Test]
        public void ShouldLogErrorAndGotToTechFaultIfUnableToRetrieveSessionObjectFromBaseController()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                ThrowException = true
            };

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SummaryController>();

            // Assert
            var ex = Assert.Throws<Exception>(() => controller.Summary());
            ex.Message.ShouldNotBeNull();
            ex.Message.ShouldEqual("Exception of type 'System.Exception' was thrown.");
        }

        [TestCase(true, "If SSE Customer display connection fee with strike through")]
        [TestCase(false, "If not SSE customer display connection fee")]
        public void ShouldDisplayConnectionFeeWithStrikeThroughForSSECustomer(bool isSSECustomer, string description)
        {
            // Arrange
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest())
                .Build();

            BroadbandJourneyDetails broadbandJourneyDetails = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails();
            broadbandJourneyDetails.Customer.IsSSECustomer = isSSECustomer;
            var fakeSessionManager = new FakeSessionManager { SessionObject = broadbandJourneyDetails, ListOfProductsSessionObject = FakeBroadbandProductsData.GetListOfAvailableProductsFromSession() };

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager);

            var packageController = controllerFactory.Build<PackagesController>();

            var controller = controllerFactory.Build<SummaryController>();

            // Act
            packageController.SelectedPackage(new SelectedPackageViewModel(), "BB18_LR");
            ActionResult result = controller.Summary();

            // Assert
            var view = result.ShouldBeType<ViewResult>();
            var viewModel = view.ViewData.Model.ShouldBeType<SummaryViewModel>();
            viewModel.YourPriceViewModel.IsExistingCustomer.ShouldEqual(isSSECustomer);
        }

        private static IEnumerable<TestCaseData> GetBroadbandJourneyDetails()
        {
            yield return new TestCaseData("Step 5 of 5", FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails(), "cli was provided, final step count should be 5");
            yield return new TestCaseData("Step 5 of 5", FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetailsWithNoCLI(), "no cli was provided, final step count should be 5");
        }

        private static IEnumerable<TestCaseData> FakeTermsAndConditionsPdfData()
        {
            yield return new TestCaseData("FF3_LR18", BroadbandProductGroup.FixAndFibreV3, 2, new List<string> { "PDF A", "PDF B" });
            yield return new TestCaseData("FIBRE18_LR", BroadbandProductGroup.None, 0, new List<string>());
        }

        [Test]
        public void InstallationParagraphShouldContainInstallationFeeFromConfig()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails()
            };
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();

            var controller = new ControllerFactory()
                .WithConfigManager(fakeConfigManager)
                .WithSessionManager(fakeSessionManager)
                .Build<SummaryController>();

            // Act

            ActionResult result = controller.Summary();

            // Assert
            var view = result.ShouldBeType<ViewResult>();
            var viewModel = view.ViewData.Model.ShouldBeType<SummaryViewModel>();
            viewModel.InstallationParagraph.ShouldContain("£60");
        }

        [Test]
        public void ShouldDisplayBankDetailsOnMandatePage()
        {
            // Arrange
            var bankDetailsResponse = new getBankDetailsResponse
            {
                sortCodeAccountNumberValid = true,
                corporateAccountValid = false,
                sortCodeValid = true,
                bankName = "Big Bank",
                bankFormattedAddress = new bankFormattedAddressType
                {
                    bankAddressLine1 = "20",
                    bankAddressLine2 = "London Street",
                    bankAddressLine3 = "Reading",
                    bankPostcode = "RG1 3BS"
                }
            };
            var fakeBankDetailsService = new FakeBankDetailsService
            {
                GetBankDetailsResponse = bankDetailsResponse
            };
            var fakeSessionManager = new FakeSessionManager { SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = true } }, ListOfProductsSessionObject = new List<BroadbandProduct>() };
            ControllerFactory controllerFactory = new ControllerFactory()
                .WithBankDetailsServiceWrapper(fakeBankDetailsService).WithSessionManager(fakeSessionManager);

            var bankDetailsController = controllerFactory.Build<BankDetailsController>();

            var summaryController = controllerFactory.Build<SummaryController>();

            var model = new BankDetailsViewModel
            {
                AccountNumber = "123456789",
                SortCode = "112233",
                AccountHolder = "Mrs Test"
            };

            // Act
            bankDetailsController.Submit(model);
            ActionResult result = summaryController.PrintMandate();

            // Assert
            var view = (ViewResult) result;
            var viewModel = view.ViewData.Model.ShouldBeType<DirectDebitMandateViewModel>();
            viewModel.AccountNumber.ShouldBeSameAs(model.AccountNumber);
            viewModel.Name.ShouldBeSameAs(model.AccountHolder);
            viewModel.Sortcode.ShouldBeSameAs(model.SortCode);

            viewModel.BankName.ShouldBeSameAs(bankDetailsResponse.bankName);
            viewModel.AddressLine1.ShouldBeSameAs(bankDetailsResponse.bankFormattedAddress.bankAddressLine1);
            viewModel.AddressLine2.ShouldBeSameAs(bankDetailsResponse.bankFormattedAddress.bankAddressLine2);
            viewModel.AddressLine3.ShouldBeSameAs(bankDetailsResponse.bankFormattedAddress.bankAddressLine3);
            viewModel.Postcode.ShouldBeSameAs(bankDetailsResponse.bankFormattedAddress.bankPostcode);
        }

        [Test]
        public async Task ShouldDisplayConfirmationPageIfTermsAndConditionsAreTicked()
        {
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails()
            };
            var cookieCollection = new HttpCookieCollection { new HttpCookie("migrateMemberid", "123456"), new HttpCookie("migrateAffiliateid", "affiliateCode1"), new HttpCookie("migrateCampaignid", "1410789843095") };

            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(cookieCollection))
                .Build();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithContextManager(fakeContextManager)
                .Build<SummaryController>();

            var model = new SummaryViewModel
            {
                IsTermsAndConditionsChecked = true
            };

            controller.ValidateViewModel(model);

            ActionResult result = await controller.Submit(model);

            ((RedirectToRouteResult) result).RouteValues["action"].ShouldEqual("Confirmation");
        }

        [Test]
        public async Task ShouldDisplayContactDetailsOnSummaryPage()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails()
            };

            var fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses };

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithBroadbandProductsServiceWrapper(fakeBroadbandProductsServiceWrapper)
                .WithSessionManager(fakeSessionManager);

            var lineCheckerController = controllerFactory.Build<LineCheckerController>();

            var customerDetailsController = controllerFactory.Build<CustomerDetailsController>();

            var summaryController = controllerFactory.Build<SummaryController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO9 1QH",
                PhoneNumber = "02092022982"
            };

            var model = new ContactDetailsViewModel
            {
                ContactNumber = "0209345678",
                EmailAddress = "Test@Test.com",
                IsMarketingConsentChecked = true
            };

            // Act
            await lineCheckerController.Submit(lineCheckerViewModel);
            ActionResult resultSelectAddressGet = await lineCheckerController.SelectAddress();
            var viewResult = resultSelectAddressGet.ShouldBeType<ViewResult>();
            var selectAddressViewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();
            AddressViewModel address = selectAddressViewModel.Addresses.FirstOrDefault();
            // ReSharper disable once PossibleNullReferenceException
            selectAddressViewModel.SelectedAddressId = address.Id;
            await lineCheckerController.SelectAddress(selectAddressViewModel);
            await customerDetailsController.SubmitContactDetails(model);
            ActionResult result = summaryController.Summary();

            // Assert
            var view = (ViewResult) result;
            var viewModel = view.ViewData.Model.ShouldBeType<SummaryViewModel>();
            //viewModel.ContactNumber.ShouldContain(model.ContactNumber);
            viewModel.Email.ShouldBeSameAs(model.EmailAddress);
            //viewModel.MarketingConsent.ShouldBeTrue();
        }

        [TestCaseSource(nameof(GetBroadbandJourneyDetails))]
        public void ShouldDisplayCorrectStepCountOnSummaryPage(string stepCount, BroadbandJourneyDetails journeyDetails, string description)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = journeyDetails
            };

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SummaryController>();

            // Act
            ActionResult result = controller.Summary();

            // Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            (viewResult.ViewBag.StepCounter as string).ShouldEqual(stepCount);
        }

        [TestCaseSource(nameof(GetBroadbandJourneyDetails))]
        public async Task ShouldDisplayCorrectStepCountOnSummaryPageIfTermsAndConditionsNotTicked(string stepCount, BroadbandJourneyDetails journeyDetails, string description)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = journeyDetails
            };

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SummaryController>();

            var model = new SummaryViewModel { IsTermsAndConditionsChecked = false };

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = await controller.Submit(model);

            // Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            (viewResult.ViewBag.StepCounter as string).ShouldEqual(stepCount);
        }

        [Test]
        public async Task ShouldDisplayDirectDebitDetailsOnSummaryPage()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails()
            };

            var fakeBankDetailsService = new FakeBankDetailsService
            {
                GetBankDetailsResponse = new getBankDetailsResponse
                {
                    sortCodeAccountNumberValid = true,
                    corporateAccountValid = false,
                    sortCodeValid = true,
                    bankName = "Big Bank"
                }
            };

            var fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses };

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithBankDetailsServiceWrapper(fakeBankDetailsService)
                .WithBroadbandProductsServiceWrapper(fakeBroadbandProductsServiceWrapper)
                .WithSessionManager(fakeSessionManager);

            var lineCheckerController = controllerFactory.Build<LineCheckerController>();

            var bankDetailsController = controllerFactory.Build<BankDetailsController>();

            var summaryController = controllerFactory.Build<SummaryController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO9 1QH",
                PhoneNumber = "02092022982"
            };

            var model = new BankDetailsViewModel
            {
                AccountNumber = "123456789",
                SortCode = "112233",
                AccountHolder = "Mrs Test"
            };

            // Act
            await lineCheckerController.Submit(lineCheckerViewModel);
            ActionResult resultSelectAddressGet = await lineCheckerController.SelectAddress();

            var viewResult = resultSelectAddressGet.ShouldBeType<ViewResult>();
            var selectAddressViewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();

            AddressViewModel address = selectAddressViewModel.Addresses.FirstOrDefault();
            // ReSharper disable once PossibleNullReferenceException
            selectAddressViewModel.SelectedAddressId = address.Id;
            await lineCheckerController.SelectAddress(selectAddressViewModel);
            bankDetailsController.Submit(model);
            ActionResult result = summaryController.Summary();

            // Assert
            var view = (ViewResult) result;
            var viewModel = view.ViewData.Model.ShouldBeType<SummaryViewModel>();
            viewModel.AccountNumber.ShouldEqual(model.AccountNumber);
            viewModel.AccountName.ShouldEqual(model.AccountHolder);
            viewModel.SortCode.ShouldEqual(model.SortCode);
        }

        [Test]
        public void ShouldDisplayInclusiveCallsSectionForAnytimePlusWhenAnytimePlusCallsPackageIsSelected()
        {
            // Arrange
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest())
                .Build();

            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails(),
                ListOfProductsSessionObject = FakeBroadbandProductsData.GetListOfAvailableProductsFromSession()
            };

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager);

            var packageController = controllerFactory.Build<PackagesController>();

            var controller = controllerFactory.Build<SummaryController>();

            // Act
            packageController.SelectedPackage(new SelectedPackageViewModel(), "BB_AP18");
            ActionResult result = controller.Summary();

            // Assert
            var view = result.ShouldBeType<ViewResult>();
            var viewModel = view.ViewData.Model.ShouldBeType<SummaryViewModel>();
            viewModel.HasAnytime.ShouldEqual(false);
            viewModel.HasAnytimePlus.ShouldEqual(true);
        }

        [Test]
        public void ShouldDisplayInclusiveCallsSectionForAnytimeWhenAnytimeCallsPackageIsSelected()
        {
            // Arrange
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest())
                .Build();

            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails(),
                ListOfProductsSessionObject = FakeBroadbandProductsData.GetListOfAvailableProductsFromSession()
            };

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager);

            var packageController = controllerFactory.Build<PackagesController>();

            var controller = controllerFactory.Build<SummaryController>();

            // Act
            packageController.SelectedPackage(new SelectedPackageViewModel(), "BB_ANY18");
            ActionResult result = controller.Summary();

            // Assert
            var view = result.ShouldBeType<ViewResult>();
            var viewModel = view.ViewData.Model.ShouldBeType<SummaryViewModel>();
            viewModel.HasAnytime.ShouldEqual(true);
            viewModel.HasAnytimePlus.ShouldEqual(false);
        }

        [Test]
        public async Task ShouldDisplayPersonalDetailsOnSummaryPage()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails()
            };

            var fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses };

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithBroadbandProductsServiceWrapper(fakeBroadbandProductsServiceWrapper)
                .WithSessionManager(fakeSessionManager);

            var lineCheckerController = controllerFactory.Build<LineCheckerController>();

            var customerDetailsController = controllerFactory.Build<CustomerDetailsController>();

            var summaryController = controllerFactory.Build<SummaryController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO9 1QH",
                PhoneNumber = "02092022982"
            };

            var model = new PersonalDetailsViewModel
            {
                DateOfBirth = "01/03/1981",
                FirstName = "Testfirst",
                LastName = "Testlast",
                Titles = Titles.Ms,
                DateOfBirthDay = "01",
                DateOfBirthMonth = "03",
                DateOfBirthYear = "1981"
            };

            // Act
            await lineCheckerController.Submit(lineCheckerViewModel);
            ActionResult resultSelectAddressGet = await lineCheckerController.SelectAddress();

            var viewResult = resultSelectAddressGet.ShouldBeType<ViewResult>();
            var selectAddressViewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();

            AddressViewModel address = selectAddressViewModel.Addresses.FirstOrDefault();
            // ReSharper disable once PossibleNullReferenceException
            selectAddressViewModel.SelectedAddressId = address.Id;
            await lineCheckerController.SelectAddress(selectAddressViewModel);
            customerDetailsController.Submit(model);
            ActionResult result = summaryController.Summary();

            // Assert
            var view = (ViewResult) result;
            var viewModel = view.ViewData.Model.ShouldBeType<SummaryViewModel>();
            viewModel.DateOfBirth.ShouldEqual(model.DateOfBirth);
            viewModel.Name.ShouldEqual($"{model.Titles} {model.FirstName} {model.LastName}");
        }

        [Test]
        public async Task ShouldDisplaySelectedAddressOnSummaryPage()
        {
            // Arrange
            var fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses };
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails()
            };
            ControllerFactory controllerFactory = new ControllerFactory()
                .WithBroadbandProductsServiceWrapper(fakeBroadbandProductsServiceWrapper)
                .WithSessionManager(fakeSessionManager);

            var lineCheckerController = controllerFactory.Build<LineCheckerController>();

            var summaryController = controllerFactory.Build<SummaryController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO91BH",
                PhoneNumber = "1234567"
            };

            // Act
            await lineCheckerController.Submit(lineCheckerViewModel);
            ActionResult resultSelectAddressGet = await lineCheckerController.SelectAddress();
            var viewResult = resultSelectAddressGet.ShouldBeType<ViewResult>();
            var selectAddressViewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();

            AddressViewModel address = selectAddressViewModel.Addresses.FirstOrDefault();
            // ReSharper disable once PossibleNullReferenceException
            selectAddressViewModel.SelectedAddressId = address.Id;
            await lineCheckerController.SelectAddress(selectAddressViewModel);
            ActionResult result = summaryController.Summary();

            // Assert
            result.ShouldNotBeNull();
            var view = result.ShouldBeType<ViewResult>();
            view.ViewName.ShouldEqual("~/Areas/Broadband/Views/Summary/Summary.cshtml");
            var viewModel = view.ViewData.Model.ShouldBeType<SummaryViewModel>();
            viewModel.Address.ShouldEqual(address.FormattedAddress);
            viewModel.PostCode.ShouldEqual(address.Postcode);
        }

        [Test]
        public void ShouldNotDisplayInclusiveCallsSectionWhenLandlineOnlyCallsPackageIsSelected()
        {
            // Arrange
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest())
                .Build();

            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails(),
                ListOfProductsSessionObject = FakeBroadbandProductsData.GetListOfAvailableProductsFromSession()
            };

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager);

            var packageController = controllerFactory.Build<PackagesController>();

            var controller = controllerFactory.Build<SummaryController>();

            // Act
            packageController.SelectedPackage(new SelectedPackageViewModel(), "BB18_LR");
            ActionResult result = controller.Summary();

            // Assert
            var view = result.ShouldBeType<ViewResult>();
            var viewModel = view.ViewData.Model.ShouldBeType<SummaryViewModel>();
            viewModel.HasAnytime.ShouldEqual(false);
            viewModel.HasAnytimePlus.ShouldEqual(false);
        }

        [TestCaseSource(nameof(FakeTermsAndConditionsPdfData))]
        public void ShouldOnlyPopulateTermsAndConditionsPdfLinkListWhenSelectedProductHasSpecificPdfs(string selectedProductCode, BroadbandProductGroup selectedBroadbandProductGroup, int expectedPdfCount, List<string> expectedTAndCsPdfList)
        {
            BroadbandJourneyDetails broadbandJourneyDetails = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetailsForBroadbandProductGroup(selectedBroadbandProductGroup);
            broadbandJourneyDetails.Customer.SelectedProductCode = selectedProductCode;
            broadbandJourneyDetails.Customer.SelectedProductGroup = selectedBroadbandProductGroup;

            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = broadbandJourneyDetails,
                ListOfProductsSessionObject = new List<BroadbandProduct>()
            };
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SummaryController>();

            ActionResult result = controller.Summary();

            var view = result.ShouldBeType<ViewResult>();
            var viewModel = view.ViewData.Model.ShouldBeType<SummaryViewModel>();
            viewModel.TermsAndConditionsPdfLinks.Count.ShouldEqual(expectedPdfCount);
            viewModel.TermsAndConditionsPdfLinks.Select(x => x.DisplayName).ShouldEqual(expectedTAndCsPdfList);
        }

        [Test]
        public async Task ShouldRemainOnSummaryPageAndDisplayInlineErrorIfTermsAndConditionsNotTicked()
        {
            var fakeSessionManager = new FakeSessionManager { SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = true } }, ListOfProductsSessionObject = new List<BroadbandProduct>() };
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SummaryController>();

            var model = new SummaryViewModel { IsTermsAndConditionsChecked = false };

            controller.ValidateViewModel(model);

            ActionResult result = await controller.Submit(model);

            ((ViewResult) result).ViewName.ShouldEqual("~/Areas/Broadband/Views/Summary/Summary.cshtml");

            controller.ModelState.Keys.Count.ShouldEqual(1);
        }

        [TestCase("1234567", "1234567", true, true)]
        [TestCase("1234567", "1234567", false, true)]
        [TestCase("1234567", "12345674556777", true, true)]
        [TestCase(null, "12345674556777", true, false)]
        [TestCase(null, null, false, false)]
        public async Task ShouldShowCorrectMessageBasedOnCLINumber(string phoneNumberEnteredOnLineChecker, string phoneNumberEnteredOnTransferYourNumber, bool keepExistingNumber, bool expectedResult)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails()
            };
            var fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses };

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithBroadbandProductsServiceWrapper(fakeBroadbandProductsServiceWrapper)
                .WithSessionManager(fakeSessionManager);

            var lineCheckerController = controllerFactory.Build<LineCheckerController>();

            var customerDetailsController = controllerFactory.Build<CustomerDetailsController>();

            var summaryController = controllerFactory.Build<SummaryController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO91BH",
                PhoneNumber = phoneNumberEnteredOnLineChecker
            };

            var model = new TransferYourNumberViewModel
            {
                KeepExistingNumber = keepExistingNumber,
                PhoneNumber = phoneNumberEnteredOnTransferYourNumber
            };

            // Act
            await lineCheckerController.Submit(lineCheckerViewModel);
            ActionResult resultSelectAddressGet = await lineCheckerController.SelectAddress();

            var viewResultAddress = resultSelectAddressGet.ShouldBeType<ViewResult>();
            var selectAddressViewModel = viewResultAddress.Model.ShouldBeType<SelectAddressViewModel>();

            AddressViewModel address = selectAddressViewModel.Addresses.FirstOrDefault();
            // ReSharper disable once PossibleNullReferenceException
            selectAddressViewModel.SelectedAddressId = address.Id;
            await lineCheckerController.SelectAddress(selectAddressViewModel);
            customerDetailsController.TransferYourNumber(model);
            ActionResult result = summaryController.Summary();

            // Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SummaryViewModel>();
            viewModel.HasCliNumber.ShouldEqual(expectedResult);
        }
    }
}