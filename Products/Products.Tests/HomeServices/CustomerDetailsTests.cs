namespace Products.Tests.HomeServices
{
    using System.Collections.Generic;
    using System.Linq;
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
    using Products.WebModel.ViewModels.Common;
    using Products.WebModel.ViewModels.HomeServices;
    using Should;

    public class CustomerDetailsTests
    {
        [Test]
        public void ShouldRedirectToDirectDebitPageWhenCustomerEntersValidContactDetails()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            var contactDetailsViewModel = new ContactDetailsViewModel
            {
                EmailAddress = "a@a.com",
                ConfirmEmailAddress = "a@a.com",
                ContactNumber = "012121212121",
                IsMarketingConsentChecked = true
            };

            // Act
            controller.ValidateViewModel(contactDetailsViewModel);
            ActionResult result = controller.ContactDetails(contactDetailsViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("BankDetails");
        }

        [TestCaseSource(nameof(GetInvalidContactNumbers))]
        public void ShouldReturnToContactDetailsWhenContactNumberIsMissingOrInvalid(string contactNumber, string errorMessage)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            var viewModel = new ContactDetailsViewModel
            {
                ContactNumber = contactNumber,
                EmailAddress = "test@test.com",
                ConfirmEmailAddress = "test@test.com",
                IsMarketingConsentChecked = true
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = controller.ContactDetails(viewModel);

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<ContactDetailsViewModel>();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Keys.Count.ShouldEqual(1);
            controller.ModelState.Values.First().Errors.First().ErrorMessage.ShouldEqual(errorMessage);
        }

        [TestCaseSource(nameof(GetInvalidEmailAddresses))]
        public void ShouldReturnToContactDetailsWhenEmailAddressIsMissingInvalidOrDoesNotMatch(string emailAddress, string confirmEmailAddress, List<string> errorMessageList, int errorCount)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            var viewModel = new ContactDetailsViewModel
            {
                ContactNumber = "012345123456",
                EmailAddress = emailAddress,
                ConfirmEmailAddress = confirmEmailAddress,
                IsMarketingConsentChecked = true
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = controller.ContactDetails(viewModel);

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<ContactDetailsViewModel>();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Keys.Count.ShouldEqual(errorCount);

            foreach (string error in errorMessageList)
            {
                controller.ModelState.Values.SelectMany(modelState => modelState.Errors).FirstOrDefault(msg => msg.ErrorMessage == error).ShouldNotBeNull();
            }
        }

        [Test]
        public void ShouldPrePopulatePersonalDetailsWhenCustomerRevisitsThePage()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();

            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer
            {
                CoverPostcode = "PO91BH",
                PersonalDetails = new PersonalDetails
                {
                    DateOfBirth = "01/01/1000",
                    FirstName = "Joe",
                    LastName = "Bloggs",
                    Title = "Mr"
                }
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            // Act
            ActionResult result = controller.PersonalDetails();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<PersonalDetailsViewModel>();
            viewModel.DateOfBirth = "01/01/1000";
            viewModel.DateOfBirthDay = "01";
            viewModel.DateOfBirthMonth = "01";
            viewModel.DateOfBirthYear = "1000";
            viewModel.FirstName = "Joe";
            viewModel.LastName = "Bloggs";
            viewModel.Titles = Titles.Mr;
        }

        [Test]
        public void ShouldPrePopulateContactDetailsWhenCustomerRevisitsThePage()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();

            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer
            {
                ContactDetails = new ContactDetails
                {
                    ContactNumber = "0488515201",
                    EmailAddress = "t@t.com",
                    MarketingConsent = true
                }
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            // Act
            ActionResult result = controller.ContactDetails();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<ContactDetailsViewModel>();
            viewModel.EmailAddress = "t@t.com";
            viewModel.ContactNumber = "0488515201";
            viewModel.IsMarketingConsentChecked = true;
        }

        private static IEnumerable<TestCaseData> GetInvalidContactNumbers()
        {
            yield return new TestCaseData("", Form_Resources.PreferredPhoneNumberRequiredError);
            yield return new TestCaseData("012345", Form_Resources.PreferredPhoneNumberRegexErrorMessage);
            yield return new TestCaseData("0123456789012345", Form_Resources.PreferredPhoneNumberRegexErrorMessage);
        }

        private static IEnumerable<TestCaseData> GetInvalidEmailAddresses()
        {
            yield return new TestCaseData("", "test@test.com", new List<string> { "Please enter your email address.", "Please confirm your email address. Addresses must match." }, 2);
            yield return new TestCaseData("test@test.com", "", new List<string> { "Please confirm your email address." }, 1);
            yield return new TestCaseData("test@test.com", "test1@test.com", new List<string> { "Please confirm your email address. Addresses must match." }, 1);
            yield return new TestCaseData("a@.com", "a@.com", new List<string> { "Please enter a valid email address.", "Please enter a valid email address." }, 2);
            yield return new TestCaseData("123456789012345678901234@123456789012345678901234.com", "123456789012345678901234@123456789012345678901234.com", new List<string> { "Please enter a valid email address.", "Please enter a valid email address." }, 2);
        }
    }
}
