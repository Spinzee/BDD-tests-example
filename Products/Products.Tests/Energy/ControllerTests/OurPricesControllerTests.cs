namespace Products.Tests.Energy.ControllerTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core;
    using Helpers;
    using NUnit.Framework;
    using Products.Model;
    using Products.Tests.Common.Fakes;
    using Products.Tests.Common.Helpers;
    using Products.Tests.Energy.Fakes.Services;
    using Products.Web.Areas.Energy.Controllers;
    using Products.WebModel.Resources.Energy;
    using Products.WebModel.ViewModels.Energy;
    using Should;

    public class OurPricesControllerTests
    {
        [Test]
        public async Task ShouldDisplayCorrectHeaderAndParagraph()
        {
            // Arrange
            var controller = new ControllerFactory()
                .Build<OurPricesController>();

            // Act
            ActionResult result = await controller.EnterPostcode();

            // Assert
            var model = result.ShouldNotBeNull()
                .ShouldBeType<ViewResult>()
                .Model.ShouldBeType<PostcodeViewModel>();

            model.Header.ShouldEqual(OurPrices_Resources.Header);
            model.ParagraphText.ShouldEqual(OurPrices_Resources.Paragraph);
        }

        [TestCase("", "CannotFindPrices")]
        [TestCase("TestGeo", "OurPrices")]
        public async Task ShouldRedirectToAppropriatePages(string geoAreaForPostCode, string expectedPage)
        {
            // Arrange
            var fakeProductServiceWrapper = new FakeProductServiceWrapper { GeoAreaForPostCode = geoAreaForPostCode };
            var controller = new ControllerFactory()
                .WithEnergyProductServiceWrapper(fakeProductServiceWrapper)
                .Build<OurPricesController>();

            var postcodeViewModel = new PostcodeViewModel { PostCode = "PO91BH" };

            // Act
            controller.ValidateViewModel(postcodeViewModel);
            ActionResult result = await controller.EnterPostcode(postcodeViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual(expectedPage);
        }

        [TestCase("", "Please enter your postcode.")]
        [TestCase("PO", "Sorry, we don’t recognise that postcode. Please try entering it again.")]
        public async Task ShouldReturnToEnterPostcodeWhenPostcodeIsMissingOrInvalid(string postcode, string errorMessage)
        {
            // Arrange
            var controller = new ControllerFactory()
                .Build<OurPricesController>();

            var postcodeViewModel = new PostcodeViewModel
            {
                PostCode = postcode
            };

            // Act
            controller.ValidateViewModel(postcodeViewModel);
            ActionResult result = await controller.EnterPostcode(postcodeViewModel);

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<PostcodeViewModel>();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Keys.Count.ShouldEqual(1);
            controller.ModelState.Values.First().Errors.First().ErrorMessage.ShouldEqual(errorMessage);
        }

        [Test]
        public async Task ShouldRedirectToAreaNotCoveredWhenNorthernIrelandPostcodeSupplied()
        {
            // Arrange
            var controller = new ControllerFactory()
                .Build<OurPricesController>();

            var viewModel = new PostcodeViewModel
            {
                PostCode = "BT1 4GT"
            };

            // Act
            ActionResult result = await controller.EnterPostcode(viewModel);

            // Assert            
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("AreaNotCovered");
        }

        [Test]

        //[TestCase("P091BH", FuelCategory.Standard, TariffStatus.ForSale)]
        //[TestCase("P091BH", FuelCategory.MultiRate, TariffStatus.ForSale)]
        [TestCase("P091BH", FuelCategory.Gas, TariffStatus.ForSale)]
        public async Task ShouldDisplayTariffsAsPerUserSelection(string postCode, FuelCategory fuelCategory, TariffStatus tariffStatus)
        {
            // Arrange
            var controller = new ControllerFactory()
                .Build<OurPricesController>();

            // Act
            ActionResult result = await controller.OurPrices(postCode, fuelCategory, tariffStatus);

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<OurPricesViewModel>();
            var viewModel = (OurPricesViewModel)viewResult.Model;
            viewModel.FuelCategory.ShouldEqual(fuelCategory);
            viewModel.Products.Count.ShouldEqual(2);
            viewModel.Products.First().TariffOptions.Count.ShouldEqual(2);
        }

        [Test]
        public async Task ShouldShowHomeServicesNotAvailablePageWhenCustomerAlertIsActive()
        {
            // Arrange
            var controller = new ControllerFactory()
                .WithCustomerAlertRepository(new FakeCustomerAlertRepository(new List<CustomerAlertResult> { new CustomerAlertResult
                    {
                        StartTime = null,
                        EndTime = null
                    }
                }))
                .Build<OurPricesController>();

            // Act
            ActionResult result = await controller.EnterPostcode();

            // Assert
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult)result).RouteValues["action"].ShouldEqual("OurPriceUnavailable");
        }
    }
}
