namespace Products.Tests.Broadband
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Helpers;
    using NUnit.Framework;
    using Products.Model.Broadband;
    using Products.Model.Common;
    using Products.Model.Constants;
    using Products.Tests.Common.Fakes;
    using Products.Web.Areas.Broadband.Controllers;
    using Products.WebModel.ViewModels.Broadband;
    using Should;
    using Web.Attributes;

    public class BroadbandCheckSessionAttributeOnControllerTests
    {
        [Test, TestCaseSource(nameof(DirectPageAccessSessionTestCases))]
        public void
            AllHttpGetControllerActionsShouldExecuteEnergyCheckSessionAttributeAndRedirectToHubPageWhenSessionDataIsMissing(
                string controllerName, string actionName, string requestPath, BroadbandJourneyDetails broadbandJourneyDetails, YourPriceViewModel yourPriceViewModel)
        {
            //Arrange
            var fakeContextManager =
                new FakeContextManager(new FakeHttpContext(new FakeHttpRequest(requestPath, actionName),
                    new FakeHttpServerUtility(), new FakeHttpSession()));

            fakeContextManager.HttpContext.Session.Add(SessionKeys.BroadbandJourney, broadbandJourneyDetails);
            fakeContextManager.HttpContext.Session.Add(SessionKeys.YourPriceDetails, yourPriceViewModel);

            var controller = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .Build<LineCheckerController>();

            controller.RouteData.Values["controller"] = controllerName;
            controller.RouteData.Values["action"] = actionName;

            var controllerContext = new ControllerContext(fakeContextManager.HttpContext, new RouteData(),controller);

            var actionExecutingContext = new ActionExecutingContext(controllerContext,
                new FakeActionDescriptor(string.Empty), new Dictionary<string, object>());
            var broadbandCheckSessionAttribute = new BroadbandCheckSessionAttribute();

            // Act
            broadbandCheckSessionAttribute.OnActionExecuting(actionExecutingContext);

            // Assert
            actionExecutingContext.Result.ShouldNotBeNull();
            var redirectResult = actionExecutingContext.Result.ShouldBeType<RedirectResult>();
            redirectResult.Url.ShouldEqual(ConfigurationManager.AppSettings["BroadbandLinkBackToHubURL"]);
        }

        private static IEnumerable<TestCaseData> DirectPageAccessSessionTestCases()
        {
            yield return new TestCaseData("LineChecker", "SelectAddress", "/broadband/select-address", new BroadbandJourneyDetails{Customer = new Customer{PostcodeEntered = string.Empty}}, null);
            yield return new TestCaseData("Packages", "AvailablePackages", "/broadband/available-packages", new BroadbandJourneyDetails { Customer = new Customer { PostcodeEntered = "RG249SS" } }, null);
            yield return new TestCaseData("Packages", "SelectedPackage", "/broadband/selected-package", new BroadbandJourneyDetails { Customer = new Customer { PostcodeEntered = "RG249SS" } }, null);
            yield return new TestCaseData("CustomerDetails", "TransferYourNumber", "/broadband/transfer-your-number", new BroadbandJourneyDetails { Customer = new Customer { PostcodeEntered = "RG249SS", SelectedAddress = new BTAddress()}}, null);
            yield return new TestCaseData("CustomerDetails", "PersonalDetails", "/broadband/personal-details", new BroadbandJourneyDetails { Customer = new Customer { PostcodeEntered = "RG249SS", SelectedAddress = new BTAddress(), SelectedProduct = new BroadbandProduct()}}, new YourPriceViewModel());
            yield return new TestCaseData("CustomerDetails", "ContactDetails", "/broadband/contact-details", new BroadbandJourneyDetails { Customer = new Customer { PostcodeEntered = "RG249SS", SelectedAddress = new BTAddress(), SelectedProduct = new BroadbandProduct(), TransferYourNumberIsSet = true}},new YourPriceViewModel());
            yield return new TestCaseData("OnlineAccount", "OnlineAccount", "/broadband/online-account", new BroadbandJourneyDetails { Customer = new Customer { PostcodeEntered = "RG249SS", SelectedAddress = new BTAddress(), SelectedProduct = new BroadbandProduct(), TransferYourNumberIsSet = true, PersonalDetails = new PersonalDetails()}},new YourPriceViewModel());
            yield return new TestCaseData("BankDetails", "BankDetails", "/broadband/bank-details", new BroadbandJourneyDetails { Customer = new Customer { PostcodeEntered = "RG249SS", SelectedAddress = new BTAddress(), SelectedProduct = new BroadbandProduct(), TransferYourNumberIsSet = true, PersonalDetails = new PersonalDetails(), ContactDetails = null } }, new YourPriceViewModel());
            yield return new TestCaseData("Summary", "Summary", "/broadband/summary", new BroadbandJourneyDetails { Customer = new Customer { PostcodeEntered = "RG249SS", SelectedAddress = new BTAddress(), SelectedProduct = new BroadbandProduct(), TransferYourNumberIsSet = true, PersonalDetails = new PersonalDetails(), ContactDetails = new ContactDetails(), UserId = Guid.NewGuid()}}, new YourPriceViewModel());
            yield return new TestCaseData("Confirmation", "Confirmation", "/broadband/confirmation", new BroadbandJourneyDetails { Customer = new Customer { PostcodeEntered = "RG249SS", SelectedAddress = new BTAddress(), SelectedProduct = new BroadbandProduct(), TransferYourNumberIsSet = true, PersonalDetails = new PersonalDetails(), ContactDetails = new ContactDetails(), UserId = Guid.NewGuid(), DirectDebitDetails = new DirectDebitDetails()}}, new YourPriceViewModel());
            yield return new TestCaseData("Summary", "PrintMandate", "/broadband/PrintMandate", new BroadbandJourneyDetails { Customer = new Customer { PostcodeEntered = "RG249SS", SelectedAddress = new BTAddress(), SelectedProduct = new BroadbandProduct(), TransferYourNumberIsSet = true, PersonalDetails = new PersonalDetails(), ContactDetails = new ContactDetails(), UserId = Guid.NewGuid() }}, new YourPriceViewModel());
        }
    }
}
