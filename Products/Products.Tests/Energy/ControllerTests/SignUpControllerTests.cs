namespace Products.Tests.Energy.ControllerTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Broadband.Fakes.Repository;
    using Broadband.Fakes.Services;
    using Broadband.Model;
    using Common.Fakes;
    using Common.Helpers;
    using ControllerHelpers;
    using ControllerHelpers.Energy;
    using Core;
    using Core.Enums;
    using Fakes.Repositories;
    using Fakes.Services;
    using Helpers;
    using NUnit.Framework;
    using Products.Infrastructure.Extensions;
    using Products.Model.Broadband;
    using Products.Model.Common;
    using Products.Model.Constants;
    using Products.Model.Energy;
    using Products.Model.Enums;
    using Service.Common.Managers;
    using Service.Helpers;
    using ServiceWrapper.BankDetailsService;
    using Should;
    using TariffChange.Fakes.Managers;
    using Web.Areas.Energy.Controllers;
    using WebModel.Resources.Common;
    using WebModel.Resources.Energy;
    using WebModel.ViewModels.Common;
    using WebModel.ViewModels.Energy;
    using FakeSessionManager = Common.Fakes.FakeSessionManager;
    using Tariff = Products.Model.Energy.Tariff;

    public class SignUpControllerTests
    {
        [Test]
        public void ShouldThrowExceptionWhenContactDetailsPageAccessedDirectly()
        {
            // Arrange
            var controller = new ControllerFactory()
                .Build<SignUpController>();

            // Act + Assert
            var ex = Assert.Throws<ArgumentException>(() => controller.ContactDetails());
            ex.Message.ShouldEqual("energyCustomer is null");
        }

        [Test]
        public void ShouldThrowExceptionWhenPersonalDetailsPageAccessedDirectly()
        {
            // Arrange
            var controller = new ControllerFactory()
                .Build<SignUpController>();

            // Act + Assert
            var ex = Assert.Throws<ArgumentException>(() => controller.PersonalDetails());
            ex.Message.ShouldEqual("energyCustomer is null");
        }

        [Test]
        public void ShouldPrePopulateContactDetailsWhenCustomerRevisitsThePage()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var contactDetails = new ContactDetails
            {
                ContactNumber = "08987777",
                EmailAddress = "test@gmail.com",
                MarketingConsent = true
            };
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                ContactDetails = contactDetails,
                SelectedTariff = new Tariff { IsBundle = true}
            });
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.ContactDetails();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<ContactDetailsViewModel>();
            viewModel.ContactNumber.ShouldEqual(contactDetails.ContactNumber);
            viewModel.EmailAddress.ShouldEqual(contactDetails.EmailAddress);
            viewModel.IsMarketingConsentChecked.ShouldEqual(contactDetails.MarketingConsent);
        }

        [Test]
        public void ShouldPrePopulatePersonalDetailsWhenCustomerRevisitsThePage()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                PersonalDetails = new PersonalDetails
                {
                    Title = "Mr",
                    FirstName = "Test",
                    LastName = "Test1",
                    DateOfBirth = "01/02/1990"
                },
                Postcode = "a",
                SelectedTariff = new Tariff()
            });
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.PersonalDetails();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<PersonalDetailsViewModel>();
            viewModel.Titles.ShouldEqual(Titles.Mr);
            viewModel.FirstName.ShouldEqual("Test");
            viewModel.LastName.ShouldEqual("Test1");
            viewModel.DateOfBirth.ShouldEqual("01/02/1990");
            viewModel.DateOfBirthDay.ShouldEqual("01");
            viewModel.DateOfBirthMonth.ShouldEqual("02");
            viewModel.DateOfBirthYear.ShouldEqual("1990");
        }

        [TestCaseSource(nameof(ValidPersonalDetails))]
        public void ShouldStorePersonalDetailsInTheSessionAndRedirectToSelectAddressPage(PersonalDetailsViewModel personalDetailsViewModel, string postcode, string description)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer,
                new EnergyCustomer { Postcode = postcode, SelectedTariff = new Tariff() });
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.PersonalDetails(personalDetailsViewModel);

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.PersonalDetails.Title.ShouldEqual(personalDetailsViewModel.Titles.ToString());
            customer.PersonalDetails.FirstName.ShouldEqual(personalDetailsViewModel.FirstName);
            customer.PersonalDetails.LastName.ShouldEqual(personalDetailsViewModel.LastName);
            customer.PersonalDetails.DateOfBirth.ShouldEqual(personalDetailsViewModel.DateOfBirth);

            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("ContactDetails");
        }

        [TestCase("BO001", "PhonePackage", "Signup")]
        [TestCase("001", "AvailableTariffs", "Tariffs")]
        public void ShouldSetBackChevronDestinationCorrectlyOnPersonalDetailsView(string selectedTariffId, string expectedActionName, string expectedControllerName)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
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

            var fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper
            {
                FakeLineSpeed = new FakeBroadbandProductsData.FakeLineSpeed
                {
                    FibreMinDownloadSpeed = "15200",
                    FibreMaxDownloadSpeed = "38000",
                    FibreMinUploadSpeed = "39800",
                    FibreMaxUploadSpeed = "20000"
                }
            };

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var tariffController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false })
                .WithBroadbandProductsServiceWrapper(fakeBroadbandProductsServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            // Act
            // ReSharper disable once UnusedVariable
            ActionResult tariffResult = tariffController.AvailableTariffs().Result;
            // ReSharper disable once UnusedVariable
            ActionResult tariffResult2 = tariffController.AvailableTariffs(selectedTariffId).Result;
            ActionResult result = signUpController.PersonalDetails();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<PersonalDetailsViewModel>();
            viewModel.BackChevronViewModel.ActionName.ShouldEqual(expectedActionName);
            viewModel.BackChevronViewModel.ControllerName.ShouldEqual(expectedControllerName);
        }

        [Test]
        public void ShouldSetBackChevronDestinationCorrectlyOnPhonePackageView()
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

            var fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper
            {
                FakeLineSpeed = new FakeBroadbandProductsData.FakeLineSpeed
                {
                    FibreMinDownloadSpeed = "15200",
                    FibreMaxDownloadSpeed = "38000",
                    FibreMinUploadSpeed = "39800",
                    FibreMaxUploadSpeed = "20000"
                }
            };

            var tariffController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false })
                .WithBroadbandProductsServiceWrapper(fakeBroadbandProductsServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<TariffsController>();

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            // Act
            // ReSharper disable once UnusedVariable
            ActionResult tariffResult = tariffController.AvailableTariffs().Result;
            // ReSharper disable once UnusedVariable
            ActionResult tariffResult2 = tariffController.AvailableTariffs("BO001").Result;
            
            ActionResult resultPhonePackage = signUpController.PhonePackage();
            var viewResult = resultPhonePackage.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<PhonePackageViewModel>();

            // Assert
            resultPhonePackage.ShouldNotBeNull();
            resultPhonePackage.ShouldBeType<ViewResult>();
            viewModel.BackChevronViewModel.ActionName.ShouldEqual("Upgrades");
            viewModel.BackChevronViewModel.ControllerName.ShouldEqual("SignUp");
        }

        [TestCase("38000", "38")]
        public void ShouldDisplayCorrectLineSpeedInformationForBundleIncludingBroadbandComponent(string fibreMaxDownloadSpeed, string expectedFibreMaxDownloadSpeed)
        {
            // Arrange
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient();
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

            var fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper
            {
                FakeLineSpeed = new FakeBroadbandProductsData.FakeLineSpeed
                {
                    FibreMinDownloadSpeed = "15200",
                    FibreMaxDownloadSpeed = fibreMaxDownloadSpeed,
                    FibreMinUploadSpeed = "39800",
                    FibreMaxUploadSpeed = "20000"
                }
            };

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var tariffController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false })
                .WithBroadbandProductsServiceWrapper(fakeBroadbandProductsServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            // Act
            // ReSharper disable once UnusedVariable
            ActionResult tariffResult = tariffController.AvailableTariffs().Result;
            // ReSharper disable once UnusedVariable
            ActionResult tariffResult2 = tariffController.AvailableTariffs("BO001").Result;
            ActionResult result = signUpController.PhonePackage();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<PhonePackageViewModel>();
            viewModel.BroadbandPackageSpeedViewModel.MaxDownload.ShouldEqual(expectedFibreMaxDownloadSpeed);
            viewModel.BroadbandPackageSpeedViewModel.MaxUpload.ShouldEqual("10");
            viewModel.BroadbandPackageSpeedViewModel.MinDownload.ShouldEqual("15");
            viewModel.BroadbandPackageSpeedViewModel.MinUpload.ShouldEqual("10");
            viewModel.BroadbandPackageSpeedViewModel.MaximumSpeedCap.ShouldEqual(76);
            viewModel.BroadbandPackageSpeedViewModel.ShowPostcodeText.ShouldBeTrue();
            viewModel.BroadbandPackageSpeedViewModel.ShowSpeedRangeText.ShouldBeTrue();
        }

        [TestCase("FF3_LR18")]
        [TestCase("FF3_EAW18")]
        [TestCase("FF3_ANY18")]
        [TestCase("FF3_AP18")]
        public async Task ShouldStoreSelectedTalkPackageInSessionForBundleIncludingBroadbandComponent(string selectedBroadbandProductCode)
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
                SelectedAddress = new QasAddress
                { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" }
            });

            var fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper
            {
                FakeLineSpeed = new FakeBroadbandProductsData.FakeLineSpeed
                {
                    FibreMinDownloadSpeed = "15200",
                    FibreMaxDownloadSpeed = "38000",
                    FibreMinUploadSpeed = "39800",
                    FibreMaxUploadSpeed = "20000"
                }
            };

            // Act
            var tariffController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false })
                .WithBroadbandProductsServiceWrapper(fakeBroadbandProductsServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<TariffsController>();

            await tariffController.AvailableTariffs();

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            await tariffController.AvailableTariffs("BO001");
            signUpController.PhonePackage(selectedBroadbandProductCode, new PhonePackageViewModel { KeepYourNumberViewModel = new KeepYourNumberViewModel() });

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.SelectedBroadbandProductCode.ShouldEqual(selectedBroadbandProductCode);
        }

        [Test]
        public void ShouldGoBackToUpgradesViewFromPhonePackageView()
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
                HasConfirmedNonMatchingBTAddress = false,
                SelectedBroadbandProduct = FakeBroadbandProductsData.GetBroadbandProductWithTalkProducts(),
                SelectedBroadbandProductCode = "FF3_LR18",
                SelectedAddress = new QasAddress
                { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" }
            });

            var fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper
            {
                FakeLineSpeed = new FakeBroadbandProductsData.FakeLineSpeed
                {
                    FibreMinDownloadSpeed = "15200",
                    FibreMaxDownloadSpeed = "38000",
                    FibreMinUploadSpeed = "39800",
                    FibreMaxUploadSpeed = "20000"
                }
            };


            // Act
            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false })
                .WithBroadbandProductsServiceWrapper(fakeBroadbandProductsServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            ActionResult result = signUpController.PhonePackage();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<PhonePackageViewModel>();
            viewModel.BackChevronViewModel.ActionName.ShouldEqual("Upgrades");
            viewModel.BackChevronViewModel.ControllerName.ShouldEqual("SignUp");
        }

        [Test]
        public void ShouldGoBackToConfirmAddressViewFromPhonePackageView()
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
                HasConfirmedNonMatchingBTAddress = true,
                SelectedBroadbandProduct = FakeBroadbandProductsData.GetBroadbandProductWithTalkProducts(),
                SelectedBroadbandProductCode = "FF3_LR18",
                SelectedAddress = new QasAddress
                { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" }
            });

            var fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper
            {
                FakeLineSpeed = new FakeBroadbandProductsData.FakeLineSpeed
                {
                    FibreMinDownloadSpeed = "15200",
                    FibreMaxDownloadSpeed = "38000",
                    FibreMinUploadSpeed = "39800",
                    FibreMaxUploadSpeed = "20000"
                }
            };


            // Act
            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false })
                .WithBroadbandProductsServiceWrapper(fakeBroadbandProductsServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            ActionResult result = signUpController.PhonePackage();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<PhonePackageViewModel>();
            viewModel.BackChevronViewModel.ActionName.ShouldEqual("Upgrades");
            viewModel.BackChevronViewModel.ControllerName.ShouldEqual("SignUp");
        }

        [Test]
        public void ShouldGoBackToExtrasViewFromPersonalDetailsViewWhenFixNFibreSelected()
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
                    BundlePackage = new BundlePackage("100", 12, 22, BundlePackageType.FixAndFibre, new List<TariffTickUsp>()),
                    IsBundle = true,
                    Extras = new List<Extra>
                    {
                        new Extra(
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
                        )
                    }
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
        public void ShouldGoBackToPhonePackageViewFromPersonalDetailsViewWhenFixNFibreSelected()
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

            ActionResult result = signUpController.PersonalDetails();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<PersonalDetailsViewModel>();
            viewModel.BackChevronViewModel.ActionName.ShouldEqual("PhonePackage");
            viewModel.BackChevronViewModel.ControllerName.ShouldEqual("Signup");
        }

        [Test]
        public void ShouldGoBackToAvailableTariffsViewFromPersonalDetailsView()
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
                    IsBundle = false
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
            viewModel.BackChevronViewModel.ActionName.ShouldEqual("AvailableTariffs");
            viewModel.BackChevronViewModel.ControllerName.ShouldEqual("Tariffs");
        }

        [Test]
        public void ShouldPopulateExtrasViewModel()
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
                    Extras = new List<Extra>
                    {
                        new Extra(
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
                        )
                    }
                },
                SelectedAddress = new QasAddress
                { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" }
            });

            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            ActionResult result = signUpController.Extras();

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
            extra.Price.ShouldEqual("£11.50");
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
        public async Task ShouldRedirectToOnlineAccountCreationPageWhenCustomerEntersValidContactDetailsAndProfileDoesNotExist()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var fakeProfileRepository = new FakeProfileRepository { ProfileExists = false };
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithCustomerProfileRepository(fakeProfileRepository)
                .Build<SignUpController>();

            var contactDetailsViewModel = new ContactDetailsViewModel
            {
                EmailAddress = "a@a.com",
                ConfirmEmailAddress = "a@a.com",
                ContactNumber = "012121212121",
                IsMarketingConsentChecked = true
            };

            // Act
            controller.ValidateViewModel(contactDetailsViewModel);
            ActionResult result = await controller.ContactDetails(contactDetailsViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("OnlineAccount");
            routeResult.RouteValues["controller"].ShouldEqual(null);

            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.ProfileExists.ShouldEqual(false);
            customer.UserId.ShouldEqual(Guid.Empty);
        }

        [Test]
        public async Task ShouldRedirectToDirectDebitPageWhenCustomerEntersValidContactDetailsAndProfileExistsAndSelectedPaymentIsDirectDebit()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer,
                new EnergyCustomer { SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit });
            var fakeProfileRepository = new FakeProfileRepository { ProfileExists = true };
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithCustomerProfileRepository(fakeProfileRepository)
                .Build<SignUpController>();

            var contactDetailsViewModel = new ContactDetailsViewModel
            {
                EmailAddress = "a@a.com",
                ConfirmEmailAddress = "a@a.com",
                ContactNumber = "012121212121",
                IsMarketingConsentChecked = true
            };

            // Act
            controller.ValidateViewModel(contactDetailsViewModel);
            ActionResult result = await controller.ContactDetails(contactDetailsViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("BankDetails");
            routeResult.RouteValues["controller"].ShouldEqual(null);

            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.ProfileExists.ShouldEqual(true);
            customer.UserId.ShouldNotEqual(Guid.Empty);
        }

        [Test]
        public async Task
            ShouldRedirectToSummaryPageWhenCustomerEntersValidContactDetailsAndProfileExistsAndSelectedPaymentIsNotDirectDebit()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer,
                new EnergyCustomer { SelectedPaymentMethod = PaymentMethod.PayAsYouGo });
            var fakeProfileRepository = new FakeProfileRepository { ProfileExists = true };
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithCustomerProfileRepository(fakeProfileRepository)
                .Build<SignUpController>();

            var contactDetailsViewModel = new ContactDetailsViewModel
            {
                EmailAddress = "a@a.com",
                ConfirmEmailAddress = "a@a.com",
                ContactNumber = "012121212121",
                IsMarketingConsentChecked = true
            };

            // Act
            controller.ValidateViewModel(contactDetailsViewModel);
            ActionResult result = await controller.ContactDetails(contactDetailsViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("ViewSummary");
            routeResult.RouteValues["controller"].ShouldEqual(null);
        }

        [TestCaseSource(nameof(GetInvalidContactNumbers))]
        public async Task ShouldReturnToContactDetailsWhenContactNumberIsMissingOrInvalid(string contactNumber, string errorMessage)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                SelectedTariff = new Tariff { IsBundle = true}
            });
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            var viewModel = new ContactDetailsViewModel
            {
                ContactNumber = contactNumber,
                EmailAddress = "test@test.com",
                ConfirmEmailAddress = "test@test.com",
                IsMarketingConsentChecked = true
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = await controller.ContactDetails(viewModel);

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<ContactDetailsViewModel>();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Keys.Count.ShouldEqual(1);
            controller.ModelState.Values.First().Errors.First().ErrorMessage.ShouldEqual(errorMessage);
        }

        [TestCaseSource(nameof(GetInvalidEmailAddresses))]
        public async Task ShouldReturnToContactDetailsWhenEmailAddressIsMissingInvalidOrDoesNotMatch(string emailAddress, string confirmEmailAddress,
            List<string> errorMessageList, int errorCount)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                SelectedTariff = new Tariff { IsBundle = true}
            });
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            var viewModel = new ContactDetailsViewModel
            {
                ContactNumber = "012345123456",
                EmailAddress = emailAddress,
                ConfirmEmailAddress = confirmEmailAddress,
                IsMarketingConsentChecked = true
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = await controller.ContactDetails(viewModel);

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<ContactDetailsViewModel>();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Keys.Count.ShouldEqual(errorCount);

            foreach (string error in errorMessageList)
            {
                controller.ModelState.Values.SelectMany(modelState => modelState.Errors).FirstOrDefault(msg => msg.ErrorMessage == error).ShouldNotBeNull();
            }
        }

        [Test]
        public async Task ShouldStoreContactDetailsInSession()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();
            var contactDetailsViewModel = new ContactDetailsViewModel
            {
                ContactNumber = "012345123456",
                EmailAddress = "test@test.com",
                ConfirmEmailAddress = "test@test.com",
                IsMarketingConsentChecked = true
            };

            // Act
            controller.ValidateViewModel(contactDetailsViewModel);
            await controller.ContactDetails(contactDetailsViewModel);

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.ContactDetails.ContactNumber.ShouldEqual(contactDetailsViewModel.ContactNumber);
            customer.ContactDetails.EmailAddress.ShouldEqual(contactDetailsViewModel.EmailAddress);
            customer.ContactDetails.MarketingConsent.ShouldEqual(contactDetailsViewModel.IsMarketingConsentChecked);
        }

        [TestCaseSource(nameof(PersonalDetailsTestCases))]
        public void IfPersonalDetailsAreInvalidRemainOnPersonalDetailsPage(
            Titles? title,
            string firstName,
            string lastName,
            string dateOfBirthDay,
            string dateOfBirthMonth,
            string dateOfBirthYear,
            string dateOfBirth,
            string errorMessage)
        {
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer,
                new EnergyCustomer { Postcode = "a", SelectedTariff = new Tariff() });
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            var personalDetails = new PersonalDetailsViewModel
            {
                Titles = title,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirthDay = dateOfBirthDay,
                DateOfBirthMonth = dateOfBirthMonth,
                DateOfBirthYear = dateOfBirthYear,
                DateOfBirth = dateOfBirth
            };

            controller.ValidateViewModel(personalDetails);
            ActionResult result = controller.PersonalDetails(personalDetails);
            result.ShouldNotBeNull()
                .ShouldBeType<ViewResult>()
                .ViewName.ShouldBeEmpty();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Keys.Count.ShouldEqual(1);
            controller.ModelState.Values.First().Errors.First().ErrorMessage.ShouldEqual(errorMessage);
        }

        [TestCaseSource(nameof(InvalidDateOfBirth))]
        public void IfDateOfBirthIsInvalidDateRemainOnPersonalDetailsPage(string dateOfBirthDay, string dateOfBirthMonth, string dateOfBirthYear,
            string dateOfBirth)
        {
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer,
                new EnergyCustomer { Postcode = "a", SelectedTariff = new Tariff() });
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            var personalDetails = new PersonalDetailsViewModel
            {
                Titles = Titles.Dr,
                FirstName = "Test",
                LastName = "Test",
                DateOfBirthDay = dateOfBirthDay,
                DateOfBirthMonth = dateOfBirthMonth,
                DateOfBirthYear = dateOfBirthYear,
                DateOfBirth = dateOfBirth
            };

            controller.ValidateViewModel(personalDetails);
            ActionResult result = controller.PersonalDetails(personalDetails);
            result.ShouldNotBeNull()
                .ShouldBeType<ViewResult>()
                .ViewName.ShouldBeEmpty();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Values.SelectMany(p => p.Errors.Select(x => x.ErrorMessage)).ShouldContain(Form_Resources.DateOfBirthRegExError);
        }

        [Test]
        public void ShouldRedirectToSummaryPageWhenValidBankDetailsAreEntered()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                SelectedTariff = new Tariff()
            });
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            var viewModel = new BankDetailsViewModel
            {
                IsAuthorisedChecked = true,
                AccountHolder = "Test",
                AccountNumber = "12345678",
                SortCode = "102030",
                SortCodeSegmentOne = "10",
                SortCodeSegmentTwo = "20",
                SortCodeSegmentThree = "30",
                DirectDebitDate = "1"
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = controller.BankDetails(viewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("ViewSummary");
            routeResult.RouteValues["controller"].ShouldEqual(null);
        }

        [TestCaseSource(nameof(GetInvalidBankDetailsViewModel))]
        public void ShouldReturnBankDetailsViewWhenDirectDebitDetailsAreInvalid(BankDetailsViewModel model, string errorMessage)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                SelectedTariff = new Tariff()
            });
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = controller.BankDetails(model);

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<BankDetailsViewModel>();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Keys.Count.ShouldEqual(1);
            controller.ModelState.Values.First().Errors.First().ErrorMessage.ShouldEqual(errorMessage);
        }

        [TestCase(100, 100, "£9", "£9", 2)]
        [TestCase(103, 103, "£9", "£9", 2)]
        [TestCase(100, null, "£9", null, 1)]
        [TestCase(null, 100, null, "£9", 1)]
        public void ShouldDisplayRoundedUpDirectDebitAmountForSelectedTariffWhenInAnEnergyOnlyJourney(
            double? electricityAnnualCost,
            double? gasAnnualCost,
            string expectedElectricityMonthlyCost,
            string expectedGasMonthlyCost,
            int expectedItemCount)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();

            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.PayAsYouGo,
                SelectedTariff = new Tariff
                {
                    ElectricityProduct = new Product
                    {
                        ProjectedYearlyCost = electricityAnnualCost
                    },
                    GasProduct = new Product
                    {
                        ProjectedYearlyCost = gasAnnualCost
                    }
                }
            });
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.BankDetails();

            // Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<BankDetailsViewModel>();
            model.AmountItemList.ShouldNotBeNull();
            model.AmountItemList.Count.ShouldEqual(expectedItemCount);

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (expectedItemCount == 2)
            {
                model.AmountItemList[0].Item1.ShouldEqual("Electricity: ");
                model.AmountItemList[0].Item2.ShouldEqual(expectedElectricityMonthlyCost);
                model.AmountItemList[1].Item1.ShouldEqual("Gas: ");
                model.AmountItemList[1].Item2.ShouldEqual(expectedGasMonthlyCost);
            }

            if (expectedItemCount == 1)
            {
                if (electricityAnnualCost != null)
                {
                    model.AmountItemList[0].Item1.ShouldEqual("Electricity: ");
                    model.AmountItemList[0].Item2.ShouldEqual(expectedElectricityMonthlyCost);
                }
                else
                {
                    model.AmountItemList[0].Item1.ShouldEqual("Gas: ");
                    model.AmountItemList[0].Item2.ShouldEqual(expectedGasMonthlyCost);
                }
            }

            model.IsBroadbandBundleSelected.ShouldBeFalse();
        }

        [TestCase(100, 100, 100, "£9", "£9")]
        [TestCase(100, 100, 103, "£9", "£9")]
        public void ShouldDisplayRoundedUpDirectDebitAmountForSelectedTariffWhenTariffIsABroadbandBundle(
            double? electricityAnnualCost,
            double? gasAnnualCost,
            double? broadbandAnnualCost,
            string expectedElectricityMonthlyCost,
            string expectedGasMonthlyCost)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();

            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                SelectedTariff = new Tariff
                {
                    ElectricityProduct = new Product
                    {
                        ProjectedYearlyCost = electricityAnnualCost
                    },
                    GasProduct = new Product
                    {
                        ProjectedYearlyCost = gasAnnualCost
                    },
                    BundlePackage = new BundlePackage("100", 12, 22, BundlePackageType.FixAndFibre, new List<TariffTickUsp>()),
                    IsBundle = true
                },
                IsBundlingJourney = true,
                SelectedBroadbandProduct = FakeBroadbandProductsData.GetBroadbandProductWithTalkProducts(),
                SelectedBroadbandProductCode = "FF3_LR18"
            });
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.BankDetails();

            // Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<BankDetailsViewModel>();
            model.AmountItemList.ShouldNotBeNull();
            model.AmountItemList.Count.ShouldEqual(2);
            model.AmountItemList[0].Item2.ShouldEqual(expectedElectricityMonthlyCost);
            model.AmountItemList[1].Item2.ShouldEqual(expectedGasMonthlyCost);
            model.BroadbandBundlePackageAmount.ShouldEqual("£12");
            model.IsBroadbandBundleSelected.ShouldBeTrue();
        }

        [TestCase(100, 100, 100, "£9", "£9", "£0", "£5")]
        public void ShouldDisplayRoundedUpDirectDebitAmountForSelectedTariffWhenTariffIsAHesBundle(double? electricityAnnualCost,
            double? gasAnnualCost,
            double? hesAnnualCost,
            string expectedElectricityMonthlyCost,
            string expectedGasMonthlyCost,
            string expectedHesMonthlyCost,
            string expectedWiringCoverMonthlyCost)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();

            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                SelectedTariff = new Tariff
                {
                    ElectricityProduct = new Product
                    {
                        ProjectedYearlyCost = electricityAnnualCost
                    },
                    GasProduct = new Product
                    {
                        ProjectedYearlyCost = gasAnnualCost
                    },
                    BundlePackage = new BundlePackage("100", 0, 12, BundlePackageType.FixAndProtect, new List<TariffTickUsp>()),
                    IsBundle = true
                },
                IsBundlingJourney = true,
                SelectedExtras = new HashSet<Extra>
                {
                    new Extra(
                        "SSE Electrical Wiring Cover",
                        5,
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
                    )
                }
            });
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.BankDetails();

            // Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<BankDetailsViewModel>();
            model.AmountItemList.ShouldNotBeNull();
            model.AmountItemList.Count.ShouldEqual(2);
            model.AmountItemList[0].Item2.ShouldEqual(expectedElectricityMonthlyCost);
            model.AmountItemList[1].Item2.ShouldEqual(expectedGasMonthlyCost);
            model.IsHesBundle.ShouldBeTrue();
            model.HesBundlePackageAmount.ShouldEqual(expectedHesMonthlyCost);
            model.ExtraDetailsList[0].Item2.ShouldEqual(expectedWiringCoverMonthlyCost);
            model.HesWhyDoIPay0ModalId.ShouldEqual("why-is-there-0-payment");
        }

        [TestCase("FF3_LR18", "£23", "Line Rental Only")]
        [TestCase("FF3_EAW18", "£27", "Evening & Weekend")]
        [TestCase("FF3_ANY18", "£31", "Anytime")]
        [TestCase("FF3_AP18", "£35", "Anytime Plus")]
        public async Task ShouldDisplayFullCostOfBroadbandIncludingTalkPackageOnDirectDebitBreakdown(string productCode, string amount, string packageName)
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
                SelectedAddress = new QasAddress
                { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" }
            });

            var fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper
            {
                FakeLineSpeed = new FakeBroadbandProductsData.FakeLineSpeed
                {
                    FibreMinDownloadSpeed = "15200",
                    FibreMaxDownloadSpeed = "38000",
                    FibreMinUploadSpeed = "39800",
                    FibreMaxUploadSpeed = "20000"
                }
            };

            // Act
            var tariffController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false })
                .WithBroadbandProductsServiceWrapper(fakeBroadbandProductsServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<TariffsController>();

            await tariffController.AvailableTariffs();

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            await tariffController.AvailableTariffs("BO001");
            signUpController.PhonePackage(productCode, new PhonePackageViewModel { KeepYourNumberViewModel = new KeepYourNumberViewModel() });

            ActionResult result = signUpController.BankDetails();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<BankDetailsViewModel>();
            model.BroadbandBundleDescription.ShouldEqual($"Broadband & Phone: Fibre broadband, {packageName}: ");
            model.BroadbandBundlePackageAmount.ShouldEqual(amount);
            model.IsBroadbandBundleSelected.ShouldBeTrue();
        }

        [Test]
        public void ShouldDisplayCorrectDirectDebitDetailsWhenFAndFPlusIsSelected()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, CustomerFactory.CustomerDetailsWithFixNFibrePlusBundleTariffChosen(FuelType.Dual, ElectricityMeterType.Standard));
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());
            // Act

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            ActionResult result = signUpController.BankDetails();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<BankDetailsViewModel>();
            model.BroadbandBundleDescription.ShouldEqual("Broadband & Phone: Fibre Plus broadband, Line Rental Only: ");
            model.BroadbandBundlePackageAmount.ShouldEqual("£12");
            model.IsBroadbandBundleSelected.ShouldBeTrue();
        }

        [Test]
        public void ShouldDisplayAllViewModelDataOnRetryWhenInAnEnergyOnlyJourney()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();

            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Dual,
                SelectedPaymentMethod = PaymentMethod.PayAsYouGo,
                SelectedTariff = new Tariff
                {
                    ElectricityProduct = new Product
                    {
                        ProjectedYearlyCost = 84
                    },
                    GasProduct = new Product
                    {
                        ProjectedYearlyCost = 84
                    }
                }
            });
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBankDetailsServiceWrapper(new FakeBankDetailsService(null))
                .Build<SignUpController>();

            // Act
            ActionResult result = signUpController.BankDetails();
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<BankDetailsViewModel>();
            model.AccountHolder = "Mr Blah";
            model.AccountNumber = "12341234";
            model.SortCodeSegmentOne = "01";
            model.SortCodeSegmentTwo = "01";
            model.SortCodeSegmentThree = "01";
            model.SortCode = "01-01-01";
            model.IsAuthorisedChecked = true;
            model.DirectDebitDate = "28";

            ActionResult postResult = signUpController.BankDetails(model);

            // Assert
            var postViewResult = postResult.ShouldBeType<ViewResult>();
            var postModel = postViewResult.Model.ShouldBeType<BankDetailsViewModel>();
            postModel.BackChevronViewModel.ShouldNotBeNull();
            postModel.BackChevronViewModel.ActionName.ShouldEqual("ContactDetails");
            postModel.BackChevronViewModel.ControllerName.ShouldEqual("SignUp");
            postModel.BackChevronViewModel.TitleAttributeText.ShouldEqual("Back to previous page");
            postModel.AccountHolder.ShouldEqual("Mr Blah");
            postModel.AccountNumber.ShouldEqual("12341234");
            postModel.SortCodeSegmentOne.ShouldEqual("01");
            postModel.SortCodeSegmentTwo.ShouldEqual("01");
            postModel.SortCodeSegmentThree.ShouldEqual("01");
            postModel.SortCode.ShouldEqual("01-01-01");
            postModel.IsAuthorisedChecked.ShouldBeTrue();
            postModel.DirectDebitDate.ShouldEqual("28");
            postModel.AmountItemList.ShouldNotBeNull();
            postModel.AmountItemList.Count.ShouldEqual(2);
            postModel.AmountItemList[0].Item1.ShouldEqual("Electricity: ");
            postModel.AmountItemList[0].Item2.ShouldEqual("£7");
            postModel.AmountItemList[1].Item1.ShouldEqual("Gas: ");
            postModel.AmountItemList[1].Item2.ShouldEqual("£7");
            postModel.IsBroadbandBundleSelected.ShouldBeFalse();
            postModel.IsRetry.ShouldBeTrue();
        }

        [Test]
        public async Task ShouldDisplayAllViewModelDataOnRetryWhenInABroadbandBundleJourney()
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
                SelectedAddress = new QasAddress
                { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" }
            });

            var fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper
            {
                FakeLineSpeed = new FakeBroadbandProductsData.FakeLineSpeed
                {
                    FibreMinDownloadSpeed = "15200",
                    FibreMaxDownloadSpeed = "38000",
                    FibreMinUploadSpeed = "39800",
                    FibreMaxUploadSpeed = "20000"
                }
            };

            // Act
            var tariffController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false })
                .WithBroadbandProductsServiceWrapper(fakeBroadbandProductsServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<TariffsController>();

            await tariffController.AvailableTariffs();

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .WithBankDetailsServiceWrapper(new FakeBankDetailsService(null))
                .Build<SignUpController>();

            await tariffController.AvailableTariffs("BO001");
            signUpController.PhonePackage("FF3_LR18", new PhonePackageViewModel { KeepYourNumberViewModel = new KeepYourNumberViewModel() });

            ActionResult result = signUpController.BankDetails();
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<BankDetailsViewModel>();
            model.AccountHolder = "Mr Blah";
            model.AccountNumber = "12341234";
            model.SortCodeSegmentOne = "01";
            model.SortCodeSegmentTwo = "01";
            model.SortCodeSegmentThree = "01";
            model.SortCode = "01-01-01";
            model.IsAuthorisedChecked = true;
            model.DirectDebitDate = "28";

            ActionResult postResult = signUpController.BankDetails(model);

            // Assert
            var postViewResult = postResult.ShouldBeType<ViewResult>();
            var postModel = postViewResult.Model.ShouldBeType<BankDetailsViewModel>();
            postModel.BackChevronViewModel.ShouldNotBeNull();
            postModel.BackChevronViewModel.ActionName.ShouldEqual("ContactDetails");
            postModel.BackChevronViewModel.ControllerName.ShouldEqual("SignUp");
            postModel.BackChevronViewModel.TitleAttributeText.ShouldEqual("Back to previous page");
            postModel.AccountHolder.ShouldEqual("Mr Blah");
            postModel.AccountNumber.ShouldEqual("12341234");
            postModel.SortCodeSegmentOne.ShouldEqual("01");
            postModel.SortCodeSegmentTwo.ShouldEqual("01");
            postModel.SortCodeSegmentThree.ShouldEqual("01");
            postModel.SortCode.ShouldEqual("01-01-01");
            postModel.IsAuthorisedChecked.ShouldBeTrue();
            postModel.DirectDebitDate.ShouldEqual("28");
            postModel.AmountItemList.ShouldNotBeNull();
            postModel.AmountItemList.Count.ShouldEqual(2);
            postModel.AmountItemList[0].Item1.ShouldEqual("Electricity: ");
            postModel.AmountItemList[0].Item2.ShouldEqual("£84");
            postModel.AmountItemList[1].Item1.ShouldEqual("Gas: ");
            postModel.AmountItemList[1].Item2.ShouldEqual("£84");
            postModel.BroadbandBundleDescription.ShouldEqual("Broadband & Phone: Fibre broadband, Line Rental Only: ");
            postModel.BroadbandBundlePackageAmount.ShouldEqual("£23");
            postModel.IsBroadbandBundleSelected.ShouldBeTrue();
            postModel.IsHesBundle.ShouldBeFalse();
            postModel.IsRetry.ShouldBeTrue();
        }

        [Test]
        public async Task ShouldDisplayAllViewModelDataOnRetryWhenInAHomeServicesBundleJourney()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            EnergyCustomer customer = CustomerFactory.CustomerDetailsWithFixAndProtectTariffChosen(FuelType.Dual, ElectricityMeterType.Standard);
            customer.Projection = new Projection
            {
                EnergyEconomy7NightElecKwh = 100,
                EnergyEconomy7DayElecKwh = 200,
                EnergyAveEcon7ElecKwh = 300,
                EnergyAveStandardElecKwh = 400,
                EnergyAveStandardGasKwh = 500
            };
            customer.IsUsageKnown = true;

            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());

            // Act
            var tariffController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithEnergyProductServiceWrapper(new FakeProductServiceWrapper())
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper { ReturnFixNProtectBundle = true })
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false })
                .Build<TariffsController>();

            await tariffController.AvailableTariffs();

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBankDetailsServiceWrapper(new FakeBankDetailsService(null))
                .Build<SignUpController>();


            await tariffController.AvailableTariffs("BO002");

            ActionResult result = signUpController.BankDetails();
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<BankDetailsViewModel>();
            model.AccountHolder = "Mr Blah";
            model.AccountNumber = "12341234";
            model.SortCodeSegmentOne = "01";
            model.SortCodeSegmentTwo = "01";
            model.SortCodeSegmentThree = "01";
            model.SortCode = "01-01-01";
            model.IsAuthorisedChecked = true;
            model.DirectDebitDate = "28";

            ActionResult postResult = signUpController.BankDetails(model);

            // Assert
            var postViewResult = postResult.ShouldBeType<ViewResult>();
            var postModel = postViewResult.Model.ShouldBeType<BankDetailsViewModel>();
            postModel.BackChevronViewModel.ShouldNotBeNull();
            postModel.BackChevronViewModel.ActionName.ShouldEqual("ContactDetails");
            postModel.BackChevronViewModel.ControllerName.ShouldEqual("SignUp");
            postModel.BackChevronViewModel.TitleAttributeText.ShouldEqual("Back to previous page");
            postModel.AccountHolder.ShouldEqual("Mr Blah");
            postModel.AccountNumber.ShouldEqual("12341234");
            postModel.SortCodeSegmentOne.ShouldEqual("01");
            postModel.SortCodeSegmentTwo.ShouldEqual("01");
            postModel.SortCodeSegmentThree.ShouldEqual("01");
            postModel.SortCode.ShouldEqual("01-01-01");
            postModel.IsAuthorisedChecked.ShouldBeTrue();
            postModel.DirectDebitDate.ShouldEqual("28");
            postModel.AmountItemList.ShouldNotBeNull();
            postModel.AmountItemList.Count.ShouldEqual(2);
            postModel.AmountItemList[0].Item1.ShouldEqual("Electricity: ");
            postModel.AmountItemList[0].Item2.ShouldEqual("£84");
            postModel.AmountItemList[1].Item1.ShouldEqual("Gas: ");
            postModel.AmountItemList[1].Item2.ShouldEqual("£84");
            postModel.IsHesBundle.ShouldBeTrue();
            postModel.HesBundlePackageAmount.ShouldEqual("£0");
            postModel.HesWhyDoIPay0ModalId.ShouldNotBeNull();
            postModel.IsBroadbandBundleSelected.ShouldBeFalse();
            postModel.BroadbandBundleDescription.ShouldBeNull();

            postModel.IsRetry.ShouldBeTrue();
        }

        [Test]
        public void ShouldStoreBankDetailsInSession()
        {
            // Arrange
            var fakeBankDetailsService = new FakeBankDetailsService();
            getBankDetailsResponse bankDetails = fakeBankDetailsService.GetBankDetailsResponse;

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            var bankDetailsViewModel = new BankDetailsViewModel
            {
                IsAuthorisedChecked = true,
                AccountHolder = "Mr Test",
                AccountNumber = "12345678",
                SortCode = "102030",
                SortCodeSegmentOne = "10",
                SortCodeSegmentTwo = "20",
                SortCodeSegmentThree = "30",
                DirectDebitDate = "1"
            };

            // Act
            controller.ValidateViewModel(bankDetailsViewModel);
            controller.BankDetails(bankDetailsViewModel);

            // Assert
            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.DirectDebitDetails.AccountName.ShouldEqual(bankDetailsViewModel.AccountHolder);
            customer.DirectDebitDetails.AccountNumber.ShouldEqual(bankDetailsViewModel.AccountNumber);
            customer.DirectDebitDetails.SortCode.ShouldEqual(bankDetailsViewModel.SortCode);
            customer.DirectDebitDetails.DirectDebitPaymentDate.ToString().ShouldEqual(bankDetailsViewModel.DirectDebitDate);

            customer.DirectDebitDetails.BankName.ShouldEqual(bankDetails.bankName);
            customer.DirectDebitDetails.BankAddressLine1.ShouldEqual(bankDetails.bankFormattedAddress.bankAddressLine1);
            customer.DirectDebitDetails.BankAddressLine2.ShouldEqual(bankDetails.bankFormattedAddress.bankAddressLine2);
            customer.DirectDebitDetails.BankAddressLine3.ShouldEqual(bankDetails.bankFormattedAddress.bankAddressLine3);
            customer.DirectDebitDetails.Postcode.ShouldEqual(bankDetails.bankFormattedAddress.bankPostcode);
        }

        [Test]
        public void IfBankDetailsServiceFailsShouldGoToTechFaultAndLogError()
        {
            // Arrange
            var fakeBankDetailsService = new FakeBankDetailsService { ThrowException = true };
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBankDetailsServiceWrapper(fakeBankDetailsService)
                .Build<SignUpController>();

            var model = new BankDetailsViewModel
            {
                AccountNumber = "12345678",
                AccountHolder = "Mr Test",
                SortCode = "050200"
            };

            // Assert
            var ex = Assert.Throws<Exception>(() => controller.BankDetails(model));
            ex.Message.ShouldNotBeNull();
            ex.Message.ShouldEqual("Exception occured while calling Bank Details Service");
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void ShouldDisplayBankDetailsPageAgainIfRetryCountIsLessThanOrEqualToThree(int retryCount)
        {
            // Arrange
            var fakeBankDetailsService = new FakeBankDetailsService(null);

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                SelectedTariff = new Tariff(),
                BankServiceRetryCount = retryCount
            });
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBankDetailsServiceWrapper(fakeBankDetailsService)
                .Build<SignUpController>();

            var model = new BankDetailsViewModel
            {
                AccountNumber = "12345678",
                AccountHolder = "Test",
                SortCode = "000000"
            };

            // Act
            ActionResult result = controller.BankDetails(model);

            // Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<BankDetailsViewModel>();
            viewModel.IsRetry.ShouldEqual(true);
        }

        [Test]
        public void ShouldGoToFalloutPageIfRetryCountIsMoreThanThree()
        {
            // Arrange
            var fakeBankDetailsService = new FakeBankDetailsService(null);

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                BankServiceRetryCount = 3
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBankDetailsServiceWrapper(fakeBankDetailsService)
                .Build<SignUpController>();

            var model = new BankDetailsViewModel
            {
                AccountNumber = "12345678",
                AccountHolder = "Test",
                SortCode = "000000"
            };

            // Act
            ActionResult result = controller.BankDetails(model);

            // Assert
            result.ShouldNotBeNull();
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("InvalidBankDetails");
        }

        [TestCase("SSE 1 Year Fixed v15", FuelType.Dual, PaymentMethod.MonthlyDirectDebit,
            "Your estimated annual energy costs will be £2,400. That’s £1,200 a year for gas and £1,200 a year for electricity. This price includes VAT at 5% and your projected discount (£30.40 a year for Direct Debit).",
            "£200", ".00")]
        [TestCase("SSE 1 Year Fixed v15", FuelType.Electricity, PaymentMethod.MonthlyDirectDebit,
            "Your estimated annual electricity costs will be £1,200. This price includes VAT at 5% and your projected discount (£10.20 a year for Direct Debit).",
            "£100", ".00")]
        [TestCase("SSE 1 Year Fixed v15", FuelType.Gas, PaymentMethod.MonthlyDirectDebit,
            "Your estimated annual gas costs will be £1,200. This price includes VAT at 5% and your projected discount (£20.20 a year for Direct Debit).",
            "£100", ".00")]
        [TestCase("SSE 1 Year Fixed v15", FuelType.Dual, PaymentMethod.Quarterly,
            "Your estimated annual energy costs will be £2,400. That’s £1,200 a year for gas and £1,200 a year for electricity. This price includes VAT at 5% and any applicable discounts.",
            "£200", ".00")]
        [TestCase("SSE 1 Year Fixed v15", FuelType.Dual, PaymentMethod.PayAsYouGo,
            "Your estimated annual energy costs will be £2,400. That’s £1,200 a year for gas and £1,200 a year for electricity. This price includes VAT at 5% and any applicable discounts.",
            "£200", ".00")]
        public void ShouldPopulateDetailsWhenCustomerIsDisplayedTheSummaryPageWhenInEnergyJourneyWithNoBundleSelected(
            string tariffName,
            FuelType fuelType,
            PaymentMethod paymentMethod,
            string disclaimerText,
            string tariffCostFullValue,
            string tariffCostPenceValue)
        {
            // Arrange
            EnergyCustomer customer = CustomerFactory.GetCustomerWithFullDetails(fuelType, ElectricityMeterType.Standard, tariffName, paymentMethod);
            customer.IsBundlingJourney = true;
            customer.SelectedBroadbandProduct = FakeBroadbandProductsData.GetBroadbandProductWithTalkProducts();
            customer.SelectedBroadbandProductCode = "FF3_LR18";

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, FakeProductsStub.GetTariffs(fuelType.ToString(), "Standard"));

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.ViewSummary();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SummaryViewModel>();

            model.CustomerFormattedName.ShouldEqual(customer.PersonalDetails.FormattedName);
            model.DateOfBirth.ShouldEqual(customer.PersonalDetails.DateOfBirth);
            model.FullAddress.ShouldNotBeNull(customer.FullAddress());

            model.ContactNumber.ShouldEqual(customer.ContactDetails.ContactNumber);
            model.EmailAddress.ShouldEqual(customer.ContactDetails.EmailAddress);

            model.DirectDebitPaymentDate.ShouldEqual("28th");

            if (fuelType == FuelType.Electricity || fuelType == FuelType.Dual)
            {
                model.SelectedTariffViewModel.ElectricityDirectDebitAmount.ShouldEqual(
                    $"£{Math.Ceiling(Convert.ToDouble(customer.SelectedTariff.GetProjectedMonthlyElectricityCost()))}");
            }

            if (fuelType == FuelType.Gas || fuelType == FuelType.Dual)
            {
                model.SelectedTariffViewModel.GasDirectDebitAmount.ShouldEqual(
                    $"£{Math.Ceiling(Convert.ToDouble(customer.SelectedTariff.GetProjectedMonthlyGasCost()))}");
            }

            model.DirectDebitAccountNumber.ShouldEqual(customer.DirectDebitDetails.AccountNumber);
            model.DirectDebitAccountName.ShouldEqual(customer.DirectDebitDetails.AccountName);
            model.DirectDebitSortCode.ShouldEqual("10-20-30");

            model.SelectedTariffViewModel.ShouldNotBeNull("Selected tariff can't be null");

            model.TariffCostFullValue.ShouldEqual(tariffCostFullValue);
            model.TariffCostPenceValue.ShouldEqual(tariffCostPenceValue);
            model.SavingsPerMonthTxt.ShouldBeEmpty();

            model.DisclaimerText.ShouldEqual(disclaimerText);
            model.IsBroadbandBundleSelected.ShouldBeFalse();
            model.TermsAndConditionsText.ShouldEqual(Form_Resources.TermsAndConditionsCheckboxEnergyLabelText);
        }

        [TestCase(FuelType.Dual, "Save £12 a month (£216 over 18 months). Saving is based on £12 discount for 18 months for Fibre broadband (18 x £12 = £216), when compared to buying Unlimited Fibre, our equivalent standalone broadband product (£22 per month). Your estimated annual energy costs will be £2,400. That’s £1,200 for gas and £1,200 for electricity.")]
        [TestCase(FuelType.Electricity, "Save £12 a month (£216 over 18 months). Saving is based on £12 discount for 18 months for Fibre broadband (18 x £12 = £216), when compared to buying Unlimited Fibre, our equivalent standalone broadband product (£22 per month). Your estimated annual electricity costs will be £1,200.")]
        public void ShouldPopulateCorrectDisclaimerDetailsWhenCustomerIsDisplayedTheSummaryPageWithSelectedBundle(
            FuelType fuelType,
            string disclaimerText)
        {
            // Arrange
            EnergyCustomer customer = CustomerFactory.CustomerDetailsWithFixNFibreBundleTariffChosen(fuelType, ElectricityMeterType.Standard);
            customer.SelectedBroadbandProduct = FakeBroadbandProductsData.GetBroadbandProductWithTalkProducts();
            customer.SelectedBroadbandProductCode = "FF3_LR18";

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());

            var fakeTariffManager = new FakeTariffManager();
            var controller = new ControllerFactory()
                .WithTariffManager(fakeTariffManager)
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.ViewSummary();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SummaryViewModel>();
            model.IsBroadbandBundleSelected.ShouldBeTrue();
            var broadbandMoreInformationViewModel = (BroadbandMoreInformationViewModel)model.BundleMegaModalViewModel;
            broadbandMoreInformationViewModel.ShouldNotBeNull();
            broadbandMoreInformationViewModel.BroadbandPackageSpeed.ShouldNotBeNull();
            model.DisclaimerText.ShouldEqual(disclaimerText);
        }

        [Test]
        public void ShouldPopulateDetailsWhenCustomerIsDisplayedTheSummaryPageWhenInBroadbandBundleJourney()
        {
            // Arrange
            const string tariffName = "Fix And Fibre";
            EnergyCustomer customer = CustomerFactory.CustomerDetailsWithFixNFibreBundleTariffChosen(FuelType.Dual, ElectricityMeterType.Standard);

            customer.SelectedBroadbandProduct = FakeBroadbandProductsData.GetBroadbandProductWithTalkProducts();
            customer.SelectedBroadbandProductCode = "FF3_LR18";

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());

            var fakeTariffManager = new FakeTariffManager();
            fakeTariffManager.PdfLinkMappings.Add(tariffName, "tariff_v1.pdf | tariff_v2.pdf");

            var controller = new ControllerFactory()
                .WithTariffManager(fakeTariffManager)
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.ViewSummary();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SummaryViewModel>();
            model.IsBroadbandBundleSelected.ShouldBeTrue();
            model.TermsAndConditionsText.ShouldEqual(Form_Resources.TermsAndConditionsCheckboxBroadbandBundleLabelText);
            model.IsHesBundleSelected.ShouldBeFalse();
            model.IsElectricalWiringSelected.ShouldBeFalse();
        }

        [Test]
        public void ShouldDisplaySelectedExtraDetailsInSummaryView()
        {
            // Arrange
            EnergyCustomer customer = CustomerFactory.CustomerDetailsWithFixAndProtectWithExtraTariffChosen(FuelType.Dual, ElectricityMeterType.Standard);

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.ViewSummary();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SummaryViewModel>();
            model.IsHesBundleSelected.ShouldBeTrue();
            model.IsElectricalWiringSelected.ShouldBeTrue();
            model.HesDDGuaranteeLinkText.ShouldEqual("Print Heating Breakdown & Electrical Wiring Cover Direct Debit instruction");
            model.HesDDGuaranteeLinkAlt.ShouldEqual("Print Heating Breakdown & Electrical Wiring Cover Direct Debit instruction");
        }

        [Test]
        public void ShouldNotDisplaySelectedExtraDetailsInSummaryView()
        {
            // Arrange
            EnergyCustomer customer = CustomerFactory.CustomerDetailsWithFixAndProtectTariffChosen(FuelType.Dual, ElectricityMeterType.Standard);

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.ViewSummary();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SummaryViewModel>();
            model.IsHesBundleSelected.ShouldBeTrue();
            model.IsElectricalWiringSelected.ShouldBeFalse();
            model.HesDDGuaranteeLinkText.ShouldEqual("Print Heating Breakdown Direct Debit instruction");
            model.HesDDGuaranteeLinkAlt.ShouldEqual("Print Heating Breakdown Direct Debit instruction");
        }

        [TestCase(FuelType.Dual,
            "Save £22 a month (£264 over 12 months). Saving is based on Heating Breakdown being free for 12 months compared to the standard price when bought separately (£22 a month): £22 x 12 = £264. Your estimated annual energy costs will be £2,400. That’s £1,200 for gas and £1,200 for electricity.")]
        [TestCase(FuelType.Gas,
            "Save £22 a month (£264 over 12 months). Saving is based on Heating Breakdown being free for 12 months compared to the standard price when bought separately (£22 a month): £22 x 12 = £264. Your estimated annual gas costs will be £1,200.")]
        public void ShouldDisplayFixNProtectBreakdownInSummaryView(FuelType fuelType, string disclaimerText)
        {
            // Arrange
            const string tariffName = "Fix And Protect";
            EnergyCustomer customer = CustomerFactory.CustomerDetailsWithFixAndProtectTariffChosen(fuelType, ElectricityMeterType.Standard);

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());

            var fakeTariffManager = new FakeTariffManager();
            fakeTariffManager.PdfLinkMappings.Add(tariffName, "tariff_v1.pdf | tariff_v2.pdf");

            var controller = new ControllerFactory()
                .WithTariffManager(fakeTariffManager)
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.ViewSummary();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SummaryViewModel>();
            model.SelectedTariffViewModel.HeatingBreakdownCoverDirectDebitAmount.ShouldEqual(
                $"£{Math.Ceiling(Convert.ToDouble(customer.SelectedTariff.BundlePackage?.MonthlyDiscountedCost))}");
            model.IsBroadbandBundleSelected.ShouldBeFalse();
            model.BundlePackageFeatures.ShouldContain("24/7 emergency repairs, including parts and labour");
            model.BundlePackageFeatures.ShouldContain("£90 excess per call-out");
            model.BundlePackageFeatures.ShouldContain("12-month contract");
            model.SelectedTariffViewModel.ProjectedBundlePackageMonthlyCost.ShouldEqual("Free");
            model.BundlePackageIconFileName.ShouldEqual("hes-icon.svg");
            model.IsBundle.ShouldBeTrue();
            model.BundlePackageType.ShouldEqual(BundlePackageType.FixAndProtect);
            model.BundlePackageHeaderText.ShouldEqual("Heating Breakdown");
            model.IsHesBundleSelected.ShouldEqual(true);
            model.DisclaimerText.ShouldEqual(disclaimerText);
            model.SavingsPerMonthTxt.ShouldEqual("Saving you £22 a month");

            model.MoreInformationModalId.ShouldEqual("#FixNProtectBundleMegaModal");
            var hesMoreInformationViewModel = (HesMoreInformationViewModel)model.BundleMegaModalViewModel;
            hesMoreInformationViewModel.BundleDisclaimerModalText.ShouldEqual("Save £264 over 12 months compared to buying this product separately.");
            hesMoreInformationViewModel.OriginalFixNProtectMonthlyCost.ShouldEqual("£22");
            hesMoreInformationViewModel.ProjectedMonthlySavingsAmount.ShouldEqual("£22");
            hesMoreInformationViewModel.ExcessAmount.ShouldEqual("£90");
            hesMoreInformationViewModel.ExcessText.ShouldEqual("An excess is the amount you'll need to pay for each claim you make. So you'd pay £90 each time you make a claim.");
            hesMoreInformationViewModel.WhatsExcluded.Count.ShouldEqual(2);
            hesMoreInformationViewModel.WhatsExcluded.Count.ShouldEqual(2);
        }

        [Test]
        public void ShouldDisplaySelectedExtrasOnBundleSummaryView()
        {
            EnergyCustomer customer = CustomerFactory.CustomerDetailsWithFixAndProtectTariffChosen(FuelType.Dual, ElectricityMeterType.Standard);
            var extraAdded = new Extra(
                "SSE Electrical Wiring Cover",
                5.0,
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

            customer.SelectedExtras.Add(extraAdded);

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.ViewSummary();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SummaryViewModel>();
            model.SelectedExtras.Count.ShouldEqual(1);
            model.SelectedExtras[0].Name = extraAdded.Name;
            model.SelectedExtras[0].Price = extraAdded.BundlePrice.ToCurrency();
            model.SelectedExtras[0].FeatureList.ShouldContain(extraAdded.TagLine);
            model.SelectedExtras[0].FeatureList.ShouldContain($"{extraAdded.ContractLength}-months contract");
            model.TariffCostFullValue.ShouldEqual((customer.SelectedTariff.GetProjectedMonthlyCost() + extraAdded.BundlePrice).AmountSplitInPounds());
            model.TariffCostPenceValue.ShouldEqual((customer.SelectedTariff.GetProjectedMonthlyCost() + extraAdded.BundlePrice).AmountSplitInPence());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ShouldDisplayFixNFibreBreakdownInSummaryView(bool applyInstallationFee)
        {
            // Arrange
            const string tariffName = "Fix And Fibre";
            EnergyCustomer customer = CustomerFactory.CustomerDetailsWithFixNFibreBundleTariffChosen(
                FuelType.Dual,
                ElectricityMeterType.Standard,
                applyInstallationFee: applyInstallationFee);

            customer.SelectedBroadbandProduct = FakeBroadbandProductsData.GetBroadbandProductWithTalkProducts();
            customer.SelectedBroadbandProductCode = "FF3_LR18";

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());

            var fakeTariffManager = new FakeTariffManager();
            fakeTariffManager.PdfLinkMappings.Add(tariffName, "tariff_v1.pdf | tariff_v2.pdf");

            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
            var controller = new ControllerFactory()
                .WithConfigManager(fakeConfigManager)
                .WithTariffManager(fakeTariffManager)
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false, InstallLine = applyInstallationFee })
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.ViewSummary();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SummaryViewModel>();
            model.IsBroadbandBundleSelected.ShouldBeTrue();
            model.BundlePackageFeatures.ShouldContain("Line rental included");
            model.BundlePackageFeatures.ShouldContain("18-month contract");
            if (applyInstallationFee)
            {
                model.BroadbandApplyInstallationFee.ShouldEqual(true);
            }

            model.SelectedTariffViewModel.ProjectedBundlePackageMonthlyCost.ShouldEqual("£12");
            model.BundlePackageIconFileName.ShouldEqual("broadband.svg");
            model.IsBundle.ShouldBeTrue();
            model.BundlePackageType.ShouldEqual(BundlePackageType.FixAndFibre);
            model.BundlePackageHeaderText.ShouldEqual("Broadband: ");
            model.BundlePackageSubHeaderText.ShouldEqual("Fibre broadband");
            model.SavingsPerMonthTxt.ShouldEqual("Saving you £12 a month");

            var broadbandMoreInformationViewModel = (BroadbandMoreInformationViewModel)model.BundleMegaModalViewModel;
            broadbandMoreInformationViewModel.OriginalBroadbandMonthlyCost.ShouldEqual("£24");
            broadbandMoreInformationViewModel.ProjectedBroadbandMonthlyCost.ShouldEqual("£12");
            model.MoreInformationModalId.ShouldEqual("#BroadbandBundleMegaModal");
            broadbandMoreInformationViewModel.BroadbandPackageSpeed.HeaderText.ShouldEqual("Your estimated maximum download speed for RG1 1AA:");
            broadbandMoreInformationViewModel.BroadbandPackageSpeed.ShowHeaderText.ShouldBeTrue();
            broadbandMoreInformationViewModel.BroadbandPackageSpeed.PostCode.ShouldEqual("RG1 1AA");
            broadbandMoreInformationViewModel.BroadbandPackageSpeed.MaxDownload.ShouldEqual("22");
            broadbandMoreInformationViewModel.BroadbandPackageSpeed.MinDownload.ShouldEqual("18");
            broadbandMoreInformationViewModel.BroadbandPackageSpeed.PackageDescription.ShouldEqual(
                "This package is great for streaming, downloading large files, watching catch-up TV or online gaming.");
        }

        [Test]
        public void ShouldPopulateDetailsWhenCustomerIsDisplayedTheSummaryPageWhenInHomeServiceTariffJourney()
        {
            // Arrange
            const string tariffName = "Fix And Protect";
            EnergyCustomer customer = CustomerFactory.CustomerDetailsWithFixAndProtectTariffChosen(FuelType.Dual, ElectricityMeterType.Standard);

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());

            var fakeTariffManager = new FakeTariffManager();
            fakeTariffManager.PdfLinkMappings.Add(tariffName, "tariff_v1.pdf | tariff_v2.pdf");

            var controller = new ControllerFactory()
                .WithTariffManager(fakeTariffManager)
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.ViewSummary();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SummaryViewModel>();
            model.IsBroadbandBundleSelected.ShouldBeFalse();
            model.TermsAndConditionsText.ShouldContain("Fix And Protect");
        }

        [TestCase(FuelType.Dual, "Standard  meter (smart)", "Standard  meter (smart)", "Standard  meter (smart) - 111111E",
            "Standard  meter (smart) - 111111G")]
        [TestCase(FuelType.Electricity, "Standard  meter (smart)", "", "Standard  meter (smart) - 111111E", "")]
        [TestCase(FuelType.Gas, "", "Standard  meter (smart)", "", "Standard  meter (smart) - 111111G")]
        public void ShouldPopulateDetailsWhenCustomerIsDisplayedTheSummaryPageInCAndCJourney(
            FuelType fuelType,
            string elecMeterTypeMessage,
            string gasMeterTypeMessage,
            string secondMessage,
            string thirdMessage)
        {
            // Arrange
            EnergyCustomer customer =
                CustomerFactory.CustomerDetailsWithFixNFibreBundleTariffChosen(fuelType, ElectricityMeterType.Standard);
            customer.SelectedBroadbandProduct = FakeBroadbandProductsData.GetBroadbandProductWithTalkProducts();
            customer.SelectedBroadbandProductCode = "FF3_LR18";

            customer.MeterDetail = FakeCAndCData.GetMeterDetailsByFuelType(fuelType);
            customer.SelectedFuelType = fuelType;

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs,
                FakeProductsStub.GetTariffs(fuelType.ToString(), "Standard"));

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.ViewSummary();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SummaryViewModel>();


            model.ElecMeterTypeMessage.ShouldEqual(elecMeterTypeMessage);
            model.GasMeterTypeMessage.ShouldEqual(gasMeterTypeMessage);
            model.MeterDetailsViewModel.SecondMessage.ShouldEqual(secondMessage);
            model.MeterDetailsViewModel.ThirdMessage.ShouldEqual(thirdMessage);
        }

        [Test]
        public void ShouldRedirectToBankDetailsWhenValidPasswordIsEnteredAndDirectDebitPaymentMethodIsSelected()
        {
            // Arrange
            EnergyCustomer customer = CustomerFactory.GetCustomerWithContactAndPersonalDetails();
            customer.SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit;
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            var viewModel = new OnlineAccountViewModel
            {
                Password = "password1",
                ConfirmPassword = "password1"
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = controller.OnlineAccount(viewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("BankDetails");
            routeResult.RouteValues["controller"].ShouldEqual(null);
        }

        [Test]
        public void ShouldRedirectToSummaryWhenValidPasswordIsEnteredAndDirectDebitPaymentMethodIsNotSelected()
        {
            // Arrange
            EnergyCustomer customer = CustomerFactory.GetCustomerWithContactAndPersonalDetails();
            customer.SelectedPaymentMethod = PaymentMethod.Quarterly;
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            var viewModel = new OnlineAccountViewModel
            {
                Password = "password1",
                ConfirmPassword = "password1"
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = controller.OnlineAccount(viewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("ViewSummary");
            routeResult.RouteValues["controller"].ShouldEqual(null);
        }

        [TestCase("asd", "asd", "Please enter a password using the format given below")]
        [TestCase("asdasdasdasdasdasdasdasdasdasdasdasdasd", "asdasdasdasdasdasdasdasdasdasdasdasdasd", "Please enter a password using the format given below")]
        [TestCase("asdasdasdasd1", "asdasdd", "Passwords must match")]
        public void ShouldReturnOnlineAccountWhenPasswordIsInvalid(string password, string confirmPassword, string errorMessage)
        {
            // Arrange
            var controller = new ControllerFactory()
                .Build<SignUpController>();

            var viewModel = new OnlineAccountViewModel
            {
                Password = password,
                ConfirmPassword = confirmPassword
            };

            // Act
            controller.ValidateViewModel(viewModel);
            ActionResult result = controller.OnlineAccount(viewModel);

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.Model.ShouldBeType<OnlineAccountViewModel>();
            controller.ModelState.IsValid.ShouldBeFalse();
            controller.ModelState.Keys.Count.ShouldEqual(1);
            controller.ModelState.Values.First().Errors.First().ErrorMessage.ShouldEqual(errorMessage);
        }

        [Test]
        public async Task ShouldCreateOnlineProfileWhenProfileDoesNotExist()
        {
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(new HttpCookieCollection()))
                .Build();

            EnergyCustomer customer = CustomerFactory.CustomerDetailsWithFixAndProtectTariffChosen(FuelType.Dual, ElectricityMeterType.Standard);
            customer.OnlineAccountPassword = "TestPa33word";

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.OpenReachResponse, FakeOpenReachData.GetOpenReachData());
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, FakeProductsStub.GetTariffs("Dual", "Standard"));

            var fakeProfileRepository = new FakeProfileRepository { ProfileExists = false };

            var fakeEmailManager = new FakeEmailManager();
            var controller = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager)
                .WithEmailManager(fakeEmailManager)
                .WithCustomerProfileRepository(fakeProfileRepository)
                .Build<SignUpController>();
            
            // Act            
            await controller.ViewSummary(new SummaryViewModel());            

            // Assert
            fakeProfileRepository.InsertProfileSqlParameters["logOnName"].ShouldEqual(customer.ContactDetails.EmailAddress);
            fakeProfileRepository.InsertProfileSqlParameters["password"].Length.ShouldEqual(89);
            fakeProfileRepository.InsertProfileSqlParameters["title"].ShouldEqual(customer.PersonalDetails.Title);
            fakeProfileRepository.InsertProfileSqlParameters["firstName"].ShouldEqual(customer.PersonalDetails.FirstName);
            fakeProfileRepository.InsertProfileSqlParameters["lastName"].ShouldEqual(customer.PersonalDetails.LastName);
            fakeProfileRepository.InsertProfileSqlParameters["marketingConsent"]
                .ShouldEqual(customer.ContactDetails.MarketingConsent.ToString());
            fakeProfileRepository.InsertProfileSqlParameters["dateOfBirth"].ShouldEqual(customer.PersonalDetails.DateOfBirth);
            fakeProfileRepository.InsertProfileSqlParameters["userInterest"].ShouldEqual("0");
            fakeProfileRepository.InsertProfileSqlParameters["accountStatus"].ShouldEqual("0");
            fakeProfileRepository.InsertProfileSqlParameters["telephoneNumber"]
                .ShouldEqual(customer.ContactDetails.ContactNumber);
            fakeProfileRepository.InsertProfileSqlParameters["signupBrand"].ShouldEqual("10");
            fakeProfileRepository.InsertProfileSqlParameters["mobileNumber"]
                .ShouldEqual(customer.ContactDetails.ContactNumber);
            fakeProfileRepository.InsertIntoProfileDB.ShouldEqual(1);
        }

        [TestCase("BankDetails", null, PaymentMethod.MonthlyDirectDebit)]
        [TestCase("ViewSummary", null, PaymentMethod.Quarterly)]
        public async Task ShouldRedirectToCorrectPageWhenAccountAlreadyExists(string actionName, string controllerName, PaymentMethod paymentMethod)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var energyCustomer = new EnergyCustomer
            {
                SelectedPaymentMethod = paymentMethod,
                ContactDetails = new ContactDetails
                {
                    EmailAddress = "abc@abc.com"
                }
            };
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, energyCustomer);
            var fakeProfileRepository = new FakeProfileRepository { ProfileExists = true };
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithCustomerProfileRepository(fakeProfileRepository)
                .Build<SignUpController>();

            // Act
            ActionResult result = await controller.OnlineAccount();

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual(actionName);
            routeResult.RouteValues["controller"].ShouldEqual(controllerName);
        }

        [TestCase(ProductType.Electric, "SSE")]
        [TestCase(ProductType.HomeServices, "SSE Home Services")]
        public void ShouldPopulateMandateWithCorrectCompanyName(ProductType productType, string expectedCompanyName)
        {
            // Arrange
            EnergyCustomer customer = CustomerFactory.GetCustomerWithFullDetails(FuelType.Dual, ElectricityMeterType.Standard);
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.PrintMandate(productType);

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.ViewName.ShouldEqual("PrintMandate");
            var model = viewResult.Model.ShouldBeType<DirectDebitMandateViewModel>();
            model.CompanyName.ShouldEqual(expectedCompanyName);
        }

        [Test]
        public async Task ShouldRedirectToThankYouPageWhenCustomerContinuesFromSummaryPage()
        {
            // Arrange
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(new HttpCookieCollection()))
                .Build();

            EnergyCustomer customer = CustomerFactory.GetCustomerWithFullDetails(FuelType.Dual, ElectricityMeterType.Standard);
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs,
                FakeProductsStub.GetTariffs("Dual", "Standard"));

            var controller = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            var summaryViewModel = new SummaryViewModel();

            // Act
            ActionResult result = await controller.ViewSummary(summaryViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("Confirmation");
            routeResult.RouteValues["controller"].ShouldEqual(null);
        }

        [TestCaseSource(nameof(FakeConfigTestData))]
        public void ShouldPopulateExtraInYourPriceViewModelWhenApplicable(FakeConfigManager fakeConfigManager)
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

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithTariffManager(new TariffManager(fakeConfigManager, new FakeConfigurationSettings()))
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            // ReSharper disable once UnusedVariable
            ActionResult result = controller.AvailableTariffs().Result;
            // ReSharper disable once UnusedVariable
            ActionResult result2 = controller.AvailableTariffs(selectedTariffId).Result;

            // Assert
            var yourPrice = fakeSessionManager.GetSessionDetails<YourPriceViewModel>(SessionKeys.EnergyYourPriceDetails);
            yourPrice.ShouldNotBeNull();
        }

        [Test]
        public void ShouldShowCorrectConfirmationDetailsForFixNFibreBundle()
        {
            // Arrange
            EnergyCustomer customer = CustomerFactory.CustomerDetailsWithFixNFibreBundleTariffChosen(FuelType.Dual, ElectricityMeterType.Standard);

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.Confirmation();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<ConfirmationViewModel>();
            viewModel.IsABundle.ShouldBeTrue();
            viewModel.HelpPageUrlAltText.ShouldEqual("View our help page");
            viewModel.HelpPageUrlText.ShouldEqual("help page");
            viewModel.BoxText.ShouldEqual(
                "Your energy supply and phone and broadband will be switched over to us within 21 days. <strong>We’ll keep you informed at every step.</strong>");

            viewModel.CrossSellBanner.Header.ShouldEqual("Protect your home with our heating cover");
            viewModel.CrossSellBanner.Paragraph.ShouldEqual(
                "We offer boiler and heating cover with parts and labour included, a 24/7 emergency helpline, and call-outs for emergency repairs 24 hours a day.");
            viewModel.CrossSellBanner.DesktopImage.ShouldEqual("cross-sell-banner-desktop.png");
            viewModel.CrossSellBanner.LinkText.ShouldEqual("Find out more");
            viewModel.CrossSellBanner.LinkAltText.ShouldEqual("Find out more about our heating cover");
            viewModel.CrossSellBanner.LinkUrl.ShouldEqual("https://www.sse.co.uk/home-services");
            viewModel.WhatHappensNext.Count.ShouldEqual(1);
            viewModel.WhatHappensNext[0]
                .ShouldEqual("We’ll then email you Welcome packs with details about your energy and broadband products, including terms and conditions.");
        }

        [Test]
        public void ShouldShowCorrectConfirmationDetailsForFixNProtectBundle()
        {
            // Arrange
            EnergyCustomer customer = CustomerFactory.CustomerDetailsWithFixAndProtectTariffChosen(FuelType.Dual, ElectricityMeterType.Standard);

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.Confirmation();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<ConfirmationViewModel>();
            viewModel.IsABundle.ShouldBeTrue();
            viewModel.HelpPageUrlAltText.ShouldEqual("View our help page");
            viewModel.HelpPageUrlText.ShouldEqual("help page");
            viewModel.BoxText.ShouldEqual(
                "Your energy supply will switch to us within 21 days. We'll keep you informed at every step. <strong>Your heating cover starts on the next working day after your energy switch date.</strong>");

            viewModel.CrossSellBanner.Header.ShouldEqual("Switch to SSE's Frustration-Free Broadband");
            viewModel.CrossSellBanner.Paragraph.ShouldEqual(
                "We offer broadband with a promise of zero price hikes during your 18-month contract, unlimited downloads and no upfront broadband charges – and you can even leave for free with our Happiness Guarantee.");
            viewModel.CrossSellBanner.DesktopImage.ShouldEqual("broadband.png");
            viewModel.CrossSellBanner.LinkText.ShouldEqual("Find out more");
            viewModel.CrossSellBanner.LinkAltText.ShouldEqual("Find out more about our broadband");
            viewModel.CrossSellBanner.LinkUrl.ShouldEqual("https://sse.co.uk/phone-and-broadband");
            viewModel.WhatHappensNext.Count.ShouldEqual(2);
            viewModel.WhatHappensNext[0]
                .ShouldEqual("We’ll then email you a Welcome pack with details about your energy product, including terms and conditions.");
            viewModel.WhatHappensNext[1].ShouldEqual("You’ll receive your Welcome pack with details about your heating cover by post.");
        }

        [Test]
        public void ShouldShowCorrectConfirmationDetailsForEnergyOnlyTariff()
        {
            // Arrange
            EnergyCustomer customer = CustomerFactory.GetCustomerWithFullDetails(FuelType.Dual, ElectricityMeterType.Standard);

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.Confirmation();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<ConfirmationViewModel>();
            viewModel.IsABundle.ShouldBeFalse();
            viewModel.HelpPageUrlAltText.ShouldEqual("View our energy help page");
            viewModel.HelpPageUrlText.ShouldEqual("energy help page");
            viewModel.BoxText.ShouldEqual("<strong>It takes no more than 21 days to switch</strong> to us and we’ll keep you informed at every step.");
            viewModel.WhatHappensNext.Count.ShouldEqual(1);
            viewModel.WhatHappensNext[0].ShouldEqual("We’ll then email you a Welcome pack with details about your product, including terms and conditions.");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ShouldDisplayCorrectHelpPageDetails(bool isBundle)
        {
            // Arrange
            EnergyCustomer customer = CustomerFactory.GetCustomerWithFullDetails(FuelType.Dual, ElectricityMeterType.Standard);
            if (isBundle)
            {
                customer.SelectedTariff.BundlePackage = new BundlePackage("100", 12, 22, BundlePackageType.FixAndFibre, new List<TariffTickUsp>());
            }

            customer.SelectedTariff.IsBundle = isBundle;

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.Confirmation();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<ConfirmationViewModel>();
            viewModel.HelpPageUrlText.ShouldEqual(isBundle ? Confirmation_Resources.BundleHelpPageUrlText : Confirmation_Resources.EnergyHelpPageUrlText);
            viewModel.HelpPageUrlAltText.ShouldEqual(isBundle ? Confirmation_Resources.BundleHelpPageUrlAlt : Confirmation_Resources.EnergyHelpPageUrlAlt);
        }

        [Test]
        public void ShouldPopulateSummaryViewPdfCountCorrectlyForFixNFibre()
        {
            // Arrange
            const FuelType fuelType = FuelType.Dual;
            EnergyCustomer customer =
                CustomerFactory.CustomerDetailsWithFixNFibreBundleTariffChosen(fuelType, ElectricityMeterType.Standard, "1 Year Fix and Fibre v2");
            customer.SelectedBroadbandProduct = FakeBroadbandProductsData.GetBroadbandProductWithTalkProducts();
            customer.SelectedBroadbandProductCode = "FF3_LR18";
            customer.SelectedTariff.TariffGroup = TariffGroup.FixAndFibre;

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs,
                FakeProductsStub.GetTariffs(fuelType.ToString(), "Standard"));

            var controller = new ControllerFactory()
                .WithTariffManager(new TariffManager(new FakeConfigManager(), new FakeConfigurationSettings()))
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.ViewSummary();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SummaryViewModel>();

            model.EnergyTermsAndConditionsPdfLinks.Count().ShouldEqual(1);
            model.BroadbandBundleTermsAndConditionsPdfLinks.Count().ShouldEqual(1);
            model.HomeServicesBundleTermsAndConditionsPdfLinks.Count.ShouldEqual(0);
        }

        [Test]
        public void ShouldPopulateSummaryViewPdfCountCorrectlyForFixAndProtect()
        {
            // Arrange
            const FuelType fuelType = FuelType.Dual;
            EnergyCustomer customer = CustomerFactory.CustomerDetailsWithFixAndProtectTariffChosen(fuelType, ElectricityMeterType.Standard, "1 Year Fix and Protect v2");

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, FakeProductsStub.GetTariffs(fuelType.ToString(), "Standard"));

            var controller = new ControllerFactory()
                .WithTariffManager(new TariffManager(new FakeConfigManager(), new FakeConfigurationSettings()))
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.ViewSummary();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SummaryViewModel>();

            model.EnergyTermsAndConditionsPdfLinks.Count().ShouldEqual(3);
            model.BroadbandBundleTermsAndConditionsPdfLinks.Count().ShouldEqual(0);
            model.HomeServicesBundleTermsAndConditionsPdfLinks.Count.ShouldEqual(2);
        }

        [Test]
        public void ShouldPopulateSummaryViewPdfCountCorrectlyForEnergyTariff()
        {
            // Arrange
            const FuelType fuelType = FuelType.Dual;

            EnergyCustomer customer = CustomerFactory.GetCustomerWithFullDetails(fuelType, ElectricityMeterType.Standard, "Tariff v2");
            customer.SelectedTariff.TariffGroup = TariffGroup.None;

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, FakeProductsStub.GetTariffs(fuelType.ToString(), "Standard"));

            var controller = new ControllerFactory()
                .WithTariffManager(new TariffManager(new FakeConfigManager(), new FakeConfigurationSettings()))
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.ViewSummary();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SummaryViewModel>();

            model.EnergyTermsAndConditionsPdfLinks.Count().ShouldEqual(2);
            model.BroadbandBundleTermsAndConditionsPdfLinks.Count().ShouldEqual(0);
            model.HomeServicesBundleTermsAndConditionsPdfLinks.Count.ShouldEqual(0);
        }

        [TestCase("1 Year Fix and Fibre v2", "Download Energy - Terms and Conditions - 1 Year Fix and Fibre v2", TariffGroup.FixAndFibre)]
        [TestCase("1 Year Fix and Protect v2", "Download Energy - Terms and Conditions - 1 Year Fix and Protect v2", TariffGroup.FixAndProtect)]
        [TestCase("Tariff v2", "My PDF Alt Text", TariffGroup.None)]
        public void ShouldPopulateSummaryViewEnergyPdfWithAccTextCorrectly(string tariffName, string accText, TariffGroup selectedTariffGroup)
        {
            // Arrange
            const FuelType fuelType = FuelType.Dual;
            EnergyCustomer customer =
                CustomerFactory.CustomerDetailsWithFixNFibreBundleTariffChosen(fuelType, ElectricityMeterType.Standard, tariffName);
            customer.SelectedBroadbandProduct = FakeBroadbandProductsData.GetBroadbandProductWithTalkProducts();
            customer.SelectedBroadbandProductCode = "FF3_LR18";
            customer.SelectedTariff.TariffGroup = selectedTariffGroup;

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs,
                FakeProductsStub.GetTariffs(fuelType.ToString(), "Standard"));

            var controller = new ControllerFactory()
                .WithTariffManager(new TariffManager(new FakeConfigManager(), new FakeConfigurationSettings()))
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.ViewSummary();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SummaryViewModel>();

            model.EnergyTermsAndConditionsPdfLinks.ToArray()[0].Title.ShouldEqual(accText);
        }

        [TestCase("1 Year Fix and Fibre v2", "Energy - Terms and Conditions - 1 Year Fix and Fibre v2", TariffGroup.FixAndFibre)]
        [TestCase("1 Year Fix and Protect v2", "Energy - Terms and Conditions - 1 Year Fix and Protect v2", TariffGroup.FixAndProtect)]
        [TestCase("Tariff v2", "tariff_v1.pdf", TariffGroup.None)]
        public void ShouldPopulateSummaryViewEnergyPdfWithDisplayNameCorrectly(string tariffName, string displayName, TariffGroup selectedTariffGroup)
        {
            // Arrange
            const FuelType fuelType = FuelType.Dual;
            EnergyCustomer customer =
                CustomerFactory.CustomerDetailsWithFixNFibreBundleTariffChosen(fuelType, ElectricityMeterType.Standard, tariffName);
            customer.SelectedBroadbandProduct = FakeBroadbandProductsData.GetBroadbandProductWithTalkProducts();
            customer.SelectedBroadbandProductCode = "FF3_LR18";
            customer.SelectedTariff.TariffGroup = selectedTariffGroup;

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs,
                FakeProductsStub.GetTariffs(fuelType.ToString(), "Standard"));

            var controller = new ControllerFactory()
                .WithTariffManager(new TariffManager(new FakeConfigManager(), new FakeConfigurationSettings()))
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.ViewSummary();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SummaryViewModel>();

            model.EnergyTermsAndConditionsPdfLinks.ToArray()[0].DisplayName.ShouldEqual(displayName);
        }

        [TestCase("1 Year Fix and Fibre v2", true, "Print energy and broadband Direct Debit instruction",
            "Print energy and broadband Direct Debit instruction")]
        [TestCase("1 Year Fixed v16", false, "Print energy Direct Debit instruction", "Print energy Direct Debit instruction")]
        public void ShouldPopulateDDLinkTextCorrectly(
            string tariffName,
            bool isBroadband,
            string expectedLinkText,
            string expectedAltText)
        {
            // Arrange
            EnergyCustomer customer = CustomerFactory.GetCustomerWithFullDetails(FuelType.Dual, ElectricityMeterType.Standard, tariffName);
            if (isBroadband)
            {
                customer = CustomerFactory.CustomerDetailsWithFixNFibreBundleTariffChosen(FuelType.Dual, ElectricityMeterType.Standard, tariffName);
                customer.SelectedBroadbandProduct = FakeBroadbandProductsData.GetBroadbandProductWithTalkProducts();
                customer.SelectedBroadbandProductCode = "FF3_LR18";
            }

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, FakeProductsStub.GetTariffs(FuelType.Dual.ToString(), "Standard"));

            var controller = new ControllerFactory()
                .WithTariffManager(new TariffManager(new FakeConfigManager(), new FakeConfigurationSettings()))
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.ViewSummary();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SummaryViewModel>();

            model.DDMandateLinkText.ShouldEqual(expectedLinkText);
            model.DDMandateLinkAltText.ShouldEqual(expectedAltText);
        }

        [TestCase("FF3_LR18", "£212", ".00", false, null)]
        [TestCase("FF3_EAW18", "£216", ".00", true, "Includes local, national and UK mobile calls during evenings and weekends")]
        [TestCase("FF3_ANY18", "£220", ".00", true, "Includes local, national and UK mobile calls made at any time")]
        [TestCase("FF3_AP18", "£224", ".00", true, "Includes local, national and UK mobile calls made at any time, plus international calls to landlines in 35 countries")]
        public void ShouldDisplayUpgradesSectionInInSummaryView(
            string selectedTalkProductCode,
            string tariffCostFullValue,
            string tariffCostPenceValue,
            bool shouldDisplayUpgradeSection,
            string talkProductTagline)
        {
            // Arrange
            const FuelType fuelType = FuelType.Dual;
            EnergyCustomer customer = CustomerFactory.CustomerDetailsWithFixNFibreBundleTariffChosen(fuelType, ElectricityMeterType.Standard);
            customer.SelectedBroadbandProduct = FakeBroadbandProductsData.GetBroadbandProductWithTalkProducts();
            customer.SelectedBroadbandProductCode = selectedTalkProductCode;

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, FakeProductsStub.GetTariffs(fuelType.ToString(), "Standard"));

            var controller = new ControllerFactory()
                .WithTariffManager(new TariffManager(new FakeConfigManager(), new FakeConfigurationSettings()))
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.ViewSummary();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SummaryViewModel>();

            model.DisplayPhonePackageSection.ShouldEqual(shouldDisplayUpgradeSection);
            model.TalkPackageTagline.ShouldEqual(talkProductTagline);
            model.TariffCostFullValue.ShouldEqual(tariffCostFullValue);
            model.TariffCostPenceValue.ShouldEqual(tariffCostPenceValue);
        }

        [Test]
        public async Task ShouldSendConfirmationEmailWhenCustomerConfirmsOrder()
        {
            // Arrange
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(new HttpCookieCollection()))
                .Build();

            EnergyCustomer customer = CustomerFactory.GetCustomerWithFullDetails(FuelType.Dual, ElectricityMeterType.Standard);
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs,
                FakeProductsStub.GetTariffs("Dual", "Standard"));

            var fakeTariffManager = new FakeTariffManager();
            fakeTariffManager.PdfLinkMappings.Add("SSE 1 Year Fixed v14", "tariff_v1.pdf");

            var fakeEmailManager = new FakeEmailManager();

            var controller = new ControllerFactory()
                .WithTariffManager(fakeTariffManager)
                .WithEmailManager(fakeEmailManager)
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            await controller.ViewSummary(new SummaryViewModel());

            // Assert
            fakeEmailManager.To.ShouldContain(customer.ContactDetails.EmailAddress);
            fakeEmailManager.Subject.ShouldContain("Welcome to SSE");
            fakeEmailManager.Body.ShouldContain($"Hi {customer.PersonalDetails.FirstName}");
            fakeEmailManager.Body.ShouldContain($"{customer.SelectedTariff.DisplayName} tariff");
        }

        [Test, TestCase(FuelType.Dual, ElectricityMeterType.Standard)]
        public async Task ShouldSendBundleConfirmationEmailWhenCustomerConfirmsOrderWithBroadbandBundle(FuelType fuelType, ElectricityMeterType meterType)
        {
            // Arrange
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(new HttpCookieCollection()))
                .Build();

            EnergyCustomer customer = CustomerFactory.GetCustomerWithFullBundleDetails(fuelType, meterType);

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.OpenReachResponse, FakeOpenReachData.GetOpenReachData());

            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, FakeProductsStub.GetTariffs("Dual", "Standard"));

            var fakeEmailManager = new FakeEmailManager();
            var controller = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager)
                .WithEmailManager(fakeEmailManager)
                .Build<SignUpController>();

            var summaryViewModel = new SummaryViewModel();

            // Act
            ActionResult result = await controller.ViewSummary(summaryViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("Confirmation");
            routeResult.RouteValues["controller"].ShouldEqual(null);

            fakeEmailManager.To.ShouldContain(customer.ContactDetails.EmailAddress);
            fakeEmailManager.Subject.ShouldContain("Welcome to SSE");
            fakeEmailManager.Body.ShouldContain($"Hi {customer.PersonalDetails.FirstName}");
            fakeEmailManager.Body.ShouldContain($"{customer.SelectedTariff.DisplayName} Bundle");
            fakeEmailManager.Body.ShouldNotContain("[$ExtrasSection]");
        }

        [TestCase(FuelType.Dual, ElectricityMeterType.Standard, true)]
        [TestCase(FuelType.Dual, ElectricityMeterType.Standard, false)]
        public async Task ShouldSendBundleConfirmationEmailWhenCustomerConfirmsOrderWithHomeServicesBundle(FuelType fuelType, ElectricityMeterType meterType,
            bool includeExtra)
        {
            // Arrange
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(new HttpCookieCollection()))
                .Build();

            EnergyCustomer customer = CustomerFactory.CustomerDetailsWithFixAndProtectTariffChosen(fuelType, meterType);
            if (includeExtra)
            {
                var extraAdded = new Extra(
                    "SSE Electrical Wiring Cover",
                    5.0,
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

                customer.SelectedExtras.Add(extraAdded);
            }

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.OpenReachResponse, FakeOpenReachData.GetOpenReachData());

            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, FakeProductsStub.GetTariffs("Dual", "Standard"));

            var fakeEmailManager = new FakeEmailManager();
            var controller = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager)
                .WithEmailManager(fakeEmailManager)
                .Build<SignUpController>();

            var summaryViewModel = new SummaryViewModel();

            // Act
            ActionResult result = await controller.ViewSummary(summaryViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("Confirmation");
            routeResult.RouteValues["controller"].ShouldEqual(null);

            fakeEmailManager.To.ShouldContain(customer.ContactDetails.EmailAddress);
            fakeEmailManager.Subject.ShouldContain("Welcome to SSE");
            fakeEmailManager.Body.ShouldContain($"Hi {customer.PersonalDetails.FirstName}");
            fakeEmailManager.Body.ShouldContain($"{customer.SelectedTariff.DisplayName} Bundle");
            fakeEmailManager.Body.ShouldContain("Heating Breakdown");
            fakeEmailManager.Body.ShouldNotContain("[$ExtrasSection]");
            if (includeExtra)
            {
                fakeEmailManager.Body.ShouldContain("Extras");
                fakeEmailManager.Body.ShouldContain(
                    "Your Electrical Wiring Cover will also start on the next working day after we switch your energy to us. We'll send you your Policy booklet in the next few days.");
            }
            else
            {
                fakeEmailManager.Body.ShouldNotContain("Extras");
                fakeEmailManager.Body.ShouldNotContain(
                    "Your Electrical Wiring Cover will also start on the next working day after we switch your energy to us. We'll send you your Policy booklet in the next few days.");
            }
        }

        [TestCaseSource(nameof(UnderAgeTestData))]
        public void ShouldRemainOnPersonalDetailsPageWhenCustomerIsUnderAge(DateTime dateOfBirth, string postcode, bool isScottishPostcode, string description)
        {
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer,
                new EnergyCustomer { Postcode = postcode, SelectedTariff = new Tariff() });
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            var personalDetails = new PersonalDetailsViewModel
            {
                Titles = Titles.Dr,
                FirstName = "Test",
                LastName = "Test",
                DateOfBirthDay = dateOfBirth.Day.ToString(CultureInfo.InvariantCulture),
                DateOfBirthMonth = dateOfBirth.Month.ToString(CultureInfo.InvariantCulture),
                DateOfBirthYear = dateOfBirth.Year.ToString(CultureInfo.InvariantCulture),
                IsScottishPostcode = isScottishPostcode
            };

            ActionResult result = controller.PersonalDetails(personalDetails);
            result.ShouldNotBeNull()
                .ShouldBeType<ViewResult>()
                .ViewName.ShouldBeEmpty();
        }

        [Test]
        public void ShouldDisplaySelectedTariffNameOnConfirmationPage()
        {
            // Arrange
            EnergyCustomer customer = CustomerFactory.GetCustomerWithFullDetails(FuelType.Dual, ElectricityMeterType.Standard);
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.Confirmation();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<ConfirmationViewModel>();
            viewModel.ProductName.ShouldEqual(customer.SelectedTariff.DisplayName);
        }

        [Test]
        public void ShouldPopulateDataLayerOnConfirmationPage()
        {
            // Arrange
            EnergyCustomer customer = CustomerFactory.GetCustomerWithFullDetails(FuelType.Dual, ElectricityMeterType.Standard);
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.Confirmation();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<ConfirmationViewModel>();
            viewModel.DataLayer.JourneyData.ShouldNotBeNull();
            viewModel.DataLayer.Products.ShouldNotBeEmpty();
        }

        [TestCase(true, SmartMeterType.Smets1, "SMETS1")]
        [TestCase(true, SmartMeterType.Smets2, "SMETS2")]
        [TestCase(true, SmartMeterType.None, "-")]
        [TestCase(false, SmartMeterType.None, "-")]
        public void ShouldPopulateCorrectSmartMeterInformationInDataLayer(bool isCAndCJourney, SmartMeterType smartMeterType, string expectedValueInDataLayer)
        {
            // Arrange
            EnergyCustomer customer = CustomerFactory.GetCustomerWithFullDetails(FuelType.Dual, ElectricityMeterType.Standard);

            if (isCAndCJourney)
            {
                if (customer.MeterDetail.MeterInformation != null)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    customer.MeterDetail.MeterInformation.FirstOrDefault().SmartType = smartMeterType;
                }
            }
            else
            {
                customer.MeterDetail = null;
            }

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.Confirmation();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<ConfirmationViewModel>();
            viewModel.DataLayer.JourneyData.ShouldNotBeNull();
            viewModel.DataLayer.Products.ShouldNotBeEmpty();
            viewModel.DataLayer.JourneyData["SmartMeterType"].ShouldEqual(expectedValueInDataLayer);
        }

        [Test]
        public void ShouldClearSessionWhenConfirmationPageIsDisplayed()
        {
            // Arrange
            EnergyCustomer customer = CustomerFactory.GetCustomerWithFullDetails(FuelType.Dual, ElectricityMeterType.Standard);
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.Confirmation();

            // Assert
            result.ShouldBeType<ViewResult>();
            fakeSessionManager.SessionObject.Count.ShouldEqual(0);
        }

        [TestCase("01234567890", null, true, "01234567890", true, true, true)]
        [TestCase("01234567890", null, false, "01234567890", true, true, false)]
        [TestCase(null, null, true, "", false, false, true)]
        [TestCase(null, null, false, "", false, false, false)]
        [TestCase(null, "01234567890", true, "01234567890", true, true, true)]
        [TestCase(null, "01234567890", false, "01234567890", true, true, false)]
        public void ShouldReturnKeepYourNumberViewModelWithExpectedValuesWhenCallGetPhonePackage(
            string landline,
            string cli,
            bool isSSECustomer,
            string expectedCLIResult,
            bool expectedReadOnlyResult,
            bool expectedKeepExisting,
            bool expectedShowExistingNumberParagraph)
        {
            // Arrange
            EnergyCustomer customer = CustomerFactory.GetCustomerWithFullDetails(FuelType.Dual, ElectricityMeterType.Standard);
            customer.CLIChoice.UserProvidedCLI = landline;
            customer.CLIChoice.OpenReachProvidedCLI = cli;
            customer.CLIChoice.KeepExisting = expectedKeepExisting;
            customer.IsBundlingJourney = true;
            customer.SelectedBroadbandProduct = FakeBroadbandProductsData.GetPopulatedFibreBroadbandProduct();
            customer.IsSSECustomerCLI = isSSECustomer;

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(FakeConfigManagerFactory.DefaultBundling())
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.PhonePackage();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<PhonePackageViewModel>();
            viewModel.KeepYourNumberViewModel.KeepExistingNumber.ShouldEqual(expectedKeepExisting);
            viewModel.KeepYourNumberViewModel.CLI.ShouldEqual(expectedCLIResult);
            viewModel.KeepYourNumberViewModel.IsReadOnly.ShouldEqual(expectedReadOnlyResult);
            viewModel.KeepYourNumberViewModel.ShowExistingPhoneNumberParagraph.ShouldEqual(expectedShowExistingNumberParagraph);
        }

        [TestCase("01234567890", "01234567899", true, "01234567899", true)]
        [TestCase("01234567890", null, false, "01234567890", false)]
        [TestCase(null, "01234567888", true, "01234567888", true)]
        [TestCase(null, "01234567888", false, "01234567888", false)]
        [TestCase(null, "", false, "", false)]
        public void ShouldComputeCLIChoiceForCustomer(
            string landline,
            string openReachProvidedCli,
            bool keepNumber,
            string expectedCLIResult,
            bool expectedKeepExistingResult)
        {
            // Arrange
            var viewModel = new PhonePackageViewModel
            {
                KeepYourNumberViewModel = new KeepYourNumberViewModel
                {
                    CLI = landline,
                    KeepExistingNumber = keepNumber
                }
            };

            EnergyCustomer customer = CustomerFactory.GetCustomerWithFullBundleDetails(FuelType.Dual, ElectricityMeterType.Standard);
            customer.CLIChoice.UserProvidedCLI = landline;
            customer.CLIChoice.OpenReachProvidedCLI = openReachProvidedCli;

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            controller.PhonePackage("FF3_LR18", viewModel);

            // Assert
            var savedEnergyCustomer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            savedEnergyCustomer.CLIChoice.FinalCLI.ShouldEqual(expectedCLIResult);
            savedEnergyCustomer.CLIChoice.KeepExisting.ShouldEqual(expectedKeepExistingResult);
        }

        [TestCase("FF3_LR18", "Included", "Line Rental Only", true, false, 1, "http://localhost:3075/Content/Svgs/icons/basket-trigger-1item.svg")]
        [TestCase("FF3_EAW18", "£4", "Evening & Weekend", false, true, 2, "http://localhost:3075/Content/Svgs/icons/basket-trigger-2item.svg")]
        [TestCase("FF3_ANY18", "£8", "Anytime", false, true, 2, "http://localhost:3075/Content/Svgs/icons/basket-trigger-2item.svg")]
        [TestCase("FF3_AP18", "£12", "Anytime Plus", false, true, 2, "http://localhost:3075/Content/Svgs/icons/basket-trigger-2item.svg")]
        public async Task ShouldDisplayBundleInformationOnYourPriceViewModel(
            string productCode
            , string talkPackagePrice
            , string packageName
            , bool applyInstallationFee
            , bool upgradesSectionDisplayed
            , int totalItemsInBasket
            , string basketTogglerIconPath)
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
                SelectedAddress = new QasAddress
                    { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" },
                CLIChoice = new CLIChoice
                {
                    UserProvidedCLI = "01234567890",
                    OpenReachProvidedCLI = null
                }
            });

            var fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper
            {
                FakeLineSpeed = new FakeBroadbandProductsData.FakeLineSpeed
                {
                    FibreMinDownloadSpeed = "15200",
                    FibreMaxDownloadSpeed = "38000",
                    FibreMinUploadSpeed = "39800",
                    FibreMaxUploadSpeed = "20000"
                }
            };

            // Act
            var tariffController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false, InstallLine = applyInstallationFee })
                .WithBroadbandProductsServiceWrapper(fakeBroadbandProductsServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<TariffsController>();

            await tariffController.AvailableTariffs();

            var signUpControllerHelper = new ControllerHelperFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpControllerHelper>();

            var signUpController = new SignUpController(signUpControllerHelper);

            var baseSignUpControllerHelper = new ControllerHelperFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpControllerHelper>();

            var commonController = new CommonController(baseSignUpControllerHelper);

            await tariffController.AvailableTariffs("BO001");
            commonController.YourPriceDetails();
            signUpController.UpdateYourPrice(productCode, false);

            // Assert
            var yourPrice =
                fakeSessionManager.GetSessionDetails<YourPriceViewModel>(SessionKeys.EnergyYourPriceDetails);
            yourPrice.ShouldNotBeNull();
            yourPrice.BundlePackagePrice.ShouldEqual("£23");
            yourPrice.BundlePackageOriginalPrice.ShouldEqual("£33");
            yourPrice.PhonePackageUpgradeViewModel.Name.ShouldEqual(packageName);
            yourPrice.PhonePackageUpgradeViewModel.Price.ShouldEqual(talkPackagePrice);
            yourPrice.PhonePackageUpgradeViewModel.RemoveUpgradeButtonAltText.ShouldEqual("Remove this Upgrade from your Bundle");
            yourPrice.PhonePackageUpgradeViewModel.RemoveUpgradeButtonIconUrl.ShouldEqual("http://localhost:3075/Content/Svgs/icons/trashcan-white.svg");
            if (applyInstallationFee)
            {
                yourPrice.BroadbandApplyInstallationFee.ShouldEqual(true);
            }

            yourPrice.ShowPhonePackage.ShouldEqual(upgradesSectionDisplayed);
            yourPrice.BasketToggleIconBaseUrl.ShouldEqual("http://localhost:3075/Content/Svgs/icons");
            yourPrice.TotalItemsInBasket.ShouldEqual(totalItemsInBasket);
            yourPrice.BasketTogglerIconFilepath.ShouldEqual(basketTogglerIconPath);
        }

        [TestCase("FF3_LR18", "Included", 0.00, "Line Rental Only", true, "£189", ".67", 1,
            "http://localhost:3075/Content/Svgs/icons/basket-trigger-1item.svg")]
        [TestCase("FF3_EAW18", "£4", 4.00, "Evening & Weekend", false, "£193", ".67", 2,
            "http://localhost:3075/Content/Svgs/icons/basket-trigger-2item.svg")]
        [TestCase("FF3_ANY18", "£8", 8.00, "Anytime", false, "£197", ".67", 2, "http://localhost:3075/Content/Svgs/icons/basket-trigger-2item.svg")]
        [TestCase("FF3_AP18", "£12", 12.00, "Anytime Plus", false, "£201", ".67", 2,
            "http://localhost:3075/Content/Svgs/icons/basket-trigger-2item.svg")]
        public async Task ShouldDisplayTalkProductDetailsInYourPriceViewModel(
            string productCode
            , string talkPackagePrice
            , double talkPackagePriceInDbl
            , string packageName
            , bool applyInstallationFee
            , string totalMonthlyCostInPounds
            , string totalMonthlyCostInPence
            , int totalItemsInBasket
            , string basketTogglerIconPath)
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
                SelectedAddress = new QasAddress
                { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" },
                CLIChoice = new CLIChoice
                {
                    UserProvidedCLI = "01234567890",
                    OpenReachProvidedCLI = null
                },
                SelectedBroadbandProduct = new BroadbandProduct
                {
                    BroadbandCode = productCode,
                    TalkProducts = new List<TalkProduct>
                    {
                        new TalkProduct
                        {
                            ProductCode = productCode,
                            Prices = new List<BroadbandPrice>
                                { new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = talkPackagePriceInDbl } }
                        }
                    }
                }
            });

            var fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper
            {
                FakeLineSpeed = new FakeBroadbandProductsData.FakeLineSpeed
                {
                    FibreMinDownloadSpeed = "15200",
                    FibreMaxDownloadSpeed = "38000",
                    FibreMinUploadSpeed = "39800",
                    FibreMaxUploadSpeed = "20000"
                }
            };

            // Act
            var tariffController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false, InstallLine = applyInstallationFee })
                .WithBroadbandProductsServiceWrapper(fakeBroadbandProductsServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<TariffsController>();

            await tariffController.AvailableTariffs();

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            await tariffController.AvailableTariffs("BO001");
            signUpController.PhonePackage();

            signUpController.UpdateYourPrice(productCode, string.IsNullOrEmpty(productCode));

            // Assert
            var yourPrice = fakeSessionManager.GetSessionDetails<YourPriceViewModel>(SessionKeys.EnergyYourPriceDetails);
            yourPrice.ShouldNotBeNull();
            yourPrice.BundlePackagePrice.ShouldEqual("£23");
            yourPrice.BundlePackageOriginalPrice.ShouldEqual("£33");
            yourPrice.PhonePackageUpgradeViewModel.Name.ShouldEqual(packageName);
            yourPrice.PhonePackageUpgradeViewModel.Price.ShouldEqual(talkPackagePrice);
            yourPrice.PhonePackageUpgradeViewModel.RemoveUpgradeButtonAltText.ShouldEqual("Remove this Upgrade from your Bundle");
            yourPrice.PhonePackageUpgradeViewModel.RemoveUpgradeButtonIconUrl.ShouldEqual("http://localhost:3075/Content/Svgs/icons/trashcan-white.svg");

            if (applyInstallationFee)
            {
                yourPrice.BroadbandApplyInstallationFee.ShouldEqual(true);
            }

            yourPrice.BundlePackageHeaderText.ShouldEqual("Broadband:");
            yourPrice.BundlePackageSubHeaderText.ShouldEqual("Fibre broadband");
            yourPrice.BundlePackageType.ShouldEqual(BundlePackageType.FixAndFibre);
            yourPrice.ProjectedMonthlyTotalFullValue.ShouldEqual(totalMonthlyCostInPounds);
            yourPrice.ProjectedMonthlyTotalPenceValue.ShouldEqual(totalMonthlyCostInPence);
            yourPrice.BasketToggleIconBaseUrl.ShouldEqual("http://localhost:3075/Content/Svgs/icons");
            yourPrice.TotalItemsInBasket.ShouldEqual(totalItemsInBasket);
            yourPrice.BasketTogglerIconFilepath.ShouldEqual(basketTogglerIconPath);
        }

        [TestCase("FF3_LR18", "FF3_LR18")]
        [TestCase("FF3_EAW18", "FF3_EAW18")]
        [TestCase("FF3_ANY18", "FF3_ANY18")]
        [TestCase("FF3_AP18", "FF3_AP18")]
        [TestCase("", "FF3_LR18")]
        public async Task ShouldUpdateSelectedTalkPackageViewModelWhenTalkPackageIsChanged(string productCode, string expectedSelectedProductCode)
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
                SelectedAddress = new QasAddress
                {
                    AddressLine1 = "21 Waterloo Road",
                    Town = "Havant",
                    County = "Hampshire",
                    PicklistEntry = "21 Waterloo Road,Hampshire Havant"
                },
                CLIChoice = new CLIChoice
                {
                    UserProvidedCLI = "01234567890",
                    OpenReachProvidedCLI = null
                },
                SelectedBroadbandProduct = new BroadbandProduct
                {
                    BroadbandCode = productCode,
                    TalkProducts = new List<TalkProduct>
                    {
                        new TalkProduct
                        {
                            ProductCode = productCode,
                            Prices = new List<BroadbandPrice>
                                { new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 0.00 } }
                        }
                    }
                }
            });

            var fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper
            {
                FakeLineSpeed = new FakeBroadbandProductsData.FakeLineSpeed
                {
                    FibreMinDownloadSpeed = "15200",
                    FibreMaxDownloadSpeed = "38000",
                    FibreMinUploadSpeed = "39800",
                    FibreMaxUploadSpeed = "20000"
                }
            };

            // Act
            var tariffController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false, InstallLine = false })
                .WithBroadbandProductsServiceWrapper(fakeBroadbandProductsServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<TariffsController>();

            await tariffController.AvailableTariffs();

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            await tariffController.AvailableTariffs("BO001");
            signUpController.PhonePackage();
            signUpController.UpdateYourPrice(productCode, string.IsNullOrEmpty(productCode));

            ActionResult result = signUpController.UpdateSelectTalkPackage();

            var viewResult = result.ShouldBeType<PartialViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SelectTalkPackageViewModel>();
            viewModel.SelectedTalkProductCode.ShouldNotBeNull();
            viewModel.SelectedTalkProductCode.ShouldEqual(expectedSelectedProductCode);
        }

        [Test]
        public async Task ShouldUpdateBankDetailsViewModelWhenTalkPackageIsRemoved()
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
                SelectedAddress = new QasAddress
                {
                    AddressLine1 = "21 Waterloo Road",
                    Town = "Havant",
                    County = "Hampshire",
                    PicklistEntry = "21 Waterloo Road,Hampshire Havant"
                },
                CLIChoice = new CLIChoice
                {
                    UserProvidedCLI = "01234567890",
                    OpenReachProvidedCLI = null
                },
                SelectedBroadbandProduct = new BroadbandProduct
                {
                    BroadbandCode = "",
                    TalkProducts = new List<TalkProduct>
                    {
                        new TalkProduct
                        {
                            ProductCode = "",
                            Prices = new List<BroadbandPrice>
                                { new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 0.00 } }
                        }
                    }
                }
            });

            var fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper
            {
                FakeLineSpeed = new FakeBroadbandProductsData.FakeLineSpeed
                {
                    FibreMinDownloadSpeed = "15200",
                    FibreMaxDownloadSpeed = "38000",
                    FibreMinUploadSpeed = "39800",
                    FibreMaxUploadSpeed = "20000"
                }
            };

            // Act
            var tariffController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false, InstallLine = false })
                .WithBroadbandProductsServiceWrapper(fakeBroadbandProductsServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<TariffsController>();

            await tariffController.AvailableTariffs();

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            await tariffController.AvailableTariffs("BO001");
            signUpController.PhonePackage();
            signUpController.UpdateYourPrice("", true);

            ActionResult result = signUpController.UpdateDirectDebitAmounts();

            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<BankDetailsViewModel>();
            viewModel.BroadbandBundleDescription.ShouldNotBeNull();
            viewModel.BroadbandBundlePackageAmount.ShouldNotBeNull();
            viewModel.BroadbandBundleDescription.ShouldEqual("Broadband & Phone: Fibre broadband, Line Rental Only: ");
            viewModel.BroadbandBundlePackageAmount.ShouldEqual("£23");
        }

        [TestCase("FF3_LR18", "£212", ".00")]
        [TestCase("FF3_EAW18", "£212", ".00")]
        [TestCase("FF3_ANY18", "£212", ".00")]
        [TestCase("FF3_AP18", "£212", ".00")]
        public void ShouldUpdateSummaryViewModelWhenTalkPackageIsRemoved(string selectedBroadbandProductCode, string expectedTotalMonthlyPounds,
            string expectedTotalMonthlyPence)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();

            EnergyCustomer customer = CustomerFactory.CustomerDetailsWithFixNFibreBundleTariffChosen(FuelType.Dual, ElectricityMeterType.Standard);
            customer.SelectedBroadbandProduct = FakeBroadbandProductsData.GetBroadbandProductWithTalkProducts();
            customer.SelectedBroadbandProductCode = selectedBroadbandProductCode;

            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            signUpController.ViewSummary();

            ActionResult result = signUpController.UpdateSummary("", true);

            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SummaryViewModel>();
            viewModel.DisplayPhonePackageSection.ShouldBeFalse();
            viewModel.TariffCostFullValue.ShouldEqual(expectedTotalMonthlyPounds);
            viewModel.TariffCostPenceValue.ShouldEqual(expectedTotalMonthlyPence);
        }

        [TestCase(FuelType.Dual, "BO001", "£166.67", "£83.33", "£83.33", true)]
        [TestCase(FuelType.Electricity, "test03", "£83.33", "£83.33", null, false)]
        public async Task ShouldDisplayCorrectMonthlyEnergyPriceInYourPriceViewModel_BroadbandBundle(
            FuelType fuelType,
            string bundleCode,
            string expectedEnergyMonthlyDisplayPrice,
            string expectedElectricityMonthlyDisplayPrice,
            string expectedGasMonthlyDisplayPrice,
            bool applyInstallationFee)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
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
                SelectedAddress = new QasAddress
                { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" },
                CLIChoice = new CLIChoice
                {
                    UserProvidedCLI = "01234567890",
                    OpenReachProvidedCLI = null
                },
                SelectedBroadbandProduct = new BroadbandProduct
                {
                    BroadbandCode = "FF3_LR18",
                    TalkProducts = new List<TalkProduct>
                    {
                        new TalkProduct
                        {
                            ProductCode = "FF3_LR18",
                            Prices = new List<BroadbandPrice> { new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyTalkCharge } }
                        }
                    }
                }
            });

            var fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper
            {
                FakeLineSpeed = new FakeBroadbandProductsData.FakeLineSpeed
                {
                    FibreMinDownloadSpeed = "15200",
                    FibreMaxDownloadSpeed = "38000",
                    FibreMinUploadSpeed = "39800",
                    FibreMaxUploadSpeed = "20000"
                }
            };

            // Act
            var tariffController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper())
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { Fallout = false, InstallLine = applyInstallationFee })
                .WithBroadbandProductsServiceWrapper(fakeBroadbandProductsServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<TariffsController>();

            await tariffController.AvailableTariffs();

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            await tariffController.AvailableTariffs(bundleCode);
            signUpController.PhonePackage();

            // Assert
            var yourPrice = fakeSessionManager.GetSessionDetails<YourPriceViewModel>(SessionKeys.EnergyYourPriceDetails);
            yourPrice.ShouldNotBeNull();
            yourPrice.EnergyPerMonth.ShouldEqual(expectedEnergyMonthlyDisplayPrice);
            yourPrice.GasPerMonth.ShouldEqual(expectedGasMonthlyDisplayPrice);
            yourPrice.ElectricityPerMonth.ShouldEqual(expectedElectricityMonthlyDisplayPrice);

            yourPrice.BundlePackageType.ShouldEqual(BundlePackageType.FixAndFibre);
            yourPrice.BundlePackageHeaderText.ShouldEqual("Broadband:");
            yourPrice.BundlePackageFeatures.ShouldContain("18-month contract");
            yourPrice.BundlePackageFeatures.ShouldContain("Line rental included");
            if (applyInstallationFee)
            {
                yourPrice.BroadbandApplyInstallationFee.ShouldEqual(true);
            }

            yourPrice.TotalItemsInBasket.ShouldEqual(1);
            yourPrice.BasketToggleIconBaseUrl.ShouldEqual("http://localhost:3075/Content/Svgs/icons");
            yourPrice.BasketTogglerIconFilepath.ShouldEqual("http://localhost:3075/Content/Svgs/icons/basket-trigger-1item.svg");
        }

        [TestCase(FuelType.Dual, "BO001", "£166.67", "£83.33", "£83.33")]
        [TestCase(FuelType.Electricity, "test03", "£83.33", "£83.33", null)]
        [TestCase(FuelType.Gas, "test04", "£83.33", null, "£83.33")]
        public void ShouldDisplayCorrectMonthlyEnergyPriceInYourPriceViewModel_EnergyOnly(
            FuelType fuelType,
            string bundleCode,
            string expectedEnergyMonthlyDisplayPrice,
            string expectedElectricityMonthlyDisplayPrice,
            string expectedGasMonthlyDisplayPrice)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, new EnergyCustomer
            {
                Postcode = "PO9 1BH",
                SelectedElectricityMeterType = ElectricityMeterType.Standard,
                SelectedFuelType = FuelType.Gas,
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

            var tariffController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            // Act
            // ReSharper disable once UnusedVariable
            ActionResult result = tariffController.AvailableTariffs().Result;
            // ReSharper disable once UnusedVariable
            ActionResult result2 = tariffController.AvailableTariffs("095").Result;
            signUpController.PersonalDetails();

            // Assert
            var yourPrice = fakeSessionManager.GetSessionDetails<YourPriceViewModel>(SessionKeys.EnergyYourPriceDetails);
            yourPrice.ShouldNotBeNull();
            yourPrice.EnergyPerMonth.ShouldEqual("£83.33");
            yourPrice.GasPerMonth.ShouldEqual("£83.33");
            yourPrice.ElectricityPerMonth.ShouldBeNull();
            yourPrice.TotalItemsInBasket.ShouldEqual(1);
            yourPrice.BasketToggleIconBaseUrl.ShouldEqual("http://localhost:3075/Content/Svgs/icons");
            yourPrice.BasketTogglerIconFilepath.ShouldEqual("http://localhost:3075/Content/Svgs/icons/basket-trigger-1item.svg");
        }

        [Test]
        public async Task ShouldDisplayCorrectFixNProtectBreakdownInBasket()
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
                SelectedAddress = new QasAddress
                { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" }
            });

            // Act
            var tariffController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBundleTariffServiceWrapper(new FakeBundleTariffServiceWrapper { ReturnFixNProtectBundle = true })
                .WithConfigManager(fakeConfigManager)
                .Build<TariffsController>();

            await tariffController.AvailableTariffs();

            var signUpController = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<SignUpController>();

            await tariffController.AvailableTariffs("BO002");
            signUpController.PersonalDetails();

            // Assert
            var yourPrice = fakeSessionManager.GetSessionDetails<YourPriceViewModel>(SessionKeys.EnergyYourPriceDetails);
            yourPrice.ShouldNotBeNull();
            yourPrice.EnergyPerMonth.ShouldEqual("£166.67");
            yourPrice.BundlePackageType.ShouldEqual(BundlePackageType.FixAndProtect);
            yourPrice.BundlePackageHeaderText.ShouldEqual("Heating Breakdown");
            yourPrice.BundlePackageFeatures.ShouldContain("24/7 emergency repairs, including parts and labour");
            yourPrice.BundlePackageFeatures.ShouldContain("£90 excess per call-out");
            yourPrice.BundlePackageFeatures.ShouldContain("12-month contract");
            yourPrice.BundlePackagePrice.ShouldEqual("Free");
            yourPrice.TotalItemsInBasket.ShouldEqual(1);
            yourPrice.BasketToggleIconBaseUrl.ShouldEqual("http://localhost:3075/Content/Svgs/icons");
            yourPrice.BasketTogglerIconFilepath.ShouldEqual("http://localhost:3075/Content/Svgs/icons/basket-trigger-1item.svg");
        }

        [Test]
        public void ShouldThrowExceptionWhenConfirmAddressPageAccessedDirectly()
        {
            // Arrange
            var controller = new ControllerFactory()
                .Build<SignUpController>();

            // Act + Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => controller.ConfirmAddress());
            ex.Message.ShouldEqual("energyCustomer is null");
        }

        [Test]
        public async Task WhenConfirmAddressPageIsDisplayedShowListOfAddresses()
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
                { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" },
                CLIChoice = new CLIChoice
                {
                    UserProvidedCLI = "01234567890",
                    OpenReachProvidedCLI = null
                }
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses })
                .Build<SignUpController>();

            List<BTAddress> listOfTestAddresses = FakeBroadbandProductsData.GetAddresses(AddressResult.AllAddresses);

            // Act
            ActionResult resultSelectAddress = await controller.ConfirmAddress();
            var viewResult = resultSelectAddress.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<ConfirmAddressViewModel>();

            // Assert
            resultSelectAddress.ShouldNotBeNull();
            resultSelectAddress.ShouldBeType<ViewResult>();
            viewModel.Addresses.Count.ShouldEqual(listOfTestAddresses.Count);
        }

        [Test]
        public async Task ShouldSetBackChevronDestinationCorrectlyOnConfirmAddressView()
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
                { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" },
                CLIChoice = new CLIChoice
                {
                    UserProvidedCLI = "01234567890",
                    OpenReachProvidedCLI = null
                }
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses })
                .Build<SignUpController>();

            // Act
            ActionResult resultSelectAddress = await controller.ConfirmAddress();
            var viewResult = resultSelectAddress.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<ConfirmAddressViewModel>();

            // Assert
            resultSelectAddress.ShouldNotBeNull();
            resultSelectAddress.ShouldBeType<ViewResult>();
            viewModel.BackChevronViewModel.ActionName.ShouldEqual("AvailableTariffs");
            viewModel.BackChevronViewModel.ControllerName.ShouldEqual("Tariffs");
        }

        [Test]
        public async Task ShouldSetNotListedDestinationCorrectlyOnConfirmAddressView()
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
                { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" },
                CLIChoice = new CLIChoice
                {
                    UserProvidedCLI = "01234567890",
                    OpenReachProvidedCLI = null
                }
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses })
                .Build<SignUpController>();

            // Act
            ActionResult resultSelectAddress = await controller.ConfirmAddress();
            var viewResult = resultSelectAddress.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<ConfirmAddressViewModel>();

            // Assert
            resultSelectAddress.ShouldNotBeNull();
            resultSelectAddress.ShouldBeType<ViewResult>();
            viewModel.NotListedAction.ShouldEqual("UnableToComplete");
            viewModel.NotListedController.ShouldEqual("SignUp");
        }

        [Test]
        public async Task ShouldSetBTAddressAndRedirectToCheckBroadbandPackageOnConfirmAddressView()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());
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
                { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" },
                CLIChoice = new CLIChoice
                {
                    UserProvidedCLI = "01234567890",
                    OpenReachProvidedCLI = null
                }
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses })
                .Build<SignUpController>();

            // Act
            ActionResult resultSelectAddress = await controller.ConfirmAddress();
            var viewResult = resultSelectAddress.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<ConfirmAddressViewModel>();
            viewModel.SelectedAddressId = 1;

            ActionResult postResultSelectAddress = controller.ConfirmAddress(viewModel);

            // Assert
            postResultSelectAddress.ShouldNotBeNull();
            var routeResult = postResultSelectAddress.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("CheckBroadbandPackage");
            routeResult.RouteValues["controller"].ShouldEqual("Tariffs");

            var customer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            customer.SelectedBTAddress.ShouldNotBeNull();
            customer.SelectedBTAddress.FormattedAddress.ShouldEqual("21 Waterloo Road,Hampshire Havant");
        }

        [Test]
        public void IfSelectedAddressIdIsInvalidRemainOnConfirmAddressView()
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
                { AddressLine1 = "21 Waterloo Road", Town = "Havant", County = "Hampshire", PicklistEntry = "21 Waterloo Road,Hampshire Havant" },
                CLIChoice = new CLIChoice
                {
                    UserProvidedCLI = "01234567890",
                    OpenReachProvidedCLI = null
                }
            });

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses })
                .Build<SignUpController>();

            // Act
            ActionResult resultSelectAddress = controller.ConfirmAddress().Result;
            var viewResult = resultSelectAddress.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<ConfirmAddressViewModel>();
            viewModel.SelectedAddressId = -1;

            controller.ValidateViewModel(viewModel);
            ActionResult postResultSelectAddress = controller.ConfirmAddress(viewModel);

            // Assert
            postResultSelectAddress.ShouldNotBeNull();
            var result = postResultSelectAddress.ShouldBeType<ViewResult>();
            result.ShouldNotBeNull().ShouldBeType<ViewResult>().ViewName.ShouldBeEmpty();
            controller.ModelState.IsValid.ShouldBeFalse();
        }

        [TestCase(false, TariffGroup.None, false)]
        [TestCase(true, TariffGroup.None, false)]
        public void ShouldDisplayCorrectSmartMessageWhenCustomerNonCAndCCustomerAndSmartTariffIsSelected(bool isSmartCustomer, TariffGroup selectedTariffGroup, bool isSmartMessageVisible)
        {
            // Arrange
            EnergyCustomer customer = CustomerFactory.GetCustomerWithFullDetails(FuelType.Dual, ElectricityMeterType.Standard);
            customer.SelectedTariff.TariffGroup = selectedTariffGroup;
            customer.HasSmartMeter = isSmartCustomer;
            customer.MeterDetail = FakeCAndCData.GetMeterDetailsWithElecNonPayGoNonSmart();
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.Confirmation();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<ConfirmationViewModel>();
            viewModel.IsSmartMessageVisible.ShouldEqual(isSmartMessageVisible);
        }

        [TestCase(SmartMeterType.None, TariffGroup.None, false)]
        [TestCase(SmartMeterType.Smets1, TariffGroup.None, false)]
        public void ShouldDisplayCorrectSmartMessageWhenCustomerCAndCCustomerAndSmartTariffIsSelected(SmartMeterType smartType, TariffGroup selectedTariffGroup,
            bool isSmartMessageVisible)
        {
            // Arrange
            EnergyCustomer customer = CustomerFactory.GetCustomerWithFullDetails(FuelType.Dual, ElectricityMeterType.Standard);
            customer.SelectedTariff.TariffGroup = selectedTariffGroup;
            customer.MeterDetail.MeterInformation[0].SmartType = smartType;
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.Confirmation();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<ConfirmationViewModel>();
            viewModel.IsSmartMessageVisible.ShouldEqual(isSmartMessageVisible);
        }

        [TestCase(FuelType.Dual, ElectricityMeterType.Standard, "WHSmith", true, "Email should be sent when membershipid is present in session")]
        public async Task ShouldSendMembershipEmailWhenMembershipIdIsPresentInSessionInBundleJourney(FuelType fuelType, ElectricityMeterType meterType, string membership, bool shouldSendEmail, string description)
        {
            // Arrange
            var cookieCollection = new HttpCookieCollection
            {
                new HttpCookie("migrateMemberid", membership), new HttpCookie("migrateAffiliateid", "affiliateCode1"),
                new HttpCookie("migrateCampaignid", "1410789843095")
            };
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(cookieCollection))
                .Build();
            FakeEnergySalesRepository fakeEnergyRepository = new FakeEnergySalesRepository().WithSubProducts();

            EnergyCustomer customer = CustomerFactory.GetCustomerWithFullBundleDetails(fuelType, meterType);

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.OpenReachResponse, FakeOpenReachData.GetOpenReachData());

            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, FakeProductsStub.GetTariffs("Dual", "Standard"));

            var fakeSalesRepository = new FakeSalesRepository();
            var fakeBroadbandRepository = new FakeBroadbandSalesRepository();
            var fakeEmailManager = new FakeEmailManager();

            var controller = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager)
                .WithFakeSalesRepository(fakeSalesRepository)
                .WithFakeEnergySalesRepository(fakeEnergyRepository)
                .WithBroadbandSalesRepository(fakeBroadbandRepository)
                .WithEmailManager(fakeEmailManager)
                .Build<SignUpController>();

            var summaryViewModel = new SummaryViewModel();

            // Act
            await controller.ViewSummary(summaryViewModel);

            // Assert
            fakeEmailManager.Subject.ShouldContain("Membership sign up details");
            fakeEmailManager.Body.ShouldContain(membership);
        }

        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        [TestCase(true, false, false)]
        [TestCase(true, true, false)]
        public void ShouldDisplayCorrectSmartMessageWhenCustomerNonCAndCCustomerAndSmartTariffIsSelected(bool isSmartCustomer, bool isSmartTariff, bool isSmartMessageVisible)
        {
            // Arrange
            EnergyCustomer customer = CustomerFactory.GetCustomerWithFullDetails(FuelType.Dual, ElectricityMeterType.Standard);
            customer.SelectedTariff.IsSmartTariff = isSmartTariff;
            customer.HasSmartMeter = isSmartCustomer;
            customer.MeterDetail = FakeCAndCData.GetMeterDetailsWithElecNonPayGoNonSmart();
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.Confirmation();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<ConfirmationViewModel>();
            viewModel.IsSmartMessageVisible.ShouldEqual(isSmartMessageVisible);
        }

        [TestCase(SmartMeterType.None, true, true)]
        [TestCase(SmartMeterType.None, false, false)]
        [TestCase(SmartMeterType.Smets1, false, false)]
        [TestCase(SmartMeterType.Smets2, true, false)]
        public void ShouldDisplayCorrectSmartMessageWhenCustomerCAndCCustomerAndSmartTariffIsSelected(SmartMeterType smartType, bool isSmartTariff, bool isSmartMessageVisible)
        {
            // Arrange
            EnergyCustomer customer = CustomerFactory.GetCustomerWithFullDetails(FuelType.Dual, ElectricityMeterType.Standard);
            customer.SelectedTariff.IsSmartTariff = isSmartTariff;
            customer.MeterDetail.MeterInformation[0].SmartType = smartType;
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, new List<Tariff>());
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpController>();

            // Act
            ActionResult result = controller.Confirmation();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<ConfirmationViewModel>();
            viewModel.IsSmartMessageVisible.ShouldEqual(isSmartMessageVisible);
        }

        private static IEnumerable<TestCaseData> GetInvalidEmailAddresses()
        {
            yield return new TestCaseData("", "test@test.com", new List<string> { Form_Resources.EmailRequiredError, Form_Resources.EmailComparisonError }, 2);
            yield return new TestCaseData("test@test.com", "", new List<string> { Form_Resources.ConfirmEmailRequiredError }, 1);
            yield return new TestCaseData("test@test.com", "test1@test.com", new List<string> { Form_Resources.EmailComparisonError }, 1);
            yield return new TestCaseData("a@.com", "a@.com", new List<string> { "Please enter a valid email address.", "Please enter a valid email address." }, 2);
            yield return new TestCaseData("123456789012345678901234@123456789012345678901234.com", "123456789012345678901234@123456789012345678901234.com", new List<string> { "Please enter a valid email address.", "Please enter a valid email address." }, 2);
        }

        private static IEnumerable<TestCaseData> ValidPersonalDetails()
        {
            DateTime dobForNonScottishPostcodes = DateTime.Today.AddYears(-18);
            DateTime dobForScottishPostcodes = DateTime.Today.AddYears(-16);
            yield return new TestCaseData(new PersonalDetailsViewModel
            {
                Titles = Titles.Dr,
                FirstName = "Test",
                LastName = "Test",
                DateOfBirth = dobForNonScottishPostcodes.ToShortDateString(),
                DateOfBirthDay = dobForNonScottishPostcodes.Day.ToString("00"),
                DateOfBirthMonth = dobForNonScottishPostcodes.Month.ToString("00"),
                DateOfBirthYear = dobForNonScottishPostcodes.Year.ToString(),
                IsScottishPostcode = false
            }, "RG1 1AA", "Non Scottish postcodes");
            yield return new TestCaseData(new PersonalDetailsViewModel
            {
                Titles = Titles.Dr,
                FirstName = "Test",
                LastName = "Test",
                DateOfBirth = dobForScottishPostcodes.ToShortDateString(),
                DateOfBirthDay = dobForScottishPostcodes.Day.ToString("00"),
                DateOfBirthMonth = dobForScottishPostcodes.Month.ToString("00"),
                DateOfBirthYear = dobForScottishPostcodes.Year.ToString(),
                IsScottishPostcode = true
            }, "G1 1AA", "Scottish postcodes");
        }

        private static IEnumerable<TestCaseData> PersonalDetailsTestCases()
        {
            yield return new TestCaseData(null, "Test", "Test", "01", "03", "1990", "01/03/1990", Form_Resources.TitlesError);
            yield return new TestCaseData(Titles.Dr, null, "Test", "01", "03", "1990", "01/03/1990", Form_Resources.FirstNameRequiredError);
            yield return new TestCaseData(Titles.Dr, "test123!", "Test", "01", "03", "1990", "01/03/1990", Form_Resources.FirstNameRegExError);
            yield return new TestCaseData(Titles.Dr, "Test", null, "01", "03", "1990", "01/03/1990", Form_Resources.LastNameRequiredError);
            yield return new TestCaseData(Titles.Dr, "Test", "Test!!qw", "01", "03", "1990", "01/03/1990", Form_Resources.LastNameRegExError);
            yield return new TestCaseData(Titles.Dr, "Test", "Test", "01", "03", "1990", "", Form_Resources.DateOfBirthRequiredError);
        }

        private static IEnumerable<TestCaseData> FakeConfigTestData()
        {
            var fakeConfigManager = new FakeConfigManager();
            yield return new TestCaseData(fakeConfigManager);
            fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("tariffManagement", "tariffGroups", "MG101", "FixAndFibre");
            fakeConfigManager.AddConfiguration("tariffManagement", "tariffGroups", "ME724", "FixAndFibre");
            fakeConfigManager.AddConfiguration("tariffManagement", "availableTariffPdfs", "1 Year Fix and Fibre v2", "Energy1.pdf|Broadband1.pdf");
            fakeConfigManager.AddConfiguration("tariffManagement", "availableTariffPdfsAltText", "Energy1.pdf", "Energy Alt Text");
            fakeConfigManager.AddConfiguration("tariffManagement", "availableTariffPdfsAltText", "Broadband1.pdf", "Broadband1 Alt Text");
            yield return new TestCaseData(fakeConfigManager);
        }
		
        private static IEnumerable<TestCaseData> UnderAgeTestData()
        {
            yield return new TestCaseData(DateTime.Today.AddYears(-18).AddDays(1), "RG1 3AB", false, "Non Scottish postcodes");
            yield return new TestCaseData(DateTime.Today.AddYears(-16).AddDays(1), "AB11 6BR", true, "Scottish postcodes");
        }

        private static IEnumerable<TestCaseData> InvalidDateOfBirth()
        {
            yield return new TestCaseData("99", "01", "1990", "99/01/1990");
            yield return new TestCaseData("01", "99", "2010", "01/99/2010");
            yield return new TestCaseData("29", "02", "1997", "29/02/1997");
            yield return new TestCaseData("28", "02", "97", "28/02/97");
        }

        private static IEnumerable<TestCaseData> GetInvalidBankDetailsViewModel()
        {
            yield return new TestCaseData(new BankDetailsViewModel
            {
                IsAuthorisedChecked = false,
                AccountHolder = "Test",
                AccountNumber = "12345678",
                SortCode = "102030",
                SortCodeSegmentOne = "10",
                SortCodeSegmentTwo = "20",
                SortCodeSegmentThree = "30",
                DirectDebitDate = "1"
            }, "Please select this box to confirm this statement is correct or contact us to complete your order.");

            yield return new TestCaseData(new BankDetailsViewModel
            {
                IsAuthorisedChecked = true,
                AccountNumber = "12345678",
                SortCode = "102030",
                SortCodeSegmentOne = "10",
                SortCodeSegmentTwo = "20",
                SortCodeSegmentThree = "30",
                DirectDebitDate = "1"
            }, "Please enter your account holder name.");

            yield return new TestCaseData(new BankDetailsViewModel
            {
                IsAuthorisedChecked = true,
                AccountHolder = "LongAccountHolderName",
                AccountNumber = "12345678",
                SortCode = "102030",
                SortCodeSegmentOne = "10",
                SortCodeSegmentTwo = "20",
                SortCodeSegmentThree = "30",
                DirectDebitDate = "1"
            },
                "The account holder name can only include letters, hyphens, and apostrophes, and must be less than 18 characters long.");

            yield return new TestCaseData(new BankDetailsViewModel
            {
                IsAuthorisedChecked = true,
                AccountHolder = "Test",
                AccountNumber = "12345678",
                SortCodeSegmentOne = "10",
                SortCodeSegmentTwo = "20",
                SortCodeSegmentThree = "30",
                DirectDebitDate = "1"
            }, "Please enter your sort code.");

            yield return new TestCaseData(new BankDetailsViewModel
            {
                IsAuthorisedChecked = true,
                AccountHolder = "Test",
                AccountNumber = "12345678",
                SortCode = "10203012",
                SortCodeSegmentOne = "10",
                SortCodeSegmentTwo = "20",
                SortCodeSegmentThree = "30",
                DirectDebitDate = "1"
            }, "The sort code must only contain numbers, in three sets of two digits.");

            yield return new TestCaseData(new BankDetailsViewModel
            {
                IsAuthorisedChecked = true,
                AccountHolder = "Test",
                SortCode = "102030",
                SortCodeSegmentOne = "10",
                SortCodeSegmentTwo = "20",
                SortCodeSegmentThree = "30",
                DirectDebitDate = "1"
            }, "Please enter your account number.");

            yield return new TestCaseData(new BankDetailsViewModel
            {
                IsAuthorisedChecked = true,
                AccountHolder = "Test",
                AccountNumber = "1234567812",
                SortCode = "102030",
                SortCodeSegmentOne = "10",
                SortCodeSegmentTwo = "20",
                SortCodeSegmentThree = "30",
                DirectDebitDate = "1"
            },
                "The account number must contain eight digits. For seven digit account numbers, add a zero before the first number. For account numbers over eight digits, ask your bank which digits to use.");

            yield return new TestCaseData(new BankDetailsViewModel
            {
                IsAuthorisedChecked = true,
                AccountHolder = "Test",
                AccountNumber = "12345678",
                SortCode = "102030",
                SortCodeSegmentOne = "10",
                SortCodeSegmentTwo = "20",
                SortCodeSegmentThree = "30"
            }, "Please enter a payment date.");

            yield return new TestCaseData(new BankDetailsViewModel
            {
                IsAuthorisedChecked = true,
                AccountHolder = "Test",
                AccountNumber = "12345678",
                SortCode = "102030",
                SortCodeSegmentOne = "10",
                SortCodeSegmentTwo = "20",
                SortCodeSegmentThree = "30",
                DirectDebitDate = "31"
            }, "Please enter a payment date of up to two digits, from 1 to 28.");
        }

        private static IEnumerable<TestCaseData> GetInvalidContactNumbers()
        {
            yield return new TestCaseData("", Form_Resources.PreferredPhoneNumberRequiredError);
            yield return new TestCaseData("012345", Form_Resources.PreferredPhoneNumberRegexErrorMessage);
            yield return new TestCaseData("0123456789012345", Form_Resources.PreferredPhoneNumberRegexErrorMessage);
        }
    }
}