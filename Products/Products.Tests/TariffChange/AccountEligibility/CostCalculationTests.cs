namespace Products.Tests.TariffChange.AccountEligibility
{
    using System;
    using System.Globalization;
    using System.Web.Mvc;
    using Common.Fakes;
    using Common.Helpers;
    using ControllerHelpers;
    using Energy.Helpers;
    using Fakes.Models;
    using Fakes.Services;
    using Helpers;
    using Model.Enums;
    using Model.TariffChange;
    using Model.TariffChange.Customers;
    using Model.TariffChange.Tariffs;
    using NUnit.Framework;
    using Should;
    using Web.Areas.TariffChange.Controllers;
    using WebModel.ViewModels.TariffChange;
    using ControllerFactory = Helpers.ControllerFactory;

    public class CostCalculationTests
    {
        [TestCase(1000, 366)]
        [TestCase(1000, 365)]
        [TestCase(1000, 364)]
        public void EPPCostShouldNotBeCalculatedWhenValueReturnedFromAERIsGreaterThanZero(double cost, int daysLeft)
        {
            // Arrange
            var fakeAerData = new FakeAERData
            {
                AnnualCost = cost,
                AnnualUsageKwh = 500,
                EndDate = DateTime.Today.AddDays(daysLeft)
            };
            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(fakeAerData.CustomerAccountNumber, new[] { fakeAerData });
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            var fakeTariffRepository = new FakeTariffRepository(GetFakeTariffData());
            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount customerAccount = GetCustomerAccount();

            customer.AddCustomerAccount(customerAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(
                new JourneyDetails
                {
                    Customer = customer,
                    CustomerAccount = customerAccount
                });
            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AtlasAnnouncementDate", "30/04/2018 9:00");

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithTariffRepository(fakeTariffRepository)
                .WithConfigManager(fakeConfigManager)
                .WithContentManagementControllerHelper(contentManagementControllerHelper);

            var accountController = controllerFactory.Build<AccountEligibilityController>();

            var tariffsController = controllerFactory.Build<TariffsController>();

            // Act
            ActionResult accountResult = accountController.CheckEligibility();
            ActionResult tariffResult = tariffsController.AvailableTariffs().Result;

            // Assert
            var redirectResult = accountResult.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("AvailableTariffs");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.FalloutReasons.ShouldBeEmpty();

            var viewResult = tariffResult.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<TariffsViewModel>();
            double annualCost = double.Parse(model.CurrentTariffViewModel.AnnualCost, NumberStyles.Currency);
            annualCost.ShouldEqual(cost);
        }

        [Test]
        public void EPPCostShouldBeCalculatedWhenNotReturnedFromAERAndCustomerOnStandardTariff()
        {
            // Arrange
            var fakeAerData = new FakeAERData
            {
                AnnualCost = 0,
                AnnualUsageKwh = 500,
                EndDate = new DateTime()
            };
            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(fakeAerData.CustomerAccountNumber, new[] { fakeAerData });
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            var fakeTariffRepository = new FakeTariffRepository(GetFakeTariffData());
            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount customerAccount = GetCustomerAccount();
            customerAccount.CurrentTariff.Name = "Standard";

            customer.AddCustomerAccount(customerAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(
                new JourneyDetails
                {
                    Customer = customer,
                    CustomerAccount = customerAccount
                });
            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AtlasAnnouncementDate", "30/04/2018 9:00");

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithTariffRepository(fakeTariffRepository)
                .WithConfigManager(fakeConfigManager)
                .WithContentManagementControllerHelper(contentManagementControllerHelper);

            var accountController = controllerFactory.Build<AccountEligibilityController>();

            var tariffsController = controllerFactory.Build<TariffsController>();

            // Act
            ActionResult accountResult = accountController.CheckEligibility();
            ActionResult tariffResult = tariffsController.AvailableTariffs().Result;

            // Assert
            var redirectResult = accountResult.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("AvailableTariffs");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.FalloutReasons.ShouldBeEmpty();

            var viewResult = tariffResult.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<TariffsViewModel>();
            double annualCost = double.Parse(model.CurrentTariffViewModel.AnnualCost, NumberStyles.Currency);
            double monthlyCost = double.Parse(model.CurrentTariffViewModel.MonthlyCost, NumberStyles.Currency);
            annualCost.ShouldEqual(129.15); // (500 * 0.10 * 1.05) + (365 * 0.20 * 1.05)
            monthlyCost.ShouldEqual(10.76); // annualCost / 12
        }

        [TestCase(0, 365)]
        [TestCase(0, 366)]
        public void EPPCostShouldBeCalculatedWhenNotReturnedFromAERAndCustomerHasMoreThanAYearLeftOnTheCurrentFixedTariff(double cost, int daysLeft)
        {
            // Arrange
            var fakeAerData = new FakeAERData
            {
                AnnualCost = cost,
                EndDate = DateTime.Today.AddDays(daysLeft),
                AnnualUsageKwh = 500
            };
            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(fakeAerData.CustomerAccountNumber, new[] { fakeAerData });
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            var fakeTariffRepository = new FakeTariffRepository(GetFakeTariffData());
            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount customerAccount = GetCustomerAccount();

            customer.AddCustomerAccount(customerAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(
                new JourneyDetails
                {
                    Customer = customer,
                    CustomerAccount = customerAccount
                });
            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AtlasAnnouncementDate", "30/04/2018 9:00");

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithTariffRepository(fakeTariffRepository)
                .WithConfigManager(fakeConfigManager)
                .WithContentManagementControllerHelper(contentManagementControllerHelper);

            var accountController = controllerFactory.Build<AccountEligibilityController>();

            var tariffsController = controllerFactory.Build<TariffsController>();

            // Act
            ActionResult accountResult = accountController.CheckEligibility();
            ActionResult tariffResult = tariffsController.AvailableTariffs().Result;

            // Assert
            var redirectResult = accountResult.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("AvailableTariffs");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.FalloutReasons.ShouldBeEmpty();

            var viewResult = tariffResult.ShouldBeType<ViewResult>();
            var model = viewResult.Model.ShouldBeType<TariffsViewModel>();
            double annualCost = double.Parse(model.CurrentTariffViewModel.AnnualCost, NumberStyles.Currency);
            double monthlyCost = double.Parse(model.CurrentTariffViewModel.MonthlyCost, NumberStyles.Currency);
            annualCost.ShouldEqual(129.15); // (500 * 0.10 * 1.05) + (365 * 0.20 * 1.05)
            monthlyCost.ShouldEqual(10.76); // annualCost / 12
        }

        private static FakeTariffData[] GetFakeTariffData()
        {
            return new[]
            {
                new FakeTariffData
                {
                    Name = "Tariff 1",
                    FuelType = FuelType.Electricity,
                    RateCode = 4,
                    ServicePlanId = "ME124"
                },
                new FakeTariffData
                {
                    Name = "Tariff 2",
                    FuelType = FuelType.Electricity,
                    RateCode = 4,
                    ServicePlanId = "ME125"
                },
                new FakeTariffData
                {
                    Name = "Standard",
                    FuelType = FuelType.Electricity,
                    RateCode = 4,
                    ServicePlanId = "ABC123"
                },
                new FakeTariffData
                {
                    Name = "Tariff 1",
                    FuelType = FuelType.Gas,
                    RateCode = 4,
                    ServicePlanId = "MG124"
                },
                new FakeTariffData
                {
                    Name = "Tariff 2",
                    FuelType = FuelType.Gas,
                    RateCode = 4,
                    ServicePlanId = "MG125"
                },
                new FakeTariffData
                {
                    Name = "Standard",
                    FuelType = FuelType.Gas,
                    RateCode = 4,
                    ServicePlanId = "ABC123"
                }
            };
        }

        private static CustomerAccount GetCustomerAccount()
        {
            return new CustomerAccount
            {
                SiteDetails = new SiteDetails
                {
                    AccountNumber = "1111111113",
                    HasSingleActiveEnergyServiceAccount = true,
                    MeterRegisterCount = 1
                },
                PaymentDetails = new PaymentDetails
                {
                    HasDirectDebitDiscount = true,
                    IsDirectDebit = true,
                    IsPaperless = true
                },
                CurrentTariff = new CurrentTariffForFuel
                {
                    AnnualUsageKwh = 1000,
                    BrandCode = "BrandCode",
                    EndDate = DateTime.Today.AddDays(1),
                    FuelType = FuelType.Electricity,
                    Name = "Tariff 1"
                }
            };
        }
    }
}