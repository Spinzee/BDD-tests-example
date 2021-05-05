namespace Products.Tests.Broadband
{
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Common.Fakes;
    using Common.Helpers;
    using Fakes.Repository;
    using Fakes.Services;
    using Helpers;
    using Model;
    using NUnit.Framework;
    using Products.Model.Broadband;
    using Products.WebModel.ViewModels.Broadband;
    using Should;
    using Web.Areas.Broadband.Controllers;
    using FakeSessionManager = Fakes.Services.FakeSessionManager;

    [TestFixture]
    public class HubPageJourneyTests
    {
        [TestCase("FIBRE_ANY19", "SelectedPackage")]
        [TestCase("FP_AP19", "SelectedPackage")]
        [TestCase("BB11_ANY19", "AvailablePackages")]
        [TestCase("", "AvailablePackages")]
        [TestCase(null, "AvailablePackages")]
        [TestCase("BB_ANY19", "AvailablePackages")]
        public async Task ShouldDirectToSelectedProductPageFromHubPageQueryString(string productCode, string expectPage)
        {
            // Arrange
            var fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses };

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithBroadbandProductsServiceWrapper(fakeBroadbandProductsServiceWrapper);

            var lineCheckerController = controllerFactory.Build<LineCheckerController>();
            
            // Act
            await lineCheckerController.LineChecker(productCode);
            
            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "P091BH",
                PhoneNumber = "02092022982",
                ProductCode = productCode
            };

            await lineCheckerController.Submit(lineCheckerViewModel);
            ActionResult resultSelectAddressGet = await lineCheckerController.SelectAddress();

            var viewResult = resultSelectAddressGet.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();

            // ReSharper disable once PossibleNullReferenceException
            viewModel.SelectedAddressId = viewModel.Addresses.FirstOrDefault().Id;
            ActionResult result = await lineCheckerController.SelectAddress(viewModel);

            // Assert
            result.ShouldNotBeNull();

            var actualObject = result.ShouldBeType<JsonResult>();
            PropertyInfo property = actualObject.Data.GetType().GetProperty("Status");
            object propertyValue = property?.GetValue(actualObject.Data);
            propertyValue.ShouldEqual(expectPage);
        }

        [TestCase("WHSmith", "WHSmith", "Membership Id longer than 0 and less than or equal to 20 characters should be valid")]
        [TestCase("", null, "Membership Id less than 1 character should be invalid")]
        [TestCase("123456789012345678901", null, "Membership Id longer than 20 characters should be invalid")]
        [TestCase("Symbol?", null, "Membership Id with non-alphanumeric characters should be invalid")]
        [TestCase("      ", null, "Membership Id with spaces should be invalid")]
        [TestCase("£$%^&*()", null, "Membership Id with special characters should be invalid")]
        public async Task ShouldStoreMembershipIdInSessionIfValid(string membershipId, string expectedStoredMembershipId, string description)
        {
            // Arrange
            var fakeSalesRepository = new FakeBroadbandSalesRepository();

            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails(),
                ListOfProductsSessionObject = FakeBroadbandProductsData.GetListOfAvailableProductsFromSession(),
                OpenReachSessionObject = FakeOpenReachData.GetOpenReachData()
            };
            var cookieCollection = new HttpCookieCollection { new HttpCookie("migrateMemberid", membershipId) };

            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(cookieCollection))
                .Build();

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSalesRepository(fakeSalesRepository)
                .WithSessionManager(fakeSessionManager);

            var controller = controllerFactory.Build<SummaryController>();

            // Act
            await controller.Submit(new SummaryViewModel { IsTermsAndConditionsChecked = true });

            // Assert
            Customer customer = fakeSessionManager.SessionObject.ShouldBeType<BroadbandJourneyDetails>().Customer;

            customer.MembershipId.ShouldEqual(expectedStoredMembershipId);
        }
    }
}