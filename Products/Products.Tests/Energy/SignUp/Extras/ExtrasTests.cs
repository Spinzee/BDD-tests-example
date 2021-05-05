namespace Products.Tests.Energy.SignUp.Extras
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Common.Fakes;
    using Common.Helpers;
    using Core;
    using Core.Enums;
    using Fakes.Services;
    using Helpers;
    using NUnit.Framework;
    using Products.Infrastructure.Extensions;
    using Products.Model.Common;
    using Products.Model.Constants;
    using Products.Model.Energy;
    using Products.Model.Enums;
    using Should;
    using TariffChange.Fakes.Managers;
    using Web.Areas.Energy.Controllers;
    using WebModel.ViewModels.Common;
    using WebModel.ViewModels.Energy;

    public class ExtrasTests
    {
        [Test]
        public async Task ShouldNotRedirectToExtrasView()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
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

            // Act
            var tariffController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper { ReturnSingleBundle = true })
                .WithConfigManager(fakeConfigManager)
                .Build<TariffsController>();

            await tariffController.AvailableTariffs();

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            await tariffController.AvailableTariffs("BO001");
            ActionResult result = signUpController.PhonePackage("FF3_LR18", new PhonePackageViewModel { KeepYourNumberViewModel = new KeepYourNumberViewModel() });

            // Assert
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("PersonalDetails");
        }

        [Test]
        public async Task ShouldRedirectToExtrasView()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
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

            // Act
            var tariffController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper { ReturnFixNProtectBundle = true })
                .WithConfigManager(fakeConfigManager)
                .Build<TariffsController>();

            await tariffController.AvailableTariffs();
            ActionResult result = await tariffController.AvailableTariffs("BO002");

            // Assert
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("Extras");
        }

        [Test]
        public void ShouldGoBackToPhonePackageFromExtrasView()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());
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
                SelectedTariff = new Tariff
                {
                    ElectricityProduct = new Product
                    {
                        ProjectedYearlyCost = 112.23
                    },
                    GasProduct = new Product
                    {
                        ProjectedYearlyCost = 234.55
                    },
                    BundlePackage = new BundlePackage("100", 12, 22, BundlePackageType.FixAndFibre, new List<TariffTickUsp>()),
                    IsBundle = true
                },
                SelectedAddress = new QasAddress
                    { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" }
            });

            // Act
            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            ActionResult result = signUpController.Extras();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<ExtrasViewModel>();
            viewModel.BackChevronViewModel.ActionName.ShouldEqual("PhonePackage");
            viewModel.BackChevronViewModel.ControllerName.ShouldEqual("SignUp");
        }

        [Test]
        public void ShouldGoBackToAvailableTariffsFromExtrasView()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());
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
                SelectedTariff = new Tariff
                {
                    ElectricityProduct = new Product
                    {
                        ProjectedYearlyCost = 112.23
                    },
                    GasProduct = new Product
                    {
                        ProjectedYearlyCost = 234.55
                    },
                    BundlePackage = new BundlePackage("100", 12, 22, BundlePackageType.FixAndProtect, new List<TariffTickUsp>()),
                    IsBundle = true
                },
                SelectedAddress = new QasAddress { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" }
            });

            // Act
            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            ActionResult result = signUpController.Extras();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<ExtrasViewModel>();
            viewModel.BackChevronViewModel.ActionName.ShouldEqual("AvailableTariffs");
            viewModel.BackChevronViewModel.ControllerName.ShouldEqual("Tariffs");
        }

        [Test]
        public void ShouldGoBackToExtrasViewFromPersonalDetailsView()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());
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
                SelectedTariff = new Tariff
                {
                    ElectricityProduct = new Product
                    {
                        ProjectedYearlyCost = 112.23
                    },
                    GasProduct = new Product
                    {
                        ProjectedYearlyCost = 234.55
                    },
                    BundlePackage = new BundlePackage("100", 12, 22, BundlePackageType.FixAndProtect, new List<TariffTickUsp>()),
                    IsBundle = true
                },
                SelectedAddress = new QasAddress
                    { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" }
            });

            // Act
            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            ActionResult result = signUpController.PersonalDetails();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<PersonalDetailsViewModel>();
            viewModel.BackChevronViewModel.ActionName.ShouldEqual("Extras");
            viewModel.BackChevronViewModel.ControllerName.ShouldEqual("Signup");
        }

        [Test]
        public void ShouldAddExtrasAndRenderCorrectly()
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

            var extraItemViewModel1 = new ExtrasItemViewModel(extra1.Name,
                extra1.BundlePrice,
                extra1.ProductCode,
                extra1.TagLine,
                extra1.BulletList1,
                extra1.BulletList2,
                true,
                "http://localhost:3075");

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
                { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" }
            };

            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, energyCustomer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            ActionResult result = signUpController.AddExtra("EC");

            // Assert YourPriceViewModel properties
            energyCustomer.SelectedExtras.Contains(extra1).ShouldBeTrue();
            var partialViewResult = result.ShouldBeType<PartialViewResult>();
            var yourPriceViewModel = partialViewResult.Model.ShouldBeType<YourPriceViewModel>();
            yourPriceViewModel.TotalItemsInBasket.ShouldEqual(2);
            yourPriceViewModel.ShowPhonePackage.ShouldBeFalse();
            yourPriceViewModel.BasketTogglerIconFilepath.ShouldEqual("http://localhost:3075/Content/Svgs/icons/basket-trigger-2item.svg");
            yourPriceViewModel.SelectedExtras.Count.ShouldEqual(1);
            yourPriceViewModel.SelectedExtras.Contains(extraItemViewModel1).ShouldBeTrue();
            yourPriceViewModel.ShowExtras.ShouldBeTrue();
            yourPriceViewModel.ProjectedMonthlyTotalFullValue.ShouldEqual("£11");
            yourPriceViewModel.ProjectedMonthlyTotalPenceValue.ShouldEqual(".50");
            yourPriceViewModel.AnnualSavingsText.ShouldEqual("");
            yourPriceViewModel.BasketCssClass.ShouldEqual("basket-2-items");

            result = signUpController.Extras();

            // Assert ExtrasViewModel properties
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.ViewName.ShouldEqual("");

            var viewModel = viewResult.Model.ShouldBeType<ExtrasViewModel>();
            viewModel.Extras.Count.ShouldEqual(1);
            ExtrasItemViewModel extra = viewModel.Extras[0];
            extra.Name.ShouldEqual("SSE Electrical Wiring Cover");
            extra.ProductCode.ShouldEqual("EC");
            extra.AddRemoveButtonText.ShouldEqual("Remove from Bundle");
            extra.AddRemoveButtonAltText.ShouldEqual("Remove this Extra from your Bundle");
            extra.IsAddedToBasket.ShouldEqual(true);
            extra.IsAddedToBasketStr.ShouldEqual("true");
            extra.AddedCssClass.ShouldEqual("added");
            extra.BulletList1.Count.ShouldEqual(5);
            extra.BulletList2.Count.ShouldEqual(4);
            extra.TagLine.ShouldEqual("Cover for your fixed electrical wiring system, giving you peace of mind");
            extra.Price.ShouldEqual("£11.50");
            extra.BulletList1[0] = "Repairs to your fixed wiring system";
            extra.RemoveButtonAltText.ShouldEqual("Remove this Extra from your Bundle");
            extra.AddButtonAltText.ShouldEqual("Add this Extra to your Bundle");
            extra.RemoveButtonIconUrl.ShouldEqual("http://localhost:3075/Content/Svgs/icons/trashcan-white.svg");
            extra.MoreInformationModalId.ShouldEqual("extraMoreInformation-EC");
            extra.ModalAddRemoveButtonText.ShouldEqual("Remove from Bundle");
            extra.ModalAddRemoveButtonCssClass.ShouldEqual("button-secondary");
            extra.ExtraContainerId.ShouldEqual("extra-container-EC");
            extra.ButtonGroup.ShouldEqual("button-extra-EC");
            extra.ModalExtraPriceHeaderId.ShouldEqual("extra-modal-priceheader-EC");
        }

        [Test]
        public void ShouldRemoveExtrasAndRenderCorrectly()
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

            var extraItemViewModel1 = new ExtrasItemViewModel(extra1.Name,
                extra1.BundlePrice,
                extra1.ProductCode,
                extra1.TagLine,
                extra1.BulletList1,
                extra1.BulletList2,
                true,
                "http://localhost:3075");

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
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            ActionResult result = signUpController.RemoveExtra("EC");

            // Assert YourPriceViewModel properties
            energyCustomer.SelectedExtras.Contains(extra1).ShouldBeFalse();
            var partialViewResult = result.ShouldBeType<PartialViewResult>();
            var yourPriceViewModel = partialViewResult.Model.ShouldBeType<YourPriceViewModel>();
            yourPriceViewModel.TotalItemsInBasket.ShouldEqual(1);
            yourPriceViewModel.ShowPhonePackage.ShouldBeFalse();
            yourPriceViewModel.BasketTogglerIconFilepath.ShouldEqual("http://localhost:3075/Content/Svgs/icons/basket-trigger-1item.svg");
            yourPriceViewModel.SelectedExtras.Count.ShouldEqual(0);
            yourPriceViewModel.SelectedExtras.Contains(extraItemViewModel1).ShouldBeFalse();
            yourPriceViewModel.ShowExtras.ShouldBeFalse();
            yourPriceViewModel.ProjectedMonthlyTotalFullValue.ShouldEqual("£0");
            yourPriceViewModel.ProjectedMonthlyTotalPenceValue.ShouldEqual(".00");
            yourPriceViewModel.AnnualSavingsText.ShouldEqual("");
            yourPriceViewModel.BasketCssClass.ShouldEqual("basket-1-items");

            result = signUpController.Extras();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<ExtrasViewModel>();
            viewModel.Extras.Count.ShouldEqual(1);
            ExtrasItemViewModel extra = viewModel.Extras[0];
            extra.Name.ShouldEqual("SSE Electrical Wiring Cover");
            extra.ProductCode.ShouldEqual("EC");
            extra.AddRemoveButtonText.ShouldEqual("Add to Bundle");
            extra.AddRemoveButtonAltText.ShouldEqual("Add this Extra to your Bundle");
            extra.IsAddedToBasket.ShouldEqual(false);
            extra.IsAddedToBasketStr.ShouldEqual("false");
            extra.AddedCssClass.ShouldEqual("");
            extra.BulletList1.Count.ShouldEqual(5);
            extra.BulletList2.Count.ShouldEqual(4);
            extra.TagLine.ShouldEqual("Cover for your fixed electrical wiring system, giving you peace of mind");
            extra.Price.ShouldEqual(11.5.ToCurrency());
            extra.BulletList1[0] = "Repairs to your fixed wiring system";
            extra.RemoveButtonAltText.ShouldEqual("Remove this Extra from your Bundle");
            extra.AddButtonAltText.ShouldEqual("Add this Extra to your Bundle");
            extra.RemoveButtonIconUrl.ShouldEqual("http://localhost:3075/Content/Svgs/icons/trashcan-white.svg");
            extra.MoreInformationModalId.ShouldEqual("extraMoreInformation-EC");
            extra.ModalAddRemoveButtonText.ShouldEqual("Add to Bundle");
            extra.ModalAddRemoveButtonCssClass.ShouldEqual("button-primary");
            extra.ExtraContainerId.ShouldEqual("extra-container-EC");
            extra.ButtonGroup.ShouldEqual("button-extra-EC");
            extra.ModalExtraPriceHeaderId.ShouldEqual("extra-modal-priceheader-EC");
        }

        [Test]
        public void ShouldRemoveExtrasAndRenderCorrectlyInSummaryView()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
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

            EnergyCustomer energyCustomer = CustomerFactory.GetCustomerWithFullDetails(FuelType.Dual, ElectricityMeterType.Standard);
            energyCustomer.SelectedTariff = new Tariff { DisplayName = "tariff1", Extras = new List<Extra> { extra1 } };
            energyCustomer.SelectedExtras = new HashSet<Extra> { extra1 };

            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, energyCustomer);
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, FakeProductsStub.GetTariffs(FuelType.Dual.ToString(), "Standard"));

            var fakeTariffManager = new FakeTariffManager();
            fakeTariffManager.TaglineMappings.Add("tariff1", "hello");

            var signUpController = new ControllerFactory()
                .WithTariffManager(fakeTariffManager)
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            ActionResult result = signUpController.RemoveExtraSummary("EC");

            // Assert YourPriceViewModel properties
            energyCustomer.SelectedExtras.Contains(extra1).ShouldBeFalse();
            var partialViewResult = result.ShouldBeType<PartialViewResult>();
            var summaryViewModel = partialViewResult.Model.ShouldBeType<SummaryViewModel>();
            summaryViewModel.SelectedExtras.Count.ShouldEqual(0);
            summaryViewModel.DisplayExtrasSection.ShouldBeFalse();
        }
    }
}