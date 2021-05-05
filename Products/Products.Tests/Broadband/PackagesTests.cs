namespace Products.Tests.Broadband
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Common.Fakes;
    using Common.Helpers;
    using Core;
    using Fakes.Services;
    using Helpers;
    using Model;
    using NUnit.Framework;
    using Products.Model.Broadband;
    using Products.Model.Common;
    using Should;
    using Web.Areas.Broadband.Controllers;
    using WebModel.ViewModels.Broadband;
    using FakeSessionManager = Fakes.Services.FakeSessionManager;
    using Tariff = ServiceWrapper.BroadbandProductsService.Tariff;

    [TestFixture]
    public class PackagesTests
    {
        [TestCase(true, "If SSE Customer display connection fee with strike through")]
        [TestCase(false, "If not SSE customer display connection fee")]
        public async Task ShouldDisplayConnectionFeeAndSurchargeWithStrikeThroughForSSECustomer(bool isSSECustomer, string description)
        {
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = isSSECustomer } },
                ListOfProductsSessionObject = new List<BroadbandProduct>()
            };

            var model = new SelectAddressViewModel { SelectedAddressId = 0 };

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithSessionManager(fakeSessionManager);

            var lineCheckerController = controllerFactory.Build<LineCheckerController>();
            lineCheckerController.TempData["AddressList"] = GetListOfAddresses();

            var packageController = controllerFactory.Build<PackagesController>();

            await lineCheckerController.SelectAddress(model);
            packageController.AvailablePackages();

            packageController.AvailablePackage("", "FIBRE_ANY19");

            ActionResult result = packageController.SelectedPackage();

            var view = result.ShouldBeType<ViewResult>();
            var viewModel = view.ViewData.Model.ShouldBeType<SelectedPackageViewModel>();
            viewModel.YourPriceViewModel.IsExistingCustomer.ShouldEqual(isSSECustomer);
        }

        [TestCaseSource(nameof(FakeTermsAndConditionsPdfData))]
        public void ShouldOnlyPopulateTermsAndConditionsPdfLinkListWhenSelectedProductHasSpecificPdfs(string selectedProductCode, BroadbandProductGroup selectedBroadbandProductGroup, int expectedPdfCount, List<string> expectedTAndCsPdfList)
        {
            BroadbandJourneyDetails broadbandJourneyDetails = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetailsForBroadbandProductGroup(selectedBroadbandProductGroup);
            broadbandJourneyDetails.Customer.SelectedProductCode = selectedProductCode;
            broadbandJourneyDetails.Customer.SelectedProductGroup = selectedBroadbandProductGroup;

            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = broadbandJourneyDetails,
                ListOfProductsSessionObject = new List<BroadbandProduct>()
            };
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<PackagesController>();

            ActionResult result = controller.SelectedPackage();

            var view = result.ShouldBeType<ViewResult>();
            var viewModel = view.ViewData.Model.ShouldBeType<SelectedPackageViewModel>();
            viewModel.TermsAndConditionsPdfLinks.Count.ShouldEqual(expectedPdfCount);
            viewModel.TermsAndConditionsPdfLinks.Select(x => x.DisplayName).ShouldEqual(expectedTAndCsPdfList);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ShouldDisplayInstallationFeeWhenApplicable(bool installRequired)
        {
            // Arrange
            BroadbandJourneyDetails broadbandJourneyDetails = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails();
            broadbandJourneyDetails.Customer.ApplyInstallationFee = installRequired;
            var fakeSessionManager = new FakeSessionManager { SessionObject = broadbandJourneyDetails };

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<PackagesController>();

            // Act
            ActionResult result = controller.SelectedPackage();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SelectedPackageViewModel>();
            model.YourPriceViewModel.ApplyInstallationFee.ShouldEqual(installRequired);
        }

        [TestCase(true, "£110.00")]
        [TestCase(false, "£50.00")]
        public void ShouldCalculateOneOffCostIncludingInstallationFeeWhenApplicable(bool installRequired, string oneOffCost)
        {
            // Arrange
            BroadbandJourneyDetails broadbandJourneyDetails = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails();
            broadbandJourneyDetails.Customer.ApplyInstallationFee = installRequired;
            broadbandJourneyDetails.Customer.IsSSECustomer = false;
            var fakeSessionManager = new FakeSessionManager { SessionObject = broadbandJourneyDetails };
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<PackagesController>();

            // Act
            ActionResult result = controller.SelectedPackage();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SelectedPackageViewModel>();
            model.YourPriceViewModel.OneOffCost.ToString("C").ShouldEqual(oneOffCost);
        }

        [TestCase(null, "TransferYourNumber", "0123456789")]
        [TestCase(null, "TransferYourNumber", null)]
        [TestCase("0234567879", "TransferYourNumber", "0123456789")]
        public async Task ShouldRedirectToExpectedPageBasedOnOpenReachCLIValueAndUpdateCustomerCLIBasedOnOpenReachCLI(string phoneNumber, string expectedPage,
            string openReachCLI)
        {
            // Arrange
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest())
                .Build();

            var fakeSessionManager = new FakeSessionManager { SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } } };

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager)
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses })
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { CLI = openReachCLI });

            var lineCheckerController = controllerFactory.Build<LineCheckerController>();

            var controller = controllerFactory.Build<PackagesController>();

            // Act
            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "P091BH",
                PhoneNumber = phoneNumber
            };

            await lineCheckerController.Submit(lineCheckerViewModel);
            ActionResult resultSelectAddressGet = await lineCheckerController.SelectAddress();

            var viewResult = resultSelectAddressGet.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();

            // ReSharper disable once PossibleNullReferenceException
            viewModel.SelectedAddressId = viewModel.Addresses.FirstOrDefault().Id;
            await lineCheckerController.SelectAddress(viewModel);
            ActionResult result = controller.SelectedPackage(new SelectedPackageViewModel(), "FIBRE_ANY19");
            var broadbandJourneyDetails = fakeSessionManager.GetSessionDetails<BroadbandJourneyDetails>("broadband_journey");

            // Assert
            result.ShouldNotBeNull();
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual(expectedPage);

            broadbandJourneyDetails.Customer.CliNumber.ShouldEqual(openReachCLI);
        }

        [TestCase("FF3_ANY18", BroadbandProductGroup.FixAndFibreV3, 0, 4, 0)]
        [TestCase("FF3_AP18", BroadbandProductGroup.FixAndFibreV3, 0, 4, 0)]
        [TestCase("FF3_LR18", BroadbandProductGroup.FixAndFibreV3, 0, 4, 0)]
        [TestCase("FF3_EAW18", BroadbandProductGroup.FixAndFibreV3, 0, 4, 0)]
        [TestCase("BB18_LR_FF", BroadbandProductGroup.NotAvailableOnline, 0, 0, 0)]
        [TestCase("BB_AP18_FF", BroadbandProductGroup.NotAvailableOnline, 0, 0, 0)]
        [TestCase("BB_ANY18_FF", BroadbandProductGroup.NotAvailableOnline, 0, 0, 0)]
        [TestCase("BB_EAW18_FF", BroadbandProductGroup.NotAvailableOnline, 0, 0, 0)]
        [TestCase("ADSLFF3_LR18", BroadbandProductGroup.NotAvailableOnline, 0, 0, 0)]
        [TestCase("ADSLFF3_AP18", BroadbandProductGroup.NotAvailableOnline, 0, 0, 0)]
        [TestCase("ADSLFF3_ANY18", BroadbandProductGroup.NotAvailableOnline, 0, 0, 0)]
        [TestCase("ADSLFF3_EAW18", BroadbandProductGroup.NotAvailableOnline, 0, 0, 0)]
        public async Task ShouldContainValidProductsInSession(string productCode, BroadbandProductGroup expectedBroadbandProductGroup, int expectedADSLCount, int expectedFibreCount, int expectedFibrePlusCount)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails
                {
                    Customer =
                        new Customer
                        {
                            IsSSECustomer = false,
                            SelectedProductGroup = expectedBroadbandProductGroup,
                            SelectedProductCode = productCode
                        }
                }
            };

            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest())
                .Build();

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithContextManager(fakeContextManager)
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses });

            var lineCheckerController = controllerFactory.Build<LineCheckerController>();

            controllerFactory.Build<PackagesController>();

            // Act
            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "P091BH",
                PhoneNumber = "12345678",
                ProductCode = productCode
            };

            await lineCheckerController.Submit(lineCheckerViewModel);
            ActionResult resultSelectAddressGet = await lineCheckerController.SelectAddress();

            var viewResult = resultSelectAddressGet.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();

            // ReSharper disable once PossibleNullReferenceException
            viewModel.SelectedAddressId = viewModel.Addresses.FirstOrDefault().Id;
            await lineCheckerController.SelectAddress(viewModel);
            var broadbandProducts = fakeSessionManager.GetSessionDetails<List<BroadbandProduct>>("broadbandProducts");

            // ReSharper disable once PossibleNullReferenceException
            List<TalkProduct> fibreProduct = broadbandProducts.FirstOrDefault(b => b.BroadbandType == BroadbandType.Fibre).TalkProducts;
            // ReSharper disable once PossibleNullReferenceException
            BroadbandProduct adslProduct = broadbandProducts.FirstOrDefault(b => b.BroadbandType == BroadbandType.ADSL);

            var asdlTalkProducts = new List<TalkProduct>();

            if (adslProduct != null)
            {
                asdlTalkProducts = adslProduct.TalkProducts;
            }

            // ReSharper disable once PossibleNullReferenceException
            List<TalkProduct> fibrePlusProduct = broadbandProducts.FirstOrDefault(b => b.BroadbandType == BroadbandType.FibrePlus).TalkProducts;

            asdlTalkProducts.Count.ShouldEqual(expectedADSLCount);
            fibreProduct.Count.ShouldEqual(expectedFibreCount);
            fibrePlusProduct.Count.ShouldEqual(expectedFibrePlusCount);

            asdlTalkProducts.All(x => x.BroadbandProductGroup == expectedBroadbandProductGroup).ShouldEqual(true);
            fibreProduct.All(x => x.BroadbandProductGroup == expectedBroadbandProductGroup).ShouldEqual(true);
            fibrePlusProduct.All(x => x.BroadbandProductGroup == expectedBroadbandProductGroup).ShouldEqual(true);
        }

        private static List<AddressViewModel> GetListOfAddresses()
        {
            var address1 = new AddressViewModel
            {
                SubPremises = "1",
                PremiseName = "Steyning Terrace",
                ThoroughfareNumber = "22",
                ThoroughfareName = "Waterloo Road",
                PostTown = "Havant",
                Postcode = "PO9 1BH"
            };

            var address2 = new AddressViewModel
            {
                SubPremises = "",
                PremiseName = "Test House",
                ThoroughfareNumber = "22",
                ThoroughfareName = "Waterloo Road",
                PostTown = "Havant",
                Postcode = "PO9 1BH"
            };

            var address3 = new AddressViewModel
            {
                SubPremises = "East",
                PremiseName = "Test House",
                ThoroughfareNumber = "26",
                ThoroughfareName = "Waterloo Road",
                PostTown = "Havant",
                Postcode = "PO9 1BH"
            };

            var listOfAddresses = new List<AddressViewModel> { address1, address2, address3 };

            return listOfAddresses;
        }

        private static IEnumerable<TestCaseData> FakeTermsAndConditionsPdfData()
        {
            yield return new TestCaseData("FF3_LR18", BroadbandProductGroup.FixAndFibreV3, 2, new List<string> { "PDF A", "PDF B" });
            yield return new TestCaseData("FIBRE18_LR", BroadbandProductGroup.None, 0, new List<string>());
        }

        [TestCase("FF3_LR18", BroadbandProductGroup.FixAndFibreV3, "SelectAddress", "LineChecker")]
        [TestCase("FIBRE_ANY18", BroadbandProductGroup.None, "AvailablePackages", "Packages")]
        public void BackChevronShouldGoToAppropriatePage(string productCode, BroadbandProductGroup broadbandProductGroup, string expectedActionName, string expectedControllerName)
        {
            // Arrange
            BroadbandJourneyDetails broadbandJourneyDetails = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetailsForBroadbandProductGroup(broadbandProductGroup);
            broadbandJourneyDetails.Customer.SelectedProductGroup = broadbandProductGroup;
            broadbandJourneyDetails.Customer.SelectedProductCode = productCode;
            var fakeSessionManager = new FakeSessionManager { SessionObject = broadbandJourneyDetails };
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .Build<PackagesController>();

            // Act
            ActionResult result = controller.SelectedPackage();

            // Assert
            var viewResult = result.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<SelectedPackageViewModel>();
            model.BackChevronViewModel.ActionName.ShouldEqual(expectedActionName);
            model.BackChevronViewModel.ControllerName.ShouldEqual(expectedControllerName);
        }

        [TestCase("6000", "6", "37500", "37", "37")]
        [TestCase("6000", "6", "38500", "38", "38")]
        [TestCase("6000", "6", "39500", "38", "39")]
        [TestCase("6000", "6", "75500", "38", "75")]
        [TestCase("6000", "6", "76500", "38", "76")]
        [TestCase("6000", "6", "77000", "38", "76")]
        [TestCase("6000", "6", "63600", "38", "63")]
        [TestCase("6000", "6", "80600", "38", "76")]
        [TestCase("10000", "10", "83600", "38", "76")]
        public async Task ShouldDisplayCorrectLineSpeedPerProductIncludingCappedValue(string adslMaxSpeed, string adslFormattedSpeed, string fibreMaxSpeed, string fibreFormattedSpeed, string fibrePlusFormattedSpeed)
        {
            var model = new SelectAddressViewModel { SelectedAddressId = 0 };
            var fakeSessionManager = new FakeSessionManager { SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }, ListOfProductsSessionObject = new List<BroadbandProduct>() };
            var fakeLineChecker = new FakeBroadbandProductsServiceWrapper
            {
                FakeLineSpeed = new FakeBroadbandProductsData.FakeLineSpeed
                {
                    ADSLMaxSpeed = adslMaxSpeed,
                    ADSLMinSpeed = "3000",
                    FibreMinDownloadSpeed = "15200",
                    FibreMaxDownloadSpeed = fibreMaxSpeed,
                    FibreMinUploadSpeed = "39800",
                    FibreMaxUploadSpeed = "20000"
                }
            };

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBroadbandProductsServiceWrapper(fakeLineChecker);

            var lineCheckerController = controllerFactory.Build<LineCheckerController>();
            lineCheckerController.TempData["AddressList"] = GetListOfAddresses();

            var packageController = controllerFactory.Build<PackagesController>();

            await lineCheckerController.SelectAddress(model);
            ActionResult result = packageController.AvailablePackages();

            var view = (ViewResult) result;

            // Prices
            var viewModel = view.ViewData.Model.ShouldBeType<AvailablePackagesViewModel>();

            AvailableProductViewModel adslProduct = viewModel.Products.FirstOrDefault(p => p.BroadbandType == BroadbandType.ADSL);
            AvailableProductViewModel fibreProduct = viewModel.Products.FirstOrDefault(p => p.BroadbandType == BroadbandType.Fibre);
            AvailableProductViewModel fibrePlusProduct = viewModel.Products.FirstOrDefault(p => p.BroadbandType == BroadbandType.FibrePlus);

            // ADSL Speeds
            adslProduct?.FormattedLineSpeed.ShouldEqual(adslFormattedSpeed);
            // Fibre Speeds
            fibreProduct?.FormattedLineSpeed.ShouldEqual(fibreFormattedSpeed);
            // Fibre Plus Speeds
            fibrePlusProduct?.FormattedLineSpeed.ShouldEqual(fibrePlusFormattedSpeed);
        }

        [Test]
        public async Task ShouldDisplayListOfProductsAfterAddressSelected()
        {
            var model = new SelectAddressViewModel { SelectedAddressId = 0 };
            var fakeSessionManager = new FakeSessionManager { SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = true } }, ListOfProductsSessionObject = new List<BroadbandProduct>() };

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithSessionManager(fakeSessionManager);

            var lineCheckerController = controllerFactory.Build<LineCheckerController>();
            lineCheckerController.TempData["AddressList"] = GetListOfAddresses();

            var packageController = controllerFactory.Build<PackagesController>();

            await lineCheckerController.SelectAddress(model);
            ActionResult result = packageController.AvailablePackages();

            var view = result.ShouldBeType<ViewResult>();

            // Prices
            var viewModel = view.ViewData.Model.ShouldBeType<AvailablePackagesViewModel>();

            AvailableProductViewModel adslProduct = viewModel.Products.FirstOrDefault(p => p.BroadbandType == BroadbandType.ADSL);
            AvailableProductViewModel fibreProduct = viewModel.Products.FirstOrDefault(p => p.BroadbandType == BroadbandType.Fibre);
            AvailableProductViewModel fibrePlusProduct = viewModel.Products.FirstOrDefault(p => p.BroadbandType == BroadbandType.FibrePlus);

            Tariff[] brandTariffs = FakeBroadbandProductsData.GetBroadbandProducts(BroadbandProductsResult.ShowProducts, null).BroadbandProducts.Broadband.Brand.Tariffs;
            adslProduct?.FormattedPriceFullValue.ShouldEqual(brandTariffs.FirstOrDefault(p => p.BroadbandCode == "UB19" && p.TalkCode == "LRO")?.PriceLines.FirstOrDefault(t => t.FeatureCode == FeatureCodes.HeadlinePricePaperlessBilling)?.Rate.ToString("C0"));
            fibreProduct?.FormattedPriceFullValue.ShouldEqual(brandTariffs.FirstOrDefault(p => p.BroadbandCode == "UF19" && p.TalkCode == "LRO")?.PriceLines.FirstOrDefault(t => t.FeatureCode == FeatureCodes.HeadlinePricePaperlessBilling)?.Rate.ToString("C0"));
            fibrePlusProduct?.FormattedPriceFullValue.ShouldEqual(brandTariffs.FirstOrDefault(p => p.BroadbandCode == "UFP19" && p.TalkCode == "LRO")?.PriceLines.FirstOrDefault(t => t.FeatureCode == FeatureCodes.HeadlinePricePaperlessBilling)?.Rate.ToString("C0"));
        }

        [Test]
        public void ShouldRedirectToCannotCompleteOnlineIfNoProductIsAvailable()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails
                {
                    Customer =
                        new Customer
                        {
                            IsSSECustomer = false,
                            SelectedAddress = new BTAddress { Postcode = "PO9 1QH" }
                        }
                },
                ListOfProductsSessionObject = new List<BroadbandProduct>()
            };

            fakeSessionManager.ListOfProductsSessionObject = FakeBroadbandProductsData.GetListOfAllUnavailableProducts();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<PackagesController>();

            // Act
            ActionResult result = controller.AvailablePackages();

            // Assert
            result.ShouldNotBeNull();
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("CannotCompleteOnline");
            redirectResult.RouteValues["controller"].ShouldEqual("LineChecker");
        }

        [Test]
        [TestCase("0121212121", "TransferYourNumber")]
        [TestCase(null, "TransferYourNumber")]
        public async Task ShouldRedirectToTransferYourNumberPageIfCLINumberWasNotProvided(string phoneNumber, string expectedRoute)
        {
            // Arrange
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest())
                .Build();
            var fakeSessionManager = new FakeSessionManager { SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } } };
            ControllerFactory controllerFactory = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses })
                .WithSessionManager(fakeSessionManager);

            var lineCheckerController = controllerFactory.Build<LineCheckerController>();

            var controller = controllerFactory.Build<PackagesController>();

            // Act
            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "P091BH",
                PhoneNumber = phoneNumber
            };

            await lineCheckerController.Submit(lineCheckerViewModel);
            ActionResult resultSelectAddressGet = await lineCheckerController.SelectAddress();

            var viewResult = resultSelectAddressGet.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();

            // ReSharper disable once PossibleNullReferenceException
            viewModel.SelectedAddressId = viewModel.Addresses.FirstOrDefault().Id;
            await lineCheckerController.SelectAddress(viewModel);
            ActionResult result = controller.SelectedPackage(new SelectedPackageViewModel(), "FIBRE_ANY19");

            // Assert
            result.ShouldNotBeNull();
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual(expectedRoute);
        }

        [TestCase(true, "£21", "£28", "£35", "SSE energy customer exclusive price")]
        [TestCase(false, "£21", "£28", "£35", "Non-SSE energy customer price")]
        public void ShouldShowPriceWithoutSurchargeForAllCustomers(bool isSSECustomer, string adslPrice, string fibrePrice, string fibrePlusPrice, string priceLabel)
        {
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails
                {
                    Customer = new Customer { IsSSECustomer = isSSECustomer, SelectedAddress = new BTAddress { Postcode = "PO9 1QH" } }
                },
                ListOfProductsSessionObject = new List<BroadbandProduct>()
            };

            fakeSessionManager.ListOfProductsSessionObject = FakeBroadbandProductsData.GetListOfAvailableProductsFromSession();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<PackagesController>();

            ActionResult result = controller.AvailablePackages();

            var view = (ViewResult) result;

            // Prices
            var viewModel = view.ViewData.Model.ShouldBeType<AvailablePackagesViewModel>();

            AvailableProductViewModel adslProduct = viewModel.Products.FirstOrDefault(p => p.BroadbandType == BroadbandType.ADSL);
            AvailableProductViewModel fibreProduct = viewModel.Products.FirstOrDefault(p => p.BroadbandType == BroadbandType.Fibre);
            AvailableProductViewModel fibrePlusProduct = viewModel.Products.FirstOrDefault(p => p.BroadbandType == BroadbandType.FibrePlus);

            adslProduct?.FormattedPriceFullValue.ShouldEqual(adslPrice);
            fibreProduct?.FormattedPriceFullValue.ShouldEqual(fibrePrice);
            fibrePlusProduct?.FormattedPriceFullValue.ShouldEqual(fibrePlusPrice);
        }

        [Test]
        public async Task ShouldStoreSelectedProductCodeInSession()
        {
            var model = new SelectAddressViewModel { SelectedAddressId = 0 };
            var fakeSessionManager = new FakeSessionManager { SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }, ListOfProductsSessionObject = new List<BroadbandProduct>() };

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithSessionManager(fakeSessionManager);

            var lineCheckerController = controllerFactory.Build<LineCheckerController>();
            lineCheckerController.TempData["AddressList"] = GetListOfAddresses();

            var packageController = controllerFactory.Build<PackagesController>();

            await lineCheckerController.SelectAddress(model);
            packageController.AvailablePackages();

            ActionResult result = packageController.AvailablePackage("", "FIBRE_ANY19");

            // Prices
            ((RedirectToRouteResult) result).RouteValues["action"].ShouldEqual("SelectedPackage");

            var broadbandJourneyDetails = (BroadbandJourneyDetails) fakeSessionManager.SessionObject;
            broadbandJourneyDetails.Customer.SelectedProduct.TalkProducts.Any(t => t.ProductCode == "FIBRE_ANY19").ShouldBeTrue();
        }

        //[TestCase(true, 2)]
        //[TestCase(false, 2)]
        //public void ShouldShowCorrectAvailablePackagesWhenFibreIsAvailable(bool isFibreAvailable, int expectedProductCount)
        //{
        //    var fakeSessionManager = new FakeSessionManager
        //    {
        //        SessionObject = new BroadbandJourneyDetails
        //        {
        //            Customer = new Customer { IsSSECustomer = true, SelectedAddress = new BTAddress { Postcode = "PO9 1QH" } }
        //        },
        //        ListOfProductsSessionObject = new List<BroadbandProduct>()
        //    };

        //    var lineCheckerController = new ControllerFactory()
        //        .WithSessionManager(fakeSessionManager)
            
        //        .Build<LineCheckerController>();
        //    lineCheckerController.TempData["AddressList"] = GetListOfAddresses();

        //    var controller = new ControllerFactory()
        //        .WithSessionManager(fakeSessionManager)
        //        .Build<PackagesController>();

        //    ActionResult lineCActionResult = lineCheckerController.SelectAddress(new SelectAddressViewModel { SelectedAddressId = 0 }).Result;
        //    ActionResult result = controller.AvailablePackages();

        //    var view = (ViewResult)result;

        //    // Prices
        //    var viewModel = view.ViewData.Model.ShouldBeType<AvailablePackagesViewModel>();

        //    viewModel.Products.Count.ShouldEqual(expectedProductCount);
        //}
    }
}