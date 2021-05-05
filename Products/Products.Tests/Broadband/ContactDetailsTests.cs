namespace Products.Tests.Broadband
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Helpers;
    using NUnit.Framework;
    using Products.Infrastructure;
    using Products.Model.Broadband;
    using Products.Tests.Common.Fakes;
    using Products.Tests.Common.Helpers;
    using Products.Web.Areas.Broadband.Controllers;
    using Products.WebModel.ViewModels.Broadband;
    using Should;
    using FakeSessionManager = Fakes.Services.FakeSessionManager;

    [TestFixture]
    public class ContactDetailsTests
    {
        [TestCase("07999999999", "Valid contact number, go to Bank details view")]
        [TestCase("0204-912-3451", "Valid contact number, go to Bank details view")]
        [TestCase("+44 0898 09876", "Valid contact number, go to Bank details view")]
        [TestCase("+44-(0898)-09876", "Valid contact number, go to Bank details view")]
        [TestCase("+44/0898/09876", "Valid contact number, go to Bank details view")]
        [TestCase("0204_912_3451", "Underscores allowed, go to Bank details view")]
        public async Task ShouldDisplayBankDetailsPageIfContactNumberIsValid(string contactNumber, string description)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory().WithSessionManager(fakeSessionManager).Build<CustomerDetailsController>();

            var model = new ContactDetailsViewModel
            {
                ContactNumber = contactNumber,
                EmailAddress = "Test@Test.com",
                ConfirmEmailAddress = "Test@Test.com",
                IsMarketingConsentChecked = true
            };

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = await controller.SubmitContactDetails(model);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult)result).RouteValues["action"].ShouldEqual("BankDetails");
        }

        [TestCase("112:3456789", 1, ": not allowed, stay on Contact details page")]
        [TestCase("123?91111116", 1, "? not allowed, stay on Contact details page view")]
        [TestCase("45678", 1, "invalid contact number, stay on Contact details page view")]
        [TestCase("1234567891111116", 1, "invalid contact number, stay on Contact details page view")]
        public async Task ShouldDisplayContactDetailsPageIfContactNumberIsNotValid(string contactNumber, int numberOfInlineErrors, string description)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };

            var controller = new ControllerFactory().WithSessionManager(fakeSessionManager).Build<CustomerDetailsController>();

            var model = new ContactDetailsViewModel
            {
                ContactNumber = contactNumber,
                EmailAddress = "Test@Test.com",
                ConfirmEmailAddress = "Test@Test.com",
                IsMarketingConsentChecked = true
            };

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = await controller.SubmitContactDetails(model);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<ViewResult>();
            controller.ModelState.Keys.Count.ShouldEqual(numberOfInlineErrors);
        }

        [TestCase("the_boss@Test1234.co.uk", 0, "BankDetails", "Valid email address, go to Bank details view")]
        [TestCase("bo__44@sse.dept.com", 0, "BankDetails", "Valid email address, go to Bank details view")]
        [TestCase("Test@Test.com", 0, "BankDetails", "Valid email address, go to Bank details view")]
        [TestCase("boss@sse.com", 0, "BankDetails", "Valid email address, go to Bank details view")]
        [TestCase("MYEMAIL@TEST.COM", 0, "BankDetails", "Valid email address, go to Bank details view")]
        public async Task ShouldDisplayBankDetailsPageIfEmailAddressIsValid(string emailAddress, int numberOfInlineErrors, string viewName, string description)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory().WithSessionManager(fakeSessionManager).Build<CustomerDetailsController>();

            var model = new ContactDetailsViewModel
            {
                ContactNumber = "07770 999 999",
                EmailAddress = emailAddress,
                ConfirmEmailAddress = emailAddress,
                IsMarketingConsentChecked = true
            };

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = await controller.SubmitContactDetails(model);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult)result).RouteValues["action"].ShouldEqual(viewName);
            controller.ModelState.Keys.Count.ShouldEqual(numberOfInlineErrors);
        }

        [Test]
        public void ShouldThrowExceptionWithErrorWhenContactNumberIsNull()
        {
            // Arrange
            var controller = new ControllerFactory().Build<CustomerDetailsController>();

            var contactDetailsViewModel = new ContactDetailsViewModel
            {
                EmailAddress = "test@test.com",
                ContactNumber = null,
                IsMarketingConsentChecked = false
            };

            // Act/Assert
            // ReSharper disable once UnusedVariable
            var ex = Assert.ThrowsAsync<ArgumentException>(() => controller.SubmitContactDetails(contactDetailsViewModel));
            //ex.Message.ShouldNotBeNull();
            //ex.Message.ShouldEqual("Exception occured with storing personal details");
        }

        [TestCase("45678", 2, "Invalid email address, stay on Contact details page view")]
        [TestCase("my name is", 2, "Invalid email address, stay on Contact details page view")]
        [TestCase("Bill at microsoft.com", 2, "Invalid email address, stay on Contact details page view")]
        [TestCase("", 2, "Invalid email address, stay on Contact details page view")]
        [TestCase(null, 2, "Invalid email address, stay on Contact details page view")]
        [TestCase("ohn@TEST.223COM", 2, "Invalid email address, stay on Contact details page view")]
        public async Task ShouldDisplayContactDetailsPageIfEmailAddressIsNotValid(string emailAddress, int numberOfInlineErrors, string description)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory().WithSessionManager(fakeSessionManager).Build<CustomerDetailsController>();

            var model = new ContactDetailsViewModel
            {
                ContactNumber = "07770 999 999",
                EmailAddress = emailAddress,
                ConfirmEmailAddress = emailAddress,
                IsMarketingConsentChecked = true
            };

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = await controller.SubmitContactDetails(model);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<ViewResult>();
            controller.ModelState.Keys.Count.ShouldEqual(numberOfInlineErrors);
        }

        [Test]
        public async Task ShouldDisplayBankDetailsPageIfEmailAddressAndConfirmEmailAddressAreTheSame()
        {
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };

            var controller = new ControllerFactory().WithSessionManager(fakeSessionManager).Build<CustomerDetailsController>();

            var model = new ContactDetailsViewModel
            {
                ContactNumber = "07770 999 999",
                EmailAddress = "the_boss@Test1234.co.uk",
                ConfirmEmailAddress = "the_boss@Test1234.co.uk",
                IsMarketingConsentChecked = true
            };

            controller.ValidateViewModel(model);

            ActionResult result = await controller.SubmitContactDetails(model);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult)result).RouteValues["action"].ShouldEqual("BankDetails");
        }

        [TestCase("Test@Test.com", "Test1@Test.com", 1, "Email addresses are not the same, stay on Contact details page view")]
        [TestCase("", "", 2, "Invalid email address, stay on Contact details page view")]
        [TestCase(null, null, 2, "Invalid email address, stay on Contact details page view")]
        public async Task ShouldDisplayContactDetailsPageIfEmailAddressAndConfirmEmailAddressAreNotSame(string emailAddress, string confirmEmailAddress, int numberOfInlineErrors, string description)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory().WithSessionManager(fakeSessionManager).Build<CustomerDetailsController>();

            var model = new ContactDetailsViewModel
            {
                ContactNumber = "07770 999 999",
                EmailAddress = emailAddress,
                ConfirmEmailAddress = confirmEmailAddress,
                IsMarketingConsentChecked = true
            };

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = await controller.SubmitContactDetails(model);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<ViewResult>();
            controller.ModelState.Keys.Count.ShouldEqual(numberOfInlineErrors);
        }

        [Test]
        public async Task ShouldStoreContactDetailsWithInSessionObject()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<CustomerDetailsController>();

            var contactDetailsViewModel = new ContactDetailsViewModel
            {
                EmailAddress = "test@test.com",
                ConfirmEmailAddress = "test@test.com",
                ContactNumber = "+(1111)111 1111",
                IsMarketingConsentChecked = false
            };

            // Act
            controller.ValidateViewModel(contactDetailsViewModel);
            ActionResult result = await controller.SubmitContactDetails(contactDetailsViewModel);

            ((RedirectToRouteResult)result).RouteValues["action"].ShouldEqual("BankDetails");

            var broadbandJourneyDetails = (BroadbandJourneyDetails)fakeSessionManager.SessionObject;

            // Assert
            broadbandJourneyDetails.Customer.ContactDetails.ContactNumber.ShouldEqual(NumberFormatter.ToDigitsOnly(contactDetailsViewModel.ContactNumber));
            broadbandJourneyDetails.Customer.ContactDetails.EmailAddress.ShouldEqual(contactDetailsViewModel.EmailAddress);
            broadbandJourneyDetails.Customer.ContactDetails.MarketingConsent.ShouldEqual(contactDetailsViewModel.IsMarketingConsentChecked);
        }

        [TestCase("", "", "BankDetails")]
        [TestCase("+(12345)111 111", "12345111111", "BankDetails")]
        [TestCase("+12345-111-999", "12345111999", "BankDetails")]
        [TestCase("+//(11-333)111 111!!", "11333111111", "BankDetails")]
        [TestCase("++--999  999  999 11111!!!", "99999999911111", "BankDetails")]
        [TestCase("   10000 111 222 ext 3443", "100001112223443", "BankDetails")]
        public async Task ShouldReturnNumericContactNumberInSessionObject(string contactNumber, string expectedContactNumber, string viewName)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
           
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<CustomerDetailsController>();

            var contactDetailsViewModel = new ContactDetailsViewModel
            {
                EmailAddress = "test@test.com",
                ContactNumber = contactNumber,
                IsMarketingConsentChecked = false
            };

            // Act
            ActionResult result = await controller.SubmitContactDetails(contactDetailsViewModel);

            ((RedirectToRouteResult)result).RouteValues["action"].ShouldEqual(viewName);

            var broadbandJourneyDetails = (BroadbandJourneyDetails)fakeSessionManager.SessionObject;

            // Assert
            broadbandJourneyDetails.Customer.ContactDetails.ContactNumber.ShouldEqual(expectedContactNumber);
        }

        //[Test]
        //public void ShouldThrowExceptionWithErrorWhenContactNumberIsNull()
        //{
        //    var fakeSession = new FakeSessionManager
        //    {
        //        SessionObject = new BroadbandJourneyDetails()
        //    };

        //    var contactDetailsViewModel = new ContactDetailsViewModel
        //    {
        //        EmailAddress = "test@test.com",
        //        ContactNumber = null,
        //        IsMarketingConsentChecked = false
        //    };

        //    var controller = new CustomerDetailsControllerFactory()
        //        .WithSessionService(fakeSession)
        //        .Build();

        //    var ex = Assert.Throws<Exception>(() => async controller.Submit(contactDetailsViewModel));
        //    ex.Message.ShouldNotBeNull();
        //    ex.Message.ShouldEqual("");
        //}

        [TestCase(true, "If SSE Customer display connection fee with strike through")]
        [TestCase(false, "If not SSE customer display connection fee")]
        public void ShouldDisplayConnectionFeeAndSurchargeWithStrikeThroughForSSECustomer(bool isSSECustomer, string description)
        {
            // Arrange
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

            // Act
            var packageController = controllerFactory.Build<PackagesController>();

            var controller = controllerFactory.Build<CustomerDetailsController>();

            packageController.SelectedPackage(new SelectedPackageViewModel(), "ABC");
            ActionResult result = controller.ContactDetails();

            // Assert
            var view = result.ShouldBeType<ViewResult>();
            var viewModel = view.ViewData.Model.ShouldBeType<ContactDetailsViewModel>();
            viewModel.YourPriceViewModel.IsExistingCustomer.ShouldEqual(isSSECustomer);
        }
    }
}
