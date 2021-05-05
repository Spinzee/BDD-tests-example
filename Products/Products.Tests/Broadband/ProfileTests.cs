namespace Products.Tests.Broadband
{
    using System;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Common.Fakes;
    using Common.Helpers;
    using Helpers;
    using Model;
    using NUnit.Framework;
    using Products.Model.Broadband;
    using Should;
    using Web.Areas.Broadband.Controllers;
    using WebModel.ViewModels.Broadband;
    using WebModel.ViewModels.Common;
    using FakeSessionManager = Fakes.Services.FakeSessionManager;

    [TestFixture]
    public class ProfileTests
    {
        [TestCase(false, "OnlineAccount", "00000000-0000-0000-0000-000000000000", "If profile does not exists, then go to create profile view")]
        [TestCase(true, "BankDetails", "fa4aa87a-bc91-4481-a29e-92609a0aefd4", "If profile does exist, skip create profile view, and store GUID in session")]
        public async Task ShouldCheckForAProfileWhenEmailAddressIsSubmitted(bool profileExists, string actionName, string guid, string description)
        {
            // Arrange            
            var fakeProfileRepository = new FakeProfileRepository { ProfileExists = profileExists };
            var fakeSessionManager = new FakeSessionManager { SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } } };
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithProfileRepository(fakeProfileRepository)
                .Build<CustomerDetailsController>();

            var model = new ContactDetailsViewModel
            {
                ContactNumber = "01212122122",
                EmailAddress = "Test@Test.com",
                ConfirmEmailAddress = "Test@Test.com",
                IsMarketingConsentChecked = true
            };

            // Act
            ActionResult result = await controller.SubmitContactDetails(model);

            // Assert
            fakeProfileRepository.CallToCheckProfileDB.ShouldEqual(1);
            ((RedirectToRouteResult) result).RouteValues["action"].ShouldEqual(actionName);
            var broadbandJourneyDetails = (BroadbandJourneyDetails) fakeSessionManager.SessionObject;
            broadbandJourneyDetails.Customer.UserId.ShouldEqual(new Guid(guid));
        }

        [Test]
        public async Task ShouldDisplayConfirmationPageIfTermsAndConditionsAreTicked()
        {
            var broadbandJourneyDetails = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails();
            broadbandJourneyDetails.Password = "TestPa33word";

            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = broadbandJourneyDetails
            };
            var cookieCollection = new HttpCookieCollection { new HttpCookie("migrateMemberid", "123456"), new HttpCookie("migrateAffiliateid", "affiliateCode1"), new HttpCookie("migrateCampaignid", "1410789843095") };

            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(cookieCollection))
                .Build();
            var fakeProfileRepository = new FakeProfileRepository { ProfileExists = false };

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithContextManager(fakeContextManager)
                .WithProfileRepository(fakeProfileRepository)
                .Build<SummaryController>();

            var model = new SummaryViewModel
            {
                IsTermsAndConditionsChecked = true
            };

            controller.ValidateViewModel(model);

            ActionResult result = await controller.Submit(model);

            fakeProfileRepository.InsertProfileSqlParameters["logOnName"].ShouldEqual("Test@test.com");
            fakeProfileRepository.InsertProfileSqlParameters["password"].Length.ShouldEqual(89);

            fakeProfileRepository.InsertAuditSqlParameters["userGuid"].ShouldEqual("fa4aa87a-bc91-4481-a29e-92609a0aefd4");
            fakeProfileRepository.InsertAuditSqlParameters["email"].ShouldEqual("Test@test.com");

            fakeProfileRepository.InsertIntoAuditDB.ShouldEqual(1);
            fakeProfileRepository.InsertIntoProfileDB.ShouldEqual(1);
        }
      

        [TestCase(null, "Step 3.1 of 5", "no cli was provided, final step count should be 5")]
        [TestCase("12345789", "Step 3.1 of 5", "cli was provided, final step count should be 5")]
        public async Task ShouldDisplayCorrectStepCountOnOnlineAccountPage(string cli, string stepCount, string description)
        {
            // Arrange
            var controllerFactory = new ControllerFactory();
            var fakeSessionManager = new FakeSessionManager { SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } } };
            var lineCheckerController = controllerFactory.WithSessionManager(fakeSessionManager).Build<LineCheckerController>();

            var controller = controllerFactory.Build<OnlineAccountController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO9 1QH",
                PhoneNumber = cli
            };

            // Act
            await lineCheckerController.Submit(lineCheckerViewModel);
            ActionResult result = controller.OnlineAccount();

            // Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            (viewResult.ViewBag.StepCounter as string).ShouldEqual(stepCount);
        }

        [TestCase(null, "Step 3.1 of 5", "no cli was provided, final step count should be 5")]
        [TestCase("12345789", "Step 3.1 of 5", "cli was provided, final step count should be 5")]
        public async Task ShouldDisplayCorrectStepCountOnOnlineAccountPageIfModelIsNotValid(string cli, string stepCount, string description)
        {
            // Arrange
            var controllerFactory = new ControllerFactory();
            var fakeSessionManager = new FakeSessionManager { SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } } };

            var lineCheckerController = controllerFactory.WithSessionManager(fakeSessionManager).Build<LineCheckerController>();

            var controller = controllerFactory.Build<OnlineAccountController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO9 1QH",
                PhoneNumber = cli
            };

            var model = new OnlineAccountViewModel();

            // Act
            await lineCheckerController.Submit(lineCheckerViewModel);
            controller.ValidateViewModel(model);
            ActionResult result = controller.OnlineAccount(model);

            // Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            (viewResult.ViewBag.StepCounter as string).ShouldEqual(stepCount);
        }

        [TestCase("Seven77")]
        [TestCase("Fourteen141414")]
        [TestCase("yesUppercas3")]
        [TestCase("yESLOWERCAS3")]
        [TestCase("Special#")]
        [TestCase("Special@")]        
        [TestCase("Number1")]
        public void ShouldRedirectToBankDetailsIfPasswordIsValid(string password)
        {
            // Arrange
            var fakeProfileRepository = new FakeProfileRepository { ProfileExists = false };
            var fakeSessionManager = new FakeSessionManager { SessionObject = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails() };

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithProfileRepository(fakeProfileRepository)
                .Build<OnlineAccountController>();

            var model = new OnlineAccountViewModel { Password = password, ConfirmPassword = password };

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = controller.OnlineAccount(model);

            // Assert
            result.ShouldNotBeNull();
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("BankDetails");            
        }

        [Test]
        public async Task ShouldRedirectToDirectDebitDetailsAndLogErrorWhenFailedToInsertAuditRecord()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { PostcodeEntered = "PO9 1QH" } }
            };
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
            fakeConfigManager.AddConfiguration("MySseBaseUrl", "mytestsse");
            fakeConfigManager.AddConfiguration("EmailFromAddress", "devandtest.externalmail@sse.com");

            var fakeProfileRepository = new FakeProfileRepository { ProfileExists = false, AuditEventException = new Exception("") };
            var fakeLogger = new FakeLogger();

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .WithLogger(fakeLogger)
                .WithProfileRepository(fakeProfileRepository);

            var customerDetailsController = controllerFactory.Build<CustomerDetailsController>();

            var onlineProfileController = controllerFactory.Build<OnlineAccountController>();

            var model = new OnlineAccountViewModel
            {
                Password = "TestPa33word",
                ConfirmPassword = "TestPa33word"
            };

            // Act
            customerDetailsController.Submit(new PersonalDetailsViewModel { Titles = Titles.Ms, FirstName = "Jane", LastName = "Bloggs", DateOfBirthDay = "03", DateOfBirthMonth = "12", DateOfBirthYear = "1945", DateOfBirth = "03/12/1945" });
            await customerDetailsController.SubmitContactDetails(new ContactDetailsViewModel { EmailAddress = "Test@test.com", ContactNumber = "07989 987 987", IsMarketingConsentChecked = true });

            ActionResult result = onlineProfileController.OnlineAccount(model);

            // Assert
            result.ShouldNotBeNull();
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("BankDetails");            
        }

        [Test]
        public async Task ShouldRedirectToDirectDebitDetailsAndLogErrorWhenFailedToSendActivationEmail()
        {           
            // Arrange
            var broadbandJourneyDetails = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails();
            broadbandJourneyDetails.Password = "TestPa33word";

            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = broadbandJourneyDetails
            };

            var cookieCollection = new HttpCookieCollection { new HttpCookie("migrateMemberid", "123456"), new HttpCookie("migrateAffiliateid", "affiliateCode1"), new HttpCookie("migrateCampaignid", "1410789843095") };
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(cookieCollection))
                .Build();

            var fakeProfileRepository = new FakeProfileRepository { ProfileExists = false };
            var fakeLogger = new FakeLogger();
            var fakeEmailManager = new FakeEmailManager(new Exception(""));
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
            fakeConfigManager.AddConfiguration("MySseBaseUrl", "aa");
            fakeConfigManager.AddConfiguration("EmailFromAddress", "aa");

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithContextManager(fakeContextManager)
                .WithConfigManager(fakeConfigManager)
                .WithLogger(fakeLogger)
                .WithEmailManager(fakeEmailManager)
                .WithProfileRepository(fakeProfileRepository);

            var customerDetailsController = controllerFactory.Build<CustomerDetailsController>();
            var summaryController = controllerFactory.Build<SummaryController>();

            var model = new SummaryViewModel
            {
                IsTermsAndConditionsChecked = true
            };

            // Act
            customerDetailsController.Submit(new PersonalDetailsViewModel { Titles = Titles.Ms, FirstName = "Jane", LastName = "Bloggs", DateOfBirthDay = "03", DateOfBirthMonth = "12", DateOfBirthYear = "1945", DateOfBirth = "03/12/1945" });
            await customerDetailsController.SubmitContactDetails(new ContactDetailsViewModel { EmailAddress = "Test@test.com", ContactNumber = "07989 987 987", IsMarketingConsentChecked = true });

            ActionResult result = await summaryController.Submit(model);

            // Assert
            result.ShouldNotBeNull();
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("Confirmation");
            fakeLogger.ErrorMessage.ShouldContain("Exception occured attempting to membership email for broadband customer, email: Test@test.com.");
        }

        [TestCase("Six666")]
        [TestCase("Fifteen15151515")]
        [TestCase("Has Space3")]
        public void ShouldRemainOnCreateProfileWhenPasswordIsInvalid(string password)
        {
            // Arrange
            var fakeProfileRepository = new FakeProfileRepository { ProfileExists = false };
            var fakeSessionManager = new FakeSessionManager { SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } } };

            var controller = new ControllerFactory()
                .WithProfileRepository(fakeProfileRepository)
                .WithSessionManager(fakeSessionManager)
                .Build<OnlineAccountController>();

            var model = new OnlineAccountViewModel { Password = password };

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = controller.OnlineAccount(model);

            // Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.ViewName.ShouldEqual("~/Areas/Broadband/Views/OnlineAccount/OnlineAccount.cshtml");
            fakeProfileRepository.InsertIntoProfileDB.ShouldEqual(0);
            fakeProfileRepository.InsertIntoAuditDB.ShouldEqual(0);
        }

        [Test]
        public async Task ShouldSendActivationEmailWhenProfileIsCreated()
        {
            var broadbandJourneyDetails = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails();
            broadbandJourneyDetails.Password = "TestPa33word";

            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = broadbandJourneyDetails
            };

            var cookieCollection = new HttpCookieCollection { new HttpCookie("migrateMemberid", "123456"), new HttpCookie("migrateAffiliateid", "affiliateCode1"), new HttpCookie("migrateCampaignid", "1410789843095") };
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(cookieCollection))
                .Build();

            var fakeProfileRepository = new FakeProfileRepository { ProfileExists = false };
            var fakeLogger = new FakeLogger();
            var fakeEmailManager = new FakeEmailManager();
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
            fakeConfigManager.AddConfiguration("MySseBaseUrl", "mytestsse");
            fakeConfigManager.AddConfiguration("EmailFromAddress", "devandtest.externalmail@sse.com");

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithContextManager(fakeContextManager)
                .WithConfigManager(fakeConfigManager)
                .WithLogger(fakeLogger)
                .WithEmailManager(fakeEmailManager)
                .WithProfileRepository(fakeProfileRepository);

            var customerDetailsController = controllerFactory.Build<CustomerDetailsController>();
            var summaryController = controllerFactory.Build<SummaryController>();

            var model = new SummaryViewModel
            {
                IsTermsAndConditionsChecked = true
            };

            // Act
            customerDetailsController.Submit(new PersonalDetailsViewModel { Titles = Titles.Ms, FirstName = "Jane", LastName = "Bloggs", DateOfBirthDay = "03", DateOfBirthMonth = "12", DateOfBirthYear = "1945", DateOfBirth = "03/12/1945" });
            await customerDetailsController.SubmitContactDetails(new ContactDetailsViewModel { EmailAddress = "Test@test.com", ContactNumber = "07989 987 987", IsMarketingConsentChecked = true });

            ActionResult result = await summaryController.Submit(model);

            // Assert
            result.ShouldNotBeNull();
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("Confirmation");
            fakeEmailManager.Body.ShouldContain("Test@test.com");
        }
    }
}