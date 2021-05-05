namespace Products.Tests.Energy.SignUp.Upgrades
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Broadband.Fakes.Services;
    using Broadband.Model;
    using Common.Fakes;
    using Common.Helpers;
    using Core;
    using Core.Enums;
    using Fakes.Services;
    using Helpers;
    using HomeServices.Helpers;
    using NUnit.Framework;
    using Products.Model.Broadband;
    using Products.Model.Common;
    using Products.Model.Constants;
    using Products.Model.Energy;
    using Products.Model.Enums;
    using Should;
    using TariffChange.Fakes.Managers;
    using Web.Areas.Energy.Controllers;
    using WebModel.ViewModels.Energy;
    using ControllerFactory = Helpers.ControllerFactory;
    using FakeSessionManager = Common.Fakes.FakeSessionManager;
    using Tariff = Products.Model.Energy.Tariff;

    /// <summary>
    /// Upgrades page tests.
    /// </summary>
    public class UpgradesTests
    {
        [Test]
        public void ShouldPopulateFixNFibrePlusBundleUpgradeViewModel()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();

            EnergyCustomer energyCustomer = CustomerFactory.CustomerDetailsWithFixNFibreBundleTariffChosen(FuelType.Dual, ElectricityMeterType.Standard);
            energyCustomer.AvailableBundleUpgrade = new Tariff
            {
                DisplayName = "Tariff1",
                ElectricityProduct = new Product(),
                GasProduct = new Product(),
                BundlePackage = new BundlePackage("FFP_LR18", 17.00, 28.00, BundlePackageType.FixAndFibre, new List<TariffTickUsp>()),
                TariffId = "BO009",
                EnergyTickUsps = new List<TariffTickUsp>(),
                IsBundle = true,
                Extras = new List<Extra>(),
                TariffGroup = TariffGroup.FixAndFibre,
                IsSmartTariff = false,
                IsUpgrade = true
            };
            var broadbandProduct1 = new BroadbandProduct
            {
                BroadbandType = BroadbandType.FibrePlus,
                ProductOrder = 3,
                IsAvailable = true,
                TalkProducts = new List<TalkProduct>
                {
                    new TalkProduct
                    {
                        TalkCode = "LRO",
                        ProductCode = "FFP_LR18",
                        Prices = new List<BroadbandPrice>
                        {
                            new BroadbandPrice {FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 35},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 16},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 0}
                        },
                        BroadbandProductGroup = BroadbandProductGroup.FixAndFibrePlus
                    }
                },
                LineSpeed = new FibreLineSpeeds
                {
                    MinDownload = "28000",
                    MaxDownload = "72000",
                    MinUpload = "8000",
                    MaxUpload = "12000"
                }
            };
            var broadbandProduct2 = new BroadbandProduct
            {
                BroadbandType = BroadbandType.FibrePlus,
                ProductOrder = 3,
                IsAvailable = true,
                TalkProducts = new List<TalkProduct>
                {
                    new TalkProduct
                    {
                        TalkCode = "AL19",
                        ProductCode = "FFP_ANY18",
                        Prices = new List<BroadbandPrice>
                        {
                            new BroadbandPrice {FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 35},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 16},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 0}
                        },
                        BroadbandProductGroup = BroadbandProductGroup.FixAndFibrePlus
                    }
                },
                LineSpeed = new FibreLineSpeeds
                {
                    MinDownload = "28000",
                    MaxDownload = "72000",
                    MinUpload = "8000",
                    MaxUpload = "12000"
                }
            };

            fakeSessionManager.SessionObject.Add(SessionKeys.BroadbandProducts, new List<BroadbandProduct> { broadbandProduct1, broadbandProduct2 });
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, energyCustomer);
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            ActionResult result = signUpController.Upgrades();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<BundleUpgradeViewModel>();
            viewModel.Header.ShouldEqual("Boost your internet up to 72Mbps");
            viewModel.ProductCode.ShouldEqual("BO009");
            viewModel.AddRemoveButtonText.ShouldEqual("Add to Bundle");
            viewModel.AddRemoveButtonAltText.ShouldEqual("Add this Upgrade to your Bundle");
            viewModel.IsAddedToBasket.ShouldEqual(false);
            viewModel.IsAddedToBasketStr.ShouldEqual("false");
            viewModel.AddedCssClass.ShouldEqual("");
            viewModel.UpgradeDescription.ShouldEqual(
                "Upgrade to our <b>Fibre Plus</b> broadband. With unlimited downloads and an average speed of 63Mbps, it's great for streaming HD movies, playing games online and downloading larger files.");
            viewModel.Price.ShouldEqual("£28");
            viewModel.DiscountedPrice.ShouldEqual("£17");
            viewModel.DiscountAmount.ShouldEqual("£11");
            viewModel.RemoveButtonAltText.ShouldEqual("Remove this Upgrade from your Bundle");
            viewModel.AddButtonAltText.ShouldEqual("Add this Upgrade to your Bundle");
            viewModel.RemoveButtonIconUrl.ShouldEqual("http://localhost:3075/Content/Svgs/icons/trashcan-white.svg");
            viewModel.MoreInformationModalId.ShouldEqual("extraMoreInformation-BO009");
            viewModel.ModalAddRemoveButtonText.ShouldEqual("Add to Bundle");
            viewModel.ModalAddRemoveButtonCssClass.ShouldEqual("button-primary");
            viewModel.ExtraContainerId.ShouldEqual("extra-container-BO009");
            viewModel.ButtonGroup.ShouldEqual("button-extra-BO009");
            viewModel.ModalExtraPriceHeaderId.ShouldEqual("extra-modal-priceheader-BO009");
            viewModel.WhatYouNeedToKnowPartial.ShouldEqual("_FixNFiberWhatYouNeedToKnow");
            viewModel.LearnMoreModalPartial.ShouldEqual("_UpgradeFixNFibrePlusLearnMoreModal");
            viewModel.ModalSavingsText.ShouldEqual("Save £9 a month (£162 over 18 months). £162 saving is based on £9 discount for 18 months for Fibre Plus broadband (18 x £9 = £162), when compared to buying Unlimited Fibre Plus, our equivalent standalone broadband product (£26 per month).");
            viewModel.BannerImageCssClass.ShouldEqual("bundle-upgrade-fixnfibreplus-sales-banner");
        }

        [Test]
        public void ShouldPopulateFixNProtectPlusBundleUpgradeViewModel()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();

            EnergyCustomer energyCustomer = CustomerFactory.CustomerDetailsWithFixAndProtectTariffChosen(FuelType.Dual, ElectricityMeterType.Standard);
            energyCustomer.AvailableBundleUpgrade = new Tariff
            {
                DisplayName = "Tariff1",
                ElectricityProduct = new Product(),
                GasProduct = new Product(),
                BundlePackage = new BundlePackage("HC", 9.95, 19.95, BundlePackageType.FixAndProtect, new List<TariffTickUsp>()),
                TariffId = "BO009",
                EnergyTickUsps = new List<TariffTickUsp>(),
                IsBundle = true,
                Extras = new List<Extra>(),
                TariffGroup = TariffGroup.FixAndFibre,
                IsSmartTariff = false,
                IsUpgrade = true
            };
            energyCustomer.AvailableBundleUpgrade.BundlePackage.HesMoreInformation = FakeHomeServicesProductStub.GetFakeProducts("BOBC");

            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, energyCustomer);
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());
            var fakeTariffManager = new FakeTariffManager();
            fakeTariffManager.PdfLinkMappings.Add("BO009", "tariff_v1.pdf | tariff_v2.pdf");

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .WithTariffManager(fakeTariffManager)
                .Build<SignUpController>();

            ActionResult result = signUpController.Upgrades();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<BundleUpgradeViewModel>();
            viewModel.Header.ShouldEqual("Get full heating cover 24/7, boiler service included");
            viewModel.ProductCode.ShouldEqual("BO009");
            viewModel.AddRemoveButtonText.ShouldEqual("Add to Bundle");
            viewModel.AddRemoveButtonAltText.ShouldEqual("Add this Upgrade to your Bundle");
            viewModel.IsAddedToBasket.ShouldEqual(false);
            viewModel.IsAddedToBasketStr.ShouldEqual("false");
            viewModel.AddedCssClass.ShouldEqual("");
            viewModel.UpgradeDescription.ShouldEqual(
                "Upgrade to our <b>Heating Cover</b> and you’ll not only get 24/7 central heating cover with unlimited call-outs, all parts and labour included. We’ll also give you an annual boiler service.");
            viewModel.Price.ShouldEqual("£19.95");
            viewModel.DiscountedPrice.ShouldEqual("£9.95");
            viewModel.DiscountAmount.ShouldEqual("£10");
            viewModel.RemoveButtonAltText.ShouldEqual("Remove this Upgrade from your Bundle");
            viewModel.AddButtonAltText.ShouldEqual("Add this Upgrade to your Bundle");
            viewModel.RemoveButtonIconUrl.ShouldEqual("http://localhost:3075/Content/Svgs/icons/trashcan-white.svg");
            viewModel.MoreInformationModalId.ShouldEqual("extraMoreInformation-BO009");
            viewModel.ModalAddRemoveButtonText.ShouldEqual("Add to Bundle");
            viewModel.ModalAddRemoveButtonCssClass.ShouldEqual("button-primary");
            viewModel.ExtraContainerId.ShouldEqual("extra-container-BO009");
            viewModel.ButtonGroup.ShouldEqual("button-extra-BO009");
            viewModel.ModalExtraPriceHeaderId.ShouldEqual("extra-modal-priceheader-BO009");
            viewModel.WhatYouNeedToKnowPartial.ShouldEqual("_FixNProtectWhatYouNeedToKnow");
            viewModel.LearnMoreModalPartial.ShouldEqual("_UpgradeFixNProtectPlusLearnMoreModal");
            viewModel.ModalSavingsText.ShouldEqual("Save £60 over 12 months. £60 saving is based on a £5 discount for 12 months for Heating Cover (12 x £5 = £60), when compared to buying Heating Cover separately (£23.95 per month).");
            viewModel.BannerImageCssClass.ShouldEqual("bundle-upgrade-fixnprotectplus-sales-banner");
            viewModel.WhatsExcluded.ShouldNotBeEmpty();
            viewModel.WhatsIncluded.ShouldNotBeEmpty();
        }

        [Test]
        public void ShouldGoBackToAvailableTariffsFromUpgradesView()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
            EnergyCustomer energyCustomer = CustomerFactory.CustomerDetailsWithFixNFibreBundleTariffChosen(FuelType.Dual, ElectricityMeterType.Standard);
            energyCustomer.AvailableBundleUpgrade = new Tariff
            {
                DisplayName = "Tariff1",
                ElectricityProduct = new Product(),
                GasProduct = new Product(),
                BundlePackage = new BundlePackage("FFP_LR18", 17.00, 28.00, BundlePackageType.FixAndFibre, new List<TariffTickUsp>()),
                TariffId = "BO009",
                EnergyTickUsps = new List<TariffTickUsp>(),
                IsBundle = true,
                Extras = new List<Extra>(),
                TariffGroup = TariffGroup.FixAndFibre,
                IsSmartTariff = false,
                IsUpgrade = true
            };

            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, energyCustomer);
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());
            var broadbandProduct1 = new BroadbandProduct
            {
                BroadbandType = BroadbandType.FibrePlus,
                ProductOrder = 3,
                IsAvailable = true,
                TalkProducts = new List<TalkProduct>
                {
                    new TalkProduct
                    {
                        TalkCode = "LRO",
                        ProductCode = "FFP_LR18",
                        Prices = new List<BroadbandPrice>
                        {
                            new BroadbandPrice {FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 35},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 16},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 0}
                        },
                        BroadbandProductGroup = BroadbandProductGroup.FixAndFibrePlus
                    }
                },
                LineSpeed = new FibreLineSpeeds
                {
                    MinDownload = "28000",
                    MaxDownload = "72000",
                    MinUpload = "8000",
                    MaxUpload = "12000"
                }
            };

            fakeSessionManager.SessionObject.Add(SessionKeys.BroadbandProducts, new List<BroadbandProduct> {broadbandProduct1});
            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false })
                .Build<SignUpController>();
            
            ActionResult result = signUpController.Upgrades();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<BundleUpgradeViewModel>();
            viewModel.BackChevronViewModel.ActionName.ShouldEqual("AvailableTariffs");
            viewModel.BackChevronViewModel.ControllerName.ShouldEqual("Tariffs");
        }

        [Test]
        public void ShouldGoBackToConfirmAddressFromUpgradesView()
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
                SelectedTariff = new Tariff
                {
                    TariffId = "B009",
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
                HasConfirmedNonMatchingBTAddress = true,
                SelectedBroadbandProduct = FakeBroadbandProductsData.GetBroadbandProductWithTalkProducts(),
                SelectedBroadbandProductCode = "FFP_LR18",
                SelectedAddress = new QasAddress
                { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" },
                AvailableBundleUpgrade = new Tariff
                {
                    DisplayName = "Tariff1",
                    ElectricityProduct = new Product(),
                    GasProduct = new Product(),
                    BundlePackage = new BundlePackage("FFP_LR18", 17.00, 28.00, BundlePackageType.FixAndFibre, new List<TariffTickUsp>()),
                    TariffId = "BO009",
                    EnergyTickUsps = new List<TariffTickUsp>(),
                    IsBundle = true,
                    Extras = new List<Extra>(),
                    TariffGroup = TariffGroup.FixAndFibre,
                    IsSmartTariff = false,
                    IsUpgrade = true
                }
            });

            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());
            var broadbandProduct1 = new BroadbandProduct
            {
                BroadbandType = BroadbandType.FibrePlus,
                ProductOrder = 3,
                IsAvailable = true,
                TalkProducts = new List<TalkProduct>
                {
                    new TalkProduct
                    {
                        TalkCode = "LRO",
                        ProductCode = "FFP_LR18",
                        Prices = new List<BroadbandPrice>
                        {
                            new BroadbandPrice {FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 35},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 16},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 0}
                        },
                        BroadbandProductGroup = BroadbandProductGroup.FixAndFibrePlus
                    }
                },
                LineSpeed = new FibreLineSpeeds
                {
                    MinDownload = "28000",
                    MaxDownload = "72000",
                    MinUpload = "8000",
                    MaxUpload = "12000"
                }
            };

            var broadbandProduct2 = new BroadbandProduct
            {
                BroadbandType = BroadbandType.FibrePlus,
                ProductOrder = 3,
                IsAvailable = true,
                TalkProducts = new List<TalkProduct>
                {
                    new TalkProduct
                    {
                        TalkCode = "LR2",
                        ProductCode = "FFP_EAW18",
                        Prices = new List<BroadbandPrice>
                        {
                            new BroadbandPrice {FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 35},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 16},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 0}
                        },
                        BroadbandProductGroup = BroadbandProductGroup.FixAndFibrePlus
                    }
                },
                LineSpeed = new FibreLineSpeeds
                {
                    MinDownload = "28000",
                    MaxDownload = "72000",
                    MinUpload = "8000",
                    MaxUpload = "12000"
                }
            };

            fakeSessionManager.SessionObject.Add(SessionKeys.BroadbandProducts, new List<BroadbandProduct> { broadbandProduct1, broadbandProduct2 });

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            ActionResult result = signUpController.Upgrades();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<BundleUpgradeViewModel>();
            viewModel.BackChevronViewModel.ActionName.ShouldEqual("ConfirmAddress");
            viewModel.BackChevronViewModel.ControllerName.ShouldEqual("SignUp");
        }

        [Test]
        public void ShouldUpgradeFnFibrePlusAndRenderCorrectly()
        {
            var fakeSessionManager = new FakeSessionManager();
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
            EnergyCustomer customer = CustomerFactory.CustomerDetailsWithFixNFibreBundleTariffChosen(FuelType.Dual, ElectricityMeterType.Standard);
            customer.CanUpgradeToFibrePlus = true;
            var upgradeBundleTariff1 = new Tariff
            {
                DisplayName = "Fix n Fibre Plus",
                BundlePackage = new BundlePackage("FFP_LR18", 12, 24, BundlePackageType.FixAndFibre, null),
                IsBundle = true,
                IsUpgrade = true,
                TariffId = "B1000"
            };
            customer.AvailableBundleUpgrade = upgradeBundleTariff1;
            var broadbandProduct1 = new BroadbandProduct
            {
                BroadbandType = BroadbandType.FibrePlus,
                ProductOrder = 3,
                IsAvailable = true,
                TalkProducts = new List<TalkProduct>
                {
                    new TalkProduct
                    {
                        TalkCode = "LRO",
                        ProductCode = "FFP_LR18",
                        Prices = new List<BroadbandPrice>
                        {
                            new BroadbandPrice {FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 35},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 16},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 0}
                        },
                        BroadbandProductGroup = BroadbandProductGroup.FixAndFibrePlus
                    }
                },
                LineSpeed = new FibreLineSpeeds
                {
                    MinDownload = "28000",
                    MaxDownload = "72000",
                    MinUpload = "8000",
                    MaxUpload = "12000"
                }
            };

            var broadbandProduct2 = new BroadbandProduct
            {
                BroadbandType = BroadbandType.FibrePlus,
                ProductOrder = 3,
                IsAvailable = true,
                TalkProducts = new List<TalkProduct>
                {
                    new TalkProduct
                    {
                        TalkCode = "AL19",
                        ProductCode = "FFP_ANY18",
                        Prices = new List<BroadbandPrice>
                        {
                            new BroadbandPrice {FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 35},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 16},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 0}
                        },
                        BroadbandProductGroup = BroadbandProductGroup.FixAndFibrePlus
                    }
                },
                LineSpeed = new FibreLineSpeeds
                {
                    MinDownload = "28000",
                    MaxDownload = "72000",
                    MinUpload = "8000",
                    MaxUpload = "12000"
                }
            };

            fakeSessionManager.SessionObject.Add(SessionKeys.BroadbandProducts, new List<BroadbandProduct> { broadbandProduct1, broadbandProduct2 });
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>{customer.SelectedTariff, upgradeBundleTariff1});
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false, InstallLine = false })
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            ActionResult result = signUpController.AddBundleUpgrade("B1000");

            var partialViewResult = result.ShouldBeType<PartialViewResult>();
            var yourPriceViewModel = partialViewResult.Model.ShouldBeType<BundleUpgradeViewModel>();
            yourPriceViewModel.ShoppingBasketViewModel.TotalItemsInBasket.ShouldEqual(1);
            yourPriceViewModel.ShoppingBasketViewModel.ShowPhonePackage.ShouldBeFalse();
            yourPriceViewModel.ShoppingBasketViewModel.BasketTogglerIconFilepath.ShouldEqual("http://localhost:3075/Content/Svgs/icons/basket-trigger-1item.svg");
            yourPriceViewModel.ShoppingBasketViewModel.SelectedExtras.Count.ShouldEqual(0);
            yourPriceViewModel.ShoppingBasketViewModel.ShowExtras.ShouldBeFalse();
            yourPriceViewModel.ShoppingBasketViewModel.ProjectedMonthlyTotalFullValue.ShouldEqual("£12");
            yourPriceViewModel.ShoppingBasketViewModel.ProjectedMonthlyTotalPenceValue.ShouldEqual(".00");
            yourPriceViewModel.ShoppingBasketViewModel.AnnualSavingsText.ShouldEqual("Saving you £216 over 18 months");
            yourPriceViewModel.ShoppingBasketViewModel.BasketCssClass.ShouldEqual("basket-1-items");

            result = signUpController.Upgrades();

            // Assert BundleUpgradeViewModel
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.ViewName.ShouldEqual("");

            var bundleUpgradeViewModel = viewResult.Model.ShouldBeType<BundleUpgradeViewModel>();
            bundleUpgradeViewModel.Header.ShouldEqual("Boost your internet up to 72Mbps");
            bundleUpgradeViewModel.ProductCode.ShouldEqual("B1000");
            bundleUpgradeViewModel.AddRemoveButtonText.ShouldEqual("Remove from Bundle");
            bundleUpgradeViewModel.AddRemoveButtonAltText.ShouldEqual("Remove this Upgrade from your Bundle");
            bundleUpgradeViewModel.IsAddedToBasket.ShouldEqual(true);
            bundleUpgradeViewModel.IsAddedToBasketStr.ShouldEqual("true");
            bundleUpgradeViewModel.AddedCssClass.ShouldEqual("added");
            bundleUpgradeViewModel.UpgradeDescription.ShouldEqual("Upgrade to our <b>Fibre Plus</b> broadband. With unlimited downloads and an average speed of 63Mbps, it's great for streaming HD movies, playing games online and downloading larger files.");
            bundleUpgradeViewModel.Price.ShouldEqual("£24");
            bundleUpgradeViewModel.RemoveButtonAltText.ShouldEqual("Remove this Upgrade from your Bundle");
            bundleUpgradeViewModel.AddButtonAltText.ShouldEqual("Add this Upgrade to your Bundle");
            bundleUpgradeViewModel.RemoveButtonIconUrl.ShouldEqual("http://localhost:3075/Content/Svgs/icons/trashcan-white.svg");
            bundleUpgradeViewModel.MoreInformationModalId.ShouldEqual("extraMoreInformation-B1000");
            bundleUpgradeViewModel.ModalAddRemoveButtonText.ShouldEqual("Remove from Bundle");
            bundleUpgradeViewModel.ModalAddRemoveButtonCssClass.ShouldEqual("button-secondary");
            bundleUpgradeViewModel.ExtraContainerId.ShouldEqual("extra-container-B1000");
            bundleUpgradeViewModel.ButtonGroup.ShouldEqual("button-extra-B1000");
            bundleUpgradeViewModel.ModalExtraPriceHeaderId.ShouldEqual("extra-modal-priceheader-B1000");
        }
        [Test]
        public void ShouldUpgradeFnProtectPlusAndRenderCorrectly()
        {
            var fakeSessionManager = new FakeSessionManager();
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
            EnergyCustomer customer = CustomerFactory.CustomerDetailsWithFixAndProtectTariffChosen(FuelType.Dual, ElectricityMeterType.Standard);
            var upgradeBundleTariff1 = new Tariff
            {
                DisplayName = "Fix n Protect Plus",
                BundlePackage = new BundlePackage("BundleFnP2", 12, 24, BundlePackageType.FixAndProtect, null),
                IsBundle = true,
                IsUpgrade = true,
                TariffId = "B2000"
            };
            upgradeBundleTariff1.BundlePackage.HesMoreInformation = FakeHomeServicesProductStub.GetFakeProducts("BOBC");

            customer.AvailableBundleUpgrade = upgradeBundleTariff1;
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff> { customer.SelectedTariff, upgradeBundleTariff1 });
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            ActionResult result = signUpController.AddBundleUpgrade("B2000");

            var partialViewResult = result.ShouldBeType<PartialViewResult>();
            var yourPriceViewModel = partialViewResult.Model.ShouldBeType<BundleUpgradeViewModel>();
            yourPriceViewModel.ShoppingBasketViewModel.TotalItemsInBasket.ShouldEqual(1);
            yourPriceViewModel.ShoppingBasketViewModel.ShowPhonePackage.ShouldBeFalse();
            yourPriceViewModel.ShoppingBasketViewModel.BasketTogglerIconFilepath.ShouldEqual("http://localhost:3075/Content/Svgs/icons/basket-trigger-1item.svg");
            yourPriceViewModel.ShoppingBasketViewModel.SelectedExtras.Count.ShouldEqual(0);
            yourPriceViewModel.ShoppingBasketViewModel.ShowExtras.ShouldBeFalse();
            yourPriceViewModel.ShoppingBasketViewModel.ProjectedMonthlyTotalFullValue.ShouldEqual("£12");
            yourPriceViewModel.ShoppingBasketViewModel.ProjectedMonthlyTotalPenceValue.ShouldEqual(".00");
            yourPriceViewModel.ShoppingBasketViewModel.AnnualSavingsText.ShouldEqual("Saving you £144 over 12 months");
            yourPriceViewModel.ShoppingBasketViewModel.BasketCssClass.ShouldEqual("basket-1-items");

            result = signUpController.Upgrades();

            // Assert BundleUpgradeViewModel
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.ViewName.ShouldEqual("");

            var bundleUpgradeViewModel = viewResult.Model.ShouldBeType<BundleUpgradeViewModel>();
            bundleUpgradeViewModel.Header.ShouldEqual("Get full heating cover 24/7, boiler service included");
            bundleUpgradeViewModel.ProductCode.ShouldEqual("B2000");
            bundleUpgradeViewModel.AddRemoveButtonText.ShouldEqual("Remove from Bundle");
            bundleUpgradeViewModel.AddRemoveButtonAltText.ShouldEqual("Remove this Upgrade from your Bundle");
            bundleUpgradeViewModel.IsAddedToBasket.ShouldEqual(true);
            bundleUpgradeViewModel.IsAddedToBasketStr.ShouldEqual("true");
            bundleUpgradeViewModel.AddedCssClass.ShouldEqual("added");
            bundleUpgradeViewModel.UpgradeDescription.ShouldEqual("Upgrade to our <b>Heating Cover</b> and you’ll not only get 24/7 central heating cover with unlimited call-outs, all parts and labour included. We’ll also give you an annual boiler service.");
            bundleUpgradeViewModel.Price.ShouldEqual("£24");
            bundleUpgradeViewModel.RemoveButtonAltText.ShouldEqual("Remove this Upgrade from your Bundle");
            bundleUpgradeViewModel.AddButtonAltText.ShouldEqual("Add this Upgrade to your Bundle");
            bundleUpgradeViewModel.RemoveButtonIconUrl.ShouldEqual("http://localhost:3075/Content/Svgs/icons/trashcan-white.svg");
            bundleUpgradeViewModel.MoreInformationModalId.ShouldEqual("extraMoreInformation-B2000");
            bundleUpgradeViewModel.ModalAddRemoveButtonText.ShouldEqual("Remove from Bundle");
            bundleUpgradeViewModel.ModalAddRemoveButtonCssClass.ShouldEqual("button-secondary");
            bundleUpgradeViewModel.ExtraContainerId.ShouldEqual("extra-container-B2000");
            bundleUpgradeViewModel.ButtonGroup.ShouldEqual("button-extra-B2000");
            bundleUpgradeViewModel.ModalExtraPriceHeaderId.ShouldEqual("extra-modal-priceheader-B2000");
        }
    }
}