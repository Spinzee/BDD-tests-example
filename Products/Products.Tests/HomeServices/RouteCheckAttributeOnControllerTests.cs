namespace Products.Tests.HomeServices
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Helpers;
    using Model.Constants;
    using NUnit.Framework;
    using Products.Model.HomeServices;
    using Products.Tests.Common.Fakes;
    using Products.Web.Areas.HomeServices.Controllers;
    using Should;
    using Web.Attributes;

    public class RouteCheckAttributeOnControllerTests
    {
        [Test, TestCaseSource(nameof(DirectAccessFailsTestData))]
        public void HomeServicesActionsShouldCheckRouteAndRedirectToHubPageIfMissingPreRequisites(string actionName, HomeServicesCustomer homeServicesCustomer)
        {
            // Arrange
            var fakeHttpSession = new FakeHttpSession();
            if (homeServicesCustomer != null)
            {
                fakeHttpSession.Add(SessionKeys.HomeServicesCustomer, homeServicesCustomer);
            }

            var fakeContextManager =
                new FakeContextManager(new FakeHttpContext(new FakeHttpRequest($"/home-services-signup/{actionName}", "GET")
                .WithWebQueryString(new System.Collections.Specialized.NameValueCollection()), new FakeHttpServerUtility(), fakeHttpSession));

            var controller = new ControllerFactory()
               .WithContextManager(fakeContextManager)
               .Build<HomeServicesController>();

            var controllerContext = new ControllerContext(fakeContextManager.HttpContext, new RouteData(), controller);

            var actionExecutingContext = new ActionExecutingContext(controllerContext, new FakeActionDescriptor(actionName), new Dictionary<string, object>());
            var routeCheckAttribute = new HomeServicesRouteCheckAttribute();

            // Act
            routeCheckAttribute.OnActionExecuting(actionExecutingContext);

            // Assert
            actionExecutingContext.Result.ShouldNotBeNull();
            actionExecutingContext.Result.ShouldBeType<RedirectResult>();
            ((RedirectResult)actionExecutingContext.Result).Url.ShouldEqual(ConfigurationManager.AppSettings["HomeServicesHubUrl"]);
        }

        private static IEnumerable<TestCaseData> DirectAccessFailsTestData()
        {
            yield return new TestCaseData("Postcode", null);
            yield return new TestCaseData("LandlordPostcode", null);
            yield return new TestCaseData("CoverDetails", new HomeServicesCustomer());
            yield return new TestCaseData("PersonalDetails", new HomeServicesCustomer { SelectedProductCode = "" });
            yield return new TestCaseData("SelectAddress", new HomeServicesCustomer { PersonalDetails = null });
            yield return new TestCaseData("LandlordBillingPostcode", new HomeServicesCustomer { SelectedCoverAddress = null });
            yield return new TestCaseData("SelectAddress", new HomeServicesCustomer { SelectedCoverAddress = null });
            yield return new TestCaseData("SelectAddress", new HomeServicesCustomer { IsLandlord = false });
            yield return new TestCaseData("ContactDetails", new HomeServicesCustomer { IsLandlord = false, SelectedCoverAddress = null });
            yield return new TestCaseData("ContactDetails", new HomeServicesCustomer { IsLandlord = true, SelectedBillingAddress = null });
            yield return new TestCaseData("BankDetails", new HomeServicesCustomer { ContactDetails = null });
            yield return new TestCaseData("Summary", new HomeServicesCustomer { DirectDebitDetails = null });
            yield return new TestCaseData("Confirmation", new HomeServicesCustomer { ApplicationIds = null });
        }
    }
}
