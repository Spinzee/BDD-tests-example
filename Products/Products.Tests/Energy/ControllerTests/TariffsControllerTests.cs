namespace Products.Tests.Energy.ControllerTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Broadband.Fakes.Services;
    using Broadband.Model;
    using Common.Fakes;
    using Common.Helpers;
    using Fakes.Services;
    using Helpers;
    using Model;
    using NUnit.Framework;
    using Products.Model.Common;
    using Products.Model.Constants;
    using Products.Model.Energy;
    using Products.Model.Enums;
    using Service.Common.Managers;
    using Should;
    using System;
    using ControllerHelpers;
    using Core;
    using Core.Enums;
    using Products.Infrastructure.Extensions;
    using TariffChange.Fakes.Managers;
    using Web.Areas.Energy.Controllers;
    using WebModel.Resources.Energy;
    using WebModel.ViewModels.Energy;
    using FakeSessionManager = Common.Fakes.FakeSessionManager;

    public class TariffsControllerTests
    {
        [TestCase(false, "Choose a new energy tariff for ", "")]
        [TestCase(true, "Your quote </br> for ", "stars")]
        public void ShouldShowCorrectHeaderTextAndBannerClassInAvailableTariffsView(
                bool isBundlingJourney,
                string expectedHeaderText,
                string expectedBannerClass)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                IsBundlingJourney = isBundlingJourney
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            viewModel.HeaderText.ShouldEqual(expectedHeaderText);
            viewModel.BannerClass.ShouldEqual(expectedBannerClass);
        }

        [Test]
        public void ShouldStoreBTAddressListForPostCodeInSessionAndRedirectToConfirmAddressWhenNoBTAddressMatchesSelectedAddress()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress
                    { AddressLine1 = "blah Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "blah Waterloo Road,Hampshire Havant" }
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper
                    { Fallout = false, NewBTLineAvailabilityServiceException = false })
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper { BroadbandProductsResult = BroadbandProductsResult.ShowProducts })
                .Build<TariffsController>();

            // Act
            // ReSharper disable once UnusedVariable
            ActionResult result1 = controller.AvailableTariffs().Result;
            ActionResult result = controller.AvailableTariffs("BO001").Result;

            // Assert
            var jsonResult = result.ShouldBeType<JsonResult>();
            jsonResult.Data.ShouldNotBeNull();
            jsonResult.Data.ToString().ShouldContain("http://localhost/enery-journey/SignUp/ConfirmAddress");

            var addresses = fakeSessionManager.GetSessionDetails<List<BTAddress>>(SessionKeys.BTAddressListForPostCode);
            addresses.ShouldNotBeNull();
            addresses.Count.ShouldEqual(8);
        }

        [TestCase(true, false,  BroadbandProductsResult.ShowProducts)]
        [TestCase(false, false, BroadbandProductsResult.Exception)]
        [TestCase(false, false, BroadbandProductsResult.LineUnsuitable)]
        public void ShouldStoreUnavailableChosenBundleInSessionAndRedirectToGetAvailableTariffs(bool openReachFallout, bool openReachException, BroadbandProductsResult broadbandProductsResult)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" }
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithContextManager(FakeContextManagerFactory.Default())
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = openReachFallout, NewBTLineAvailabilityServiceException = openReachException })
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper { BroadbandProductsResult = broadbandProductsResult})
                .Build<TariffsController>();

            // Act
            // ReSharper disable once UnusedVariable
            ActionResult result1 =  controller.AvailableTariffs().Result;
            ActionResult result =  controller.AvailableTariffs("BO001").Result;

            // Assert
            var jsonResult = result.ShouldBeType<JsonResult>();
            jsonResult.Data.ShouldNotBeNull();
            //jsonResult.Data.ShouldEqual("");
            jsonResult.Data.ToString().ShouldContain("AvailableTariffs");

            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.UnavailableBundles.ShouldContain("BO001");
        }

        [TestCase(true, false, "")]
        [TestCase(false, true, "You might also be interested in")]
        public void ShowYouMightBeInterestedInTextShouldBeWhenChosenProductIsNotNullAndAllTariffsCountIsZero(bool returnSingleProduct, bool expectedResult, string expectedText)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { HouseName = "Town House", Town = "Havant" },
                ChosenProduct = "Standard",
                IsBundlingJourney = false
            });

            var fakeEnergyProductServiceWrapper = new FakeProductServiceWrapper { ReturnSingleDualFuelFakeProduct = returnSingleProduct };
            var fakeBundleTariffServiceWrapper = new FakeBundleTariffServiceWrapper { ReturnNoBundles = true };

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithEnergyProductServiceWrapper(fakeEnergyProductServiceWrapper)
                .WithBundleTariffServiceWrapper(fakeBundleTariffServiceWrapper)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            viewModel.ShowYouMightBeInterestedInText.ShouldEqual(expectedResult);
            if (string.IsNullOrEmpty(expectedText))
            {
                string.IsNullOrEmpty(viewModel.YouMightBeInterestedInText).ShouldBeTrue();
            }
            else
            {
                viewModel.YouMightBeInterestedInText.ShouldEqual(expectedText);
            }
        }

        [TestCase("Choose your product", "All prices on this page include VAT and any applicable discounts.", true)]
        [TestCase("Choose your product", "All prices on this page include VAT and any applicable discounts.", false)]
        public void ShouldShowCorrectSubHeaderTextWhenChosenProductIsNullAndAllTariffsAreAvailable(string expectedSubHeader, string expectedSubParagraph, bool isBundlingJourney)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { HouseName = "Town House", Town = "Havant" },
                ChosenProduct = null,
                IsBundlingJourney = isBundlingJourney
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            viewModel.SubHeaderText.ShouldEqual(expectedSubHeader);
            viewModel.SubHeaderParagraph.ShouldEqual(expectedSubParagraph);
            viewModel.ShowYouMightBeInterestedInText.ShouldBeFalse();
            string.IsNullOrEmpty(viewModel.YouMightBeInterestedInText).ShouldBeTrue();
        }

        [TestCase(FuelType.Dual, PaymentMethod.Quarterly, false)]
        [TestCase(FuelType.Gas, PaymentMethod.MonthlyDirectDebit, true)]
        public void ShouldShowCorrectTabs(FuelType fuelType, PaymentMethod paymentMethod, bool showTabs)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = fuelType,
                SelectedPaymentMethod = paymentMethod,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { HouseName = "Town House", Town = "Havant" },
                IsBundlingJourney = true
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper { ReturnNoBundles = true })
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            viewModel.ShowTabs.ShouldEqual(showTabs);
        }

        [TestCase(false, true, "", "Energy", null, null, "")]
        [TestCase(true, true, "", "Energy", null, null, "")]
        [TestCase(false, false, "", "View all", ".available-packages-container .bundle-card-wrapper, .available-packages-container .tariff-wrapper", "", "")]
        [TestCase(true, false, "", "Bundles", ".available-packages-container .bundle-card-wrapper", ".available-packages-container .tariff-wrapper", "")]
        [TestCase(false, false, "Fix and Fibre Bundle", "Bundles", ".available-packages-container .bundle-card-wrapper", ".available-packages-container .tariff-wrapper", "Bundles")]
        [TestCase(false, false, "Standard", "View all", ".available-packages-container .bundle-card-wrapper, .available-packages-container .tariff-wrapper", "", "Energy")]
        public void ShouldSetDisplayTypeCorrectly(
            bool isBundlingJourney, 
            bool excludeBundles, 
            string chosenProduct, 
            string expectedDisplayType, 
            string expectedInitialShowTariffs, 
            string expectedInitialHideTariffs, 
            string expectedChosenTariffType)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { HouseName = "Town House", Town = "Havant" },
                IsBundlingJourney = isBundlingJourney,
                ChosenProduct = chosenProduct
            });

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper { ReturnNoBundles = excludeBundles })
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            viewModel.DisplayType.ShouldEqual(expectedDisplayType);
            viewModel.InitialShowTariffs.ShouldEqual(expectedInitialShowTariffs);
            viewModel.InitialHideTariffs.ShouldEqual(expectedInitialHideTariffs);
            viewModel.ChosenTariffType.ShouldEqual(expectedChosenTariffType);
        }

        [Test]
        public void ShouldShowCorrectSubHeaderTextWhenChosenProductIsABundleAndIsAvailable()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { HouseName = "Town House", Town = "Havant" },
                ChosenProduct = "Fix and Fibre Bundle",
                IsBundlingJourney = true
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            viewModel.SubHeaderText.ShouldEqual("Your chosen Bundle");
            viewModel.SubHeaderParagraph.ShouldEqual("All prices on this page include VAT and any applicable discounts.");
            viewModel.ShowYouMightBeInterestedInText.ShouldBeTrue();
            viewModel.YouMightBeInterestedInText.ShouldEqual("You might also be interested in");
        }

        [TestCase("Sorry, our Bundles aren’t currently available in your area", "But you might be interested in some of our energy tariffs below. All prices on this page include VAT and any applicable discounts.", true)]
        [TestCase("Sorry, your chosen Bundle isn’t currently available in your area", "But you might be interested in some of our other products below. All prices on this page include VAT and any applicable discounts.", false)]
        public void ShouldShowCorrectSubHeaderTextWhenChosenProductIsABundleAndIsNotAvailable(string expectedSubHeader, string expectedSubParagraph, bool returnSingleBundle)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { HouseName = "Town House", Town = "Havant" },
                UnavailableBundles = new List<string> { "BO001" }
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper { ReturnSingleBundle = returnSingleBundle })
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            viewModel.SubHeaderText.ShouldEqual(expectedSubHeader);
            viewModel.SubHeaderParagraph.ShouldEqual(expectedSubParagraph);
            viewModel.ShowYouMightBeInterestedInText.ShouldBeTrue();
            viewModel.YouMightBeInterestedInText.ShouldEqual("You might also be interested in");
        }

        [Test]
        public void ShouldShowCorrectSubHeaderTextWhenChosenProductIsAnEnergyTariffAndIsAvailable()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { HouseName = "Town House", Town = "Havant" },
                ChosenProduct = "Standard",
                IsBundlingJourney = false
            });

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            viewModel.SubHeaderText.ShouldEqual("Choose your product");
            viewModel.SubHeaderParagraph.ShouldEqual("All prices on this page include VAT and any applicable discounts.");
            viewModel.ShowYouMightBeInterestedInText.ShouldBeTrue();
            viewModel.YouMightBeInterestedInText.ShouldEqual("You might also be interested in");
        }

        [Test]
        public void ShouldShowCorrectSubHeaderTextWhenChosenProductIsAnEnergyTariffAndIsNotAvailable()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                ChosenProduct = "Test Tariff description"
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<TariffsController>();
            
            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            viewModel.HasChosenTariff.ShouldEqual(true);
            viewModel.ChosenTariff.ShouldBeNull();
            viewModel.IsChosenTariffAvailable.ShouldEqual(false);
            viewModel.SubHeaderText.ShouldEqual("Sorry, your chosen tariff isn’t currently available in your area");
            viewModel.SubHeaderParagraph.ShouldEqual("Sorry, but based on the information provided you won’t be able to get the tariff you selected.");
            viewModel.ShowYouMightBeInterestedInText.ShouldBeTrue();
            viewModel.YouMightBeInterestedInText.ShouldEqual("You might also be interested in");
        }

        [TestCase("Bundles", ".available-packages-container .bundle-card-wrapper", ".available-packages-container .tariff-wrapper", "", "View all", ".available-packages-container .bundle-card-wrapper, .available-packages-container .tariff-wrapper", "active", false)]
        [TestCase("Bundles", ".available-packages-container .bundle-card-wrapper", ".available-packages-container .tariff-wrapper", "active", "View all", ".available-packages-container .bundle-card-wrapper, .available-packages-container .tariff-wrapper", "", true)]
        public void ShouldShowCorrectTabsWhenChosenProductIsNullAndAllTariffsAreAvailable(
            string firstTabLabel,
            string firstTabShowTariffSelector,
            string firstTabHideTariffSelector,
            string firstTabActiveClass,
            string lastTabLabel,
            string lastTabShowTariffSelector,
            string lastTabActiveClass,
            bool isBundlingJourney)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { HouseName = "Town House", Town = "Havant" },
                ChosenProduct = null,
                IsBundlingJourney = isBundlingJourney
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            viewModel.FirstTabLabel.ShouldEqual(firstTabLabel);
            viewModel.FirstTabShowTariffSelector.ShouldEqual(firstTabShowTariffSelector);
            viewModel.FirstTabHideTariffSelector.ShouldEqual(firstTabHideTariffSelector);
            viewModel.FirstTabActiveClass.ShouldEqual(firstTabActiveClass);
            viewModel.MiddleTabLabel.ShouldEqual("Energy tariffs");
            viewModel.LastTabLabel.ShouldEqual(lastTabLabel);
            viewModel.LastTabShowTariffSelector.ShouldEqual(lastTabShowTariffSelector);
            viewModel.LastTabActiveClass.ShouldEqual(lastTabActiveClass);
        }


        [TestCase(FuelType.Dual, PaymentMethod.Quarterly)]
        [TestCase(FuelType.Gas, PaymentMethod.Quarterly)]
        public void ShouldShowCorrectSubHeaderTextWhenChosenProductIsNullAndInABundlingJourneyButNoInitialBundles(FuelType fuelType, PaymentMethod paymentMethod)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = fuelType,
                SelectedPaymentMethod = paymentMethod,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { HouseName = "Town House", Town = "Havant" },
                IsBundlingJourney = true
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper { ReturnNoBundles = true })
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            viewModel.SubHeaderText.ShouldEqual(AvailableTariffs_Resources.AllBundlesUnavailableHeader);
            viewModel.SubHeaderParagraph.ShouldEqual(AvailableTariffs_Resources.AllBundlesUnavailablePara);
            viewModel.ShowYouMightBeInterestedInText.ShouldBeTrue();
            viewModel.YouMightBeInterestedInText.ShouldEqual("You might also be interested in");
        }

        [TestCase(FuelType.Gas, "Your estimated annual gas costs will be £1,000. Payment by Direct Debit only." )]
        [TestCase(FuelType.Dual, "Your estimated annual energy costs will be £2,000. That’s £1,000 for gas and £1,000 for electricity. Payment by Direct Debit only.")]
        public void ShouldHaveDetailsForFixNProtectBundle(FuelType fuelType,
            string bundleDisclaimerText2)
        { 
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = fuelType,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { HouseName = "Town House", Town = "Havant" },
                IsBundlingJourney = true
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper { ReturnFixNProtectBundle = true })
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            TariffsViewModel fixNProtectBundle = viewModel.AllTariffs.FirstOrDefault(t => t.BundlePackageType == BundlePackageType.FixAndProtect);
            fixNProtectBundle.ShouldNotBeNull();
            fixNProtectBundle?.HesMoreInformation.ShouldNotBeNull();
            fixNProtectBundle?.BundlePackageIconFileName.ShouldEqual("hes-icon.svg");
            fixNProtectBundle?.BundlePackageDisplayText.ShouldEqual("Heating Breakdown");
            fixNProtectBundle?.BundlePackagePriceLbl.ShouldEqual("Free");
            fixNProtectBundle?.ProjectedBundlePackageMonthlyCost.ShouldEqual("Free");
            fixNProtectBundle?.BundleDisclaimer1Text.ShouldEqual("£114 saving is based on Heating Breakdown being free for 12 months compared to the standard price when bought separately (£9.50 a month): £9.50 x 12 = £114.");
            fixNProtectBundle?.BundleDisclaimer2Text.ShouldEqual(bundleDisclaimerText2);

            // FixNFibre bundle 'More Information' modal property tests.
            fixNProtectBundle?.MoreInformationModalId.ShouldEqual("#FixNProtectBundleMegaModal");
            fixNProtectBundle?.HesMoreInformation.BundleDisclaimerModalText.ShouldEqual("Save £114 over 12 months compared to buying this product separately.");
            fixNProtectBundle?.HesMoreInformation.OriginalFixNProtectMonthlyCost.ShouldEqual("£9.50");
            fixNProtectBundle?.HesMoreInformation.ProjectedMonthlySavingsAmount.ShouldEqual("£9.50");
            fixNProtectBundle?.HesMoreInformation.ExcessAmount.ShouldEqual("£90");
            fixNProtectBundle?.HesMoreInformation.ExcessText.ShouldEqual("An excess is the amount you'll need to pay for each claim you make. So you'd pay £90 each time you make a claim.");
            fixNProtectBundle?.HesMoreInformation.WhatsExcluded.Count.ShouldEqual(2);
            fixNProtectBundle?.HesMoreInformation.WhatsExcluded.Count.ShouldEqual(2);
        }

        [TestCase(FuelType.Electricity)]
        public void ShouldNotListFixNProtectBundle(FuelType fuelType)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = fuelType,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { HouseName = "Town House", Town = "Havant" },
                IsBundlingJourney = true
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper { ReturnFixNProtectBundle = true })
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            TariffsViewModel fixNProtectBundle = viewModel.AllTariffs.FirstOrDefault(t => t.BundlePackageType == BundlePackageType.FixAndProtect);
            fixNProtectBundle.ShouldBeNull();
        }

        [Test]
        public void ShouldShowCorrectTabsWhenChosenProductIsABundleAndIsAvailable()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { HouseName = "Town House", Town = "Havant" },
                ChosenProduct = "Fix and Fibre Bundle",
                IsBundlingJourney = true
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            viewModel.FirstTabLabel.ShouldEqual("Bundles");
            viewModel.FirstTabShowTariffSelector.ShouldEqual(".available-packages-container .bundle-card-wrapper");
            viewModel.FirstTabHideTariffSelector.ShouldEqual(".available-packages-container .tariff-wrapper");
            viewModel.FirstTabActiveClass.ShouldEqual("active");
            viewModel.MiddleTabLabel.ShouldEqual("Energy tariffs");
            viewModel.LastTabLabel.ShouldEqual("View all");
            viewModel.LastTabShowTariffSelector.ShouldEqual(".available-packages-container .bundle-card-wrapper, .available-packages-container .tariff-wrapper");
            viewModel.LastTabActiveClass.ShouldEqual("");
        }

        [Test]
        public void ShouldHaveCorrectFixNFibreBundlePackageDetails()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { HouseName = "Town House", Town = "Havant" },
                ChosenProduct = "Fix and Fibre Bundle",
                IsBundlingJourney = true
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();

            TariffsViewModel fixNFibreBundle = viewModel.AllTariffs.FirstOrDefault(t => t.BundlePackageType == BundlePackageType.FixAndFibre);
            fixNFibreBundle?.BundlePackageIconFileName.ShouldEqual("broadband.svg");
            fixNFibreBundle?.BundlePackageDisplayText.ShouldEqual("Fibre broadband");
            fixNFibreBundle?.BundlePackagePriceLbl.ShouldEqual("40% OFF");
            fixNFibreBundle?.ProjectedBundlePackageMonthlyCost.ShouldEqual("£23");
            fixNFibreBundle?.BundleDisclaimer1Text.ShouldEqual("£180 saving is based on £10 (30%) discount for 18 months for fibre broadband (18 x £10 = £180), when compared to buying Unlimited Fibre, our equivalent standalone broadband product (£33).");
            fixNFibreBundle?.BundleDisclaimer2Text.ShouldEqual("Your estimated annual energy costs will be £2,000. That’s £1,000 for gas and £1,000 for electricity. Payment by Direct Debit only.");

            // FixNFibre bundle 'More Information' modal property tests.
            fixNFibreBundle?.MoreInformationModalId.ShouldEqual("#BroadbandBundleMegaModal");
            fixNFibreBundle?.BroadbandMoreInformation.BroadbandPackageSpeed.HeaderText.ShouldEqual("Average download speed");
            fixNFibreBundle?.BroadbandMoreInformation.BroadbandPackageSpeed.ShowHeaderText.ShouldBeTrue();
            fixNFibreBundle?.BroadbandMoreInformation.BroadbandPackageSpeed.PostCode.ShouldBeNull();
            fixNFibreBundle?.BroadbandMoreInformation.BroadbandPackageSpeed.MaxDownload.ShouldEqual("35");
            fixNFibreBundle?.BroadbandMoreInformation.BroadbandPackageSpeed.MinDownload.ShouldBeNull();
            fixNFibreBundle?.BroadbandMoreInformation.BroadbandPackageSpeed.PackageDescription.ShouldEqual("This package is great for streaming, downloading large files, watching catch-up TV or online gaming.");
        }

        [Test]
        public void ShouldShowCorrectTabsWhenChosenProductIsABundleAndIsNotAvailable()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { HouseName = "Town House", Town = "Havant" },
                ChosenProduct = "Fix and Fibre Bundle",
                IsBundlingJourney = true
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            viewModel.FirstTabLabel.ShouldEqual("Bundles");
            viewModel.FirstTabShowTariffSelector.ShouldEqual(".available-packages-container .bundle-card-wrapper");
            viewModel.FirstTabHideTariffSelector.ShouldEqual(".available-packages-container .tariff-wrapper");
            viewModel.FirstTabActiveClass.ShouldEqual("active");
            viewModel.MiddleTabLabel.ShouldEqual("Energy tariffs");
            viewModel.LastTabLabel.ShouldEqual("View all");
            viewModel.LastTabShowTariffSelector.ShouldEqual(".available-packages-container .bundle-card-wrapper, .available-packages-container .tariff-wrapper");
            viewModel.LastTabActiveClass.ShouldEqual("");
        }

        [Test]
        public void ShouldShowCorrectTabsWhenChosenProductIsAnEnergyTariffAndIsAvailable()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { HouseName = "Town House", Town = "Havant" },
                ChosenProduct = "Standard",
                IsBundlingJourney = false
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            viewModel.FirstTabLabel.ShouldEqual("Bundles");
            viewModel.FirstTabShowTariffSelector.ShouldEqual(".available-packages-container .bundle-card-wrapper");
            viewModel.FirstTabHideTariffSelector.ShouldEqual(".available-packages-container .tariff-wrapper");
            viewModel.FirstTabActiveClass.ShouldEqual(string.Empty);
            viewModel.MiddleTabLabel.ShouldEqual("Energy tariffs");
            viewModel.LastTabLabel.ShouldEqual("View all");
            viewModel.LastTabShowTariffSelector.ShouldEqual(".available-packages-container .bundle-card-wrapper, .available-packages-container .tariff-wrapper");
            viewModel.LastTabActiveClass.ShouldEqual("active");
        }

        [Test]
        public void ShouldShowCorrectTabsWhenChosenProductIsAnEnergyTariffAndIsNotAvailable()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                ChosenProduct = "Test Tariff description"
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            viewModel.FirstTabLabel.ShouldEqual("Bundles");
            viewModel.FirstTabShowTariffSelector.ShouldEqual(".available-packages-container .bundle-card-wrapper");
            viewModel.FirstTabHideTariffSelector.ShouldEqual(".available-packages-container .tariff-wrapper");
            viewModel.FirstTabActiveClass.ShouldEqual(string.Empty);
            viewModel.MiddleTabLabel.ShouldEqual("Energy tariffs");
            viewModel.LastTabLabel.ShouldEqual("View all");
            viewModel.LastTabShowTariffSelector.ShouldEqual(".available-packages-container .bundle-card-wrapper, .available-packages-container .tariff-wrapper");
            viewModel.LastTabActiveClass.ShouldEqual("active");
        }

        [Test]
        public void ShouldDisplayYourChosenTariffSectionAndRemoveChosenTariffFromAvailableTariffsWhenCustomerSelectedTariffFromHubPageIsAvailable()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                ChosenProduct = "Standard"
            });

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            viewModel.HasChosenTariff.ShouldBeTrue();
            viewModel.ChosenTariff.ShouldNotBeNull();
            viewModel.IsChosenTariffAvailable.ShouldBeTrue();
            viewModel.ChosenTariff.DisplayName.ShouldEqual("Standard tariff");
            viewModel.AllTariffs.FirstOrDefault(t => t.DisplayName == viewModel.ChosenTariff.DisplayName).ShouldBeNull();
        }

        [Test]
        public void ShouldDisplayYourChosenTariffWithCorrectDetailsHeaderWhenCustomerSelectedTariffFromHubPageIsAvailable()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                ChosenProduct = "Fix and Fibre Bundle",
                IsBundlingJourney = true
            });
            var fakeTariffManager = new FakeTariffManager();
            fakeTariffManager.TariffGroupMappings.Add("ME724", TariffGroup.FixAndFibre.ToString());
            fakeTariffManager.TariffGroupMappings.Add("MG101", TariffGroup.FixAndFibre.ToString());
            fakeTariffManager.TariffGroupMappings.Add("ME123", TariffGroup.FixAndFibre.ToString());
            fakeTariffManager.TariffGroupMappings.Add("MG123", TariffGroup.FixAndFibre.ToString());

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffManager(fakeTariffManager)
                .WithSessionManager(fakeSessionManager)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            viewModel.ChosenTariff.DisplayName.ShouldEqual("Fix and Fibre Bundle Bundle");
            viewModel.ChosenTariff.DetailsHeaderIconClass.ShouldEqual("collapse");
            viewModel.ChosenTariff.DetailsHeader.ShouldEqual("Show less");
            viewModel.ChosenTariff.IsDataShown.ShouldBeTrue();
            foreach (TariffsViewModel tariffViewModel in viewModel.AllTariffs)
            {
                tariffViewModel.DetailsHeaderIconClass.ShouldEqual("expand");
                tariffViewModel.DetailsHeader.ShouldEqual(tariffViewModel.IsBundle ? "Show more" : null);
                tariffViewModel.IsDataShown.ShouldBeFalse();
            }
        }

        [Test]
        public void ShouldRemainOnAvailableTariffsPageWhenSelectedTariffIsNotFound()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true
            });
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithContextManager(FakeContextManagerFactory.Default())
                .Build<TariffsController>();

            // Act
            // ReSharper disable once UnusedVariable
            ActionResult result1 =  controller.AvailableTariffs().Result;
            ActionResult result = controller.AvailableTariffs("1234567890").Result;

            // Assert
            var jsonResult = result.ShouldBeType<JsonResult>();
            jsonResult.Data.ShouldNotBeNull();
            jsonResult.Data.ToString().ShouldContain("/AvailableTariffs");
        }


        [Test]
        public void ShouldRedirectToPhonePackageViewWhenSelectedTariffIsBundleIncludingBroadbandComponent()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" }
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithContextManager(FakeContextManagerFactory.Default())
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false })
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper())
                .Build<TariffsController>();

            // Act
            // ReSharper disable once UnusedVariable
            ActionResult result1 = controller.AvailableTariffs().Result;
            ActionResult result = controller.AvailableTariffs("BO001").Result;

            // Assert
            var jsonResult = result.ShouldBeType<JsonResult>();
            jsonResult.Data.ShouldNotBeNull();
            jsonResult.Data.ToString().ShouldContain("http://localhost/enery-journey/SignUp/PhonePackage");
            var addresses = fakeSessionManager.GetSessionDetails<List<BTAddress>>(SessionKeys.BTAddressListForPostCode);
            addresses.ShouldBeNull();
        }

        [Test]
        public void ShouldUpdateYourPriceViewModelWhenSelectedTariffIsBundleIncludingBroadbandComponent()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" }
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithContextManager(FakeContextManagerFactory.Default())
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false, InstallLine = true})
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper())
                .Build<TariffsController>();

            // Act
            // ReSharper disable once UnusedVariable
            ActionResult result1 = controller.AvailableTariffs().Result;
            ActionResult result = controller.AvailableTariffs("BO001").Result;

            // Assert
            var jsonResult = result.ShouldBeType<JsonResult>();
            jsonResult.Data.ShouldNotBeNull();
            jsonResult.Data.ToString().ShouldContain("http://localhost/enery-journey/SignUp/PhonePackage");

            var yourPrice = fakeSessionManager.GetSessionDetails<YourPriceViewModel>(SessionKeys.EnergyYourPriceDetails);
            yourPrice.ShouldNotBeNull();
            yourPrice.BroadbandApplyInstallationFee.ShouldEqual(true);
            yourPrice.BundlePackageFeatures.ShouldContain("Line rental included");
            yourPrice.BundlePackageFeatures.ShouldContain("18-month contract");
        }

        [Test]
        public void ShouldRedirectToPersonalDetailsViewWhenSelectedTariffIsNotBundleIncludingBroadbandComponent()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" }
            });

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithContextManager(FakeContextManagerFactory.Default())
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false })
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper())
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            // ReSharper disable once UnusedVariable
            ActionResult result1 = controller.AvailableTariffs().Result;
            ActionResult result =  controller.AvailableTariffs("001").Result;

            // Assert
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("PersonalDetails");
            redirectResult.RouteValues["controller"].ShouldEqual("SignUp");
        }

        [Test]
        public void ShouldStoreSelectedTariffInTheSession()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());

            const FuelType fuelType = FuelType.Dual;
            const ElectricityMeterType meterType = ElectricityMeterType.Standard;
            const string selectedTariffId = "001";

            Tariff selectedProduct = FakeProductsStub.GetSelectedProduct(fuelType.ToString(), meterType.ToDescription(), selectedTariffId);

            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = meterType,
                SelectedFuelType = fuelType,
                SelectedPaymentMethod = PaymentMethod.PayAsYouGo,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true
            });

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            // ReSharper disable once UnusedVariable
            ActionResult x = controller.AvailableTariffs().Result;
            // ReSharper disable once UnusedVariable
            ActionResult y = controller.AvailableTariffs(selectedTariffId).Result;

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.SelectedTariff.TariffId.ShouldEqual(selectedProduct.TariffId);
            customer.SelectedTariff.DisplayName.ShouldEqual(selectedProduct.DisplayName);
            customer.SelectedTariff.GasProduct.ServicePlanId.ShouldEqual(selectedProduct.GasProduct.ServicePlanId);
            customer.SelectedTariff.ElectricityProduct.ServicePlanId.ShouldEqual(selectedProduct.ElectricityProduct.ServicePlanId);
        }

        [Test]
        public void ShouldPopulateAndStoreYourPriceDetails()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());

            const FuelType fuelType = FuelType.Dual;
            const ElectricityMeterType meterType = ElectricityMeterType.Standard;
            const string selectedTariffId = "001";

            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = meterType,
                SelectedFuelType = fuelType,
                SelectedPaymentMethod = PaymentMethod.PayAsYouGo,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" }
            });

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            // ReSharper disable once UnusedVariable
            ActionResult x =  controller.AvailableTariffs().Result;
            // ReSharper disable once UnusedVariable
            ActionResult y =  controller.AvailableTariffs(selectedTariffId).Result;

            // Assert
            var yourPrice = fakeSessionManager.GetSessionDetails<YourPriceViewModel>(SessionKeys.EnergyYourPriceDetails);
            yourPrice.ShouldNotBeNull();
            yourPrice.GasPerMonth.ShouldEqual("£100.00");
            yourPrice.ElectricityPerMonth.ShouldEqual("£83.33");
            yourPrice.ProjectedMonthlyTotalFullValue.ShouldEqual("£183");
            yourPrice.ProjectedMonthlyTotalPenceValue.ShouldEqual(".33");
        }

        [TestCase(PaymentMethod.PayAsYouGo, FuelType.Dual, TariffType.Fixed, null)]
        [TestCase(PaymentMethod.Quarterly, FuelType.Dual, TariffType.Fixed, null)]
        [TestCase(PaymentMethod.MonthlyDirectDebit, FuelType.Dual, TariffType.Fixed, "Includes £80 discount per year (£40 for gas and £40 for electricity)")]
        [TestCase(PaymentMethod.MonthlyDirectDebit, FuelType.Electricity, TariffType.Fixed, "Includes £40 discount per year")]
        [TestCase(PaymentMethod.MonthlyDirectDebit, FuelType.Gas, TariffType.Fixed, "Includes £40 discount per year")]
        [TestCase(PaymentMethod.MonthlyDirectDebit, FuelType.Dual, TariffType.Evergreen, "Includes £30.40 discount per year (£20.20 for gas and £10.20 for electricity)")]
        [TestCase(PaymentMethod.MonthlyDirectDebit, FuelType.Electricity, TariffType.Evergreen, "Includes £10.20 discount per year")]
        public void ShouldPopulateDiscountInYourPriceViewModelWhenApplicable(PaymentMethod paymentMethod, FuelType fuelType, TariffType tariffType, string expectedDiscount)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var energyCustomer = new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = fuelType,
                SelectedPaymentMethod = paymentMethod,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" }
            };

            Tariff selectedProduct = FakeProductsStub.GetTariffByTariffType(fuelType.ToString(), energyCustomer.SelectedElectricityMeterType.ToDescription(), tariffType);

            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, energyCustomer);

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            // ReSharper disable once UnusedVariable
            ActionResult tariffResult = controller.AvailableTariffs().Result;
            // ReSharper disable once UnusedVariable
            ActionResult tariffResult2 = controller.AvailableTariffs(selectedProduct.TariffId).Result;

            // Assert
            var yourPrice = fakeSessionManager.GetSessionDetails<YourPriceViewModel>(SessionKeys.EnergyYourPriceDetails);
            yourPrice.ShouldNotBeNull();
            yourPrice.Discount.ShouldEqual(expectedDiscount);
        }

        [TestCase(TariffGroup.FixAndProtect, "<p>Includes 1 year free SSE Heating Breakdown Cover worth £114 (£9.50 per month) – £90 excess per call out and other terms and conditions apply</p>")]
        [TestCase(TariffGroup.FixAndFibre, null)]
        public void ShouldPopulateExtraInYourPriceViewModelWhenApplicable(TariffGroup tariffGroup, string expectedExtra)
        {
            // Arrange
            const FuelType fuelType = FuelType.Dual;
            const string selectedTariffId = "672";

            var fakeSessionManager = new FakeSessionManager();
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var energyCustomer = new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = fuelType,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" }
            };

            FakeProductsStub.GetSelectedProduct(fuelType.ToString(), energyCustomer.SelectedElectricityMeterType.ToDescription(), selectedTariffId);

            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, energyCustomer);

            var fakeConfigurationSettings = new FakeConfigurationSettings();
            var fakeConfigManager = new FakeConfigManager();
            if (tariffGroup == TariffGroup.FixAndFibre)
            {
                fakeConfigManager.AddConfiguration("tariffManagement", "availableTariffPdfsAltText", "blah", "blah");
                fakeConfigManager.AddConfiguration("tariffManagement", "tariffGroups", "MG101", "FixAndFibre");
                fakeConfigManager.AddConfiguration("tariffManagement", "tariffGroups", "ME724", "FixAndFibre");
                fakeConfigurationSettings.TariffGroupSettings = new Dictionary<string, TariffGroup>
                {
                    { "MG101", TariffGroup.FixAndFibre },
                    { "ME724", TariffGroup.FixAndFibre }
                };
            }

            if (tariffGroup == TariffGroup.FixAndProtect)
            {
                fakeConfigManager.AddConfiguration("tariffManagement", "tariffGroups", "MG095", "FixAndProtect");
                fakeConfigManager.AddConfiguration("tariffManagement", "tariffGroups", "ME672", "FixAndProtect");
                fakeConfigurationSettings.TariffGroupSettings = new Dictionary<string, TariffGroup>
                {
                    { "MG095", TariffGroup.FixAndProtect },
                    { "ME672", TariffGroup.FixAndProtect }
                };
            }

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithTariffManager(new TariffManager(fakeConfigManager, fakeConfigurationSettings))
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            // ReSharper disable once UnusedVariable
            ActionResult result =  controller.AvailableTariffs().Result;
            // ReSharper disable once UnusedVariable
            ActionResult x = controller.AvailableTariffs(selectedTariffId).Result;

            // Assert
            var yourPrice = fakeSessionManager.GetSessionDetails<YourPriceViewModel>(SessionKeys.EnergyYourPriceDetails);
            yourPrice.ShouldNotBeNull();
            yourPrice.Extra.ShouldEqual(expectedExtra);
        }

        [TestCase(ElectricityMeterType.Economy7, true)]
        [TestCase(ElectricityMeterType.Standard, false)]
        public void ShouldDisplayEconomy7SectionWhenCustomerSelectedEconomy7MeterType(ElectricityMeterType meterType, bool e7Flag)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = meterType,
                SelectedFuelType = FuelType.Electricity,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            viewModel.HasE7Meter.ShouldEqual(e7Flag); // View contains logic to display the section based on this flag.
        }

        [Test]
        [TestCase(ElectricityMeterType.Economy7, FuelType.Dual, false,
            "The costs below are based on the estimated usage we worked out from the answers you gave us about your property. We estimate you’ll use: <strong> Gas = 500 kWh per year&nbsp;&nbsp; Electricity day = 200 kWh per year  &nbsp; Electricity night = 100 kWh per year</strong>.<br/>These figures are based on 58% usage during the day and 42% usage at night.")]
        [TestCase(ElectricityMeterType.Economy7, FuelType.Electricity, false,
            "The costs below are based on the estimated usage we worked out from the answers you gave us about your property. We estimate you’ll use: <strong>  Electricity day = 200 kWh per year  &nbsp; Electricity night = 100 kWh per year</strong>.<br/>These figures are based on 58% usage during the day and 42% usage at night.")]
        [TestCase(ElectricityMeterType.Standard, FuelType.Electricity, false,
            "The costs below are based on the estimated usage we worked out from the answers you gave us about your property. We estimate you’ll use:  <strong> Electricity = 400 kWh per year </strong>.")]
        [TestCase(ElectricityMeterType.Standard, FuelType.Gas, false,
            "The costs below are based on the estimated usage we worked out from the answers you gave us about your property. We estimate you’ll use:  <strong> Gas = 500 kWh per year </strong>.")]
        [TestCase(ElectricityMeterType.Standard, FuelType.Dual, false,
            "The costs below are based on the estimated usage we worked out from the answers you gave us about your property. We estimate you’ll use:  <strong> Gas = 500 kWh per year&nbsp;&nbsp; Electricity = 400 kWh per year </strong>.")]
        [TestCase(ElectricityMeterType.Standard, FuelType.Dual, true, "The costs below are based on the usage you just gave us.")]
        public void ShouldDisplayAppropriateUsageHeaderText(ElectricityMeterType meterType, FuelType fuelType,
            bool isUsageKnown, string expectedText)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = meterType,
                SelectedFuelType = fuelType,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = isUsageKnown
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            Assert.AreEqual(expectedText, viewModel.HeaderParagraph);
        }

        [Test]
        public void ShouldSeeAllTheAvailableTariffsWhenCustomerIsOnTariffsPage()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                Projection = new Projection
                {
                    EnergyAveStandardGasKwh = 1000,
                    EnergyAveStandardElecKwh = 2000
                }
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;
            var viewModel = (AvailableTariffsViewModel) ((ViewResult) result).Model;

            // Assert
            viewModel.ShouldNotBeNull();
        }

        [Test]
        public void ShouldSetTariffGroup()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true
            });

            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("tariffManagement", "tariffGroups", "MG098", "FixAndProtect");
            fakeConfigManager.AddConfiguration("tariffManagement", "tariffGroups", "ME697", "FixAndProtect");

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithTariffManager(new TariffManager(fakeConfigManager, new FakeConfigurationSettings()))
                .WithConfigManager(fakeConfigManager)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            model.EnergyTariffs.Count(t => t.TariffGroup == TariffGroup.FixAndProtect).ShouldEqual(1);
        }

        [Test]
        public void ShouldSetTariffGroupForBundle()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());

            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true
            });

            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("tariffManagement", "tariffGroups", "MG101", "FixAndFibre");
            fakeConfigManager.AddConfiguration("tariffManagement", "tariffGroups", "ME724", "FixAndFibre");

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithTariffManager(new TariffManager(fakeConfigManager, new FakeConfigurationSettings()))
                .WithConfigManager(fakeConfigManager)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            model.BundleTariffs.Count(t => t.TariffGroup == TariffGroup.FixAndFibre).ShouldEqual(1);
        }

        [Test]
        public async Task ShouldRedirectToGenericFalloutPageWhenQuotationApiThrowsException()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                Projection = new Projection
                {
                    EnergyAveStandardGasKwh = 1000,
                    EnergyAveStandardElecKwh = 2000
                }
            });

            var fakeEnergyQuotationServiceWrapper = new FakeProductServiceWrapper { ThrowException = true };

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithEnergyProductServiceWrapper(fakeEnergyQuotationServiceWrapper)
                .Build<TariffsController>();

            // Act
            ActionResult result = await controller.AvailableTariffs();

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("UnableToComplete");
            routeResult.RouteValues["controller"].ShouldEqual("SignUp");
        }

        private static IEnumerable<TestCaseData> ProductRequestTestData()
        {
            yield return new TestCaseData("PO9 1QH", PaymentMethod.MonthlyDirectDebit, FuelType.Electricity,
                ElectricityMeterType.Economy7, null, null, 1000.0, 2000.0, null, null, "NonPrepay", "DD", "Electricity",
                "Economy 7", 1000.0, 2000.0, "Paperless");
            yield return new TestCaseData("PO9 1QH", PaymentMethod.MonthlyDirectDebit, FuelType.Dual,
                ElectricityMeterType.Economy7, null, 1500.0, 1000.0, 2000.0, null, 1500.0, "NonPrepay", "DD", "Dual", "Economy 7",
                1000.0, 2000.0, "Paperless");
            yield return new TestCaseData("PO9 1QH", PaymentMethod.MonthlyDirectDebit, FuelType.Gas, ElectricityMeterType.Standard,
                null, 1500.0, null, null, null, 1500.0, "NonPrepay", "DD", "Gas", "Standard", null, null, "Paperless");
            yield return new TestCaseData("PO9 1QH", PaymentMethod.MonthlyDirectDebit, FuelType.Electricity,
                ElectricityMeterType.Standard, 1000.0, null, null, null, 1000.0, null, "NonPrepay", "DD", "Electricity",
                "Standard", null, null, "Paperless");
            yield return new TestCaseData("PO9 1QH", PaymentMethod.MonthlyDirectDebit, FuelType.Dual,
                ElectricityMeterType.Standard, 1000.0, 1500.0, null, null, 1000.0, 1500.0, "NonPrepay", "DD", "Dual", "Standard",
                null, null, "Paperless");
            yield return new TestCaseData("PO9 1QH", PaymentMethod.Quarterly, FuelType.Dual, ElectricityMeterType.Standard, 1000.0,
                1500.0, null, null, 1000.0, 1500.0, "NonPrepay", "QC", "Dual", "Standard", null, null, "Paperless");
            yield return new TestCaseData("PO9 1QH", PaymentMethod.PayAsYouGo, FuelType.Dual, ElectricityMeterType.Standard,
                1000.0, 1500.0, null, null, 1000.0, 1500.0, "Prepay", "QC", "Dual", "Standard", null, null, "Paper");
        }

        [Test]
        [TestCaseSource(nameof(ProductRequestTestData))]
        public async Task ProductRequestShouldBePopulatedWithCorrectParameterValues(
            string postcode, 
            PaymentMethod paymentMethod, 
            FuelType fuelType, 
            ElectricityMeterType meterType,
            double? elecProjection, 
            double? gasProjection,
            double? energyEconomy7DayElecKwh, 
            double? energyEconomy7NightElecKwh,
            double? elecExpectedProjection, 
            double? gasExpectedProjection,
            string expectedAccountType, 
            string expectedPaymentType, 
            string expectedFuelType, 
            string expectedMeterType,
            double? expectedEnergyEconomy7DayElecKwh, 
            double? expectedEnergyEconomy7NightElecKwh,
            string expectedBillingPreference)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = postcode,
                SelectedPaymentMethod = paymentMethod,
                SelectedElectricityMeterType = meterType,
                SelectedFuelType = fuelType,
                Projection = new Projection
                {
                    EnergyAveStandardGasKwh = gasProjection,
                    EnergyAveStandardElecKwh = elecProjection,
                    EnergyEconomy7DayElecKwh = energyEconomy7DayElecKwh,
                    EnergyEconomy7NightElecKwh = energyEconomy7NightElecKwh
                }
            });

            var fakeEnergyProductServiceWrapper = new FakeProductServiceWrapper();
            var fakeBundleTariffServiceWrapper = new FakeBundleTariffServiceWrapper();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(FakeConfigManagerFactory.DefaultBundling())
                .WithEnergyProductServiceWrapper(fakeEnergyProductServiceWrapper)
                .WithBundleTariffServiceWrapper(fakeBundleTariffServiceWrapper)
                .Build<TariffsController>();

            // Act
            ActionResult result = await controller.AvailableTariffs();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();
            viewModel.ShouldNotBeNull();
            Assert.AreEqual(fakeEnergyProductServiceWrapper.ProductsRequest.PostCode, postcode);
            Assert.AreEqual(fakeEnergyProductServiceWrapper.ProductsRequest.AccountType, expectedAccountType);
            Assert.AreEqual(fakeEnergyProductServiceWrapper.ProductsRequest.PaymentType, expectedPaymentType);
            Assert.AreEqual(fakeEnergyProductServiceWrapper.ProductsRequest.MeterType, expectedMeterType);
            Assert.AreEqual(fakeEnergyProductServiceWrapper.ProductsRequest.FuelType, expectedFuelType);
            Assert.AreEqual(fakeEnergyProductServiceWrapper.ProductsRequest.StandardElectricityKwh,
                elecExpectedProjection);
            Assert.AreEqual(fakeEnergyProductServiceWrapper.ProductsRequest.StandardGasKwh, gasExpectedProjection);
            Assert.AreEqual(fakeEnergyProductServiceWrapper.ProductsRequest.Economy7ElectricityDayKwh,
                expectedEnergyEconomy7DayElecKwh);
            Assert.AreEqual(fakeEnergyProductServiceWrapper.ProductsRequest.Economy7ElectricityNightKwh,
                expectedEnergyEconomy7NightElecKwh);
            Assert.AreEqual(fakeEnergyProductServiceWrapper.ProductsRequest.BillingPreference,
                expectedBillingPreference);
        }

        [TestCase("PO9 1QH", PaymentMethod.MonthlyDirectDebit, FuelType.Electricity, ElectricityMeterType.Economy7, null, null, 1000.0, 2000.0)]
        public async Task ShouldContainBundleTariffForElectricityE7Meter(
            string postcode
            , PaymentMethod paymentMethod
            , FuelType fuelType
            , ElectricityMeterType meterType
            , double? elecProjection
            , double? gasProjection
            , double? energyEconomy7DayElecKwh
            , double? energyEconomy7NightElecKwh)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = postcode,
                SelectedPaymentMethod = paymentMethod,
                SelectedElectricityMeterType = meterType,
                SelectedFuelType = fuelType,
                Projection = new Projection
                {
                    EnergyAveStandardGasKwh = gasProjection,
                    EnergyAveStandardElecKwh = elecProjection,
                    EnergyEconomy7DayElecKwh = energyEconomy7DayElecKwh,
                    EnergyEconomy7NightElecKwh = energyEconomy7NightElecKwh
                }
            });

            var fakeEnergyProductServiceWrapper = new FakeProductServiceWrapper();
            var fakeBundleTariffServiceWrapper = new FakeBundleTariffServiceWrapper();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(FakeConfigManagerFactory.DefaultBundling())
                .WithEnergyProductServiceWrapper(fakeEnergyProductServiceWrapper)
                .WithBundleTariffServiceWrapper(fakeBundleTariffServiceWrapper)
                .Build<TariffsController>();

            // Act
            ActionResult result = await controller.AvailableTariffs();
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();

            // Assert
            viewModel.ShouldNotBeNull();
            TariffsViewModel tariff = viewModel.BundleTariffs.FirstOrDefault(t => t.IsBundle);
            tariff.ShouldNotBeNull();
            tariff?.TariffId.ShouldEqual("test04");
        }

        [TestCase("PO9 1QH", PaymentMethod.MonthlyDirectDebit, FuelType.Electricity, ElectricityMeterType.Standard, null, null, 1000.0, 2000.0)]
        public async Task ShouldContainBundleTariffForElectricityStandard(
            string postcode
            , PaymentMethod paymentMethod
            , FuelType fuelType
            , ElectricityMeterType meterType
            , double? elecProjection
            , double? gasProjection
            , double? energyEconomy7DayElecKwh
            , double? energyEconomy7NightElecKwh)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = postcode,
                SelectedPaymentMethod = paymentMethod,
                SelectedElectricityMeterType = meterType,
                SelectedFuelType = fuelType,
                Projection = new Projection
                {
                    EnergyAveStandardGasKwh = gasProjection,
                    EnergyAveStandardElecKwh = elecProjection,
                    EnergyEconomy7DayElecKwh = energyEconomy7DayElecKwh,
                    EnergyEconomy7NightElecKwh = energyEconomy7NightElecKwh
                }
            });

            var fakeEnergyProductServiceWrapper = new FakeProductServiceWrapper();
            var fakeBundleTariffServiceWrapper = new FakeBundleTariffServiceWrapper();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(FakeConfigManagerFactory.DefaultBundling())
                .WithEnergyProductServiceWrapper(fakeEnergyProductServiceWrapper)
                .WithBundleTariffServiceWrapper(fakeBundleTariffServiceWrapper)
                .Build<TariffsController>();

            // Act
            ActionResult result = await controller.AvailableTariffs();
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();

            // Assert
            viewModel.ShouldNotBeNull();
            TariffsViewModel tariff = viewModel.BundleTariffs.FirstOrDefault(t => t.IsBundle);
            tariff.ShouldNotBeNull();
            tariff?.TariffId.ShouldEqual("test03");
        }

        [TestCase("PO9 1QH", PaymentMethod.MonthlyDirectDebit, FuelType.Dual, ElectricityMeterType.Economy7, null, null, 1000.0, 2000.0)]
        public async Task ShouldContainBundleTariffForDualE7(
            string postcode
            , PaymentMethod paymentMethod
            , FuelType fuelType
            , ElectricityMeterType meterType
            , double? elecProjection
            , double? gasProjection
            , double? energyEconomy7DayElecKwh
            , double? energyEconomy7NightElecKwh)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = postcode,
                SelectedPaymentMethod = paymentMethod,
                SelectedElectricityMeterType = meterType,
                SelectedFuelType = fuelType,
                Projection = new Projection
                {
                    EnergyAveStandardGasKwh = gasProjection,
                    EnergyAveStandardElecKwh = elecProjection,
                    EnergyEconomy7DayElecKwh = energyEconomy7DayElecKwh,
                    EnergyEconomy7NightElecKwh = energyEconomy7NightElecKwh
                }
            });

            var fakeEnergyProductServiceWrapper = new FakeProductServiceWrapper();
            var fakeBundleTariffServiceWrapper = new FakeBundleTariffServiceWrapper();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(FakeConfigManagerFactory.DefaultBundling())
                .WithEnergyProductServiceWrapper(fakeEnergyProductServiceWrapper)
                .WithBundleTariffServiceWrapper(fakeBundleTariffServiceWrapper)
                .Build<TariffsController>();

            // Act
            ActionResult result = await controller.AvailableTariffs();
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();

            // Assert
            viewModel.ShouldNotBeNull();
            TariffsViewModel tariff = viewModel.BundleTariffs.FirstOrDefault(t => t.IsBundle);
            tariff.ShouldNotBeNull();
            tariff?.TariffId.ShouldEqual("test01");
        }

        [TestCase("PO9 1QH", PaymentMethod.MonthlyDirectDebit, FuelType.Dual, ElectricityMeterType.Standard, null, null, 1000.0, 2000.0)]
        public async Task ShouldContainBundleTariffForDualStandard(
            string postcode
            , PaymentMethod paymentMethod
            , FuelType fuelType
            , ElectricityMeterType meterType
            , double? elecProjection
            , double? gasProjection
            , double? energyEconomy7DayElecKwh
            , double? energyEconomy7NightElecKwh)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = postcode,
                SelectedPaymentMethod = paymentMethod,
                SelectedElectricityMeterType = meterType,
                SelectedFuelType = fuelType,
                Projection = new Projection
                {
                    EnergyAveStandardGasKwh = gasProjection,
                    EnergyAveStandardElecKwh = elecProjection,
                    EnergyEconomy7DayElecKwh = energyEconomy7DayElecKwh,
                    EnergyEconomy7NightElecKwh = energyEconomy7NightElecKwh
                }
            });

            var fakeEnergyProductServiceWrapper = new FakeProductServiceWrapper();
            var fakeBundleTariffServiceWrapper = new FakeBundleTariffServiceWrapper();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(FakeConfigManagerFactory.DefaultBundling())
                .WithEnergyProductServiceWrapper(fakeEnergyProductServiceWrapper)
                .WithBundleTariffServiceWrapper(fakeBundleTariffServiceWrapper)
                .Build<TariffsController>();

            // Act
            ActionResult result = await controller.AvailableTariffs();
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();

            // Assert
            viewModel.ShouldNotBeNull();
            TariffsViewModel tariff = viewModel.BundleTariffs.FirstOrDefault(t => t.IsBundle);
            tariff.ShouldNotBeNull();
            tariff?.TariffId.ShouldEqual("BO001");
        }

        [TestCase("PO9 1QH", PaymentMethod.MonthlyDirectDebit, FuelType.Dual, ElectricityMeterType.Standard, null, null, 1000.0, 2000.0, "Fix and Fibre Bundle", true)]
        [TestCase("PO9 1QH", PaymentMethod.MonthlyDirectDebit, FuelType.Dual, ElectricityMeterType.Standard, null, null, 1000.0, 2000.0, "Standard", false)]
        public async Task ShouldHaveChosenTariffOrBundleInAppropriateSection(
           string postcode
           , PaymentMethod paymentMethod
           , FuelType fuelType
           , ElectricityMeterType meterType
           , double? elecProjection
           , double? gasProjection
           , double? energyEconomy7DayElecKwh
           , double? energyEconomy7NightElecKwh
           , string chosenProductName
           , bool isBundle)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = postcode,
                SelectedPaymentMethod = paymentMethod,
                SelectedElectricityMeterType = meterType,
                SelectedFuelType = fuelType,
                Projection = new Projection
                {
                    EnergyAveStandardGasKwh = gasProjection,
                    EnergyAveStandardElecKwh = elecProjection,
                    EnergyEconomy7DayElecKwh = energyEconomy7DayElecKwh,
                    EnergyEconomy7NightElecKwh = energyEconomy7NightElecKwh
                },
                ChosenProduct = chosenProductName,
                IsBundlingJourney = isBundle
            });

            var fakeEnergyProductServiceWrapper = new FakeProductServiceWrapper();
            var fakeBundleTariffServiceWrapper = new FakeBundleTariffServiceWrapper();
            var fakeTariffManager = new FakeTariffManager();
            fakeTariffManager.TariffGroupMappings.Add("ME724", TariffGroup.FixAndFibre.ToString());
            fakeTariffManager.TariffGroupMappings.Add("MG101", TariffGroup.FixAndFibre.ToString());
            fakeTariffManager.TariffGroupMappings.Add("ME123", TariffGroup.FixAndFibre.ToString());
            fakeTariffManager.TariffGroupMappings.Add("MG123", TariffGroup.FixAndFibre.ToString());

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(FakeConfigManagerFactory.DefaultBundling())
                .WithEnergyProductServiceWrapper(fakeEnergyProductServiceWrapper)
                .WithBundleTariffServiceWrapper(fakeBundleTariffServiceWrapper)
                .WithTariffManager(fakeTariffManager)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = await controller.AvailableTariffs();
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();

            // Assert
            viewModel.ShouldNotBeNull();
            viewModel.ChosenTariff.ShouldNotBeNull();
            viewModel.ChosenTariff.DisplayName.ShouldContain(chosenProductName);
            viewModel.AllTariffs.ShouldNotContain(viewModel.ChosenTariff);
        }

        [TestCase("PO9 1QH", PaymentMethod.None, FuelType.Dual, ElectricityMeterType.Standard, null, null, 1000.0, 2000.0)]
        [TestCase("PO9 1QH", PaymentMethod.PayAsYouGo, FuelType.Dual, ElectricityMeterType.Standard, null, null, 1000.0, 2000.0)]
        [TestCase("PO9 1QH", PaymentMethod.Quarterly, FuelType.Dual, ElectricityMeterType.Standard, null, null, 1000.0, 2000.0)]
        public async Task ShouldNotContainBundleTariffForNonDirectDebit(
            string postcode
            , PaymentMethod paymentMethod
            , FuelType fuelType
            , ElectricityMeterType meterType
            , double? elecProjection
            , double? gasProjection
            , double? energyEconomy7DayElecKwh
            , double? energyEconomy7NightElecKwh)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = postcode,
                SelectedPaymentMethod = paymentMethod,
                SelectedElectricityMeterType = meterType,
                SelectedFuelType = fuelType,
                Projection = new Projection
                {
                    EnergyAveStandardGasKwh = gasProjection,
                    EnergyAveStandardElecKwh = elecProjection,
                    EnergyEconomy7DayElecKwh = energyEconomy7DayElecKwh,
                    EnergyEconomy7NightElecKwh = energyEconomy7NightElecKwh
                }
            });

            var fakeEnergyProductServiceWrapper = new FakeProductServiceWrapper();
            var fakeBundleTariffServiceWrapper = new FakeBundleTariffServiceWrapper();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(FakeConfigManagerFactory.DefaultBundling())
                .WithEnergyProductServiceWrapper(fakeEnergyProductServiceWrapper)
                .WithBundleTariffServiceWrapper(fakeBundleTariffServiceWrapper)
                .Build<TariffsController>();

            // Act
            ActionResult result = await controller.AvailableTariffs();
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();

            // Assert
            viewModel.ShouldNotBeNull();
            TariffsViewModel tariff = viewModel.EnergyTariffs.FirstOrDefault(t => t.IsBundle);
            tariff.ShouldBeNull();
        }

        [TestCase("PO9 1QH", PaymentMethod.MonthlyDirectDebit, FuelType.Gas, ElectricityMeterType.Standard, null, null, 1000.0, 2000.0)]
        [TestCase("PO9 1QH", PaymentMethod.MonthlyDirectDebit, FuelType.Gas, ElectricityMeterType.Standard, null, null, 1000.0, 2000.0)]
        [TestCase("PO9 1QH", PaymentMethod.MonthlyDirectDebit, FuelType.Gas, ElectricityMeterType.Standard, null, null, 1000.0, 2000.0)]
        public async Task ShouldNotContainBundleTariffForGasFuelType(
            string postcode
            , PaymentMethod paymentMethod
            , FuelType fuelType
            , ElectricityMeterType meterType
            , double? elecProjection
            , double? gasProjection
            , double? energyEconomy7DayElecKwh
            , double? energyEconomy7NightElecKwh)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = postcode,
                SelectedPaymentMethod = paymentMethod,
                SelectedElectricityMeterType = meterType,
                SelectedFuelType = fuelType,
                Projection = new Projection
                {
                    EnergyAveStandardGasKwh = gasProjection,
                    EnergyAveStandardElecKwh = elecProjection,
                    EnergyEconomy7DayElecKwh = energyEconomy7DayElecKwh,
                    EnergyEconomy7NightElecKwh = energyEconomy7NightElecKwh
                }
            });

            var fakeEnergyProductServiceWrapper = new FakeProductServiceWrapper();
            var fakeBundleTariffServiceWrapper = new FakeBundleTariffServiceWrapper();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(FakeConfigManagerFactory.DefaultBundling())
                .WithEnergyProductServiceWrapper(fakeEnergyProductServiceWrapper)
                .WithBundleTariffServiceWrapper(fakeBundleTariffServiceWrapper)
                .Build<TariffsController>();

            // Act
            ActionResult result = await controller.AvailableTariffs();
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();

            // Assert
            viewModel.ShouldNotBeNull();
            TariffsViewModel tariff = viewModel.EnergyTariffs.FirstOrDefault(t => t.IsBundle);
            tariff.ShouldBeNull();
        }

        [TestCase("PO9 1QH", PaymentMethod.MonthlyDirectDebit, FuelType.Gas, ElectricityMeterType.Other, null, null, 1000.0, 2000.0)]
        [TestCase("PO9 1QH", PaymentMethod.MonthlyDirectDebit, FuelType.Gas, ElectricityMeterType.Other, null, null, 1000.0, 2000.0)]
        [TestCase("PO9 1QH", PaymentMethod.MonthlyDirectDebit, FuelType.Gas, ElectricityMeterType.Other, null, null, 1000.0, 2000.0)]
        public async Task ShouldNotContainBundleTariffForMeterTypeOther(
            string postcode
            , PaymentMethod paymentMethod
            , FuelType fuelType
            , ElectricityMeterType meterType
            , double? elecProjection
            , double? gasProjection
            , double? energyEconomy7DayElecKwh
            , double? energyEconomy7NightElecKwh)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = postcode,
                SelectedPaymentMethod = paymentMethod,
                SelectedElectricityMeterType = meterType,
                SelectedFuelType = fuelType,
                Projection = new Projection
                {
                    EnergyAveStandardGasKwh = gasProjection,
                    EnergyAveStandardElecKwh = elecProjection,
                    EnergyEconomy7DayElecKwh = energyEconomy7DayElecKwh,
                    EnergyEconomy7NightElecKwh = energyEconomy7NightElecKwh
                }
            });

            var fakeEnergyProductServiceWrapper = new FakeProductServiceWrapper();
            var fakeBundleTariffServiceWrapper = new FakeBundleTariffServiceWrapper();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(FakeConfigManagerFactory.DefaultBundling())
                .WithEnergyProductServiceWrapper(fakeEnergyProductServiceWrapper)
                .WithBundleTariffServiceWrapper(fakeBundleTariffServiceWrapper)
                .Build<TariffsController>();

            // Act
            ActionResult result = await controller.AvailableTariffs();
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<AvailableTariffsViewModel>();

            // Assert
            viewModel.ShouldNotBeNull();
            TariffsViewModel tariff = viewModel.EnergyTariffs.FirstOrDefault(t => t.IsBundle);
            tariff.ShouldBeNull();
        }
        
        [TestCase("BundleBBB2", -1, 24)]
        public void ShouldNotCreateBroadbandWhenMonthlyDiscountIsLessThanZero(string code, double monthlyDiscountedCost, double monthlyOriginalCost)
        {
            // ReSharper disable once ObjectCreationAsStatement
            void Create() => new BundlePackage(code, monthlyDiscountedCost, monthlyOriginalCost, BundlePackageType.FixAndFibre, null);
            Assert.Throws<ArgumentException>(Create);
        }

        [TestCase("BundleBBB1", 12, 0)]
        [TestCase("BundleBBB2", 11, -1)]
        public void ShouldNotCreateBroadbandWhenMonthlyOriginalCostIsLessThanOrEqualToZero(string code, double monthlyDiscountedCost, double monthlyOriginalCost)
        {
            // ReSharper disable once ObjectCreationAsStatement
            void Create() => new BundlePackage(code, monthlyDiscountedCost, monthlyOriginalCost, BundlePackageType.FixAndFibre, null);
            Assert.Throws<ArgumentException>(Create);
        }

        [TestCase("BundleBBB1", 12, 0)]
        [TestCase("BundleBBB2", 11, -1)]
        [TestCase("BundleBBB2", 11, 10)]
        public void ShouldNotCreateBroadbandWhenMonthlyDiscountIsGreaterThanMonthlyOriginalCost(string code, double monthlyDiscountedCost, double monthlyOriginalCost)
        {
            // ReSharper disable once ObjectCreationAsStatement
            void Create() => new BundlePackage(code, monthlyDiscountedCost, monthlyOriginalCost, BundlePackageType.FixAndFibre, null);
            Assert.Throws<ArgumentException>(Create);
        }

        [TestCase("BundleBBB1", 12, 24, 144)]
        [TestCase("BundleBBB2", 11, 11, 132)]
        public void ShouldCalculateBroadbandProjectedYearlyCost(string code, double monthlyDiscountedCost, double monthlyOriginalCost, double expectedYearlyCost)
        {
            var broadbandPackage = new BundlePackage(code, monthlyDiscountedCost, monthlyOriginalCost, BundlePackageType.FixAndFibre, null);
            broadbandPackage.ProjectedYearlyCost.ShouldEqual(expectedYearlyCost);
        }

        [TestCase("BundleBBB1", 12, 24, 144)]
        [TestCase("BundleBBB2", 11, 11, 132)]
        public void ShouldCalculateBroadbandYearlySavings(string code, double monthlyDiscountedCost, double monthlyOriginalCost, double expectedYearlyCost)
        {
            var broadbandPackage = new BundlePackage(code, monthlyDiscountedCost, monthlyOriginalCost, BundlePackageType.FixAndFibre, null);
            broadbandPackage.ProjectedYearlyCost.ShouldEqual(expectedYearlyCost);
        }

        [TestCase("BundleBBB1", 11.5, 23, 11.5)]
        [TestCase("BundleBBB2", 12, 24, 12)]
        public void ShouldCalculateBroadbandMonthlySavings(string code, double monthlyDiscountedCost, double monthlyOriginalCost, double expectedMonthlySavings)
        {
            var broadbandPackage = new BundlePackage(code, monthlyDiscountedCost, monthlyOriginalCost, BundlePackageType.FixAndFibre, null);
            broadbandPackage.MonthlySavings.ShouldEqual(expectedMonthlySavings);
        }

        [TestCase("BundleBBB1", 11.5, 23, 50.00)]
        [TestCase("BundleBBB2", 10, 24, 58)]
        public void ShouldCalculateBroadbandMonthlySavingsPercentage(string code, double monthlyDiscountedCost, double monthlyOriginalCost, double expectedMonthlySavings)
        {
            var broadbandPackage = new BundlePackage(code, monthlyDiscountedCost, monthlyOriginalCost, BundlePackageType.FixAndFibre, null);
            broadbandPackage.MonthlySavingsPercentage.ShouldEqual(expectedMonthlySavings);
        }

        [TestCase("PO9 1QH", PaymentMethod.MonthlyDirectDebit, FuelType.Electricity, ElectricityMeterType.Economy7, null, null, 1000.0, 2000.0, 2)]
        [TestCase("PO9 1QH", PaymentMethod.MonthlyDirectDebit, FuelType.Dual, ElectricityMeterType.Economy7, null, 1500.0, 1000.0, 2000.0, 2)]
        [TestCase("PO9 1QH", PaymentMethod.MonthlyDirectDebit, FuelType.Gas, ElectricityMeterType.Standard, null, 1500.0, null, null, 2)]
        [TestCase("PO9 1QH", PaymentMethod.MonthlyDirectDebit, FuelType.Electricity, ElectricityMeterType.Standard, 1000.0, null, null, null, 3)]
        [TestCase("PO9 1QH", PaymentMethod.MonthlyDirectDebit, FuelType.Dual, ElectricityMeterType.Standard, 1000.0, 1500.0, null, null, 5)]
        [TestCase("PO9 1QH", PaymentMethod.Quarterly, FuelType.Dual, ElectricityMeterType.Standard, 1000.0, 1500.0, null, null, 5)]
        [TestCase("PO9 1QH", PaymentMethod.PayAsYouGo, FuelType.Dual, ElectricityMeterType.Standard, 1000.0, 1500.0, null, null, 5)]
        public async Task ProductViewModelShouldBePopulatedWithAppropriateValues(
            string postcode
            , PaymentMethod paymentMethod
            , FuelType fuelType
            , ElectricityMeterType meterType
            , double? elecProjection
            , double? gasProjection
            , double? energyEconomy7DayElecKwh
            , double? energyEconomy7NightElecKwh
            , int expectedTariffCount)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = postcode,
                SelectedPaymentMethod = paymentMethod,
                SelectedElectricityMeterType = meterType,
                SelectedFuelType = fuelType,
                Projection = new Projection
                {
                    EnergyAveStandardGasKwh = gasProjection,
                    EnergyAveStandardElecKwh = elecProjection,
                    EnergyEconomy7DayElecKwh = energyEconomy7DayElecKwh,
                    EnergyEconomy7NightElecKwh = energyEconomy7NightElecKwh
                }
            });

            var fakeEnergyProductServiceWrapper = new FakeProductServiceWrapper();
            var fakeBundleTariffServiceWrapper = new FakeBundleTariffServiceWrapper();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(FakeConfigManagerFactory.DefaultBundling())
                .WithEnergyProductServiceWrapper(fakeEnergyProductServiceWrapper)
                .WithBundleTariffServiceWrapper(fakeBundleTariffServiceWrapper)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = await controller.AvailableTariffs();
            var viewModel = (AvailableTariffsViewModel)((ViewResult) result).Model;
            viewModel.ShouldNotBeNull();

            IEnumerable<TariffsViewModel> dualFuelTariffs = viewModel.EnergyTariffs.Where(t => t.FuelType == FuelType.Dual);
            IEnumerable<TariffsViewModel> gasTariffs = viewModel.EnergyTariffs.Where(t => t.FuelType == FuelType.Gas);
            IEnumerable<TariffsViewModel> elecTariffs = viewModel.EnergyTariffs.Where(t => t.FuelType == FuelType.Electricity);

            Assert.AreEqual(expectedTariffCount, viewModel.EnergyTariffs.Count());
            Assert.IsTrue(dualFuelTariffs.All(t => t.ElectricityTariffInformationLabel != null && t.GasTariffInformationLabel != null));
            Assert.IsTrue(gasTariffs.All(t => t.GasTariffInformationLabel != null));
            Assert.IsTrue(elecTariffs.All(t => t.ElectricityTariffInformationLabel != null));
        }

        [TestCase(FuelType.Electricity, null, 1000.0, 2000.0, false, true, "Your estimated annual electricity costs will be £1,000. Payment by Direct Debit only.", "/Content/Svgs/icons/fuel/electricity-2colour.svg", "Save £10 a month (£180 over 18 months). Saving is based on £10 discount for 18 months for fibre broadband (18 x £10 = £180), when compared to buying Unlimited Fibre, our equivalent standalone broadband product (£33 per month).")]
        [TestCase(FuelType.Dual, 1500.0, 1000.0, 2000.0, true, true, "Your estimated annual energy costs will be £2,000. That’s £1,000 for gas and £1,000 for electricity. Payment by Direct Debit only.", "/Content/Svgs/icons/fuel/dual-fuel-2colour.svg", "Save £10 a month (£180 over 18 months). Saving is based on £10 discount for 18 months for fibre broadband (18 x £10 = £180), when compared to buying Unlimited Fibre, our equivalent standalone broadband product (£33 per month).")]
        public async Task TariffsViewModelForABroadbandBundleShouldBePopulatedWithAppropriateValues(
            FuelType fuelType,
            double? gasProjection,
            double? energyEconomy7DayElecKwh,
            double? energyEconomy7NightElecKwh,
            bool expectedHasGas,
            bool expectedHasElectric,
            string expectedDisclaimer2Text, 
            string expectedEnergyIconPath,
            string expectedBundleDisclaimerModalText)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1QH",
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                SelectedElectricityMeterType = ElectricityMeterType.Economy7,
                SelectedFuelType = fuelType,
                Projection = new Projection
                {
                    EnergyAveStandardGasKwh = gasProjection,
                    EnergyAveStandardElecKwh = null,
                    EnergyEconomy7DayElecKwh = energyEconomy7DayElecKwh,
                    EnergyEconomy7NightElecKwh = energyEconomy7NightElecKwh
                }
            });

            var fakeEnergyProductServiceWrapper = new FakeProductServiceWrapper();
            var fakeBundleTariffServiceWrapper = new FakeBundleTariffServiceWrapper();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(FakeConfigManagerFactory.DefaultBundling())
                .WithEnergyProductServiceWrapper(fakeEnergyProductServiceWrapper)
                .WithBundleTariffServiceWrapper(fakeBundleTariffServiceWrapper)
                .Build<TariffsController>();

            // Act
            ActionResult result = await controller.AvailableTariffs();
            var viewModel = (AvailableTariffsViewModel)((ViewResult)result).Model;
            viewModel.ShouldNotBeNull();

            TariffsViewModel bundleTariff = viewModel.BundleTariffs.FirstOrDefault(t => t.IsBundle);

            bundleTariff.ShouldNotBeNull();
            // ReSharper disable once PossibleNullReferenceException
            bundleTariff.BundleDisclaimer1Text.ShouldEqual("£180 saving is based on £10 (30%) discount for 18 months for fibre broadband (18 x £10 = £180), when compared to buying Unlimited Fibre, our equivalent standalone broadband product (£33).");
            bundleTariff.BundleDisclaimer2Text.ShouldEqual(expectedDisclaimer2Text);
            bundleTariff.BroadbandMoreInformation.BundleDisclaimerModalText.ShouldEqual(expectedBundleDisclaimerModalText);
            bundleTariff.EnergyIconPath.ShouldEqual(expectedEnergyIconPath);
            bundleTariff.HasGas.ShouldEqual(expectedHasGas);
            bundleTariff.HasElectric.ShouldEqual(expectedHasElectric);
        }

        [Test]
        [TestCase(ElectricityMeterType.Economy7, null, 1500, 1000, 1000)]
        [TestCase(ElectricityMeterType.Standard, 1200, 1500, null, null)]
        public async Task ShouldRedirectToAvailableTariffsWhenKnownUsageIsEntered(ElectricityMeterType electricityMeterType, int? standardElec, int? standardGas, int? economy7Day, int? economy7Night)
        {
            // Arrange

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                SelectedFuelType = FuelType.Dual,
                SelectedElectricityMeterType = electricityMeterType
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<TariffsController>();

            var viewModel = new EnergyUsageViewModel
            {
                KnownEnergyUsageViewModel = new KnownEnergyUsageViewModel
                {
                    StandardGasUsage = standardGas,
                    StandardElectricityUsage = standardElec,
                    Economy7ElectricityNightUsage = economy7Night,
                    Economy7ElectricityDayUsage = economy7Day,
                    SelectedElectricityMeterType = electricityMeterType,
                    SelectedFuelType = FuelType.Dual
                }
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = await controller.EnergyUsage(viewModel);

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            Assert.AreEqual(customer.Projection.EnergyAveStandardGasKwh, standardGas);
            Assert.AreEqual(customer.Projection.EnergyAveStandardElecKwh, standardElec);
            Assert.AreEqual(customer.Projection.EnergyEconomy7DayElecKwh, economy7Day);
            Assert.AreEqual(customer.Projection.EnergyEconomy7NightElecKwh, economy7Night);

            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("AvailableTariffs");
            routeResult.RouteValues["controller"].ShouldEqual("Tariffs");
        }

        private static IEnumerable<TestCaseData> ProjectionTestCaseData()
        {
            Projection fakeProjectionData = FakeProjectionData.GetProjection();
            yield return new TestCaseData(new EnergyCustomer
            {
                Postcode = "PO91QH",
                SelectedElectricityMeterType = ElectricityMeterType.Economy7,
                SelectedFuelType = FuelType.Dual
            }, new Projection
            {
                EnergyAveStandardGasKwh = fakeProjectionData.EnergyAveStandardGasKwh,
                EnergyEconomy7DayElecKwh = fakeProjectionData.EnergyEconomy7DayElecKwh,
                EnergyEconomy7NightElecKwh = fakeProjectionData.EnergyEconomy7NightElecKwh
            }, "Dual fuel with E7 meter");

            yield return new TestCaseData(new EnergyCustomer
            {
                Postcode = "PO91QH",
                SelectedElectricityMeterType = ElectricityMeterType.Economy7,
                SelectedFuelType = FuelType.Electricity
            }, new Projection
            {
                EnergyEconomy7DayElecKwh = fakeProjectionData.EnergyEconomy7DayElecKwh,
                EnergyEconomy7NightElecKwh = fakeProjectionData.EnergyEconomy7NightElecKwh
            }, "Elec with E7 meter");

            yield return new TestCaseData(new EnergyCustomer
            {
                Postcode = "PO91QH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual
            }, new Projection
            {
                EnergyAveStandardGasKwh = fakeProjectionData.EnergyAveStandardGasKwh,
                EnergyAveStandardElecKwh = fakeProjectionData.EnergyAveStandardElecKwh
            }, "Dual fuel with standard");

            yield return new TestCaseData(new EnergyCustomer
            {
                Postcode = "PO91QH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Gas
            }, new Projection
            {
                EnergyAveStandardGasKwh = fakeProjectionData.EnergyAveStandardGasKwh
            }, "Gas with standard");

            yield return new TestCaseData(new EnergyCustomer
            {
                Postcode = "PO91QH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Electricity
            }, new Projection
            {
                EnergyAveStandardElecKwh = fakeProjectionData.EnergyAveStandardElecKwh
            }, "Elec with standard");
        }

        [Test]
        [TestCaseSource(nameof(ProjectionTestCaseData))]
        public async Task ShouldStoreProjectionInSessionAndRedirectToAvailableTariffsWhenUnknownUsageIsEntered(
            EnergyCustomer customer, Projection fakeProjectionData, string description)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<TariffsController>();

            var viewModel = new EnergyUsageViewModel
            {
                ActiveTabIndex = 1,
                UnknownEnergyUsageViewModel = new UnknownEnergyUsageViewModel
                {
                    SelectedPropertyTypeId = "Mid Terrace",
                    SelectedNumberOfAdultsId = "1",
                    SelectedNumberOfBedroomsId = "2 Bedrooms"
                }
            };


            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = await controller.EnergyUsage(viewModel);

            // Assert
            var sessionCustomer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);

            Assert.AreEqual(sessionCustomer.Projection.EnergyAveStandardGasKwh,
                fakeProjectionData.EnergyAveStandardGasKwh);
            Assert.AreEqual(sessionCustomer.Projection.EnergyAveStandardElecKwh,
                fakeProjectionData.EnergyAveStandardElecKwh);
            Assert.AreEqual(sessionCustomer.Projection.EnergyEconomy7DayElecKwh,
                fakeProjectionData.EnergyEconomy7DayElecKwh);
            Assert.AreEqual(sessionCustomer.Projection.EnergyEconomy7NightElecKwh,
                fakeProjectionData.EnergyEconomy7NightElecKwh);

            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("AvailableTariffs");
            routeResult.RouteValues["controller"].ShouldEqual("Tariffs");
        }

        [Test]
        public async Task ShouldRedirectToGenericFalloutPageWhenProjectionApiThrowsException()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer { Postcode = "PO91QH" });

            var fakeEnergyProjectionServiceWrapper = new FakeEnergyProjectionServiceWrapper { ThrowException = true };

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithEnergyProjectionServiceWrapper(fakeEnergyProjectionServiceWrapper)
                .Build<TariffsController>();

            var viewModel = new EnergyUsageViewModel
            {
                ActiveTabIndex = 1,
                UnknownEnergyUsageViewModel = new UnknownEnergyUsageViewModel
                {
                    SelectedPropertyTypeId = "Mid Terrace",
                    SelectedNumberOfAdultsId = "1",
                    SelectedNumberOfBedroomsId = "2 Bedrooms"
                }
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = await controller.EnergyUsage(viewModel);

            // Assert

            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("UnableToComplete");
            routeResult.RouteValues["controller"].ShouldEqual("SignUp");
        }

        [Test]
        [TestCase(null, 12000, "Please enter your electricity usage.")]
        [TestCase(12000, null, "Please enter your gas usage.")]
        [TestCase(1200000, 12, "Please enter your electricity usage up to six digits.")]
        [TestCase(12, 1200000, "Please enter your gas usage up to six digits.")]
        public async Task ShouldReturnToIKnowMyUsageViewWhenUsageDataIsMissingOrInvalid(int? electricityUsage,
            int? gasUsage, string errorMessage)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer,
                new EnergyCustomer { Postcode = "PO91QH", SelectedFuelType = FuelType.Dual });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<TariffsController>();

            var viewModel = new EnergyUsageViewModel
            {
                KnownEnergyUsageViewModel = new KnownEnergyUsageViewModel
                {
                    StandardGasUsage = gasUsage,
                    StandardElectricityUsage = electricityUsage
                }
            };


            // Act
            controller.ValidateViewModel(viewModel.KnownEnergyUsageViewModel);
            ActionResult result = await controller.EnergyUsage(viewModel);

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<EnergyUsageViewModel>();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Keys.Count.ShouldEqual(1);
            controller.ModelState.Values.First().Errors.First().ErrorMessage.ShouldEqual(errorMessage);
        }

        [Test]
        [TestCase(null, "1", "2 Bedrooms", "Please choose your property type.")]
        [TestCase("Mid Terrace", null, "2 Bedrooms", "Please choose the number of adults.")]
        [TestCase("Mid Terrace", "2 Bedrooms", null, "Please choose the number of bedrooms.")]
        public async Task ShouldReturnToIDoNotKnowMyUsageViewWhenModelIsInvalid(string propertyType,
            string numberOfAdults, string noOfBedRooms, string errorMessage)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer,
                new EnergyCustomer { Postcode = "PO91QH", SelectedFuelType = FuelType.Dual });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<TariffsController>();

            var viewModel = new EnergyUsageViewModel
            {
                ActiveTabIndex = 1,
                UnknownEnergyUsageViewModel = new UnknownEnergyUsageViewModel
                {
                    SelectedPropertyTypeId = propertyType,
                    SelectedNumberOfAdultsId = numberOfAdults,
                    SelectedNumberOfBedroomsId = noOfBedRooms
                }
            };


            // Act
            controller.ValidateViewModel(viewModel.UnknownEnergyUsageViewModel);
            ActionResult result = await controller.EnergyUsage(viewModel);

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<EnergyUsageViewModel>();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Keys.Count.ShouldEqual(1);
            controller.ModelState.Values.First().Errors.First().ErrorMessage.ShouldEqual(errorMessage);
        }

        [TestCase("02392123456", "02392123456", true)]
        [TestCase(null, null, false)]
        public async Task ShouldPopulateEnergyCustomerCLIAndIsSSECustomerCLICorrectlyWhenBTLineAvailabilityRespondsAndBroadbandBundleChosen(string cliResponse, string expectedCLIResult, bool expectedIsSSECustomerCLIResult)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" }
            });

             var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false, CLI = cliResponse })
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper())
                .Build<TariffsController>();

            // Act
            await controller.AvailableTariffs();
            await controller.AvailableTariffs("BO001");

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.CLIChoice.OpenReachProvidedCLI.ShouldEqual(expectedCLIResult);
            customer.IsSSECustomerCLI.ShouldEqual(expectedIsSSECustomerCLIResult);
        }

        [Test]
        public async Task ShouldRedirectToExtrasView()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" }
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithContextManager(FakeContextManagerFactory.Default())
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper { ReturnFixNProtectBundle = true})
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false })
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper())
                .Build<TariffsController>();

            // Act
            await controller.AvailableTariffs();
            ActionResult result = await controller.AvailableTariffs("BO002");

            // Assert
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("Extras");
            redirectResult.RouteValues["controller"].ShouldEqual("SignUp");
        }

        [Test]
        public void ShouldNotRedirectToExtrasView()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                Projection = new Projection
                {
                    EnergyEconomy7NightElecKwh = 100,
                    EnergyEconomy7DayElecKwh = 200,
                    EnergyAveEcon7ElecKwh = 300,
                    EnergyAveStandardElecKwh = 400,
                    EnergyAveStandardGasKwh = 500
                },
                IsUsageKnown = true,
                SelectedAddress = new QasAddress { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" }
            });

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithContextManager(FakeContextManagerFactory.Default())
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper { ReturnSingleBundle = true })
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false })
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper())
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            // ReSharper disable once UnusedVariable
            ActionResult result1 = controller.AvailableTariffs().Result;
            ActionResult result2 = controller.AvailableTariffs("004").Result;

            // Assert
            var redirectResult = result2.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("PersonalDetails");
            redirectResult.RouteValues["controller"].ShouldEqual("SignUp");
        }
    }
}