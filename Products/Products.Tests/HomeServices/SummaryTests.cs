namespace Products.Tests.HomeServices
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Helpers;
    using Model.Constants;
    using NUnit.Framework;
    using Products.Model.Common;
    using Products.Model.HomeServices;
    using Products.Tests.Common.Fakes;
    using Products.Tests.Common.Helpers;
    using Products.Web.Areas.HomeServices.Controllers;
    using Products.WebModel.Resources.Common;
    using Products.WebModel.Resources.HomeServices;
    using Products.WebModel.ViewModels.Common;
    using Products.WebModel.ViewModels.HomeServices;
    using Should;

    public class SummaryTests
    {
        [Test]
        public async Task ShouldRedirectToConfirmationPageWhenCustomerContinuesFromSummaryPage()
        {
            // Arrange
            HomeServicesCustomer customer = FakeCustomerFactory.GetHomeservicesCustomerWithExtras();
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, customer);
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultHomeServices();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<HomeServicesController>();

            var viewModel = new SummaryViewModel
            {
                IsTermsAndConditionsChecked = true
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = await controller.Summary(viewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("Confirmation");
        }

        [Test]
        public void ShouldPopulateResidentialCustomerDetailsWhenCustomerIsDisplayedTheSummaryPage()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var customer = new HomeServicesCustomer
            {
                DirectDebitDetails = new DirectDebitDetails
                {
                    DirectDebitPaymentDate = 1,
                    AccountName = "Mr Test",
                    AccountNumber = "12345678",
                    SortCode = "102030"
                },
                PersonalDetails = new PersonalDetails
                {
                    Title = "Mr",
                    FirstName = "Joe",
                    LastName = "Bloggs",
                    DateOfBirth = "01/01/1990"
                },
                ContactDetails = new ContactDetails
                {
                    ContactNumber = "02392123456",
                    EmailAddress = "t@t.com"
                },
                SelectedCoverAddress = new QasAddress
                {
                    AddressLine1 = "Forbury Place",
                    AddressLine2 = "Forbury",
                    County = "Berkshire",
                    HouseName = "1",
                    Town = "Reading"
                },
                CoverPostcode = "RG1 4JT",
                AvailableProduct = FakeHomeServicesProductStub.GetFakeProducts("BOBC"),
                SelectedExtraCodes = new List<string> { "EC" },
                SelectedProductCode = "BOBC",
                IsLandlord = false
            };
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, customer);

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            // Act
            ActionResult result = controller.Summary();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SummaryViewModel>();
            model.DirectDebitPaymentDay.ShouldEqual(customer.DirectDebitDetails.DirectDebitPaymentDate);
            model.DirectDebitAccountNumber.ShouldEqual(customer.DirectDebitDetails.AccountNumber);
            model.DirectDebitAccountName.ShouldEqual(customer.DirectDebitDetails.AccountName);
            model.DirectDebitSortCode.ShouldEqual(customer.DirectDebitDetails.SortCode);

            model.CustomerFormattedName.ShouldEqual(customer.PersonalDetails.FormattedName);
            model.DateOfBirth.ShouldEqual(customer.PersonalDetails.DateOfBirth);
            model.Address.ShouldEqual(customer.SelectedCoverAddress.FullAddress(customer.CoverPostcode));

            model.ContactNumber.ShouldEqual(customer.ContactDetails.ContactNumber);
            model.EmailAddress.ShouldEqual(customer.ContactDetails.EmailAddress);
            model.CoverAddress.ShouldEqual(string.Empty);
        }

        [Test]
        public void ShouldPopulateLandlordCustomerDetailsWhenCustomerIsDisplayedTheSummaryPage()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var customer = new HomeServicesCustomer
            {
                DirectDebitDetails = new DirectDebitDetails
                {
                    DirectDebitPaymentDate = 1,
                    AccountName = "Mr Test",
                    AccountNumber = "12345678",
                    SortCode = "102030"
                },
                PersonalDetails = new PersonalDetails
                {
                    Title = "Mr",
                    FirstName = "Joe",
                    LastName = "Bloggs",
                    DateOfBirth = "01/01/1990"
                },
                ContactDetails = new ContactDetails
                {
                    ContactNumber = "02392123456",
                    EmailAddress = "t@t.com"
                },
                SelectedCoverAddress = new QasAddress
                {
                    AddressLine1 = "Forbury Place",
                    AddressLine2 = "Forbury",
                    County = "Berkshire",
                    HouseName = "1",
                    Town = "Reading"
                },
                SelectedBillingAddress = new QasAddress
                {
                    AddressLine1 = "Forbury Place",
                    AddressLine2 = "Forbury",
                    County = "Berkshire",
                    HouseName = "2",
                    Town = "Reading"
                },
                CoverPostcode = "RG1 4JT",
                BillingPostcode = "RG1 4JX",
                AvailableProduct = FakeHomeServicesProductStub.GetFakeProducts("BOBC"),
                SelectedExtraCodes = new List<string> { "EC" },
                SelectedProductCode = "BOBC",
                IsLandlord = true
            };
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, customer);

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            // Act
            ActionResult result = controller.Summary();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SummaryViewModel>();
            model.DirectDebitPaymentDay.ShouldEqual(customer.DirectDebitDetails.DirectDebitPaymentDate);
            model.DirectDebitAccountNumber.ShouldEqual(customer.DirectDebitDetails.AccountNumber);
            model.DirectDebitAccountName.ShouldEqual(customer.DirectDebitDetails.AccountName);
            model.DirectDebitSortCode.ShouldEqual(customer.DirectDebitDetails.SortCode);

            model.CustomerFormattedName.ShouldEqual(customer.PersonalDetails.FormattedName);
            model.DateOfBirth.ShouldEqual(customer.PersonalDetails.DateOfBirth);
            model.Address.ShouldEqual(customer.SelectedBillingAddress.FullAddress(customer.BillingPostcode));
            model.CoverAddress.ShouldEqual(customer.SelectedCoverAddress.FullAddress(customer.CoverPostcode));

            model.ContactNumber.ShouldEqual(customer.ContactDetails.ContactNumber);
            model.EmailAddress.ShouldEqual(customer.ContactDetails.EmailAddress);
        }

        [Test]
        public void ShouldPopulateDirectDebitMandateWithCorrectBankAndCompanyInformation()
        {
            // Arrange
            var directDebitDetails = new DirectDebitDetails
            {
                DirectDebitPaymentDate = 1,
                AccountName = "Mr Test",
                AccountNumber = "12345678",
                SortCode = "102030"
            };

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer
            {
                DirectDebitDetails = directDebitDetails
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            // Act
            ActionResult result = controller.PrintMandate();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.ViewName.ShouldEqual("PrintMandate");
            var model = viewResult.Model.ShouldBeType<DirectDebitMandateViewModel>();
            model.CompanyName.ShouldEqual(DirectDebitMandate_Resources.CompanyNameHomeServices);
            model.ServiceUserNumber.ShouldEqual(DirectDebitMandate_Resources.ServiceUserNumberHomeServices);
            model.Name.ShouldEqual(directDebitDetails.AccountName);
            model.AccountNumber.ShouldEqual(directDebitDetails.AccountNumber);
            model.Sortcode.ShouldEqual(directDebitDetails.SortCode);
        }

        [Test]
        public async Task ShouldSendConfirmationEmailWhenCustomerConfirmsOrder()
        {
            // Arrange
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
               .WithHttpRequest(new FakeHttpRequest().WithWebRequest(new HttpCookieCollection()))
               .Build();

            HomeServicesCustomer customer = FakeCustomerFactory.GetHomeservicesCustomerWithExtras();
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, customer);

            var fakeEmailManager = new FakeEmailManager();

            var controller = new ControllerFactory()
                .WithEmailManager(fakeEmailManager)
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            // Act
            await controller.Summary(new SummaryViewModel());

            // Assert
            fakeEmailManager.To.ShouldContain(customer.ContactDetails.EmailAddress);
            fakeEmailManager.Subject.ShouldContain("Welcome to SSE");
        }

        [Test]
        public async Task ShouldSendMembershipEmailWhenCustomerConfirmsOrder()
        {
            // Arrange
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
               .WithHttpRequest(new FakeHttpRequest().WithWebRequest(new HttpCookieCollection()))
               .Build();

            HomeServicesCustomer customer = FakeCustomerFactory.GetHomeservicesCustomerWithExtras();
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, customer);

            var fakeEmailManager = new FakeEmailManager();

            var controller = new ControllerFactory()
                .WithEmailManager(fakeEmailManager)
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            // Act
            await controller.Summary(new SummaryViewModel());

            // Assert
            fakeEmailManager.To.ShouldContain("ics.reporting@sse.com");
            fakeEmailManager.Subject.ShouldContain("Membership sign up details");
        }


        [TestCaseSource(nameof(CoverSummaryTestData))]
        public void ShouldPopulateCoverSummaryWithCorrectCoverInformation(string availableProductGroup, List<string> selectedExtraCodes, string selectedProductCode,
                                                                          string productName, string contractStartDate, string contractLength, string excessAmount, int extrasCount, bool hasOffers,
                                                                          string totalMonthlyCost, string totalYearlyCost, string coverMonthlyAmount, string offerParagraph, string offerHeader,
                                                                          string totalYearlyExtraCost, string totalYearlyProductCost, string monthlyExtraCost
                                                                          )
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer 
            {
                ContactDetails =  new ContactDetails(),
                DirectDebitDetails = new DirectDebitDetails(),
                PersonalDetails =  new PersonalDetails(),
                SelectedCoverAddress = new QasAddress(),
                AvailableProduct = FakeHomeServicesProductStub.GetFakeProducts(availableProductGroup),
                SelectedExtraCodes = selectedExtraCodes,
                SelectedProductCode = selectedProductCode
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            // Act
            ActionResult result =  controller.Summary();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var summaryViewModel =  viewResult.Model.ShouldBeType<SummaryViewModel>();
            summaryViewModel.CoverSummaryViewModel.ShouldNotBeNull();
            summaryViewModel.CoverSummaryViewModel.ContractLength.ShouldEqual(contractLength);
            summaryViewModel.CoverSummaryViewModel.ContractStartDate.ShouldEqual(contractStartDate);
            summaryViewModel.CoverSummaryViewModel.CoverMonthlyPaymentAmount.ShouldEqual(coverMonthlyAmount);
            summaryViewModel.CoverSummaryViewModel.ExcessAmount.ShouldEqual(excessAmount);
            summaryViewModel.CoverSummaryViewModel.Extras.Count.ShouldEqual(extrasCount);
            summaryViewModel.CoverSummaryViewModel.HasOffers.ShouldEqual(hasOffers);
            summaryViewModel.CoverSummaryViewModel.OfferHeader.ShouldEqual(offerHeader);
            summaryViewModel.CoverSummaryViewModel.OfferParagraph.ShouldEqual(offerParagraph);
            summaryViewModel.CoverSummaryViewModel.YearlyTotalCost.ShouldEqual(totalYearlyCost);
            summaryViewModel.CoverSummaryViewModel.TotalYearlyProductCost.ShouldEqual(totalYearlyProductCost);
            summaryViewModel.CoverSummaryViewModel.SelectedProductName.ShouldEqual(productName);
            summaryViewModel.CoverSummaryViewModel.TotalMonthlyCost.ShouldEqual(totalMonthlyCost);
            if (summaryViewModel.CoverSummaryViewModel.Extras.Count > 0)
            {
                summaryViewModel.CoverSummaryViewModel.Extras?.First().ExtraMonthlyCost.ShouldEqual(monthlyExtraCost);
                summaryViewModel.CoverSummaryViewModel.Extras?.First().ExtraYearlyCost.ShouldEqual(totalYearlyExtraCost);
            }
        }

        [TestCaseSource(nameof(CoverSummaryExtraTestData))]
        public void ShouldPopulateCoverSummaryExtrasWithCorrectCoverInformation(string availableProductGroup, List<string> selectedExtraCodes, string selectedProductCode, int extrasCount,
                                                                    string extra1, string extra1Amount, string extra2, string extra2Amount, string totalMonthlyCost, 
                                                                    string projectedYearlyCosts, string coverMonthlyAmount)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer
            {
                ContactDetails = new ContactDetails(),
                DirectDebitDetails = new DirectDebitDetails(),
                PersonalDetails = new PersonalDetails(),
                SelectedCoverAddress = new QasAddress(),
                AvailableProduct = FakeHomeServicesProductStub.GetFakeProducts(availableProductGroup),
                SelectedExtraCodes = selectedExtraCodes,
                SelectedProductCode = selectedProductCode
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            // Act
            ActionResult result = controller.Summary();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var summaryViewModel = viewResult.Model.ShouldBeType<SummaryViewModel>();
            summaryViewModel.CoverSummaryViewModel.Extras.Count.ShouldEqual(extrasCount);
            summaryViewModel.CoverSummaryViewModel.Extras.First(e => e.ExtraName == extra1).ExtraMonthlyCost.ShouldEqual(extra1Amount);
            summaryViewModel.CoverSummaryViewModel.YearlyTotalCost.ShouldEqual(projectedYearlyCosts);
            summaryViewModel.CoverSummaryViewModel.TotalMonthlyCost.ShouldEqual(totalMonthlyCost);
        }

        [TestCase("BC", "EC", 3, 1)]
        [TestCase("BC50", "EC", 3, 1)]
        [TestCase("HC", "EC", 3, 1)]
        [TestCase("HC50", "EC", 3, 1)]
        [TestCase("LANDBC", "EC", 2, 1)]
        [TestCase("LANDHC", "EC", 2, 1)]
        [TestCase("BOBC", "EC", 2, 1)]
        [TestCase("BOHC", "EC", 2, 1)]
        public void ShouldSetProductPdfUrlsBasedOnSelectedProductAndExtras(string selectedProductCode, string selectedProductExtraCode, int noOfProductPDFs, int noOfExtrasPDFs)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var customer = new HomeServicesCustomer
            {
                DirectDebitDetails = new DirectDebitDetails
                {
                    DirectDebitPaymentDate = 1,
                    AccountName = "Mr Test",
                    AccountNumber = "12345678",
                    SortCode = "102030"
                },
                PersonalDetails = new PersonalDetails
                {
                    Title = "Mr",
                    FirstName = "Joe",
                    LastName = "Bloggs",
                    DateOfBirth = "01/01/1990"
                },
                ContactDetails = new ContactDetails
                {
                    ContactNumber = "02392123456",
                    EmailAddress = "t@t.com"
                },
               SelectedCoverAddress = new QasAddress
                {
                    AddressLine1 = "Forbury Place",
                    AddressLine2 = "Forbury",
                    County = "Berkshire",
                    HouseName = "1",
                    Town = "Reading"
                },
                CoverPostcode = "RG1 4JT",
                AvailableProduct = FakeHomeServicesProductStub.GetFakeProducts(selectedProductCode),
                SelectedExtraCodes = new List<string> { selectedProductExtraCode },
                SelectedProductCode = selectedProductCode
            };
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, customer);
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultHomeServices();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<HomeServicesController>();

            // Act
            ActionResult result = controller.Summary();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SummaryViewModel>();

            model.AccordionViewModel.ProductPDFs.Count.ShouldEqual(noOfProductPDFs);
            model.AccordionViewModel.ExtraProductPDFs.Count.ShouldEqual(noOfExtrasPDFs);
        }

        private static IEnumerable<TestCaseData> CoverSummaryTestData()
        {
            yield return new TestCaseData("BOBC", new List<string> { "EC" }, "BOBC", "Boiler cover", Summary_Resources.ContractStartDate, "12 months", "£90",
                                                                            1, true, "£30.00", "£358.00", "£20.00", "If this applies to you, your gas account will be credited in the next 46 calendar days of the cancellation period being complete."
                                                                            , "with £0 excess SSE gas customers get £55 credit on their account", "£120", "£238.00", "£10");
            yield return new TestCaseData("BOBC", new List<string> { "EC" }, "BOBC", "Boiler cover", Summary_Resources.ContractStartDate, "12 months", "£90",
                1, true, "£30.00", "£358.00", "£20.00", "If this applies to you, your gas account will be credited in the next 46 calendar days of the cancellation period being complete."
                , "with £0 excess SSE gas customers get £55 credit on their account", "£120", "£238.00", "£10");
        }

        private static IEnumerable<TestCaseData> CoverSummaryExtraTestData()
        {
            yield return new TestCaseData("BOBC", new List<string> { "EC", "EC" }, "BOBC", 1, "Extra1", "£10", "Extra2", "£15", "£30.00", "£358.00", "£20.00");
        }
    }
}
