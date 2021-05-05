namespace Products.Tests.TariffChange.Tariffs
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using Common.Fakes;
    using Common.Helpers;
    using ControllerHelpers;
    using Core;
    using Energy.Helpers;
    using Fakes.Managers;
    using Fakes.Models;
    using Fakes.Services;
    using Helpers;
    using Model.Enums;
    using Model.TariffChange;
    using Model.TariffChange.Customers;
    using Model.TariffChange.Enums;
    using Model.TariffChange.Tariffs;
    using NUnit.Framework;
    using Products.Infrastructure.Extensions;
    using Should;
    using Web.Areas.TariffChange.Controllers;
    using WebModel.Resources.TariffChange;
    using WebModel.ViewModels.TariffChange;
    using ControllerFactory = Helpers.ControllerFactory;

    public class TariffControllerTests
    {
        [TestCase(true, true, 4, new double[] { 1, 2, 3, 4 }, "When DD and paperless.")]
        [TestCase(true, false, 3, new double[] { 1, 2, 3, 4 }, "When DD and not paperless.")]
        [TestCase(false, true, 2, new double[] { 1, 2, 3, 4 }, "When not DD and paperless.")]
        [TestCase(false, false, 1, new double[] { 1, 2, 3, 4 }, "When not DD and not paperless.")]
        public void TariffsShownAreBasedOnDirectDebitOrPaperlessPreferences(bool isDirectDebit, bool isPaperless, double expectedUnitRate, double[] unitRates, string description)
        {
            // Arrange
            var fakeTariffRepository = new FakeTariffRepository(unitRates.Select((t, index) => new FakeTariffData
            {
                Name = "T1",
                FuelType = FuelType.Electricity,
                UnitRate1InclVat = t,
                StandingChargeInclVat = 1,
                RateCode = index + 1,
                ServicePlanId = "ABC124"
            }).ToArray());

            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount customerAccount = DefaultDomainModel.ForTariffs();
            customerAccount.PaymentDetails.HasDirectDebitDiscount = isDirectDebit;
            customerAccount.PaymentDetails.IsPaperless = isPaperless;
            customerAccount.CurrentTariff.Name = "T2";
            customerAccount.CurrentTariff.ServicePlanId = "ABC123";

            customer.AddCustomerAccount(customerAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = customerAccount
            });

            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithTariffRepository(fakeTariffRepository)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = (ViewResult) result;
            var viewModel = (TariffsViewModel) viewResult.Model;
            viewModel.AvailableTariffs.Count.ShouldEqual(1);
            viewModel.AvailableTariffs[0].ElectricityDetails.UnitRate1.ShouldEqual($"{expectedUnitRate}p");
        }

        [TestCase(true, "Evergreen", "Standard", true)]
        [TestCase(false, "Evergreen", "Standard", false)]
        [TestCase(true, "Fixed", "Fixed 1 year", false)]
        [TestCase(false, "Fixed", "Fixed 1 year", false)]
        public void ShouldCorrectShowWHDTextForCurrentTariff(bool hasWHDFlag, string tariffType, string name, bool expectedResult)
        {
            // Arrange
            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount account = DefaultDomainModel.ForWHDAndName(hasWHDFlag, name, tariffType);
            customer.ElectricityAccount = account;
            var sessionObject = new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = account
            };

            var fakeTariffRepository = new FakeTariffRepository(new[]
            {
                new FakeTariffData
                {
                    Name = "Standard", Type = "Evergreen", FuelType = FuelType.Electricity, StandingChargeExclVat = 20, UnitRate1ExclVat = 5,
                    ServicePlanId = "ABC125"
                },
                new FakeTariffData { Name = "1 Year Fixed", FuelType = FuelType.Gas, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "ABC123" },
                new FakeTariffData { Name = "2 Year Fixed", FuelType = FuelType.Gas, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "ABC124" },
                new FakeTariffData { Name = "2 Year Fixed", FuelType = FuelType.Electricity, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "ABC124" }
            });

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(sessionObject);

            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithTariffRepository(fakeTariffRepository)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = (ViewResult) result;
            var viewModel = (TariffsViewModel) viewResult.Model;

            viewModel.CurrentTariffViewModel.ShowWHDText.ShouldEqual(expectedResult);
        }

        [TestCase(TariffGroup.FixAndControl, "ABC123", true)]
        [TestCase(TariffGroup.FixAndDrive, "ABC124", false)]
        [TestCase(TariffGroup.FixAndFibre, "ABC1245", false)]
        [TestCase(TariffGroup.Standard, "ABC125", false)]
        public void ShouldDisplayCorrectBulletTextForCurrentTariff(TariffGroup tariffGroup, string servicePlanId, bool expectedShowFixAndControlExitFee)
        {
            // Arrange
            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount account = DefaultDomainModel.ForTariffs();
            customer.ElectricityAccount = account;
            
            var sessionObject = new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = account
            };

            var fakeTariffRepository = new FakeTariffRepository( new[]
            {
                new FakeTariffData { Name = "Standard", Type = "Evergreen", FuelType = FuelType.Electricity, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "ABC125" },
                new FakeTariffData { Name = "Fix and Control", FuelType = FuelType.Electricity, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "ABC123" },
                new FakeTariffData { Name = "Fix and Drive", FuelType = FuelType.Electricity, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "ABC124" },
                new FakeTariffData { Name = "Fix and Fibre", FuelType = FuelType.Electricity, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "ABC1245" }
            });

            var fakeTariffManager = new FakeTariffManager();
            fakeTariffManager.TariffGroupMappings.Add(servicePlanId, tariffGroup.ToString());


            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(sessionObject);

            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .WithTariffRepository(fakeTariffRepository)
                .WithTariffManager(fakeTariffManager)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = (ViewResult)result;
            var viewModel = (TariffsViewModel)viewResult.Model;
            viewModel.CurrentTariffViewModel.ShowFixAndControlExitFee.ShouldEqual(expectedShowFixAndControlExitFee);
        }

        [TestCase(TariffGroup.FixAndControl, "ABC123", 0, "<strong>Early exit fee: </strong>£75 per fuel")]
        [TestCase(TariffGroup.FixAndDrive, "ABC124", 0, "No exit fees")]
        [TestCase(TariffGroup.FixAndFibre, "ABC1245", 25.00, "£25 early exit fee per fuel")]
        public void ShouldDisplayCorrectBulletTextForFollowOnTariff(TariffGroup tariffGroup, string servicePlanId, double exitFee, string expectedBulletText)
        {
            // Arrange
            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount account = DefaultDomainModel.ForTariffs();
            account.FollowOnTariffServicePlanID = "ABC123";
            customer.ElectricityAccount = account;

            var sessionObject = new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = account
            };

            var fakeTariffRepository = new FakeTariffRepository(new[]
            {
                new FakeTariffData { Name = "Standard", Type = "Evergreen", ExitFee = exitFee, FuelType = FuelType.Electricity, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "ABC125" },
                new FakeTariffData { Name = "Fix and Control",  ExitFee = exitFee,  FuelType = FuelType.Electricity, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "ABC123" },
                new FakeTariffData { Name = "Fix and Drive",  ExitFee = exitFee,  FuelType = FuelType.Electricity, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "ABC124" },
                new FakeTariffData { Name = "Fix and Fibre",  ExitFee = exitFee,  FuelType = FuelType.Electricity, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "ABC1245" }
            });

            var fakeTariffManager = new FakeTariffManager();
            fakeTariffManager.TariffGroupMappings.Add(servicePlanId, tariffGroup.ToString());


            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(sessionObject);

            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .WithTariffRepository(fakeTariffRepository)
                .WithTariffManager(fakeTariffManager)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = (ViewResult)result;
            var viewModel = (TariffsViewModel)viewResult.Model;
            viewModel.BulletText.ShouldEqual(expectedBulletText);
        }

        [Test]
        public void ShouldCorrectShowWHDTextForStandardTariffInAvailableList()
        {
            // Arrange
            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount account = DefaultDomainModel.ForWHDAndName(true, "Standard", "Evergreen");
            customer.ElectricityAccount = account;
            var sessionObject = new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = account
            };

            var fakeTariffRepository = new FakeTariffRepository(new[]
            {
                new FakeTariffData
                {
                    Name = "Standard", Type = "Evergreen", FuelType = FuelType.Electricity, StandingChargeExclVat = 20, UnitRate1ExclVat = 5,
                    ServicePlanId = "ABC125"
                },
                new FakeTariffData { Name = "1 Year Fixed", FuelType = FuelType.Gas, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "ABC123" },
                new FakeTariffData { Name = "2 Year Fixed", FuelType = FuelType.Gas, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "ABC124" },
                new FakeTariffData { Name = "2 Year Fixed", FuelType = FuelType.Electricity, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "ABC124" }
            });

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(sessionObject);

            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithTariffRepository(fakeTariffRepository)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = (ViewResult) result;
            var viewModel = (TariffsViewModel) viewResult.Model;
            viewModel.AvailableTariffs.Count.ShouldEqual(2);
            AvailableTariff testTariff = viewModel.AvailableTariffs.First(s => s.Name == "Standard");
            testTariff.ShouldNotBeNull();
            testTariff.ShowWHDText.ShouldBeTrue();
            int whdTextCount = viewModel.AvailableTariffs.Where(s => s.ShowWHDText).ToList().Count;
            whdTextCount.ShouldEqual(1);
        }

        [TestCaseSource(nameof(CurrentTariffData)), Description("Validation of current tariff in view model")]
        public void ShouldDisplayCurrentTariffDetails(Customer customer, string expectedTariffName, string expectedAnnualCost, string expectedMonthlyCost, double expectedElectricityAnnualUsage, double expectedGasAnnualUsage, string description)
        {
            // Arrange
            var fakeTariffRepository = new FakeTariffRepository(new[]
            {
                new FakeTariffData { Name = "1 Year Fixed", FuelType = FuelType.Electricity, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "ABC123" },
                new FakeTariffData { Name = "1 Year Fixed", FuelType = FuelType.Gas, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "ABC123" },
                new FakeTariffData { Name = "2 Year Fixed", FuelType = FuelType.Gas, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "ABC124" },
                new FakeTariffData { Name = "2 Year Fixed", FuelType = FuelType.Electricity, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "ABC124" }
            });

            DateTime date = DateTime.Today.AddDays(1);

            var sessionObject = new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = customer.GasAccount ?? customer.ElectricityAccount
            };

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(sessionObject);

            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("tariffManagement", "excludedTariffs", "TariffIds", "ME672,MG095");

            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithConfigManager(fakeConfigManager)
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithTariffRepository(fakeTariffRepository)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = (ViewResult) result;
            var viewModel = (TariffsViewModel) viewResult.Model;

            viewModel.CurrentTariffViewModel.Name.ShouldEqual(expectedTariffName);
            viewModel.CurrentTariffViewModel.ExpirationMessage.ShouldNotBeEmpty();
            viewModel.CurrentTariffViewModel.ExpirationMessage.ShouldEqual($"Expires tomorrow on {date.Day}{GetSuffix(date.Day)} {date.ToString("MMMM yyyy", CultureInfo.InvariantCulture)}");
            viewModel.CurrentTariffViewModel.AnnualCost.ShouldEqual(expectedAnnualCost);
            viewModel.CurrentTariffViewModel.MonthlyCost.ShouldEqual(expectedMonthlyCost);
            if (customer.HasElectricityAccount())
            {
                viewModel.CurrentTariffViewModel.ElectricityAnnualUsage.ShouldEqual(expectedElectricityAnnualUsage);
                viewModel.CurrentTariffViewModel.ElectricityDetails.ShouldNotBeNull();
            }
            else if (customer.HasGasAccount())
            {
                viewModel.CurrentTariffViewModel.GasAnnualUsage.ShouldEqual(expectedGasAnnualUsage);
                viewModel.CurrentTariffViewModel.GasDetails.ShouldNotBeNull();
            }
        }

        [TestCase(true, false, false, 2, "MG456,ME856")]
        [TestCase(true, false, true, 2, "MG456,ME856")]
        [TestCase(true, true, false, 6, "MG095,MG456,ME856,ME789,ME672,ME808")]
        [TestCase(true, true, true, 4, "MG456,ME856,ME789,ME808")]
        [TestCase(false, true, false, 3, "ME789,ME672,ME808")]
        [TestCase(false, true, true, 2, "ME789,ME808")]
        public void ShouldDisplayFilteredAvailableTariffDetails(bool hasGas, bool hasElectric, bool hasMultiRateMeter, int expectedCount, string expectedServicePlans)
        {
            // Arrange
            var fakeTariffRepository = new FakeTariffRepository(new[]
            {
                new FakeTariffData { Name = "2 Year Fixed", FuelType = FuelType.Electricity, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "ME789" },
                new FakeTariffData { Name = "SMART Fixed (Elec)", FuelType = FuelType.Electricity, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "ME672" },
                new FakeTariffData { Name = "SMART Fixed (Gas)", FuelType = FuelType.Gas, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "MG095" },
                new FakeTariffData { Name = "1 Year Fixed (Gas)", FuelType = FuelType.Gas, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "MG456" },
                new FakeTariffData { Name = "FixAndFibre", FuelType = FuelType.Electricity, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "ME808" },
                new FakeTariffData { Name = "FixAndProtect", FuelType = FuelType.Gas, StandingChargeExclVat = 20, UnitRate1ExclVat = 5, ServicePlanId = "ME856" }
            });

            Customer customer = GetAvailableTariffsCustomer(hasGas, hasElectric, hasMultiRateMeter);

            var sessionObject = new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = customer.GasAccount ?? customer.ElectricityAccount
            };

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(sessionObject);
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("tariffManagement", "excludedTariffs", "TariffIds", "ME672,MG095");
            fakeConfigManager.AddConfiguration("MultiRateTariffs", "Economy 7,Economy 10,Domestic Economy");

            var tariffManager = new FakeTariffManager { TariffGroupMappings = new Dictionary<string, string> { { "ME808", "FixAndFibre" }, { "ME856", "FixAndProtect" } }};

            var controller = new ControllerFactory()
                .WithConfigManager(fakeConfigManager)
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithTariffRepository(fakeTariffRepository)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .WithTariffManager(tariffManager)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = (ViewResult) result;
            var viewModel = (TariffsViewModel) viewResult.Model;

            viewModel.AvailableTariffs.Count.ShouldEqual(expectedCount);

            var expectedIdList = new List<string>();
            foreach (AvailableTariff availableTariff in viewModel.AvailableTariffs)
            {
                if (availableTariff.ElectricityDetails != null)
                {
                    expectedIdList.Add(availableTariff.ElectricityDetails.ServicePlanId);
                }

                if (availableTariff.GasDetails != null)
                {
                    expectedIdList.Add(availableTariff.GasDetails.ServicePlanId);
                }
            }

            string actualServicePlans = string.Join(",", expectedIdList);
            actualServicePlans.ShouldEqual(expectedServicePlans);
        }

        [TestCase(1, "Expires on 1st January {0}", "Date should say 1st")]
        [TestCase(2, "Expires on 2nd January {0}", "Date should say 2nd")]
        [TestCase(3, "Expires on 3rd January {0}", "Date should say 3rd")]
        [TestCase(4, "Expires on 4th January {0}", "Date should say 4th")]
        [TestCase(21, "Expires on 21st January {0}", "Date should say 21st")]
        [TestCase(22, "Expires on 22nd January {0}", "Date should say 22nd")]
        [TestCase(23, "Expires on 23rd January {0}", "Date should say 23rd")]
        [TestCase(30, "Expires on 30th January {0}", "Date should say 30th")]
        [TestCase(31, "Expires on 31st January {0}", "Date should say 31st")]
        public void DateShouldHaveCorrectSuffix(int day, string expectedString, string description)
        {
            // Arrange
            var date = new DateTime(DateTime.Today.AddYears(2).Year, 1, day);

            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount customerAccount = DefaultDomainModel.ForTariffs();
            customerAccount.CurrentTariff.EndDate = date;
            customerAccount.CurrentTariff.ServicePlanId = "ABC124";
            customer.AddCustomerAccount(customerAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = customerAccount
            });

            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = (ViewResult) result;
            var viewModel = (TariffsViewModel) viewResult.Model;

            viewModel.CurrentTariffViewModel.ExpirationMessage.ShouldEqual(string.Format(expectedString, date.ToString("yyyy")));
        }

        [TestCase(-1, "Expired", "Shouldn't occur, but handled if it does")]
        [TestCase(0, "Expires today on {0}{1} {2}", "Should expire today")]
        [TestCase(1, "Expires tomorrow on {0}{1} {2}", "Should expire tomorrow")]
        [TestCase(2, "Expires in 2 days on {0}{1} {2}", "Should have 2 days remaining")]
        [TestCase(30, "Expires in 30 days on {0}{1} {2}", "Should have 30 days remaining")]
        [TestCase(60, "Expires in 60 days on {0}{1} {2}", "Should have 60 days remaining")]
        [TestCase(61, "Expires on {0}{1} {2}", "Shouldn't display days remaining")]
        public void ExpiryDateShouldBeCorrectDaysRemainingOnNonStandardTariff(int daysRemaining, string expectedString, string description)
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(daysRemaining);

            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount customerAccount = DefaultDomainModel.ForTariffs();
            customerAccount.CurrentTariff.EndDate = date;
            customerAccount.CurrentTariff.ServicePlanId = "ABC124";
            customer.AddCustomerAccount(customerAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = customerAccount
            });
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = (ViewResult) result;
            var viewModel = (TariffsViewModel) viewResult.Model;

            viewModel.CurrentTariffViewModel.ExpirationMessage.ShouldEqual(string.Format(expectedString, date.Day, GetSuffix(date.Day), date.ToString("MMMM yyyy", CultureInfo.InvariantCulture)));
        }

        [Test, Description("Expiration text should be blank if customer on standard tariff")]
        public void ExpirationTextShouldBeBlankIfStandardTariff()
        {
            // Arrange
            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount customerAccount = DefaultDomainModel.ForTariffs();
            customerAccount.CurrentTariff.EndDate = DateTime.MinValue;
            customerAccount.CurrentTariff.Name = "Standard";
            customerAccount.CurrentTariff.ServicePlanId = "ABC124";
            customer.AddCustomerAccount(customerAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = customerAccount
            });

            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = (ViewResult) result;
            var viewModel = (TariffsViewModel) viewResult.Model;

            viewModel.CurrentTariffViewModel.ExpirationMessage.ShouldEqual(string.Empty);
            viewModel.CurrentTariffViewModel.Name.ShouldEqual("Standard");
        }

        [Test]
        public void CurrentTariffShouldCalculateProjectedCostFromAvailableTariffs()
        {
            // Arrange
            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount customerAccount = DefaultDomainModel.ForTariffs();
            customerAccount.CurrentTariff.AnnualUsageKwh = 1000;
            customerAccount.CurrentTariff.Name = "Tariff 1";
            customerAccount.CurrentTariff.ServicePlanId = "ABC123";
            customerAccount.CurrentTariff.EndDate = DateTime.Today.AddDays(366);
            customer.AddCustomerAccount(customerAccount);
            customer.TariffCalculationMethod = TariffCalculationMethod.CurrentRate;
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = customerAccount
            });

            var fakeTariffRepository = new FakeTariffRepository(new[]
            {
                new FakeTariffData
                {
                    Name = "Tariff 1",
                    FuelType = FuelType.Electricity,
                    UnitRate1ExclVat = 15.00,
                    StandingChargeExclVat = 25.00,
                    ServicePlanId = "ABC123"
                },
                new FakeTariffData
                {
                    Name = "Tariff 2",
                    FuelType = FuelType.Electricity,
                    UnitRate1ExclVat = 15.00,
                    StandingChargeExclVat = 25.00,
                    ServicePlanId = "ABC178"
                }
            });

            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithTariffRepository(fakeTariffRepository)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            var viewResult = (ViewResult) result;
            var viewModel = (TariffsViewModel) viewResult.Model;
            viewModel.CurrentTariffViewModel.AnnualCost.ShouldEqual("£253.31");
            viewModel.CurrentTariffViewModel.MonthlyCost.ShouldEqual("£21.11");
        }

        [TestCase(1, "When an available tariff exception is thrown for Single Rate Meter, then the user is redirected to the error page.")]
        [TestCase(2, "When an available tariff exception is thrown for Multi Rate Meter, then the user is redirected to the error page.")]
        public void IfGetAvailableTariffsThrowsExceptionThenShouldBeRedirectedToErrorPage(int meterCount, string description)
        {
            // Arrange
            var availableTariffException = new Exception("Fake exception");
            var fakeTariffRepository = new FakeTariffRepository(availableTariffException);

            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount customerAccount = DefaultDomainModel.ForTariffs();
            customerAccount.SiteDetails.MeterRegisterCount = meterCount;
            customer.AddCustomerAccount(customerAccount);

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = customerAccount
            });

            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithTariffRepository(fakeTariffRepository)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            //Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => controller.AvailableTariffs());
            ex.Message.ShouldNotBeNull();
            ex.Message.ShouldEqual("Exception occurred, Accounts = 1111111113, retrieving available tariffs.");
        }

        [TestCase(1, "When an current tariff exception is thrown for Single Rate Meter, then the user is redirected to the error page.")]
        [TestCase(2, "When an current tariff exception is thrown for Multi Rate Meter, then the user is redirected to the error page.")]
        public void IfGetCurrentTariffThrowsExceptionThenShouldBeRedirectedToErrorPage(int meterCount, string description)
        {
            // Arrange
            var currentTariffException = new Exception("Fake exception");
            var fakeTariffRepository = new FakeTariffRepository(currentTariffException, new[]
            {
                new FakeTariffData
                {
                    Name = "Tariff 1",
                    FuelType = FuelType.Electricity,
                    RateCode = 4,
                    ServicePlanId = "AWE1234455"
                }
            });

            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount customerAccount = DefaultDomainModel.ForTariffs();
            customerAccount.SiteDetails.MeterRegisterCount = meterCount;
            customer.AddCustomerAccount(customerAccount);

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = customerAccount
            });

            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithTariffRepository(fakeTariffRepository)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            //Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => controller.AvailableTariffs());
            ex.Message.ShouldNotBeNull();
            ex.Message.ShouldEqual("Exception occurred, Accounts = 1111111113, retrieving available tariffs.");
        }

        [Test, Description("GetAvailableTariffs will return an empty list if multiple PES Ids are returned for a postcode on the border of two distribution areas")]
        public void IfGetTariffsReturnsNothingRedirectToFalloutPage()
        {
            // Arrange 
            var fakeTariffRepository = new FakeTariffRepository(new FakeTariffData[0]);
            Customer customer = DefaultDomainModel.CustomerForTariffs();
            customer.AddCustomerAccount(DefaultDomainModel.ForTariffs());

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer
            });

            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithTariffRepository(fakeTariffRepository)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) result).RouteValues["action"].ShouldEqual("CallUs");
        }

        private static IEnumerable<TestCaseData> FakeSingleFuelAvailableTariffsSource()
        {
            FakeTariffData[] tariffs = GetSingleRateFakeTariffData();
            yield return new TestCaseData(FuelType.Electricity, tariffs, tariffs.Count(tariff => tariff.FuelType == FuelType.Electricity) - 1);
            yield return new TestCaseData(FuelType.Gas, tariffs, tariffs.Count(tariff => tariff.FuelType == FuelType.Gas) - 1);
        }

        private static FakeTariffData[] GetSingleRateFakeTariffData()
        {
            return new[]
            {
                new FakeTariffData
                {
                    Name = "Tariff 1",
                    FuelType = FuelType.Electricity,
                    UnitRate1ExclVat = 15.00,
                    StandingChargeExclVat = 25.00,
                    ServicePlanId = "ABC123"
                },
                new FakeTariffData
                {
                    Name = "Tariff 2",
                    FuelType = FuelType.Electricity,
                    UnitRate1ExclVat = 10.00,
                    StandingChargeExclVat = 25.00,
                    ServicePlanId = "ABC124"
                },
                new FakeTariffData
                {
                    Name = "Tariff 3",
                    FuelType = FuelType.Electricity,
                    UnitRate1ExclVat = 25.00,
                    StandingChargeExclVat = 25.00,
                    ServicePlanId = "ABC125"
                },
                new FakeTariffData
                {
                    Name = "Tariff 1",
                    FuelType = FuelType.Gas,
                    UnitRate1ExclVat = 15.00,
                    StandingChargeExclVat = 20.00,
                    ServicePlanId = "ABC123"
                },
                new FakeTariffData
                {
                    Name = "Tariff 2",
                    FuelType = FuelType.Gas,
                    UnitRate1ExclVat = 10.00,
                    StandingChargeExclVat = 20.00,
                    ServicePlanId = "ABC124"
                }
            };
        }

        [Test, TestCaseSource(nameof(FakeSingleFuelAvailableTariffsSource)), Description("")]
        public void ASingleFuelCustomerIsPresentedWithAvailableTariffsInAscendingAnnualCostOrder(FuelType fuelType, FakeTariffData[] fakeTariffs, int numberOfAvailableTariffs)
        {
            // Arrange
            var fakeTariffRepository = new FakeTariffRepository(fakeTariffs);

            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount customerAccount = DefaultDomainModel.ForTariffs();
            customerAccount.CurrentTariff.FuelType = fuelType;
            customer.AddCustomerAccount(customerAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = customerAccount
            });

            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("tariffManagement", "excludedTariffs", "TariffIds", "ME672,MG095");

            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithTariffRepository(fakeTariffRepository)
                .WithConfigManager(fakeConfigManager)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = (ViewResult) result;
            var viewModel = (TariffsViewModel) viewResult.Model;
            viewModel.AvailableTariffs.Count.ShouldEqual(numberOfAvailableTariffs);

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (fuelType == FuelType.Electricity)
            {
                foreach (AvailableTariff viewModelAvailableTariff in viewModel.AvailableTariffs)
                {
                    viewModelAvailableTariff.ElectricityDetails.ShouldNotBeNull();
                    viewModelAvailableTariff.GasDetails.ShouldBeNull();
                }

                viewModel.AvailableTariffs[0].ProjectedMonthlyCost.ShouldEqual("£16.73");
                viewModel.AvailableTariffs[0].ProjectedAnnualCost.ShouldEqual("£200.81");
                viewModel.AvailableTariffs[1].ProjectedMonthlyCost.ShouldEqual("£29.86");
                viewModel.AvailableTariffs[1].ProjectedAnnualCost.ShouldEqual("£358.31");
            }
            else if (fuelType == FuelType.Gas)
            {
                foreach (AvailableTariff viewModelAvailableTariff in viewModel.AvailableTariffs)
                {
                    viewModelAvailableTariff.ElectricityDetails.ShouldBeNull();
                    viewModelAvailableTariff.GasDetails.ShouldNotBeNull();
                }

                viewModel.AvailableTariffs[0].ProjectedMonthlyCost.ShouldEqual("£15.14");
                viewModel.AvailableTariffs[0].ProjectedAnnualCost.ShouldEqual("£181.65");
            }
        }

        private static FakeTariffData[] GetMultiRateFakeTariffData()
        {
            return new[]
            {
                new FakeTariffData
                {
                    Name = "Standard Economy 7",
                    FuelType = FuelType.Electricity,
                    UnitRate1ExclVat = 18.89,
                    UnitRate1InclVat = 19.84,
                    UnitRate1SPCOBillingDesc = "Day",
                    UnitRate2ExclVat = 8.67,
                    UnitRate2InclVat = 9.11,
                    UnitRate2SPCOBillingDesc = "Night",
                    StandingChargeExclVat = 14.09,
                    StandingChargeInclVat = 14.80,
                    ServicePlanId = "ABC123"
                },
                new FakeTariffData
                {
                    Name = "Fixed 2020 Economy 10",
                    FuelType = FuelType.Electricity,
                    UnitRate1ExclVat = 19.46,
                    UnitRate1InclVat = 20.44,
                    UnitRate1SPCOBillingDesc = "Standard",
                    UnitRate2ExclVat = 10.52,
                    UnitRate2InclVat = 11.05,
                    UnitRate2SPCOBillingDesc = "Off-peak",
                    StandingChargeExclVat = 14.09,
                    StandingChargeInclVat = 14.80,
                    ServicePlanId = "MG123"
                },
                new FakeTariffData
                {
                    Name = "Fixed Three Years Economy",
                    FuelType = FuelType.Electricity,
                    UnitRate1ExclVat = 18.89,
                    UnitRate1InclVat = 19.84,
                    UnitRate1SPCOBillingDesc = "Day",
                    UnitRate2ExclVat = 8.67,
                    UnitRate2InclVat = 9.11,
                    UnitRate2SPCOBillingDesc = "Night",
                    StandingChargeExclVat = 14.09,
                    StandingChargeInclVat = 14.80,
                    ServicePlanId = "ME124"
                }
            };
        }

        [Test]
        public void AMultiRateElectricityCustomerIsPresentedWithAvailableTariffs()
        {
            // Arrange
            FakeTariffData[] fakeTariffs = GetMultiRateFakeTariffData();
            var fakeTariffRepository = new FakeTariffRepository(fakeTariffs);
            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount customerAccount = DefaultDomainModel.ForTariffs();
            customerAccount.SiteDetails.MeterRegisterCount = 2;
            customer.AddCustomerAccount(customerAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = customerAccount
            });

            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultTariffChange();
            fakeConfigManager.AddConfiguration("MultiRateTariffs", "MultiRateTariffs");
            fakeConfigManager.AddConfiguration("tariffManagement", "excludedTariffs", "TariffIds", "ME672,MG095");

            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithConfigManager(fakeConfigManager)
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithTariffRepository(fakeTariffRepository)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = (ViewResult) result;
            var viewModel = (TariffsViewModel) viewResult.Model;
            viewModel.AvailableTariffs.Count.ShouldEqual(fakeTariffs.Length - 1);
            foreach (AvailableTariff viewModelAvailableTariff in viewModel.AvailableTariffs)
            {
                viewModelAvailableTariff.ElectricityDetails.ShouldNotBeNull();
            }

            viewModel.AvailableTariffs[0].ProjectedMonthlyCost.ShouldEqual("£16.11");
            viewModel.AvailableTariffs[0].ProjectedAnnualCost.ShouldEqual("£193.33");
            viewModel.AvailableTariffs[1].ProjectedMonthlyCost.ShouldEqual("£17.23");
            viewModel.AvailableTariffs[1].ProjectedAnnualCost.ShouldEqual("£206.70");
        }

        private static IEnumerable<TestCaseData> FakeDualFuelAvailableTariffsSource()
        {
            FakeTariffData[] tariffs = GetDualFuelFakeTariffData();
            string currentTariffName = GetCurrentFakeTariffDataForPreserved();
            yield return new TestCaseData(tariffs, currentTariffName, tariffs.Length / 2);
            tariffs = GetDualFuelFakeTariffData();
            currentTariffName = GetCurrentFakeTariffData();
            yield return new TestCaseData(tariffs, currentTariffName, tariffs.Length / 2 - 1);
        }

        private static string GetCurrentFakeTariffData()
        {
            return "ABC123";
        }

        private static string GetCurrentFakeTariffDataForPreserved()
        {
            return "XYZ123";
        }

        private static FakeTariffData[] GetDualFuelFakeTariffData()
        {
            return new[]
            {
                new FakeTariffData
                {
                    Name = "Tariff 1",
                    FuelType = FuelType.Electricity,
                    RateCode = 4,
                    ServicePlanId = "ABC123"
                },
                new FakeTariffData
                {
                    Name = "Tariff 2",
                    FuelType = FuelType.Electricity,
                    RateCode = 4,
                    ServicePlanId = "ABC124"
                },
                new FakeTariffData
                {
                    Name = "Tariff 3",
                    FuelType = FuelType.Electricity,
                    RateCode = 4,
                    ServicePlanId = "ABC125"
                },
                new FakeTariffData
                {
                    Name = "Tariff 1",
                    FuelType = FuelType.Gas,
                    RateCode = 4,
                    ServicePlanId = "ABC123"
                },
                new FakeTariffData
                {
                    Name = "Tariff 2",
                    FuelType = FuelType.Gas,
                    RateCode = 4,
                    ServicePlanId = "ABC124"
                },
                new FakeTariffData
                {
                    Name = "Tariff 3",
                    FuelType = FuelType.Gas,
                    RateCode = 4,
                    ServicePlanId = "ABC125"
                }
            };
        }

        [TestCaseSource(nameof(FakeDualFuelAvailableTariffsSource)), Description("")]
        public void ADualFuelCustomerIsPresentedWithAvailableTariffs(FakeTariffData[] fakeTariffs, string servicePlanId, int numberOfAvailableTariffs)
        {
            // Arrange
            var fakeTariffRepository = new FakeTariffRepository(fakeTariffs);
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount electricAccount = DefaultDomainModel.ForTariffs();
            CustomerAccount gasAccount = DefaultDomainModel.ForTariffs();
            gasAccount.CurrentTariff.FuelType = FuelType.Gas;
            gasAccount.CurrentTariff.ServicePlanId = servicePlanId;
            gasAccount.CurrentTariff.AnnualCost = 100.00;
            electricAccount.CurrentTariff.ServicePlanId = servicePlanId;
            electricAccount.CurrentTariff.AnnualCost = 100.00;
            customer.AddCustomerAccount(electricAccount);
            customer.AddCustomerAccount(gasAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = electricAccount
            });

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithTariffRepository(fakeTariffRepository)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = (ViewResult) result;
            var viewModel = (TariffsViewModel) viewResult.Model;
            viewModel.AvailableTariffs.Count.ShouldEqual(numberOfAvailableTariffs);
            foreach (AvailableTariff viewModelAvailableTariff in viewModel.AvailableTariffs)
            {
                viewModelAvailableTariff.ElectricityDetails.ShouldNotBeNull();
                viewModelAvailableTariff.GasDetails.ShouldNotBeNull();
            }
        }

        [Test]
        public void DirectAccessShouldRedirectToLandingPageWhenCustomerNull()
        {
            // Arrange
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService();
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) result).RouteValues["action"].ShouldEqual("IdentifyCustomer");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.ShouldBeNull();
        }

        [Test]
        public void DirectAccessShouldRedirectToLandingPageWhenFalloutReasonsNull()
        {
            // Arrange 
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = new Customer()
            });
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) result).RouteValues["action"].ShouldEqual("IdentifyCustomer");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.ShouldBeNull();
        }

        [Test]
        public void DirectAccessShouldRedirectToLandingPageWhenFalloutReasonsGreaterThanZero()
        {
            // Arrange
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = new Customer
                {
                    FalloutReasons = new List<FalloutReasonResult>
                    {
                        new FalloutReasonResult { FalloutReason = FalloutReason.Ineligible }
                    }
                }
            });

            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) result).RouteValues["action"].ShouldEqual("IdentifyCustomer");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.ShouldBeNull();
        }

        [Test]
        public void DirectAccessToSelectTariffShouldRedirectToLandingPageWhenCustomerNull()
        {
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails());

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<TariffsController>();

            ActionResult result = controller.SelectTariff("", false);

            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) result).RouteValues["action"].ShouldEqual("IdentifyCustomer");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.ShouldBeNull();
        }

        [Test]
        public void ShouldRedirectToErrorIfExceptionIsThrown()
        {
            //Arrange
            var customer = new Customer
            {
                FalloutReasons = new List<FalloutReasonResult>()
            };
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = new CustomerAccount()
            });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<TariffsController>();

            //Act & Assert
            var ex = Assert.Throws<Exception>(() => controller.SelectTariff("", false));
            ex.Message.ShouldNotBeNull();
            ex.Message.ShouldEqual("Exception occurred, Accounts = , showing Summary page.");
        }

        [Test]
        public void AvailableTariffShouldShowTheCorrectTaglineWhereTariffGroupIsNotNone()
        {
            // Arrange
            var fakeTariffData = new FakeTariffData
            {
                Name = "Fix and Fibre Bundle",
                ServicePlanId = "ABC124"
            };
            var fakeTariffRepository = new FakeTariffRepository(new[] { fakeTariffData });

            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount electricAccount = DefaultDomainModel.ForTariffs();
            CustomerAccount gasAccount = DefaultDomainModel.ForTariffs();
            gasAccount.CurrentTariff.FuelType = FuelType.Gas;
            customer.AddCustomerAccount(electricAccount);
            customer.AddCustomerAccount(gasAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = electricAccount
            });

            var fakeTariffManager = new FakeTariffManager();
            fakeTariffManager.TariffGroupMappings.Add("ABC124", TariffGroup.FixAndFibre.ToString());
            fakeTariffManager.TaglineMappings.Add("Fix and Fibre Bundle", "Our simple energy tariff with no ties");

            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithTariffRepository(fakeTariffRepository)
                .WithTariffManager(fakeTariffManager)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = (ViewResult) result;
            var viewModel = (TariffsViewModel) viewResult.Model;
            viewModel.AvailableTariffs[0].Tagline.ShouldEqual("Our simple energy tariff with no ties");
        }

        [Test]
        public void AvailableTariffShouldShowTheCorrectTaglineWhereTariffGroupIsNone()
        {
            // Arrange
            var fakeTariffData = new FakeTariffData
            {
                Name = "Tariff v1",
                ServicePlanId = "ABC124"
            };
            var fakeTariffRepository = new FakeTariffRepository(new[] { fakeTariffData });
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient();
            var fakeSessionManager = new FakeSessionManager();

            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount electricAccount = DefaultDomainModel.ForTariffs();
            CustomerAccount gasAccount = DefaultDomainModel.ForTariffs();
            gasAccount.CurrentTariff.FuelType = FuelType.Gas;
            customer.AddCustomerAccount(electricAccount);
            customer.AddCustomerAccount(gasAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = electricAccount
            });

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithTariffRepository(fakeTariffRepository)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = (ViewResult) result;
            var viewModel = (TariffsViewModel) viewResult.Model;
            viewModel.AvailableTariffs[0].Tagline.ShouldEqual("Dummy tariff");
        }

        [TestCase("Tariff v1", "tariff_v1.pdf", 1)]
        [TestCase("Tariff v2", "tariff_v1.pdf | tariff_v2.pdf", 2)]
        public void AvailableTariffShouldShowTheCorrectPdfLink(string tariffName, string tariffPdfLinks, int numberOfPdfLinks)
        {
            // Arrange
            var fakeTariffData = new FakeTariffData
            {
                Name = tariffName,
                ServicePlanId = "ABC124"
            };
            var fakeTariffRepository = new FakeTariffRepository(new[] { fakeTariffData });

            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount electricAccount = DefaultDomainModel.ForTariffs();
            CustomerAccount gasAccount = DefaultDomainModel.ForTariffs();
            gasAccount.CurrentTariff.FuelType = FuelType.Gas;
            customer.AddCustomerAccount(electricAccount);
            customer.AddCustomerAccount(gasAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = electricAccount
            });

            var fakeTariffManager = new FakeTariffManager();
            fakeTariffManager.PdfLinkMappings.Add(tariffName, tariffPdfLinks);

            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient();
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithTariffRepository(fakeTariffRepository)
                .WithTariffManager(fakeTariffManager)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = (ViewResult) result;
            var viewModel = (TariffsViewModel) viewResult.Model;
            viewModel.AvailableTariffs[0].TermsAndConditionsPdfLinks.Count.ShouldEqual(numberOfPdfLinks);
        }

        [Test]
        public void AvailableTariffShouldShowTheFormatForTickUsps()
        {
            // Arrange
            var fakeTariffData = new FakeTariffData
            {
                Name = "Tariff v2",
                ServicePlanId = "ABC124"
            };
            var fakeTariffRepository = new FakeTariffRepository(new[] { fakeTariffData });

            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount electricAccount = DefaultDomainModel.ForTariffs();
            CustomerAccount gasAccount = DefaultDomainModel.ForTariffs();
            gasAccount.CurrentTariff.FuelType = FuelType.Gas;
            customer.AddCustomerAccount(electricAccount);
            customer.AddCustomerAccount(gasAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = electricAccount
            });

            var fakeTariffManager = new FakeTariffManager();

            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient();
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithTariffRepository(fakeTariffRepository)
                .WithTariffManager(fakeTariffManager)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = (ViewResult) result;
            var viewModel = (TariffsViewModel) viewResult.Model;
            viewModel.AvailableTariffs[0].TickUsps["Flexible energy"].ShouldEqual(": energy prices may go up or down");
            string.IsNullOrEmpty(viewModel.AvailableTariffs[0].TickUsps["No early exit fee"]).ShouldBeTrue();
        }

        [Test]
        public void ShouldRedirectToGetCustomerEmailWhenAValidTariffIsSelected()
        {
            // Arrange
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = DefaultDomainModel.CustomerForTariffs()
            });
            var fakeAvailableTariffService = new FakeAvailableTariffService
            {
                AvailableTariffs = new List<AvailableTariff>
                {
                    new AvailableTariff { Name = "Standard Economy 7" },
                    new AvailableTariff { Name = "Fixed 2020 Economy 7" },
                    new AvailableTariff { Name = "1 year Fixed Economy 7" }
                }
            };

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAvailableTariffService(fakeAvailableTariffService)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.SelectTariff("Standard Economy 7", false);

            // Assert 
            var redirectToRouteResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectToRouteResult.RouteValues["action"].ShouldEqual("GetCustomerEmail");
            redirectToRouteResult.RouteValues["controller"].ShouldEqual("AdditionalDetails");
        }

        [Test]
        public void ShouldRedirectBackToAvailableTariffsWhenAnInValidTariffIsSelected()
        {
            // Arrange
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = DefaultDomainModel.CustomerForTariffs()
            });
            var fakeAvailableTariffService = new FakeAvailableTariffService
            {
                AvailableTariffs = new List<AvailableTariff>
                {
                    new AvailableTariff { Name = "Standard Economy 7" },
                    new AvailableTariff { Name = "Fixed 2020 Economy 7" },
                    new AvailableTariff { Name = "1 year Fixed Economy 7" }
                }
            };

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAvailableTariffService(fakeAvailableTariffService)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.SelectTariff("Random Tariff", false);

            // Assert 
            var redirectToRouteResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectToRouteResult.RouteValues["action"].ShouldEqual("AvailableTariffs");
        }

        [TestCase(59, 0, true, "When the Tariff End Date is less than days 60 hence and isImmediateRenewal, Effective Date should be today. ")]
        [TestCase(59, 59, false, "When the Tariff End Date is less than days 60 hence and not isImmediateRenewal, Effective Date should be Tariff End Date.")]
        public void CorrectEffectiveDateIsSetWhenElectricTariffSelectedForRenewals(int remainingDaysOnTariff, int effectiveDateDays, bool isImmediateRenewal, string description)
        {
            // Arrange
            var fakeAvailableTariffService = new FakeAvailableTariffService
            {
                AvailableTariffs = new List<AvailableTariff>
                {
                    new AvailableTariff { Name = "Tariff 1", ElectricityDetails = new TariffInformationLabel() }
                }
            };

            Customer customer = DefaultDomainModel.CustomerForTariffs();

            CustomerAccount gasAccount = DefaultDomainModel.ForTariffs();
            gasAccount.CurrentTariff.Name = "Tariff 1";
            gasAccount.CurrentTariff.EndDate = DateTime.Today.AddDays(remainingDaysOnTariff);
            customer.AddCustomerAccount(gasAccount);

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails { Customer = customer });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAvailableTariffService(fakeAvailableTariffService)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.SelectTariff("Tariff 1", isImmediateRenewal);

            // Assert
            JourneyDetails journey = fakeInMemoryTariffChangeSessionService.GetJourneyDetails();
            journey.Customer.CustomerSelectedTariff.EffectiveDate.ShouldEqual(DateTime.Today.AddDays(effectiveDateDays));
            var redirectToRouteResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectToRouteResult.RouteValues["action"].ShouldEqual("GetCustomerEmail");
            redirectToRouteResult.RouteValues["controller"].ShouldEqual("AdditionalDetails");
        }

        [TestCase(59, 0, true, "When the Tariff End Date is less than days 60 hence and isImmediateRenewal, Effective Date should be today. ")]
        [TestCase(59, 59, false, "When the Tariff End Date is less than days 60 hence and not isImmediateRenewal, Effective Date should be Tariff End Date.")]
        public void CorrectEffectiveDateIsSetWhenGasTariffSelectedForRenewals(int remainingDaysOnTariff, int effectiveDateDays, bool isImmediateRenewal, string description)
        {
            // Arrange
            var fakeAvailableTariffService = new FakeAvailableTariffService
            {
                AvailableTariffs = new List<AvailableTariff>
                {
                    new AvailableTariff { Name = "Tariff 1", GasDetails = new TariffInformationLabel() }
                }
            };

            Customer customer = DefaultDomainModel.CustomerForTariffs();

            CustomerAccount gasAccount = DefaultDomainModel.ForTariffs();
            gasAccount.CurrentTariff.FuelType = FuelType.Gas;
            gasAccount.SiteDetails.AccountNumber = "1111111114";
            gasAccount.CurrentTariff.Name = "Tariff 1";
            gasAccount.CurrentTariff.EndDate = DateTime.Today.AddDays(remainingDaysOnTariff);
            customer.AddCustomerAccount(gasAccount);

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer
            });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAvailableTariffService(fakeAvailableTariffService)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.SelectTariff("Tariff 1", isImmediateRenewal);

            // Assert
            JourneyDetails journey = fakeInMemoryTariffChangeSessionService.GetJourneyDetails();
            journey.Customer.CustomerSelectedTariff.EffectiveDate.ShouldEqual(DateTime.Today.AddDays(effectiveDateDays));
            var redirectToRouteResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectToRouteResult.RouteValues["action"].ShouldEqual("GetCustomerEmail");
            redirectToRouteResult.RouteValues["controller"].ShouldEqual("AdditionalDetails");
        }

        [TestCase(59, 0, true, "When the Tariff End Date is less than days 60 hence and isImmediateRenewal, Effective Date should be today. ")]
        [TestCase(59, 59, false, "When the Tariff End Date is less than days 60 hence and not isImmediateRenewal, Effective Date should be Tariff End Date.")]
        public void CorrectEffectiveDateIsSetWhenDualFuelTariffSelectedForRenewals(int remainingDaysOnTariff, int effectiveDateDays, bool isImmediateRenewal, string description)
        {
            // Arrange
            var fakeAvailableTariffService = new FakeAvailableTariffService
            {
                AvailableTariffs = new List<AvailableTariff>
                {
                    new AvailableTariff { Name = "Tariff 1", ElectricityDetails = new TariffInformationLabel(), GasDetails = new TariffInformationLabel() }
                }
            };

            Customer customer = DefaultDomainModel.CustomerForTariffs();

            CustomerAccount electricAccount = DefaultDomainModel.ForTariffs();
            electricAccount.CurrentTariff.Name = "Tariff 1";
            electricAccount.CurrentTariff.EndDate = DateTime.Today.AddDays(remainingDaysOnTariff);
            customer.AddCustomerAccount(electricAccount);

            CustomerAccount gasAccount = DefaultDomainModel.ForTariffs();
            gasAccount.CurrentTariff.FuelType = FuelType.Gas;
            gasAccount.SiteDetails.AccountNumber = "1111111114";
            gasAccount.CurrentTariff.Name = "Tariff 1";
            gasAccount.CurrentTariff.EndDate = DateTime.Today.AddDays(remainingDaysOnTariff);
            customer.AddCustomerAccount(gasAccount);

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer
            });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAvailableTariffService(fakeAvailableTariffService)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.SelectTariff("Tariff 1", isImmediateRenewal);

            // Assert
            JourneyDetails journey = fakeInMemoryTariffChangeSessionService.GetJourneyDetails();
            journey.Customer.CustomerSelectedTariff.EffectiveDate.ShouldEqual(DateTime.Today.AddDays(effectiveDateDays));
            var redirectToRouteResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectToRouteResult.RouteValues["action"].ShouldEqual("GetCustomerEmail");
            redirectToRouteResult.RouteValues["controller"].ShouldEqual("AdditionalDetails");
        }

        [TestCase(59, true, "When the Tariff End Date is less than days 60 hence the renewals flag should be true. ")]
        [TestCase(60, true, "When the Tariff End Date is equal days 60 hence the renewals flag should be true.")]
        [TestCase(61, false, "When the Tariff End Date is more than 60 hence the renewals flag is should be false.")]
        public void RenewalFlagShouldBeSetForRenewalCustomers(int remainingDaysOnTariff, bool isRenewal, string description)
        {
            // Arrange
            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount electricAccount = DefaultDomainModel.ForTariffs();
            electricAccount.CurrentTariff.Name = "Tariff1";
            electricAccount.CurrentTariff.ServicePlanId = "ABC124";
            customer.AddCustomerAccount(electricAccount);
            electricAccount.CurrentTariff.EndDate = DateTime.Today.AddDays(remainingDaysOnTariff);

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = electricAccount
            });
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act 
            ActionResult result = controller.AvailableTariffs().Result;

            // Assert
            var viewResult = (ViewResult) result;
            var viewModel = (TariffsViewModel) viewResult.Model;
            viewModel.IsRenewal.ShouldEqual(isRenewal);
        }

        [TestCase(FuelType.Gas, 2, "me672")]
        [TestCase(FuelType.Gas, 2, "mg095")]
        [TestCase(FuelType.Gas, 2, "ME672")]
        [TestCase(FuelType.Gas, 2, "MG095")]
        [TestCase(FuelType.Gas, 2, "ABC123")]
        [TestCase(FuelType.Electricity, 3, "ME672")]
        [TestCase(FuelType.Electricity, 3, "MG095")]
        [TestCase(FuelType.Electricity, 2, "ABC123")]
        public void ShouldRemoveExcludedTariffsFromAvailableTariffsIfGasOnlyCustomer(FuelType fuelType, int numberOfAvailableTariffs, string servicePlanId)
        {
            var fakeTariffData = new[]
            {
                new FakeTariffData
                {
                    ServicePlanId = servicePlanId,
                    Name = "Tariff 1",
                    FuelType = fuelType,
                    UnitRate1ExclVat = 15.00,
                    StandingChargeExclVat = 25.00
                },
                new FakeTariffData
                {
                    Name = "Tariff 2",
                    FuelType = fuelType,
                    UnitRate1ExclVat = 15.00,
                    StandingChargeExclVat = 25.00,
                    ServicePlanId = "ABC124"
                },
                new FakeTariffData
                {
                    Name = "Tariff 3",
                    FuelType = fuelType,
                    UnitRate1ExclVat = 15.00,
                    StandingChargeExclVat = 25.00,
                    ServicePlanId = "ABC125"
                }
            };

            var fakeTariffRepository = new FakeTariffRepository(fakeTariffData);

            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount customerAccount = DefaultDomainModel.ForTariffs();
            customerAccount.CurrentTariff.FuelType = fuelType;
            customer.AddCustomerAccount(customerAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = customerAccount
            });

            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("tariffManagement", "excludedTariffs", "TariffIds", "ME672,MG095");
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithConfigManager(fakeConfigManager)
                .WithTariffRepository(fakeTariffRepository)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            //Assert
            var viewResult = (ViewResult) result;
            var viewModel = (TariffsViewModel) viewResult.Model;
            viewModel.AvailableTariffs.Count.ShouldEqual(numberOfAvailableTariffs);
        }

        [TestCase(FuelType.Electricity, "ME672", 2)]
        [TestCase(FuelType.Electricity, "MG095", 2)]
        [TestCase(FuelType.Electricity, "me672", 2)]
        [TestCase(FuelType.Electricity, "mg095", 2)]
        [TestCase(FuelType.Electricity, "ABC123", 2)]
        public void ShouldRemoveExcludedTariffsIdsFromAvailableTariffsIfMultiRateCustomer(FuelType fuelType, string servicePlanId, int numberOfAvailableTariffs)
        {
            // Arrange
            var fakeTariffData = new[]
            {
                new FakeTariffData
                {
                    ServicePlanId = servicePlanId,
                    Name = "Tariff 1",
                    FuelType = FuelType.Electricity,
                    UnitRate1ExclVat = 15.00,
                    StandingChargeExclVat = 25.00
                },
                new FakeTariffData
                {
                    Name = "Tariff 2",
                    FuelType = FuelType.Electricity,
                    UnitRate1ExclVat = 15.00,
                    StandingChargeExclVat = 25.00,
                    ServicePlanId = "ABC124"
                },
                new FakeTariffData
                {
                    Name = "Tariff 3",
                    FuelType = FuelType.Electricity,
                    UnitRate1ExclVat = 15.00,
                    StandingChargeExclVat = 25.00,
                    ServicePlanId = "ABC125"
                }
            };

            var fakeTariffRepository = new FakeTariffRepository(fakeTariffData);

            Customer customer = DefaultDomainModel.CustomerForTariffs();

            CustomerAccount customerAccount = DefaultDomainModel.ForTariffs();
            customerAccount.SiteDetails.MeterRegisterCount = 2;
            customerAccount.CurrentTariff.FuelType = FuelType.Electricity;
            customer.AddCustomerAccount(customerAccount);

            var fakeInMemoryTariffChangeSessionService =
                new FakeInMemoryTariffChangeSessionService(
                    new JourneyDetails
                    {
                        Customer = customer,
                        CustomerAccount = customerAccount
                    });

            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("tariffManagement", "excludedTariffs", "TariffIds", "ME672,MG095");
            fakeConfigManager.AddConfiguration("MultiRateTariffs", "MultiRateTariffs");

            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithConfigManager(fakeConfigManager)
                .WithTariffRepository(fakeTariffRepository)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            //Assert
            var viewResult = (ViewResult) result;
            var viewModel = (TariffsViewModel) viewResult.Model;
            viewModel.AvailableTariffs.Count.ShouldEqual(numberOfAvailableTariffs);
        }

        private static Customer GetAvailableTariffsCustomer(bool hasGas, bool hasElectric, bool hasMultiRateMeter)
        {
            Customer customer = DefaultDomainModel.CustomerForTariffs();
            if (hasGas)
            {
                CustomerAccount gasAccount = DefaultDomainModel.ForTariffs();
                gasAccount.CurrentTariff.AnnualUsageKwh = 100;
                gasAccount.CurrentTariff.AnnualCost = 81.90;
                gasAccount.CurrentTariff.Name = "2 Year Fixed";
                gasAccount.CurrentTariff.FuelType = FuelType.Gas;
                gasAccount.CurrentTariff.ServicePlanId = "MG111";
                customer.AddCustomerAccount(gasAccount);
            }

            if (hasElectric)
            {
                CustomerAccount electricAccount = DefaultDomainModel.ForTariffs();
                electricAccount.CurrentTariff.AnnualUsageKwh = 1000;
                electricAccount.CurrentTariff.AnnualCost = 129.15;
                electricAccount.CurrentTariff.Name = "1 Year Fixed";
                electricAccount.CurrentTariff.ServicePlanId = "ME111";
                electricAccount.CurrentTariff.FuelType = FuelType.Electricity;
                customer.AddCustomerAccount(electricAccount);
                if (hasMultiRateMeter)
                {
                    customer.ElectricityAccount.SiteDetails.MeterRegisterCount = 2;
                }
            }

            return customer;
        }

        [Test]
        public void ShouldPopulateFollowOnTariffWhenCustomerHasAFollowOnServicePlanID()
        {
            // Arrange
            var fakeTariffData = new[]
            {
                new FakeTariffData
                {
                    Name = "Tariff 1",
                    FuelType = FuelType.Electricity,
                    UnitRate1ExclVat = 15.00,
                    StandingChargeExclVat = 25.00,
                    ServicePlanId = "ME111",
                    RateCode = 4
                },
                new FakeTariffData
                {
                    Name = "Tariff 2",
                    FuelType = FuelType.Electricity,
                    UnitRate1ExclVat = 15.00,
                    StandingChargeExclVat = 25.00,
                    ServicePlanId = "ME112",
                    RateCode = 4
                },
                new FakeTariffData
                {
                    Name = "Tariff 3",
                    FuelType = FuelType.Electricity,
                    UnitRate1ExclVat = 15.00,
                    StandingChargeExclVat = 25.00,
                    ServicePlanId = "ME113",
                    RateCode = 4
                }
            };

            var fakeTariffRepository = new FakeTariffRepository(fakeTariffData);

            Customer customer = DefaultDomainModel.CustomerForTariffs();

            CustomerAccount customerAccount = DefaultDomainModel.ForTariffs();
            customerAccount.FollowOnTariffServicePlanID = "ME111";
            customerAccount.SiteDetails.MeterRegisterCount = 2;
            customer.AddCustomerAccount(customerAccount);

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = customerAccount
            });

            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("MultiRateTariffs", "MultiRateTariffs");

            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithConfigManager(fakeConfigManager)
                .WithTariffRepository(fakeTariffRepository)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;

            //Assert
            var model = result.ShouldNotBeNull()
                .ShouldBeType<ViewResult>()
                .Model.ShouldBeType<TariffsViewModel>();

            model.FollowOnTariff.ShouldNotBeNull();
            model.NewTariffSubHeader.ShouldEqual($"Your Default Domain Model Tariff Name tariff expires in <span class=text-red>1 day</span> on {customerAccount.CurrentTariff.EndDate.ToSseString()}");
            model.NewTariffStartMessage.ShouldEqual($"Starts on {customerAccount.CurrentTariff.EndDate.AddDays(1).ToSseString()}");
            model.AvailableTariffs.Count.ShouldEqual(2);
            model.AvailableTariffs.FirstOrDefault(t => t.ElectricityDetails.ServicePlanId == customerAccount.FollowOnTariffServicePlanID).ShouldBeNull(); // Available tariffs should not contain follow on tariff
        }

        [Test]
        public void ShouldSetFollowOnTariffAsSelectedTariffWhenCustomerSelectedFollowOnTariffAndRedirectToConfirmationPage()
        {
            // Arrange
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = new Customer
                {
                    FollowOnTariff = new SelectedTariff()
                }
            });
            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.SelectFollowOnTariff();

            // Assert
            var redirectToRouteResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectToRouteResult.RouteValues["action"].ShouldEqual("ShowConfirmation");
            redirectToRouteResult.RouteValues["controller"].ShouldEqual("Confirmation");

            fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.CustomerSelectedTariff.IsFollowOnTariff.ShouldBeTrue();
        }

        [TestCaseSource(nameof(EPPContentTestCases))]
        public void AccordionEPPContentShouldPopulateWithCorrectContent(TariffCalculationMethod tariffCalculationMethod, string content, DateTime renewalDays)
        {
            // Arrange
            Customer customer = DefaultDomainModel.CustomerForTariffs();
            CustomerAccount customerAccount = DefaultDomainModel.ForTariffs();
            customerAccount.CurrentTariff.EndDate = renewalDays;
            customerAccount.CurrentTariff.Name = "Standard";
            customerAccount.CurrentTariff.ServicePlanId = "ABC124";
            customer.TariffCalculationMethod = tariffCalculationMethod;
            customer.AddCustomerAccount(customerAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = customerAccount
            });
            var fakeContentManagementAPIClient = new FakeContentManagementAPIClient(FakeContentManagementStub.GetDummyCMSResponseModelForProducts());
            var fakeSessionManager = new FakeSessionManager();

            ControllerHelperFactory controllerHelperFactory = new ControllerHelperFactory()
                .WithContentManagementAPIClient(fakeContentManagementAPIClient)
                .WithSessionManager(fakeSessionManager);

            var contentManagementControllerHelper = controllerHelperFactory.Build<ContentManagementControllerHelper>();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithContentManagementControllerHelper(contentManagementControllerHelper)
                .Build<TariffsController>();

            // Act
            ActionResult result = controller.AvailableTariffs().Result;
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<TariffsViewModel>();

            // Assert
            viewModel.AccordionEPPContent.ShouldEqual(content);
        }

        private static IEnumerable<TestCaseData> EPPContentTestCases()
        {
            yield return new TestCaseData(TariffCalculationMethod.CurrentRate, AvailableTariffs_Resources.AccordionEPPContent_CurrentRate, DateTime.Now.AddDays(100));
            yield return new TestCaseData(TariffCalculationMethod.Original, AvailableTariffs_Resources.AccordionEPPContent_Original, DateTime.Now.AddDays(100));
            yield return new TestCaseData(TariffCalculationMethod.CurrentRate, AvailableTariffs_Resources.AccordionEPPContent_Renewal, DateTime.Now.AddDays(20));
            yield return new TestCaseData(TariffCalculationMethod.CurrentRate, AvailableTariffs_Resources.AccordionEPPContent_Renewal, DateTime.Now);
            yield return new TestCaseData(TariffCalculationMethod.CurrentRate, AvailableTariffs_Resources.AccordionEPPContent_Renewal, DateTime.Now.AddDays(56));
            yield return new TestCaseData(TariffCalculationMethod.CurrentRate, AvailableTariffs_Resources.AccordionEPPContent_CurrentRate, DateTime.MaxValue);
            yield return new TestCaseData(TariffCalculationMethod.CurrentRate, AvailableTariffs_Resources.AccordionEPPContent_CurrentRate, DateTime.MinValue);
        }

        private static string GetSuffix(int day)
        {
            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }

        private static IEnumerable<TestCaseData> CurrentTariffData()
        {
            CustomerAccount electricAccount = DefaultDomainModel.ForTariffs();
            electricAccount.CurrentTariff.AnnualUsageKwh = 1000;
            electricAccount.CurrentTariff.AnnualCost = 129.15;
            electricAccount.CurrentTariff.Name = "1 Year Fixed";
            Customer electricCustomer = DefaultDomainModel.CustomerForTariffs();
            electricCustomer.AddCustomerAccount(electricAccount);
            yield return new TestCaseData(electricCustomer, "1 Year Fixed", "£129.15", "£10.76", 1000, 0, "Should display correct details");
            CustomerAccount gasAccount = DefaultDomainModel.ForTariffs();
            gasAccount.CurrentTariff.AnnualUsageKwh = 100;
            gasAccount.CurrentTariff.AnnualCost = 81.90;
            gasAccount.CurrentTariff.Name = "2 Year Fixed";
            gasAccount.CurrentTariff.FuelType = FuelType.Gas;
            Customer gasCustomer = DefaultDomainModel.CustomerForTariffs();
            gasCustomer.AddCustomerAccount(gasAccount);
            yield return new TestCaseData(gasCustomer, "2 Year Fixed", "£81.90", "£6.83", 0, 100, "Should display correct details");
            Customer customer = DefaultDomainModel.CustomerForTariffs();
            electricAccount = DefaultDomainModel.ForTariffs();
            electricAccount.CurrentTariff.Name = "1 Year Fixed";
            electricAccount.CurrentTariff.AnnualCost = 100.00;
            electricAccount.CurrentTariff.AnnualUsageKwh = 1000;
            electricAccount.CurrentTariff.EndDate = DateTime.Today.AddDays(1);
            gasAccount = DefaultDomainModel.ForTariffs();
            //gasAccount.SiteDetails.AccountNumber = "2222222226";
            gasAccount.CurrentTariff.AnnualCost = 111.05;
            gasAccount.CurrentTariff.Name = "1 Year Fixed";
            gasAccount.CurrentTariff.AnnualUsageKwh = 100;
            gasAccount.CurrentTariff.EndDate = DateTime.Today.AddDays(1);
            gasAccount.CurrentTariff.FuelType = FuelType.Gas;
            customer.AddCustomerAccount(electricAccount);
            customer.AddCustomerAccount(gasAccount);
            yield return new TestCaseData(customer, "1 Year Fixed", "£211.05", "£17.59", 1000, 100, "Should display correct details");
        }
    }
}