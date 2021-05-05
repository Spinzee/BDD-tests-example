namespace Products.Tests.HomeServices
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Helpers;
    using Model.Constants;
    using Model.Enums;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using Products.Model.HomeServices;
    using Products.Tests.Common.Fakes;
    using Products.Tests.Common.Helpers;
    using Products.Web.Areas.HomeServices.Controllers;
    using Products.WebModel.ViewModels.HomeServices;
    using Should;

    public class CoverDetailsTests
    {
        [Test]
        public void ApplyingForCoverShouldRedirectToPersonalDetails()
        {
            // Arrange
            var controller = new ControllerFactory()
                .Build<HomeServicesController>();

            // Act
            ActionResult result = controller.CoverDetails(new CoverDetailsViewModel());

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("PersonalDetails");
        }

        [TestCaseSource(nameof(ProductRequestTestData))]
        public async Task CoverDetailsViewModelShouldBePopulatedWithCorrectValues(string productCode, string displayName, bool hasOffers, string monthlyCost, string offerText, string yearlyCost,
                                                                                    int numberOfExcess, bool hasExcess, int numberOfProductExtras, bool productExtrasAvailable,
                                                                                    string singleExcessAmount, int numberOfWhatsExcluded, int numberOfWhatsIncluded, bool hasCoverBullet, string coverBulletText)
        {
            // Arrange
            var controller = new ControllerFactory()
                .Build<HomeServicesController>();

            // Act
            var postcodeViewModel = new PostcodeViewModel { Postcode = "PO9 1QH", ProductCode = productCode, AddressTypes = AddressTypes.Cover };

            controller.ValidateViewModel(postcodeViewModel);
            await controller.Postcode(postcodeViewModel);
            ActionResult result = controller.CoverDetails();

            // Assert
            var coverDetailsViewResult = result.ShouldBeType<ViewResult>();
            var coverDetailsViewModel = coverDetailsViewResult.Model.ShouldBeType<CoverDetailsViewModel>();
            coverDetailsViewModel.CoverDetailsHeaderViewModel.DisplayName.ShouldEqual(displayName);
            coverDetailsViewModel.CoverDetailsHeaderViewModel.HasOffers.ShouldEqual(hasOffers);
            coverDetailsViewModel.CoverDetailsHeaderViewModel.MonthlyCost.ShouldEqual(monthlyCost);
            coverDetailsViewModel.CoverDetailsHeaderViewModel.OfferText.ShouldEqual(offerText);
            coverDetailsViewModel.CoverDetailsHeaderViewModel.YearlyCost.ShouldEqual(yearlyCost);
            coverDetailsViewModel.HasExcess.ShouldEqual(hasExcess);
            coverDetailsViewModel.ProductExtras?.Count.ShouldEqual(numberOfProductExtras);
            coverDetailsViewModel.ProductExtrasAvailable.ShouldEqual(productExtrasAvailable);
            coverDetailsViewModel.SingleExcessAmount.ShouldEqual(singleExcessAmount);
            coverDetailsViewModel.WhatsIncluded.Count.ShouldEqual(numberOfWhatsIncluded);
            coverDetailsViewModel.WhatsExcluded.Count.ShouldEqual(numberOfWhatsExcluded);
            coverDetailsViewModel.CoverDetailsHeaderViewModel.HasCoverbullet.ShouldEqual(hasCoverBullet);
            coverDetailsViewModel.CoverDetailsHeaderViewModel.CoverbulletText.ShouldEqual(coverBulletText);
        }

        [Test]
        public async Task ShouldSetProductCodeInSessionOnChangeOfExcess()
        {
            // Arrange
            const string productCodeFromHub = "BC";
            const string updatedProductCode = "BC50";

            var fakeSessionManager = new FakeSessionManager();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            var postcodeViewModel = new PostcodeViewModel { Postcode = "PO9 1QH", ProductCode = productCodeFromHub, AddressTypes = AddressTypes.Cover };

            controller.ValidateViewModel(postcodeViewModel);
            await controller.Postcode(postcodeViewModel);
            controller.CoverDetails();
            ActionResult result = controller.UpdateSelectedProductExcess(updatedProductCode);

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            customer.SelectedProductCode.ShouldEqual(updatedProductCode);
            customer.SelectedProductCode.ShouldNotEqual(productCodeFromHub);

            var coverDetailsJsonResult = result.ShouldBeType<JsonResult>();

            JObject parsedJson = JObject.Parse(JsonConvert.SerializeObject(coverDetailsJsonResult.Data));
            parsedJson["status"].ToString().ShouldEqual("True");
        }

        [Test]
        public async Task ShouldSetProductExtrasInSessionOnAdd()
        {
            // Arrange
            const string productCodeFromHub = "BOBC";
            const string productExtraCode = "EC";

            var fakeSessionManager = new FakeSessionManager();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            var postcodeViewModel = new PostcodeViewModel { Postcode = "PO9 1QH", ProductCode = productCodeFromHub, AddressTypes = AddressTypes.Cover };

            controller.ValidateViewModel(postcodeViewModel);
            await controller.Postcode(postcodeViewModel);
            controller.CoverDetails();
            ActionResult result = controller.UpdateSelectedProductExtra(productExtraCode);

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            customer.SelectedExtraCodes.First().ShouldEqual(productExtraCode);

            var coverDetailsJsonResult = result.ShouldBeType<JsonResult>();

            JObject parsedJson = JObject.Parse(JsonConvert.SerializeObject(coverDetailsJsonResult.Data));
            parsedJson["status"].ToString().ShouldEqual("True");
        }

        [Test]
        public async Task ShouldSetProductExtrasInSessionOnRemove()
        {
            // Arrange
            const string productCodeFromHub = "BOBC";
            const string productExtraCode = "EC";

            var fakeSessionManager = new FakeSessionManager();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<HomeServicesController>();

            var postcodeViewModel = new PostcodeViewModel { Postcode = "PO9 1QH", ProductCode = productCodeFromHub, AddressTypes = AddressTypes.Cover };

            controller.ValidateViewModel(postcodeViewModel);
            // ReSharper disable once UnusedVariable
            ActionResult postCodeViewResult = await controller.Postcode(postcodeViewModel);
            // ReSharper disable once UnusedVariable
            ActionResult coverDetailsResult = controller.CoverDetails();
            // ReSharper disable once RedundantAssignment
            ActionResult result = controller.UpdateSelectedProductExtra(productExtraCode);
            result = controller.UpdateSelectedProductExtra(productExtraCode);

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            customer.SelectedExtraCodes.ShouldBeEmpty();

            var coverDetailsJsonResult = result.ShouldBeType<JsonResult>();

            JObject parsedJson = JObject.Parse(JsonConvert.SerializeObject(coverDetailsJsonResult.Data));
            parsedJson["status"].ToString().ShouldEqual("True");
        }

        [TestCase("BC", "EC", 3, 1)]
        [TestCase("BC50", "EC", 3, 1)]
        [TestCase("HC", "EC", 3, 1)]
        [TestCase("HC50", "EC", 3, 1)]
        [TestCase("LANDBC", "EC", 2, 1)]
        [TestCase("LANDHC", "EC", 2, 1)]
        [TestCase("BOBC", "EC", 2, 1)]
        [TestCase("BOHC", "EC", 2, 1)]
        public async Task ShouldSetProductPdfUrlsBasedOnSelectedProductAndAvailableExtras(string selectedProductCode, string availableProductExtraCode, int noofproductpdfs, int noofextraspdfs)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultHomeServices();
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<HomeServicesController>();
            var postcodeViewModel = new PostcodeViewModel { Postcode = "PO9 1QH", ProductCode = selectedProductCode, AddressTypes = AddressTypes.Cover };

            // Act
            controller.ValidateViewModel(postcodeViewModel);
            await controller.Postcode(postcodeViewModel);
            ActionResult result = controller.CoverDetails();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<CoverDetailsViewModel>();

            model.AccordionViewModel.ProductPDFs.Count.ShouldEqual(noofproductpdfs);
            model.AccordionViewModel.ExtraProductPDFs.Count.ShouldEqual(noofextraspdfs);
        }

        private static IEnumerable<TestCaseData> ProductRequestTestData()
        {
            yield return new TestCaseData("BOBC", "Boiler cover", true, "£20.00", "with £0 excess SSE gas customers get £55 credit on their account", "£238.00", 2, true, 1, true, "£90", 2, 2, false, "");
            yield return new TestCaseData("BOHC", "Boiler cover", true, "£20.00", "with £0 excess SSE gas customers get £55 credit on their account", "£238.00", 2, true, 1, true, "£90", 2, 2, false, "");
            yield return new TestCaseData("BC", "Boiler cover 1", false, "£20.00", "", "£240.00", 1, true, 1, true, "", 2, 2, false, "");
            yield return new TestCaseData("EC", "Electric Wiring cover", true, "£20.00", "with £0 excess SSE gas customers get £55 credit on their account 90", "£150.00", 1, true, 1, true, "£90", 2, 2, true, "coverBulletText");
        }
    }
}
