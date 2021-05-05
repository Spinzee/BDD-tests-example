namespace Products.Tests.Energy.ControllerTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Common.Fakes;
    using Common.Fakes.Services;
    using Common.Helpers;
    using Core;
    using Core.Enums;
    using Fakes.Services;
    using Helpers;
    using NUnit.Framework;
    using Products.Model.Common;
    using Products.Model.Constants;
    using Products.Model.Energy;
    using Products.Model.Enums;
    using Should;
    using Web.Areas.Energy.Controllers;
    using WebModel.Resources.Energy;
    using WebModel.ViewModels.Common;
    using WebModel.ViewModels.Energy;
   
    [TestFixture]
    public class QuoteControllerTests
    {
        [TestCase("1 Test House", "1", "Test House")]
        [TestCase("1TestHouse", "1TestHouse", null)]
        [TestCase("1", "1", null)]
        [TestCase("1A", "1A", null)]
        [TestCase("2/L 179a", "2/L", "179a")]
        [TestCase("100/1", "100/1", null)]
        [TestCase("100/1a building", "100/1a", "building")]
        [TestCase("FLAT 54YEOMAN COURT", null, "FLAT 54YEOMAN COURT")]
        public void ShouldSplitHouseNumberAndNameBasedOnRegex(string qasHouseName, string expectedHouseNumber, string expectedHouseName)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                SelectedAddress = new QasAddress
                {
                    Moniker = "ABC",
                    HouseName = qasHouseName
                }
            });
            var fakeServiceWrapper = new FakeQASServiceWrapper();
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithQASServiceWrapper(fakeServiceWrapper)
                .Build<QuoteController>();

            // Act
            ActionResult result = controller.SelectAddress().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();
            viewModel.IsManual.ShouldBeFalse();
            fakeSessionManager.SessionObject.TryGetValue(SessionKeys.EnergyCustomer, out object energyCustomer);
            var customer = (EnergyCustomer) energyCustomer;
            // ReSharper disable once PossibleNullReferenceException
            Assert.AreEqual(customer.SelectedAddress.GetHouseName(), expectedHouseName);
            Assert.AreEqual(customer.SelectedAddress.GetHouseNumber(), expectedHouseNumber);
        }

        [TestCase("1")]
        [TestCase("0")]
        public void BundlingJourneyFlagStoredInSessionWhenPassedFromHubOnQueryString(string cli)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var viewModel = new ExistingCustomerViewModel
                { IsBundlingJourney = cli == "1", IsSseCustomer = false };

            // Act
            controller.ExistingCustomer(viewModel);

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.IsBundlingJourney = viewModel.IsBundlingJourney;
        }

        [TestCase("1 Year Fix and Fibre v2", "1", true, true)]
        [TestCase("1 Year Fix and Fibre v2", "0", false, false)]
        public void ShouldStoreDataFromBundleHubToViewModel(string productCode, string cli, bool isBundle, bool expectedCli)
        {
            // Arrange
            var controller = new ControllerFactory()
                .Build<QuoteController>();

            // Act
            ActionResult result = controller.ExistingCustomer(productCode, cli, isBundle);

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<ExistingCustomerViewModel>();
            var model = (ExistingCustomerViewModel) viewResult.Model;
            model.ShowCli.ShouldEqual(expectedCli);
            model.IsBundlingJourney.ShouldEqual(isBundle);
            model.ProductCode.ShouldEqual(productCode);
        }

        [TestCase("1 Year Fix and Fibre v2", true, true)]
        [TestCase("1 Year Fix and Fibre v2", false, false)]
        public void ShouldStoreDataFromViewModelToEnergyCustomerInSession(string productCode, bool showCli, bool isBundle)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var viewModel = new ExistingCustomerViewModel
            {
                ProductCode = productCode,
                IsBundlingJourney = isBundle,
                ShowCli = showCli,
                IsSseCustomer = false
            };

            // Act
            controller.ExistingCustomer(viewModel);

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.IsBundlingJourney.ShouldEqual(isBundle);
            customer.ShowCli.ShouldEqual(showCli);
            customer.ChosenProduct.ShouldEqual(productCode);
        }

        [TestCase(false, "EnterPostcode", null, "Energy")]
        [TestCase(true, "IdentifyCustomer", "CustomerIdentification", "TariffChange")]
        public void ExistingEnergyCustomerShouldRedirectToTariffChangeJourney(bool isSseCustomer, string actionName, string controllerName, string areaName)
        {
            // Arrange
            var controller = new ControllerFactory()
                .Build<QuoteController>();

            var viewModel = new ExistingCustomerViewModel { IsSseCustomer = isSseCustomer };

            // Act
            ActionResult result = controller.ExistingCustomer(viewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["controller"].ShouldEqual(controllerName);
            routeResult.RouteValues["action"].ShouldEqual(actionName);

            if (isSseCustomer)
            {
                routeResult.RouteValues["area"].ShouldEqual(areaName);
            }
        }

        [TestCaseSource(nameof(CAndCTestCases))]
        public void ModelShouldPopulateWithCorrectPaymentMethodBasedOnCAndCData(MeterDetail meterDetails, string expectedValue)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer,
                new EnergyCustomer { SelectedFuelType = FuelType.Dual, MeterDetail = meterDetails });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            // Act
            ActionResult result = controller.PaymentMethod();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<SelectPaymentMethodViewModel>();
            var model = (SelectPaymentMethodViewModel) viewResult.Model;
            RadioButton paymentMethod = model.PaymentMethods.Items.FirstOrDefault(item => item.Value == PaymentMethod.PayAsYouGo.ToString());
            Assert.AreEqual(paymentMethod?.Value, expectedValue);
        }

        [TestCase("1 Year Fix and Fibre v2", true, true)]
        [TestCase("1 Year Fix and Protect v1", false, false)]
        public void ShouldDisplayEnterPostcodeViewWhenPostcodeNotGiven(string productCode, bool isBundle, bool showCli)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var energyCustomer = new EnergyCustomer { ShowCli = showCli, IsBundlingJourney = isBundle, ChosenProduct = productCode };
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, energyCustomer);
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            // Act
            ActionResult result = controller.EnterPostcode(string.Empty, "0").Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<PostcodeViewModel>();
            viewResult.ViewName.ShouldEqual(string.Empty);
            var model = (PostcodeViewModel) viewResult.Model;
            model.PostCode.ShouldBeNull();
            model.ProductCode.ShouldEqual(productCode);
            model.OurPrices.ShouldBeFalse();
            model.IsBundle.ShouldEqual(isBundle);
            model.ShowLandline.ShouldEqual(showCli);
        }

        [TestCase("1 Year Fix and Fibre v2", true, "RG2 0WR")]
        [TestCase("1 Year Fix and Protect v1", false, "PO6 1RU")]
        public void ShouldDisplaySelectAddressViewWhenEnteringFromOurPricesPage(
            string productCode,
            bool isBundle,
            string postcode)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            // Act
            ActionResult result = controller.EnterPostcode(productCode, "true", isBundle, postcode).Result;

            // Assert
            var viewResult = result.ShouldBeType<RedirectToRouteResult>();
            viewResult.RouteName.ShouldEqual("");
            viewResult.RouteValues["action"].ShouldEqual("SelectAddress");

            //var model = (PostcodeViewModel)viewResult.Model;
            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.Postcode.ShouldEqual(postcode);
            customer.CLIChoice?.UserProvidedCLI?.ShouldBeEmpty();
            customer.IsBundlingJourney.ShouldEqual(isBundle);
            customer.ChosenProduct.ShouldEqual(productCode);
        }

        [TestCase("EC1A 1BB")]
        [TestCase("W1A 0AX")]
        [TestCase("M1 1AE")]
        [TestCase("B33 8TH")]
        [TestCase("CR2 6XH")]
        [TestCase("DN55 1PT")]
        [TestCase("EC1A1BB")]
        [TestCase("W1A0AX")]
        [TestCase("M11AE")]
        [TestCase("B338TH")]
        [TestCase("CR26XH")]
        [TestCase("DN551PT")]
        [TestCase(" DN551PT")]
        [TestCase("DN551PT ")]
        public void ShouldRedirectToSelectAddressPageWhenAValidPostcodeIsEntered(string postcode)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var postcodeViewModel = new PostcodeViewModel { PostCode = postcode };

            // Act
            controller.ValidateViewModel(postcodeViewModel);
            ActionResult result = controller.EnterPostcode(postcodeViewModel).Result;

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("SelectAddress");
            routeResult.RouteValues["controller"].ShouldEqual(null);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void ShouldRedirectToSelectAddressPageAndSaveIsBundlingJourneyFlagWhenAValidPostcodeIsEntered(bool isBundle, bool expectedIsBundlingJourneyFlag)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer { IsBundlingJourney = !isBundle });
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var postcodeViewModel = new PostcodeViewModel { PostCode = "EC1A 1BB", IsBundle = isBundle };

            // Act
            controller.ValidateViewModel(postcodeViewModel);
            ActionResult result = controller.EnterPostcode(postcodeViewModel).Result;

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            routeResult.RouteValues["action"].ShouldEqual("SelectAddress");
            routeResult.RouteValues["controller"].ShouldEqual(null);
            customer.IsBundlingJourney.ShouldEqual(expectedIsBundlingJourneyFlag);
        }

        [TestCase("PO9 1BH")]
        [TestCase("PA9 1BH")]
        public void EnterPostCodeShouldResetEnergyCustomer(string enteredPostCode)
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
                .Build<QuoteController>();

            var postcodeViewModel = new PostcodeViewModel { PostCode = enteredPostCode };

            // Act
            // ReSharper disable once UnusedVariable
            ActionResult result = controller.EnterPostcode(postcodeViewModel).Result;

            // Assert
            var energyCustomer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            AssertEnergyCustomerHasBeenReset(energyCustomer);
        }

        private static void AssertEnergyCustomerHasBeenReset(EnergyCustomer energyCustomer)
        {
            energyCustomer.UnavailableBundles.ShouldNotBeNull();
            energyCustomer.UnavailableBundles.Count.ShouldEqual(0);
            energyCustomer.SelectedBTAddress.ShouldBeNull();
            energyCustomer.SelectedBroadbandProduct.ShouldBeNull();
            energyCustomer.SelectedBroadbandProductCode.ShouldBeNull();
            energyCustomer.SelectedTariff.ShouldBeNull();
        }

        [Test]
        public void PayGoGasCustomerShouldHavePaymentMethodPayAsYouGoMeterTypeStandard()
        {
            var fakeSessionManager = new FakeSessionManager();
            var energyCustomer = new EnergyCustomer
            {
                MeterDetail = FakeCAndCData.GetMeterDetailsWithGasPayGoNonSmart(),
                SelectedFuelType = FuelType.Gas
            };

            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, energyCustomer);
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var selectFuelViewModel = new SelectFuelViewModel
            {
                FuelType = FuelType.Gas
            };

            // Act
            controller.ValidateViewModel(selectFuelViewModel);
            controller.SelectFuel(selectFuelViewModel);

            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.SelectedPaymentMethod.ShouldEqual(PaymentMethod.PayAsYouGo);
            customer.SelectedElectricityMeterType.ShouldEqual(ElectricityMeterType.Standard);
            customer.IsPrePay().ShouldBeTrue();
        }

        [Test]
        public void PayGoSmets1GasCustomerShouldHavePaymentMethodPayAsYouGo()
        {
            var fakeSessionManager = new FakeSessionManager();
            var energyCustomer = new EnergyCustomer
            {
                MeterDetail = FakeCAndCData.GetMeterDetailsWithGasPayGoSmartSmets1(),
                SelectedFuelType = FuelType.Gas
            };

            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, energyCustomer);
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var selectFuelViewModel = new SelectFuelViewModel
            {
                FuelType = FuelType.Gas
            };

            // Act
            controller.ValidateViewModel(selectFuelViewModel);
            controller.SelectFuel(selectFuelViewModel);

            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.SelectedPaymentMethod.ShouldEqual(PaymentMethod.None);
            customer.SelectedElectricityMeterType.ShouldEqual(ElectricityMeterType.Standard);
        }

        [Test]
        public void SelectAddressShouldResetEnergyCustomer()
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
                .Build<QuoteController>();

            var selectAddressViewModel = new SelectAddressViewModel
            {
                Postcode = "KK19 6BT",
                AddressLine1 = "43 Forbury Road",
                AddressLine2 = "Reading",
                County = "Berkshire",
                SelectedAddressId = "12122121~~43 Forbury Road, Reading, Berkshire"
            };

            // Act
            // ReSharper disable once UnusedVariable
            ActionResult result = controller.SelectAddress(selectAddressViewModel).Result;

            // Assert
            var energyCustomer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            AssertEnergyCustomerHasBeenReset(energyCustomer);
        }

        [Test]
        public void ShouldDisplayCorrectHeaderAndParagraph()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            // Act
            ActionResult result = controller.EnterPostcode("MG112", "0").Result;

            // Assert
            var model = result.ShouldNotBeNull()
                .ShouldBeType<ViewResult>()
                .Model.ShouldBeType<PostcodeViewModel>();

            model.Header.ShouldEqual(Postcode_Resources.Header);
            model.ParagraphText.ShouldBeNull();
        }

        [Test]
        public void ShouldDisplayInlineErrorWhenFuelTypeIsNotSelected()
        {
            // Arrange
            var controller = new ControllerFactory()
                .Build<QuoteController>();

            var selectFuelViewModel = new SelectFuelViewModel();

            // Act
            controller.ValidateViewModel(selectFuelViewModel);
            ActionResult result = controller.SelectFuel(selectFuelViewModel);

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<SelectFuelViewModel>();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Keys.Count.ShouldEqual(1);
        }


        [Test]
        public void ShouldDisplayInlineErrorWhenMeterTypeIsNotSelected()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var selectSmartMeterViewModel = new SelectMeterTypeViewModel();

            // Act
            controller.ValidateViewModel(selectSmartMeterViewModel);
            ActionResult result = controller.MeterType(selectSmartMeterViewModel);

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<SelectMeterTypeViewModel>();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Keys.Count.ShouldEqual(1);
        }


        [Test]
        public void ShouldDisplayInlineErrorWhenSmartMeterIsNotSelected()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var selectSmartMeterViewModel = new SelectSmartMeterViewModel();

            // Act
            controller.ValidateViewModel(selectSmartMeterViewModel);
            ActionResult result = controller.SmartMeter(selectSmartMeterViewModel);

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<SelectSmartMeterViewModel>();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Keys.Count.ShouldEqual(1);
        }

        [Test]
        public void ShouldPrePopulateManuallyEnteredAddressWhenCustomerRevisitsThePage()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var energyAddress = new QasAddress
            {
                HouseName = "1",
                AddressLine1 = "Waterloo Road",
                Town = "Havant"
            };
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                SelectedAddress = energyAddress
            });

            var fakeServiceWrapper = new FakeQASServiceWrapper();
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithQASServiceWrapper(fakeServiceWrapper)
                .Build<QuoteController>();

            // Act
            ActionResult result = controller.SelectAddress().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();
            viewModel.IsManual.ShouldBeTrue();
            viewModel.PropertyNumber.ShouldEqual(energyAddress.HouseName);
            viewModel.AddressLine1.ShouldEqual(energyAddress.AddressLine1);
            viewModel.Town.ShouldEqual(energyAddress.Town);
        }
        
        [Test]
        public void ShouldPrePopulateSelectedAddressWhenCustomerRevisitsThePage()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                SelectedAddress = new QasAddress
                {
                    Moniker = "ABC"
                }
            });
            var fakeServiceWrapper = new FakeQASServiceWrapper();
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithQASServiceWrapper(fakeServiceWrapper)
                .Build<QuoteController>();

            // Act
            ActionResult result = controller.SelectAddress().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();
            viewModel.IsManual.ShouldBeFalse();
            viewModel.SelectedAddressId.ShouldEqual("ABC");
        }

        [TestCase(FakeCAndCServiceWrapper.CAndCTestDataScenario.ElecPayGoNonSmart, FuelType.Electricity, "EnergyUsage", "Tariffs", BillingPreference.Paper)]
        [TestCase(FakeCAndCServiceWrapper.CAndCTestDataScenario.ElecPayGoSmartSmets1, FuelType.Electricity, "PaymentMethod", null, BillingPreference.None)]
        [TestCase(FakeCAndCServiceWrapper.CAndCTestDataScenario.ElecNonPayGoSmartSmets2, FuelType.Electricity, "PaymentMethod", null, BillingPreference.None)]
        [TestCase(FakeCAndCServiceWrapper.CAndCTestDataScenario.ElecNonPayGoNonSmart, FuelType.Electricity, "PaymentMethod", null, BillingPreference.None)]
        [TestCase(FakeCAndCServiceWrapper.CAndCTestDataScenario.OtherMeterTypes, FuelType.Dual, "OtherMeterType", null, BillingPreference.None)]
        public void ShouldRedirectToAppropriatePageWhenFuelTypeIsSelected(FakeCAndCServiceWrapper.CAndCTestDataScenario scenario, FuelType fuelType, string expectedAction, string expectedController, BillingPreference billingPreference)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var fakeCAndCServiceWrapper = new FakeCAndCServiceWrapper { TestDataScenario = scenario };

            var quoteController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithCAndCServiceWrapper(fakeCAndCServiceWrapper)
                .Build<QuoteController>();

            var viewModel = new SelectAddressViewModel
            {
                SelectedAddressId = "123~~43 Forbury Road, Reading, Berkshire"
            };

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithCAndCServiceWrapper(fakeCAndCServiceWrapper)
                .Build<QuoteController>();

            var selectFuelViewModel = new SelectFuelViewModel
            {
                FuelType = fuelType
            };

            // Act
            // ReSharper disable once UnusedVariable
            ActionResult quoteResult = quoteController.SelectAddress(viewModel).Result;
            controller.ValidateViewModel(selectFuelViewModel);
            ActionResult result = controller.SelectFuel(selectFuelViewModel);

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual(expectedAction);
            routeResult.RouteValues["controller"].ShouldEqual(expectedController);
            customer.SelectedBillingPreference.ShouldEqual(billingPreference);
        }

        [Test]
        public void ShouldRedirectToAreaNotCoveredWhenNorthernIrelandPostcodeSupplied()
        {
            // Arrange
            var controller = new ControllerFactory()
                .Build<QuoteController>();

            var viewModel = new PostcodeViewModel
            {
                PostCode = "BT1 4GT"
            };

            // Act
            ActionResult result = controller.EnterPostcode(viewModel).Result;

            // Assert            
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("AreaNotCovered");
        }

        [Test]
        [TestCase(FuelType.Electricity)]
        [TestCase(FuelType.Dual)]
        public void ShouldRedirectToElectricityMeterTypePageWhenPaymentMethodIsAndAppropriateFuelTypeIsSelected(FuelType fuelType)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());

            var selectFuelController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var selectFuelViewModel = new SelectFuelViewModel
            {
                FuelType = fuelType
            };

            var selectPaymentMethodViewModel = new SelectPaymentMethodViewModel
            {
                SelectedPaymentMethodId = PaymentMethod.MonthlyDirectDebit
            };

            // Act
            selectFuelController.SelectFuel(selectFuelViewModel);
            selectFuelController.ValidateViewModel(selectPaymentMethodViewModel);
            ActionResult result = selectFuelController.PaymentMethod(selectPaymentMethodViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("MeterType");
            routeResult.RouteValues["controller"].ShouldBeNull();
        }

        [Test]
        public void ShouldRedirectToEnergyUsagePageWhenSmartMeterFrequencyIsSelected()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var selectSmartMeterViewModel = new SmartMeterFrequencyViewModel { SmartMeterFrequencyId = "Daily" };

            // Act
            ActionResult result = controller.SmartMeterFrequency(selectSmartMeterViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("EnergyUsage");
            routeResult.RouteValues["controller"].ShouldEqual("Tariffs");
        }

        [Test]
        public void ShouldRedirectToEnergyUsagePageWhenSmartMeterIsSelected()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var selectSmartMeterViewModel = new SelectSmartMeterViewModel { HasSmartMeter = true };

            // Act
            ActionResult result = controller.SmartMeter(selectSmartMeterViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("EnergyUsage");
            routeResult.RouteValues["controller"].ShouldEqual("Tariffs");
        }

        [Test]
        public void ShouldRedirectToEnergyUsageScreenWhenPaymentMethodIsSelectedAndCustomerIsCAndCNonSmart()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                MeterDetail = FakeCAndCData.GetMeterDetailsWithElecNonPayGoNonSmart()
            });

            var selectFuelController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var selectFuelViewModel = new SelectFuelViewModel
            {
                FuelType = FuelType.Dual
            };

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var selectPaymentMethodViewModel = new SelectPaymentMethodViewModel
            {
                SelectedPaymentMethodId = PaymentMethod.MonthlyDirectDebit
            };

            // Act
            selectFuelController.SelectFuel(selectFuelViewModel);
            controller.ValidateViewModel(selectPaymentMethodViewModel);
            ActionResult result = controller.PaymentMethod(selectPaymentMethodViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("EnergyUsage");
            routeResult.RouteValues["controller"].ShouldEqual("Tariffs");
        }

        [Test]
        public void ShouldRedirectToFalloutPageWhenCAndCGeographicalAreaIsEmpty()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());

            var fakeCAndCServiceWrapper = new FakeCAndCServiceWrapper
            {
                TestDataScenario = FakeCAndCServiceWrapper.CAndCTestDataScenario.EmptyGeoCode
            };

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithCAndCServiceWrapper(fakeCAndCServiceWrapper)
                .Build<QuoteController>();

            var viewModel = new SelectAddressViewModel
            {
                SelectedAddressId = "123"
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = controller.SelectAddress(viewModel).Result;

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("UnableToComplete");
            routeResult.RouteValues["controller"].ShouldEqual("SignUp");
        }
        
        [Test]
        public void ShouldRedirectToFalloutWhenQASServiceDoesNotReturnAddress()
        {
            // Arrange
            var fakeServiceWrapper = new FakeQASServiceWrapper
            {
                ReturnAddressList = false
            };
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithQASServiceWrapper(fakeServiceWrapper)
                .Build<QuoteController>();

            // Act
            ActionResult result = controller.SelectAddress().Result;

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("UnableToComplete");
            routeResult.RouteValues["controller"].ShouldEqual("SignUp");
        }

        [Test]
        public void ShouldDisplayManualAddressPagetWhenQASEnabledIsFalse()
        {
            // Arrange
            var fakeServiceWrapper = new FakeQASServiceWrapper
            {
                ReturnAddressList = false
            };
            var fakeSessionManager = new FakeSessionManager();
            var fakeconfigManager = new FakeConfigManager();
            fakeconfigManager.AddConfiguration("QASEnabled","false");
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithQASServiceWrapper(fakeServiceWrapper)
                .WithConfigManager(fakeconfigManager)
                .Build<QuoteController>();

            // Act
            ActionResult result = controller.SelectAddress().Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();

           var viewmodel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();
           viewmodel.IsManual.ShouldBeTrue();
        }

        [Test]
        public void ShouldRedirectToFalloutWhenQASServiceDoesNotReturnAddressForSelectedAddress()
        {
            // Arrange
            var fakeServiceWrapper = new FakeQASServiceWrapper
            {
                ReturnAddressByMoniker = false
            };
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithQASServiceWrapper(fakeServiceWrapper)
                .Build<QuoteController>();

            var viewModel = new SelectAddressViewModel
            {
                SelectedAddressId = "123"
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = controller.SelectAddress(viewModel).Result;

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("UnableToComplete");
            routeResult.RouteValues["controller"].ShouldEqual("SignUp");
        }

        [Test]
        public void ShouldRedirectToFalloutWhenQASServiceThrowsExceptionWhileGettingAddressByMoniker()
        {
            // Arrange
            var fakeServiceWrapper = new FakeQASServiceWrapper
            {
                ThrowException = true
            };
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithQASServiceWrapper(fakeServiceWrapper)
                .Build<QuoteController>();

            var viewModel = new SelectAddressViewModel
            {
                SelectedAddressId = "123"
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = controller.SelectAddress(viewModel).Result;

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("UnableToComplete");
            routeResult.RouteValues["controller"].ShouldEqual("SignUp");
        }
        
        [Test]
        public void ShouldRedirectToFalloutWhenQASServiceThrowsExceptionWhileGettingListOfAddresses()
        {
            // Arrange
            var fakeServiceWrapper = new FakeQASServiceWrapper
            {
                ThrowException = true
            };
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithQASServiceWrapper(fakeServiceWrapper)
                .Build<QuoteController>();

            // Act
            ActionResult result = controller.SelectAddress().Result;

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("UnableToComplete");
            routeResult.RouteValues["controller"].ShouldEqual("SignUp");
        }

        [Test]
        public void ShouldRedirectToFalloutWhenSelectAddressHasAnInvalidSelectAddressId()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var viewModel = new SelectAddressViewModel
            {
                SelectedAddressId = "123"
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = controller.SelectAddress(viewModel).Result;

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("UnableToComplete");
            routeResult.RouteValues["controller"].ShouldEqual("SignUp");
        }

        [Test]
        public void ShouldRedirectToOtherMeterTypeFalloutWhenSelectedMeterTypeIsOther()
        {
            // Arrange
            var controller = new ControllerFactory()
                .Build<QuoteController>();

            var selectSmartMeterViewModel = new SelectMeterTypeViewModel
            {
                SelectedElectricityMeterType = ElectricityMeterType.Other
            };

            // Act
            controller.ValidateViewModel(selectSmartMeterViewModel);
            ActionResult result = controller.MeterType(selectSmartMeterViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("OtherMeterType");
        }

        [Test]
        public void ShouldRedirectToPaymentMethodPageWhenFuelTypeIsSelected()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var selectFuelViewModel = new SelectFuelViewModel
            {
                FuelType = FuelType.Dual
            };

            // Act
            controller.ValidateViewModel(selectFuelViewModel);
            ActionResult result = controller.SelectFuel(selectFuelViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("PaymentMethod");
            routeResult.RouteValues["controller"].ShouldBeNull();
        }

        [Test]
        public void ShouldRedirectToSelectFuelTypeWhenAddressIsSelected()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var viewModel = new SelectAddressViewModel
            {
                SelectedAddressId = "123~~43 Forbury Road, Reading, Berkshire"
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = controller.SelectAddress(viewModel).Result;

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            var energyCustomer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            AssertEnergyCustomerHasBeenReset(energyCustomer);
            routeResult.RouteValues["action"].ShouldEqual("SelectFuel");
            routeResult.RouteValues["controller"].ShouldEqual(null);
        }

        [Test]
        public void ShouldRedirectToSelectFuelTypeWhenAValidAddressIsEnteredManually()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var viewModel = new SelectAddressViewModel
            {
                IsManual = true,
                PropertyNumber = "123456789012345678901234567890",
                AddressLine1 = "12345678901234567890123456789012345678901234567890",
                Town = "abcdefghijklmnopqrstuvwxyzABCD"
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = controller.SelectAddress(viewModel).Result;

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            var energyCustomer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            AssertEnergyCustomerHasBeenReset(energyCustomer);
            routeResult.RouteValues["action"].ShouldEqual("SelectFuel");
            routeResult.RouteValues["controller"].ShouldEqual(null);
        }

        [Test]
        public void ShouldRedirectToSmartFrequencyPageWhenCustomerHasSmartMeter()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                MeterDetail = FakeCAndCData.GetMeterDetailsWithElecPayGoSmartSmets1(),
                SelectedFuelType = FuelType.Electricity
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var selectPaymentMethodViewModel = new SelectPaymentMethodViewModel
            {
                SelectedPaymentMethodId = PaymentMethod.MonthlyDirectDebit
            };

            // Act
            controller.ValidateViewModel(selectPaymentMethodViewModel);
            ActionResult result = controller.PaymentMethod(selectPaymentMethodViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("SmartMeterFrequency");
            routeResult.RouteValues["controller"].ShouldBeNull();
        }

        [Test]
        public void ShouldRedirectToSmartMeterPageWhenElectricityMeterTypeIsSelected()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var selectMeterTypeViewModel = new SelectMeterTypeViewModel { SelectedElectricityMeterType = ElectricityMeterType.Economy7 };

            // Act
            ActionResult result = controller.MeterType(selectMeterTypeViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("SmartMeter");
            routeResult.RouteValues["controller"].ShouldBeNull();
        }

        [Test]
        public void ShouldRedirectToSmartMeterTypePageWhenPaymentMethodIsSelectedAndFuelTypeIsGas()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());

            var selectFuelController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var selectFuelViewModel = new SelectFuelViewModel
            {
                FuelType = FuelType.Gas
            };

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var selectPaymentMethodViewModel = new SelectPaymentMethodViewModel
            {
                SelectedPaymentMethodId = PaymentMethod.MonthlyDirectDebit
            };

            // Act
            selectFuelController.SelectFuel(selectFuelViewModel);
            controller.ValidateViewModel(selectPaymentMethodViewModel);
            ActionResult result = controller.PaymentMethod(selectPaymentMethodViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("SmartMeter");
            routeResult.RouteValues["controller"].ShouldBeNull();
        }

        [Test]
        public void ShouldRedirectToTheCannotCompleteOnlineWhenCAndCApiGeographicalAreaIsEmpty()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());

            var fakeCAndCWrapper = new FakeCAndCServiceWrapper
            {
                TestDataScenario = FakeCAndCServiceWrapper.CAndCTestDataScenario.EmptyGeoCode
            };

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithCAndCServiceWrapper(fakeCAndCWrapper)
                .Build<QuoteController>();

            var viewModel = new SelectAddressViewModel
            {
                SelectedAddressId = "123"
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = controller.SelectAddress(viewModel).Result;

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("UnableToComplete");
            routeResult.RouteValues["controller"].ShouldEqual("SignUp");
        }

        [Test]
        public void ShouldRedirectToTheSelectFuelWhenExceptionIsThrownFromCAndCService()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());

            var fakeCAndCServiceWrapper = new FakeCAndCServiceWrapper { ThrowException = true };

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithCAndCServiceWrapper(fakeCAndCServiceWrapper)
                .Build<QuoteController>();

            var viewModel = new SelectAddressViewModel
            {
                SelectedAddressId = "123~~43 Forbury Road, Reading, Berkshire"
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = controller.SelectAddress(viewModel).Result;

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("SelectFuel");
            routeResult.RouteValues["controller"].ShouldEqual(null);
        }

        [Test]
        public async Task ShouldResetSelectedExtras()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
            var extra1 = new Extra(
                "SSE Electrical Wiring Cover",
                11.5,
                0.0,
                "EC",
                3,
                12,
                "Cover for your fixed electrical wiring system, giving you peace of mind",
                new List<string>
                {
                    "Repairs to your fixed wiring system",
                    "Parts, labour and unlimited call-outs",
                    "24/7 emergency helpline",
                    "24-hour call-outs for emergency repairs",
                    "An inspection every five years of continuous cover"
                },
                new List<string>
                {
                    "Repairing the power supply to your property or the electricity meter",
                    "Any items that aren't part of the fixed electrical wiring system",
                    "Electrical heating equipment",
                    "Major rewiring works. This product only covers repairing faults"
                },
                ExtraType.ElectricalWiring
            );

            var energyCustomer = new EnergyCustomer
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
                SelectedTariff = new Tariff { Extras = new List<Extra> { extra1 } },
                SelectedAddress = new QasAddress
                    { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" },
                SelectedExtras = new HashSet<Extra> { extra1 }
            };

            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, energyCustomer);
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());

            var tariffsController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<TariffsController>();

            await tariffsController.AvailableTariffs();
            await tariffsController.AvailableTariffs("EC");
            energyCustomer.SelectedExtras.ShouldBeEmpty();
        }

        [TestCaseSource(nameof(GetInvalidAddresses))]
        public void ShouldReturnToEnterAddressManuallyWhenManuallyEnteredAddressIsNotValid(SelectAddressViewModel model, int errorCount, string description)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = controller.SelectAddress(model).Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<SelectAddressViewModel>();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Keys.Count.ShouldEqual(errorCount);
        }

        [TestCase("", "Please enter your postcode.")]
        // ReSharper disable once StringLiteralTypo
        [TestCase("PO", "Sorry, we don’t recognise that postcode. Please try entering it again.")]
        public void ShouldReturnToEnterPostcodeWhenPostcodeIsMissingOrInvalid(string postcode, string errorMessage)
        {
            // Arrange
            var controller = new ControllerFactory()
                .Build<QuoteController>();

            var postcodeViewModel = new PostcodeViewModel
            {
                PostCode = postcode
            };

            // Act
            controller.ValidateViewModel(postcodeViewModel);
            ActionResult result = controller.EnterPostcode(postcodeViewModel).Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<PostcodeViewModel>();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Keys.Count.ShouldEqual(1);
            controller.ModelState.Values.First().Errors.First().ErrorMessage.ShouldEqual(errorMessage);
        }

        [Test]
        public void ShouldReturnToSelectAddressWhenAddressIsNotSelected()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var viewModel = new SelectAddressViewModel();

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = controller.SelectAddress(viewModel).Result;

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<SelectAddressViewModel>();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Keys.Count.ShouldEqual(1);
            controller.ModelState.Values.First().Errors.First().ErrorMessage.ShouldEqual("Please select your address.");
        }

        [TestCase("MEX01")]
        [TestCase(null)]
        public async Task ShouldSetChosenTariffWhenCustomerSelectedATariffOnHubPage(string productCode)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var postcodeViewModel = new PostcodeViewModel
            {
                PostCode = "PO9 1BH",
                ProductCode = productCode
            };

            // Act
            controller.ValidateViewModel(postcodeViewModel);
            await controller.EnterPostcode(postcodeViewModel);

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.ChosenProduct.ShouldEqual(productCode);
        }

        [Test]
        public void ShouldSetManuallyEnteredAddressInSession()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var viewModel = new SelectAddressViewModel
            {
                IsManual = true,
                PropertyNumber = "123456789012345678901234567890",
                AddressLine1 = "12345678901234567890123456789012345678901234567890",
                Town = "abcdefghijklmnopqrstuvwxyzABCD"
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = controller.SelectAddress(viewModel).Result;

            // Assert
            result.ShouldBeType<RedirectToRouteResult>();
            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.SelectedAddress.HouseName.ShouldEqual(viewModel.PropertyNumber);
            customer.SelectedAddress.AddressLine1.ShouldEqual(viewModel.AddressLine1);
            customer.SelectedAddress.Town.ShouldEqual(viewModel.Town);
            customer.SelectedAddress.PicklistEntry.ShouldEqual("123456789012345678901234567890 12345678901234567890123456789012345678901234567890 abcdefghijklmnopqrstuvwxyzABCD ");
        }

        [Test]
        public void ShouldSetSelectedQASAddressInSession()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var viewModel = new SelectAddressViewModel
            {
                SelectedAddressId = "123~~1 Waterloo Road, Havant"
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = controller.SelectAddress(viewModel).Result;

            // Assert
            result.ShouldBeType<RedirectToRouteResult>();
            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.SelectedAddress.HouseName.ShouldEqual("1");
            customer.SelectedAddress.AddressLine1.ShouldEqual("Waterloo Road");
            customer.SelectedAddress.Town.ShouldEqual("Havant");
            customer.SelectedAddress.PicklistEntry.ShouldEqual("1 Waterloo Road, Havant");
        }

        [Test]
        public void ShouldStayOnPaymentMethodPageAndShowErrorMessageIfPaymentMethodIsNotSelected()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var paymentMethodViewModel = new SelectPaymentMethodViewModel
            {
                SelectedPaymentMethodId = null
            };

            // Act
            controller.ValidateViewModel(paymentMethodViewModel);
            ActionResult result = controller.PaymentMethod(paymentMethodViewModel);

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<SelectPaymentMethodViewModel>();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Keys.Count.ShouldEqual(1);
            controller.ModelState.Values.First().Errors.First().ErrorMessage.ShouldEqual("Please choose your payment method");
        }

        [Test]
        [TestCase(PaymentMethod.MonthlyDirectDebit, BillingPreference.Paperless)]
        [TestCase(PaymentMethod.Quarterly, BillingPreference.Paperless)]
        [TestCase(PaymentMethod.PayAsYouGo, BillingPreference.Paper)]
        public void ShouldStoreBillingPreferenceInTheSession(PaymentMethod? selectedPaymentMethod, BillingPreference billingPreference)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var paymentMethodViewModel = new SelectPaymentMethodViewModel
            {
                SelectedPaymentMethodId = selectedPaymentMethod
            };

            // Act
            controller.ValidateViewModel(paymentMethodViewModel);
            controller.PaymentMethod(paymentMethodViewModel);

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            // ReSharper disable once PossibleInvalidOperationException
            customer.SelectedPaymentMethod.ShouldEqual(selectedPaymentMethod.Value);
            customer.SelectedBillingPreference.ShouldEqual(billingPreference);
        }

        [Test]
        [TestCase("Standard", "Standard")]
        [TestCase("Argos_2_Year_Fixed", "Argos 2 Year Fixed")]
        [TestCase("", "")]
        public async Task ShouldStoreCorrectProductCodeInTheSessionWhenProductCodeIsPassedFromHubPage(string productCodeFromHubPage, string expectedProductCode)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            const string postcode = "PO9 1BH";

            var postcodeViewModel = new PostcodeViewModel
            {
                PostCode = postcode,
                ProductCode = productCodeFromHubPage
            };

            // Act
            controller.ValidateViewModel(postcodeViewModel);
            await controller.EnterPostcode(postcodeViewModel);

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.Postcode.ShouldEqual(postcode);
            customer.ChosenProduct.ShouldEqual(expectedProductCode);
        }

        [Test]
        public void ShouldStoreFuelTypeInTheSessionWhenCustomerSelectedAFuelType()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();
            const FuelType fuelType = FuelType.Dual;
            var selectFuelViewModel = new SelectFuelViewModel
            {
                FuelType = fuelType
            };

            // Act
            controller.ValidateViewModel(selectFuelViewModel);
            controller.SelectFuel(selectFuelViewModel);

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.SelectedFuelType.ShouldEqual(fuelType);
        }

        [Test]
        public void ShouldStoreHasSmartMeterInTheSession()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();
            var meterTypeViewModel = new SelectSmartMeterViewModel { HasSmartMeter = true };

            // Act
            controller.ValidateViewModel(meterTypeViewModel);
            controller.SmartMeter(meterTypeViewModel);

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.HasSmartMeter.ShouldEqual(true);
        }

        [Test]
        [TestCase("")]
        [TestCase("02392123456")]
        [TestCase("02392 123456")]
        public async Task ShouldStorePostcodeAndOptionalLandlineInTheSessionWhenCustomerEnteredAValidPostcode(string phoneNumber)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            const string postcode = "PO9 1BH";

            var postcodeViewModel = new PostcodeViewModel
            {
                PostCode = postcode,
                Landline = phoneNumber
            };

            // Act
            controller.ValidateViewModel(postcodeViewModel);
            await controller.EnterPostcode(postcodeViewModel);

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.Postcode.ShouldEqual(postcode);
            customer.CLIChoice.UserProvidedCLI.ShouldEqual(postcodeViewModel.Landline?.Replace(" ", string.Empty));
        }

        [Test]
        [TestCase(FuelType.Gas, ElectricityMeterType.Economy7, ElectricityMeterType.None)]
        [TestCase(FuelType.Electricity, ElectricityMeterType.Economy7, ElectricityMeterType.Economy7)]
        [TestCase(FuelType.Dual, ElectricityMeterType.Economy7, ElectricityMeterType.Economy7)]
        public void ShouldStoreSelectedMeterTypeInTheSession(FuelType selectedFuelType, ElectricityMeterType selectedMeterType,
            ElectricityMeterType expectedMeterType)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer { SelectedFuelType = selectedFuelType });
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();
            var meterTypeViewModel = new SelectMeterTypeViewModel { SelectedElectricityMeterType = selectedMeterType };

            // Act
            controller.ValidateViewModel(meterTypeViewModel);
            controller.MeterType(meterTypeViewModel);

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.SelectedElectricityMeterType.ShouldEqual(expectedMeterType);
        }

        [Test]
        [TestCase(PaymentMethod.MonthlyDirectDebit)]
        [TestCase(PaymentMethod.Quarterly)]
        [TestCase(PaymentMethod.PayAsYouGo)]
        public void ShouldStoreSelectedPaymentMethodInTheSession(PaymentMethod? selectedPaymentMethod)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            var paymentMethodViewModel = new SelectPaymentMethodViewModel
            {
                SelectedPaymentMethodId = selectedPaymentMethod
            };

            // Act
            controller.ValidateViewModel(paymentMethodViewModel);
            controller.PaymentMethod(paymentMethodViewModel);

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            // ReSharper disable once PossibleInvalidOperationException
            customer.SelectedPaymentMethod.ShouldEqual(selectedPaymentMethod.Value);
            customer.DirectDebitDetails.ShouldBeNull();
        }

        [Test]
        public void ShouldTakeToSmartMeterFrequencyPage()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                MeterDetail = FakeCAndCData.GetMeterDetailsWithElecNonPayGoSmartSSEInstalled()
            });
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            // Act
            ActionResult result = controller.SmartMeterFrequency();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewmodel = viewResult.Model.ShouldBeType<SmartMeterFrequencyViewModel>();

            viewmodel.SmartMeterFrequencyEnabled.ShouldBeTrue();
            Assert.AreEqual("~/Areas/Energy/Views/Quote/SmartMeterConsent.cshtml", viewResult.ViewName);
        }

        [Test]
        public void ShouldTakeToSmartMeterMessagePage()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                MeterDetail = FakeCAndCData.GetMeterDetailsWithElecPayGoSmartSmets1()
            });
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<QuoteController>();

            // Act
            ActionResult result = controller.SmartMeterFrequency();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewmodel = viewResult.Model.ShouldBeType<SmartMeterFrequencyViewModel>();

            viewmodel.SmartMeterFrequencyEnabled.ShouldBeFalse();
            Assert.AreEqual("~/Areas/Energy/Views/Quote/SmartMeterConsent.cshtml", viewResult.ViewName);
        }

        [Test]
        public void ShouldThrowExceptionWhenPageAccessedDirectly()
        {
            // Arrange
            var controller = new ControllerFactory()
                .Build<QuoteController>();

            // Act + Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => controller.SelectAddress());
            ex.Message.ShouldEqual("energyCustomer is null");
        }

        private static IEnumerable<TestCaseData> CAndCTestCases()
        {
            yield return new TestCaseData(null, "PayAsYouGo");
            yield return new TestCaseData(FakeCAndCData.GetMeterDetailsWithElecNonPayGoNonSmart(), null);
            yield return new TestCaseData(FakeCAndCData.GetMeterDetailsWithElecPayGoSmartSmets2(), "PayAsYouGo");
        }

        private static IEnumerable<TestCaseData> GetInvalidAddresses()
        {
            yield return new TestCaseData(new SelectAddressViewModel
            {
                IsManual = true
            }, 3, "3 required errors for mandatory fields.");

            yield return new TestCaseData(new SelectAddressViewModel
            {
                IsManual = true,
                PropertyNumber = "1234567890123456789012345678901",
                AddressLine1 = "123456789012345678901234567890123456789012345678901",
                Town = "abcdefghijklmnopqrstuvwxyzABCDE"
            }, 3, "3 regex errors for mandatory fields.");

            yield return new TestCaseData(new SelectAddressViewModel
            {
                IsManual = true,
                PropertyNumber = "123456789012345678901234567890",
                AddressLine1 = "12345678901234567890123456789012345678901234567890",
                AddressLine2 = "123456789012345678901234567890123456789012345678901",
                Town = "abcdefghijklmnopqrstuvwxyzABCD",
                County = "1234567890123456789012345678901"
            }, 2, "2 regex errors for optional fields.");
        }
    }
}