namespace Products.Tests.HomeServices
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using Helpers;
    using Model.Constants;
    using NUnit.Framework;
    using Products.Model.HomeServices;
    using Products.Tests.Common.Fakes;
    using Products.Tests.Common.Helpers;
    using Products.Web.Areas.HomeServices.Controllers;
    using Products.WebModel.Resources.Common;
    using Products.WebModel.ViewModels.Common;
    using Products.WebModel.ViewModels.HomeServices;
    using Should;

    public class PersonalDetailsTests
    {
        [Test]
        public void ShouldRedirectToSelectAddressPageWhenCustomerEnteredValidPersonalDetails()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer { CoverPostcode = "PO91BH"});

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            var personalDetailsViewModel = new PersonalDetailsViewModel
            {
                Titles = Titles.Dr,
                FirstName = "Test",
                LastName = "Test",
                DateOfBirth = "01/01/1990",
                DateOfBirthDay = "01",
                DateOfBirthMonth = "01",
                DateOfBirthYear = "1990"
            };

            // Act
            controller.ValidateViewModel(personalDetailsViewModel);
            ActionResult result = controller.PersonalDetails(personalDetailsViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("SelectAddress");
        }

        [Test]
        public void ShouldReturnToPersonalDetailsPageWhenPersonalDetailsAreInvalid()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer { CoverPostcode = "PO91BH" });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            var personalDetailsViewModel = new PersonalDetailsViewModel
            {
                Titles = null,
                FirstName = null,
                LastName = null,
                DateOfBirthDay = "01",
                DateOfBirthMonth = "03",
                DateOfBirthYear = "1966",
                DateOfBirth = ""
            };

            // Act
            controller.ValidateViewModel(personalDetailsViewModel);
            ActionResult result = controller.PersonalDetails(personalDetailsViewModel);

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<PersonalDetailsViewModel>();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Keys.Count.ShouldEqual(4);
            controller.ModelState.Values.ToList()[0].Errors.First().ErrorMessage.ShouldEqual(Form_Resources.TitlesError);
            controller.ModelState.Values.ToList()[1].Errors.First().ErrorMessage.ShouldEqual(Form_Resources.FirstNameRequiredError);
            controller.ModelState.Values.ToList()[2].Errors.First().ErrorMessage.ShouldEqual(Form_Resources.LastNameRequiredError);
            controller.ModelState.Values.ToList()[3].Errors.First().ErrorMessage.ShouldEqual(Form_Resources.DateOfBirthRequiredError);
        }

        [TestCaseSource(nameof(CustomerAgeValidTestCases))]
        public void ShouldRedirectToSelectAddressPageWhenCustomerDateOfBirthIsValid(
            string dateOfBirthDay,
            string dateOfBirthMonth,
            string dateOfBirthYear,
            string dateOfBirth,
            string coverPostcode,
            string expectedAction)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer { CoverPostcode = coverPostcode });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            var personalDetailsViewModel = new PersonalDetailsViewModel
            {
                Titles = Titles.Dr,
                FirstName = "Joe",
                LastName = "Bloggs",
                DateOfBirthDay = dateOfBirthDay,
                DateOfBirthMonth = dateOfBirthMonth,
                DateOfBirthYear = dateOfBirthYear,
                DateOfBirth = dateOfBirth
            };

            // Act
            ActionResult result = controller.PersonalDetails(personalDetailsViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual(expectedAction);
        }

        [Test, TestCaseSource(nameof(CustomerAgeInvalidTestCases))]
        public void ShouldRemainOnPersonalDetailsPageWhenCustomerDateOfBirthIsInvalid(
            string dateOfBirthDay,
            string dateOfBirthMonth,
            string dateOfBirthYear,
            string dateOfBirth,
            string coverPostcode,
            string expectedViewName)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer { CoverPostcode = coverPostcode });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            var personalDetailsViewModel = new PersonalDetailsViewModel
            {
                Titles = Titles.Dr,
                FirstName = "Joe",
                LastName = "Bloggs",
                DateOfBirthDay = dateOfBirthDay,
                DateOfBirthMonth = dateOfBirthMonth,
                DateOfBirthYear = dateOfBirthYear,
                DateOfBirth = dateOfBirth
            };

            // Act
            ActionResult result = controller.PersonalDetails(personalDetailsViewModel);

            // Assert
            result.ShouldBeType<ViewResult>()
                .ViewName.ShouldEqual(expectedViewName);
        }

        [Test]
        public void ShouldStorePersonalDetailsInTheSession()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, new HomeServicesCustomer { CoverPostcode = "PO91BH" });
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            var personalDetailsViewModel = new PersonalDetailsViewModel
            {
                Titles = Titles.Dr,
                FirstName = "Test",
                LastName = "Test",
                DateOfBirth = "01/01/1990",
                DateOfBirthDay = "01",
                DateOfBirthMonth = "01",
                DateOfBirthYear = "1990"
            };

            // Act
            controller.ValidateViewModel(personalDetailsViewModel);
            ActionResult result = controller.PersonalDetails(personalDetailsViewModel);

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            customer.PersonalDetails.Title.ShouldEqual(personalDetailsViewModel.Titles.ToString());
            customer.PersonalDetails.FirstName.ShouldEqual(personalDetailsViewModel.FirstName);
            customer.PersonalDetails.LastName.ShouldEqual(personalDetailsViewModel.LastName);
            customer.PersonalDetails.DateOfBirth.ShouldEqual(personalDetailsViewModel.DateOfBirth);

            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("SelectAddress");
        }

        private static IEnumerable<TestCaseData> CustomerAgeValidTestCases()
        {
            DateTime eighteenYearsOldDoB = DateTime.Today.AddYears(-18);
            DateTime nineteenYearsOldDoB = DateTime.Today.AddYears(-19);
            DateTime sixteenYearsOldDoB = DateTime.Today.AddYears(-16);
            DateTime seventeenYearsOldDoB = DateTime.Today.AddYears(-17);
            
            yield return new TestCaseData(eighteenYearsOldDoB.Day.ToString(CultureInfo.InvariantCulture), eighteenYearsOldDoB.Month.ToString(CultureInfo.InvariantCulture), eighteenYearsOldDoB.Year.ToString(CultureInfo.InvariantCulture), eighteenYearsOldDoB.ToString("dd/MM/yyyy"), "PO91BH", "SelectAddress");
            yield return new TestCaseData(nineteenYearsOldDoB.Day.ToString(CultureInfo.InvariantCulture), nineteenYearsOldDoB.Month.ToString(CultureInfo.InvariantCulture), nineteenYearsOldDoB.Year.ToString(CultureInfo.InvariantCulture), nineteenYearsOldDoB.ToString("dd/MM/yyyy"), "PO91BH", "SelectAddress");
            yield return new TestCaseData(sixteenYearsOldDoB.Day.ToString(CultureInfo.InvariantCulture), sixteenYearsOldDoB.Month.ToString(CultureInfo.InvariantCulture), sixteenYearsOldDoB.Year.ToString(CultureInfo.InvariantCulture), sixteenYearsOldDoB.ToString("dd/MM/yyyy"), "G673BJ", "SelectAddress");
            yield return new TestCaseData(seventeenYearsOldDoB.Day.ToString(CultureInfo.InvariantCulture), seventeenYearsOldDoB.Month.ToString(CultureInfo.InvariantCulture), seventeenYearsOldDoB.Year.ToString(CultureInfo.InvariantCulture), seventeenYearsOldDoB.ToString("dd/MM/yyyy"), "G673BJ", "SelectAddress");
        }

        private static IEnumerable<TestCaseData> CustomerAgeInvalidTestCases()
        {
            DateTime fifteenYearsOldDoB = DateTime.Today.AddYears(-15);
            DateTime seventeenYearsOldDoB = DateTime.Today.AddYears(-17);

            yield return new TestCaseData(seventeenYearsOldDoB.Day.ToString(CultureInfo.InvariantCulture), seventeenYearsOldDoB.Month.ToString(CultureInfo.InvariantCulture), seventeenYearsOldDoB.Year.ToString(CultureInfo.InvariantCulture), seventeenYearsOldDoB.ToString("dd/MM/yyyy"), "PO91BH", "PersonalDetails");
            yield return new TestCaseData(fifteenYearsOldDoB.Day.ToString(CultureInfo.InvariantCulture), fifteenYearsOldDoB.Month.ToString(CultureInfo.InvariantCulture), fifteenYearsOldDoB.Year.ToString(CultureInfo.InvariantCulture), fifteenYearsOldDoB.ToString("dd/MM/yyyy"), "G673BJ", "PersonalDetails");
        }
    }
}
