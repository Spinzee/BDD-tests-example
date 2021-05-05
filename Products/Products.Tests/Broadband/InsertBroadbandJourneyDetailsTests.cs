namespace Products.Tests.Broadband
{
    using System;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Helpers;
    using Model;
    using NUnit.Framework;
    using Products.Model.Broadband;
    using Products.Service.Broadband.Mappers;
    using Products.Tests.Broadband.Fakes.Repository;
    using Products.Tests.Common.Fakes;
    using Products.Tests.Common.Helpers;
    using Products.Web.Areas.Broadband.Controllers;
    using Products.WebModel.ViewModels.Broadband;
    using Products.WebModel.ViewModels.Common;
    using Should;
    using FakeSessionManager = Fakes.Services.FakeSessionManager;

    [TestFixture]
    public class InsertBroadbandJourneyDetailsTests
    {
        [TestCase("affiliateCode1", "1410789843095", "US00CI", "If migrateAffiliateid and migrateCampaignid are not null check override affiliate flag is true")]
        [TestCase("", "1410789777092", "AV26CI", "If no migrateAffiliateid and migrateCampaignid are not null check override affiliate flag is false")]
        [TestCase("affiliateCode1", "1410789777025", "WADXCI", "If migrateAffiliateid and migrateCampaignid are not null check override affiliate flag is false")]
        [TestCase("", "1410789843132", "MM00CI", "If no migrateAffiliateid and valid migrateCampaignid ignore override affiliate flag and display masuscode")]
        [TestCase("", "123456789", "WWQTCI", "If no migrateAffiliateid and invalid migrateCampaignid ignore override affiliate flag and display speculative masuscode")]
        [TestCase("affiliateCode1", "", "WADXCI", "If migrateAffiliateid and no migrateCampaignid ignore override affiliate flag and display affiliate campaign code")]
        [TestCase("", "", "WWQTCI", "If no migrateAffiliateid and no migrateCampaignid display speculative masuscode")]
        [TestCase(null, null, "WWQTCI", "If no migrateAffiliateid and no migrateCampaignid display speculative masuscode")]
        public void ShouldInsertAllDetailsIntoRedevProjDbWhenUserCompletesJourney(string migrateAffiliateid, string migrateCampaignid, string expectedValue, string description)
        {
            // Arrange
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
            fakeConfigManager.AddConfiguration("campaignManagement", "campaignCodesOverrideAffiliateYes", "1410789843095", "US00CI");
            fakeConfigManager.AddConfiguration("campaignManagement", "campaignCodesOverrideAffiliateYes", "1410790805950", "AV4567");
            fakeConfigManager.AddConfiguration("campaignManagement", "campaignCodesOverrideAffiliateYes", "1410790805907", "NE00CI");
            fakeConfigManager.AddConfiguration("campaignManagement", "campaignCodesOverrideAffiliateYes", "1410789843132", "MM00CI");
            fakeConfigManager.AddConfiguration("campaignManagement", "campaignCodesOverrideAffiliateYes", "1410790974558", "US00CI");
            fakeConfigManager.AddConfiguration("campaignManagement", "campaignCodesOverrideAffiliateNo", "1410789777025", "AV26CI");
            fakeConfigManager.AddConfiguration("campaignManagement", "campaignCodesOverrideAffiliateNo", "1410789777072", "US00CI");
            fakeConfigManager.AddConfiguration("campaignManagement", "campaignCodesOverrideAffiliateNo", "1410789777092", "AV26CI");
            fakeConfigManager.AddConfiguration("campaignManagement", "campaignCodesOverrideAffiliateNo", "1410789777111", "AV26CI");

            var cookieCollection = new HttpCookieCollection { new HttpCookie("migrateMemberid", "123456"), new HttpCookie("migrateAffiliateid", migrateAffiliateid), new HttpCookie("migrateCampaignid", migrateCampaignid) };

            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(cookieCollection))
                .Build();            
            
            var fakeSalesRepository = new FakeBroadbandSalesRepository();

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithConfigManager(fakeConfigManager)
                .WithContextManager(fakeContextManager)
                .WithSalesRepository(fakeSalesRepository);

            var lineCheckerController = controllerFactory.Build<LineCheckerController>();
            var packageController = controllerFactory.Build<PackagesController>();
            var customerDetailsController = controllerFactory.Build<CustomerDetailsController>();
            var bankDetailsController = controllerFactory.Build<BankDetailsController>();
            var summaryController = controllerFactory.Build<SummaryController>();

            // Act        
            // ReSharper disable once UnusedVariable
            ActionResult checkerResult = lineCheckerController.LineChecker("BB_ANY19").Result;
            // ReSharper disable once UnusedVariable
            ActionResult submitResult = lineCheckerController.Submit(new LineCheckerViewModel { PhoneNumber = "0208 999 8888", ProductCode = "BB_ANY19", PostCode = "PO9 1QH" }).Result;
            // ReSharper disable once UnusedVariable
            ActionResult selectResult = lineCheckerController.SelectAddress().Result;
            // ReSharper disable once UnusedVariable
            ActionResult select2Result = lineCheckerController.SelectAddress(new SelectAddressViewModel { SelectedAddressId = 1 }).Result;
            packageController.SelectedPackage(new SelectedPackageViewModel(), "FIBRE_LRO19");
            customerDetailsController.Submit(new PersonalDetailsViewModel { DateOfBirthDay = "01", DateOfBirthMonth = "02", DateOfBirthYear = "1980", DateOfBirth = "01/02/1980", FirstName = "Jane", LastName = "Blogs", Titles = Titles.Ms });
            // ReSharper disable once UnusedVariable
            ActionResult submit3Result = customerDetailsController.SubmitContactDetails(new ContactDetailsViewModel { EmailAddress = "test@test.com", ContactNumber = "07987 678 678", IsMarketingConsentChecked = true }).Result;
            var bankDetailsViewModel = new BankDetailsViewModel { AccountHolder = "Test Account Holder", AccountNumber = "87654321", SortCode = "112233" };
            bankDetailsController.Submit(bankDetailsViewModel);
            // ReSharper disable once UnusedVariable
            ActionResult submit4Result = summaryController.Submit(new SummaryViewModel { IsTermsAndConditionsChecked = true }).Result;

            // Assert
            fakeSalesRepository.InsertSaleCount.ShouldEqual(1);
            fakeSalesRepository.InsertSaleSqlParameters["AccountName"].ShouldEqual("Gxdl8oLqV1SAgvKzxHusSzVnS4WxX3WO");
            fakeSalesRepository.InsertSaleSqlParameters["AccountNumber"].ShouldEqual("Id2R52yL3sgMxiGka21ycA==");
            fakeSalesRepository.InsertSaleSqlParameters["SortCode"].ShouldEqual("CWiVdW8Aqx4=");
            fakeSalesRepository.InsertSaleSqlParameters["AddressLine1"].ShouldEqual("Waterloo Road");
            fakeSalesRepository.InsertSaleSqlParameters["AddressLine2"].ShouldEqual(null);
            fakeSalesRepository.InsertSaleSqlParameters["AddressLine3"].ShouldEqual(null);
            fakeSalesRepository.InsertSaleSqlParameters["DateOfBirth"].ShouldEqual("01/02/1980 00:00:00");
            fakeSalesRepository.InsertSaleSqlParameters["DayPhone"].ShouldEqual("02089998888");
            fakeSalesRepository.InsertSaleSqlParameters["Email"].ShouldEqual("test@test.com");
            fakeSalesRepository.InsertSaleSqlParameters["FirstName"].ShouldEqual("Jane");
            fakeSalesRepository.InsertSaleSqlParameters["HouseName"].ShouldEqual(null);
            fakeSalesRepository.InsertSaleSqlParameters["HouseNumber"].ShouldEqual("21");
            fakeSalesRepository.InsertSaleSqlParameters["LineSpeed"].ShouldEqual("0");
            fakeSalesRepository.InsertSaleSqlParameters["MarketingConsent"].ShouldEqual("True");
            fakeSalesRepository.InsertSaleSqlParameters["Mobile"].ShouldEqual("07987678678");
            fakeSalesRepository.InsertSaleSqlParameters["Postcode"].ShouldEqual("PO9 1BH");
            fakeSalesRepository.InsertSaleSqlParameters["ProductCode"].ShouldEqual("FIBRE_LRO19");
            fakeSalesRepository.InsertSaleSqlParameters["Surname"].ShouldEqual("Blogs");
            fakeSalesRepository.InsertSaleSqlParameters["Title"].ShouldEqual("Ms");
            fakeSalesRepository.InsertSaleSqlParameters["Town"].ShouldEqual("Havant");
            fakeSalesRepository.InsertSaleSqlParameters["CampaignCode"].ShouldEqual(expectedValue);
            fakeSalesRepository.InsertSaleSqlParameters["CsUserGUID"].ShouldEqual("fa4aa87a-bc91-4481-a29e-92609a0aefd4");

            fakeSalesRepository.InsertOpenreachAuditCount.ShouldEqual(1);
        }

        [Test]
        public async Task ShouldInsertMasusReferenceIntoProfilesDbAfterSuccessfulSale()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails(),
                ListOfProductsSessionObject = FakeBroadbandProductsData.GetListOfAvailableProductsFromSession(),
                OpenReachSessionObject = FakeOpenReachData.GetOpenReachData()
            };

            var fakeSalesRepository = new FakeBroadbandSalesRepository();

            var cookieCollection = new HttpCookieCollection { new HttpCookie("migrateMemberid", "123456"), new HttpCookie("migrateAffiliateid", "affiliateCode1"), new HttpCookie("migrateCampaignid", "1410789843095") };

            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(cookieCollection))
                .Build();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithSalesRepository(fakeSalesRepository)
                .WithContextManager(fakeContextManager)
                .Build<SummaryController>();

            // Act
            await controller.Submit(new SummaryViewModel { IsTermsAndConditionsChecked = true });

            // Assert
            fakeSalesRepository.InsertMasusReferenceCount.ShouldEqual(1);

            // TODO Look at how we can assert the correct parameters are being pass to the DB 
            fakeSalesRepository.InsertOpenreachAuditCount.ShouldEqual(1);
        }

        [Test]
        public async Task ShouldUpdateLastUserSuccessfulSale()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails(),
                ListOfProductsSessionObject = FakeBroadbandProductsData.GetListOfAvailableProductsFromSession(),
                OpenReachSessionObject = FakeOpenReachData.GetOpenReachData(),
                YourPriceSessionObject = new YourPriceViewModel()
            };

            var fakeSalesRepository = new FakeBroadbandSalesRepository();

            var cookieCollection = new HttpCookieCollection { new HttpCookie("migrateMemberid", "123456"), new HttpCookie("migrateAffiliateid", "affiliateCode1"), new HttpCookie("migrateCampaignid", "1410789843095") };

            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(cookieCollection))
                .Build();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithSalesRepository(fakeSalesRepository)
                .WithContextManager(fakeContextManager)
                .Build<SummaryController>();

            // Act
            await controller.Submit(new SummaryViewModel { IsTermsAndConditionsChecked = true });

            // Assert
            fakeSalesRepository.UpdateLastUserCount.ShouldEqual(1);
            fakeSalesRepository.UpdateLastUserParameters["UserId"].ShouldEqual("bf1d2e09-0d4a-4265-a6fa-d3d06446aba5");
            fakeSalesRepository.InsertOpenreachAuditCount.ShouldEqual(1);
        }

        [Test]
        public void ShouldRedirectToErrorAndLogErrorMessageWhenExceptionThrownByDatabase()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager { SessionObject = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails() };
            var fakeSalesRepository = new FakeBroadbandSalesRepository { Exception = new Exception("Database Exception Error") };

            var cookieCollection = new HttpCookieCollection { new HttpCookie("migrateMemberid", "123456"), new HttpCookie("migrateAffiliateid", "affiliateCode1"), new HttpCookie("migrateCampaignid", "1410789843095") };

            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(cookieCollection))
                .Build();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithSalesRepository(fakeSalesRepository)
                .WithContextManager(fakeContextManager)
                .Build<SummaryController>();

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => controller.Submit(new SummaryViewModel { IsTermsAndConditionsChecked = true }));
            ex.Message.ShouldNotBeNull();
            ex.Message.ShouldNotBeEmpty();
        }

        [Test]
        public void ShouldRedirectToErrorAndLogErrorMessageWhenProductCodeIsInvalid()
        {
            // Arrange
            BroadbandJourneyDetails broadbandJourneyDetails = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails();

            string errorMessage = $"GetSubProductByLoyalty returned no results for product code: {broadbandJourneyDetails.Customer.SelectedProductCode}";
            var fakeSalesRepository = new FakeBroadbandSalesRepository
            {
                ArgumentException = new ArgumentException(errorMessage)
            };

            var fakeSessionManager = new FakeSessionManager { SessionObject = broadbandJourneyDetails, OpenReachSessionObject = FakeOpenReachData.GetOpenReachData() };

            var cookieCollection = new HttpCookieCollection { new HttpCookie("migrateMemberid", "123456"), new HttpCookie("migrateAffiliateid", "affiliateCode1"), new HttpCookie("migrateCampaignid", "1410789843095") };

            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(cookieCollection))
                .Build();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithSalesRepository(fakeSalesRepository)
                .WithContextManager(fakeContextManager)
                .Build<SummaryController>();

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(
                () => controller.Submit(new SummaryViewModel { IsTermsAndConditionsChecked = true }));
            ex.Message.ShouldNotBeNull();
            ex.Message.ShouldEqual(errorMessage);
        }

        [TestCase(true, "60", BroadbandProductGroup.None)]
        [TestCase(false, "0", BroadbandProductGroup.None)]
        [TestCase(true, "60", BroadbandProductGroup.FixAndFibreV3)]
        [TestCase(false, "0", BroadbandProductGroup.FixAndFibreV3)]
        public async Task ShouldInsertAuditEntryIntoDbWhenApplicationIsSuccessfullySaved(bool installLine, string installationCharge, BroadbandProductGroup selectedBroadbandProductGroup)
        {
            // Arrange
            var fakeSalesRepository = new FakeBroadbandSalesRepository();
            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("BroadbandSignupEncryptPublicKey", "asd");
            fakeConfigManager.AddConfiguration("MembershipEmailTo", "asd");
            fakeConfigManager.AddConfiguration("EmailBaseUrl", "asd");
            fakeConfigManager.AddConfiguration("Surcharge", "");
            fakeConfigManager.AddConfiguration("ConnectionFee", "");
            fakeConfigManager.AddConfiguration("InstallationFee", installationCharge);

            BroadbandJourneyDetails completeBroadbandJourneyDetails = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails();

            completeBroadbandJourneyDetails.Customer.ApplyInstallationFee = installLine;

            completeBroadbandJourneyDetails.Customer.SelectedProductGroup = selectedBroadbandProductGroup;

            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = completeBroadbandJourneyDetails,
                ListOfProductsSessionObject = FakeBroadbandProductsData.GetListOfAvailableProductsFromSession(),
                OpenReachSessionObject = FakeOpenReachData.GetOpenReachData(),
                YourPriceSessionObject = YourPriceViewModelMapper.MapCustomerToYourPriceViewModel(completeBroadbandJourneyDetails.Customer, 0, 0, Convert.ToDouble(installationCharge))
            };

            var cookieCollection = new HttpCookieCollection { new HttpCookie("migrateMemberid", "123456"), new HttpCookie("migrateAffiliateid", "affiliateCode1"), new HttpCookie("migrateCampaignid", "1410789843095") };

            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(cookieCollection))
                .Build();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithSalesRepository(fakeSalesRepository)
                .WithConfigManager(fakeConfigManager)
                .WithContextManager(fakeContextManager)
                .Build<SummaryController>();


            // Act
            ActionResult result = await controller.Submit(new SummaryViewModel { IsTermsAndConditionsChecked = true });

            // Assert
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();

            redirectResult.RouteValues["action"].ShouldEqual("Confirmation");

            fakeSalesRepository.InsertAuditSqlParameters["CLIProvided"].ShouldEqual("True");
            fakeSalesRepository.InsertAuditSqlParameters["MonthlyDDPrice"].ShouldEqual("21");
            fakeSalesRepository.InsertAuditSqlParameters["ProductCode"].ShouldEqual("FIBRE18_LR");
            fakeSalesRepository.InsertAuditSqlParameters["ConnectionCharge"].ShouldEqual("0");
            fakeSalesRepository.InsertAuditSqlParameters["InstallationCharge"].ShouldEqual(installationCharge);
            fakeSalesRepository.InsertAuditSqlParameters["IsSSECustomer"].ShouldEqual("True");
            fakeSalesRepository.InsertSaleCount.ShouldEqual(1);
            fakeSalesRepository.InsertAuditCount.ShouldEqual(1);
            fakeSalesRepository.InsertOpenreachAuditCount.ShouldEqual(1);
        }

        [Test]
        public async Task ShouldRedirectToConfirmationAndLogErrorWhenAuditEntryIsNotSaved()
        {
            // Arrange
            var fakeLogger = new FakeLogger();
            var fakeSalesRepository = new FakeBroadbandSalesRepository { AuditException = new Exception("Audit db connection failed") };
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails(),
                ListOfProductsSessionObject = FakeBroadbandProductsData.GetListOfAvailableProductsFromSession(),
                YourPriceSessionObject = new YourPriceViewModel()
            };

            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
            fakeConfigManager.AddConfiguration("PreLoginDomain", "aa");
            fakeConfigManager.AddConfiguration("EmailFromAddress", "aa");

            var cookieCollection = new HttpCookieCollection { new HttpCookie("migrateMemberid", "123456"), new HttpCookie("migrateAffiliateid", "affiliateCode1"), new HttpCookie("migrateCampaignid", "1410789843095") };

            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(cookieCollection))
                .Build();

            var controller = new ControllerFactory()
                .WithConfigManager(fakeConfigManager)
                .WithSessionManager(fakeSessionManager)
                .WithLogger(fakeLogger)
                .WithSalesRepository(fakeSalesRepository)
                .WithContextManager(fakeContextManager)
                .Build<SummaryController>();

            // Act
            ActionResult result = await controller.Submit(new SummaryViewModel { IsTermsAndConditionsChecked = true });

            // Assert
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();

            redirectResult.RouteValues["action"].ShouldEqual("Confirmation");
            fakeLogger.ErrorMessage.ShouldContain("Exception occured while inserting the audit record. Audit db connection failed");
        }

        [Test]
        public async Task ShouldInsertOpenreachAuditEntryIntoDbWhenApplicationIsSuccessfullySavedAndOpenReachBackOfficeFlagIsTrue()
        {
            // Arrange
            var fakeSalesRepository = new FakeBroadbandSalesRepository();
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails(),
                ListOfProductsSessionObject = FakeBroadbandProductsData.GetListOfAvailableProductsFromSession(),
                OpenReachSessionObject = FakeOpenReachData.GetOpenReachData()
            };

            var cookieCollection = new HttpCookieCollection { new HttpCookie("migrateMemberid", "123456"), new HttpCookie("migrateAffiliateid", "affiliateCode1"), new HttpCookie("migrateCampaignid", "1410789843095") };

            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(cookieCollection))
                .Build();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithSalesRepository(fakeSalesRepository)
                .WithContextManager(fakeContextManager)
                .Build<SummaryController>();


            // Act
            ActionResult result = await controller.Submit(new SummaryViewModel { IsTermsAndConditionsChecked = true });

            // Assert
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();

            redirectResult.RouteValues["action"].ShouldEqual("Confirmation");

            fakeSalesRepository.InsertOpenreachAuditSqlParameters["ApplicationId"].ShouldEqual("1265");
            fakeSalesRepository.InsertOpenreachAuditSqlParameters["CLI"].ShouldEqual("02081231234");
            fakeSalesRepository.InsertOpenreachAuditSqlParameters["LineStatus"].ShouldEqual("New Connection");
            fakeSalesRepository.InsertOpenreachAuditSqlParameters["AddressLineKey"].ShouldEqual("Ljk12345");

            fakeSalesRepository.InsertSaleCount.ShouldEqual(1);
            fakeSalesRepository.InsertOpenreachAuditCount.ShouldEqual(1);
        }

        [Test]
        public async Task ShouldRedirectToConfirmationAndLogErrorWhenOpenreachAuditEntryExceptionIsRaised()
        {
            // Arrange
            var fakeLogger = new FakeLogger();
            var fakeSalesRepository = new FakeBroadbandSalesRepository { OpenreachAuditException = new Exception("Openreach audit record insert failed") };

            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails(),
                ListOfProductsSessionObject = FakeBroadbandProductsData.GetListOfAvailableProductsFromSession(),
                OpenReachSessionObject = FakeOpenReachData.GetOpenReachData(),
                YourPriceSessionObject = new YourPriceViewModel()
            };

            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
            fakeConfigManager.AddConfiguration("PreLoginDomain", "aa");
            fakeConfigManager.AddConfiguration("EmailFromAddress", "aa");

            var cookieCollection = new HttpCookieCollection { new HttpCookie("migrateMemberid", "123456"), new HttpCookie("migrateAffiliateid", "affiliateCode1"), new HttpCookie("migrateCampaignid", "1410789843095") };

            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(cookieCollection))
                .Build();

            var controller = new ControllerFactory()
                .WithConfigManager(fakeConfigManager)
                .WithSessionManager(fakeSessionManager)
                .WithLogger(fakeLogger)
                .WithSalesRepository(fakeSalesRepository)
                .WithContextManager(fakeContextManager)
                .Build<SummaryController>();

            // Act
            ActionResult result = await controller.Submit(new SummaryViewModel { IsTermsAndConditionsChecked = true });

            // Assert
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();

            redirectResult.RouteValues["action"].ShouldEqual("Confirmation");
            fakeLogger.ErrorMessage.ShouldContain("Exception occured while inserting the openreach audit record for back office file Application Id - 1265. Openreach audit record insert failed");
        }

        [Test]
        public async Task ShouldNotInsertOpenreachAuditEntryIntoDbWhenApplicationIsSuccessfullySavedAndOpenReachBackOfficeFlagIsFalse()
        {
            // Arrange
            var fakeSalesRepository = new FakeBroadbandSalesRepository();
            BroadbandJourneyDetails sessionObject = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails();
            OpenReachData openReachData = FakeOpenReachData.GetOpenReachData();
            openReachData.LineavailabilityFlags.BackOfficeFile = false;

            var fakeSessionManager = new FakeSessionManager
            {

                ListOfProductsSessionObject = FakeBroadbandProductsData.GetListOfAvailableProductsFromSession(),
                SessionObject = sessionObject,
                OpenReachSessionObject = openReachData
            };

            var cookieCollection = new HttpCookieCollection { new HttpCookie("migrateMemberid", "123456"), new HttpCookie("migrateAffiliateid", "affiliateCode1"), new HttpCookie("migrateCampaignid", "1410789843095") };

            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(cookieCollection))
                .Build();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithSalesRepository(fakeSalesRepository)
                .WithContextManager(fakeContextManager)
                .Build<SummaryController>();


            // Act
            ActionResult result = await controller.Submit(new SummaryViewModel { IsTermsAndConditionsChecked = true });

            // Assert
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();

            redirectResult.RouteValues["action"].ShouldEqual("Confirmation");


            fakeSalesRepository.InsertSaleCount.ShouldEqual(1);
            fakeSalesRepository.InsertOpenreachAuditCount.ShouldEqual(0);
        }

        [Test]
        public async Task ShouldInsertAppropriateValueInDbWhenCookie()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails(),
                ListOfProductsSessionObject = FakeBroadbandProductsData.GetListOfAvailableProductsFromSession(),
                OpenReachSessionObject = FakeOpenReachData.GetOpenReachData()
            };

            var fakeSalesRepository = new FakeBroadbandSalesRepository();

            var cookieCollection = new HttpCookieCollection();

            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(cookieCollection))
                .Build();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithSalesRepository(fakeSalesRepository)
                .WithContextManager(fakeContextManager)
                .Build<SummaryController>();

            // Act
            await controller.Submit(new SummaryViewModel { IsTermsAndConditionsChecked = true });

            // Assert
            var broadbandJourneyDetails = fakeSessionManager.GetSessionDetails<BroadbandJourneyDetails>("broadband_journey");
            broadbandJourneyDetails.Customer.MigrateAffiliateId.ShouldBeEmpty();
            broadbandJourneyDetails.Customer.MigrateCampaignId.ShouldBeEmpty();
            broadbandJourneyDetails.Customer.MembershipId.ShouldBeNull();
        }
    }
}
