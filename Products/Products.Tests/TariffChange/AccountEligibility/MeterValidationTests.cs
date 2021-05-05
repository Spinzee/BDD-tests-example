namespace Products.Tests.TariffChange.AccountEligibility
{
    using System.Configuration;
    using Common.Fakes;
    using Common.Helpers;
    using Fakes.Models;
    using Fakes.Services;
    using Helpers;
    using Model.TariffChange;
    using Model.TariffChange.Customers;
    using NUnit.Framework;
    using ServiceWrapper.ManageCustomerInformationService;
    using Should;
    using Web.Areas.TariffChange.Controllers;
    using FuelType = Model.Enums.FuelType;

    public class MeterValidationTests
    {
        [TestCase("00", false, "When there are 0 meter registers, should be ineligible.")]
        [TestCase("01", true, "When there is 1 meter register, should be eligible.")]
        [TestCase("02", false, "When there are 2 meter registers, should be ineligible.")]
        [TestCase("03", false, "When there are 3 meter registers, should be ineligible. Also covers multiple meters at site based on AER service logic.")]
        public void MeterIneligibleWhenNumberOfMeterRegistersNotOne(string numberOfRegisters, bool isValid, string description)
        {
            // Arrange
            // ReSharper disable once UseObjectOrCollectionInitializer
            var fakeAerData = new FakeAERData();
            fakeAerData.CustomerAccountVariables["RegisterCount"] = numberOfRegisters;
            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(fakeAerData.CustomerAccountNumber, new[] { fakeAerData });

            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.SiteDetails.AccountNumber = fakeAerData.CustomerAccountNumber;
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                CustomerAccount = customerAccount
            });
            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AtlasAnnouncementDate", "30/04/2018 9:00");

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<AccountEligibilityController>();

            // Act + Assert
            TestHelper.GetResultUrlString(controller.CheckEligibility())
                .ShouldEqual(isValid ? "AvailableTariffs" : "CustomerAccountIneligible");
            if (isValid)
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.FalloutReasons.ShouldBeEmpty();
            }
            else
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.ShouldBeNull();
            }
        }

        [TestCase("Standard Domestic Economy", true, "domestic economy should go to acquisition journey")]
        [TestCase("Standard Economy 7", true, "economy 7 should go to acquisition journey")]
        [TestCase("AllOtherTwoRateTariffs", false, "neither domestic economy nor economy 7 tariff should go to ineligible page")]
        public void TwoRateMeterFallouts(string tariffName, bool isAcquisition, string description)
        {
            // Arrange
            // ReSharper disable once UseObjectOrCollectionInitializer
            var fakeAerData = new FakeAERData();
            fakeAerData.CustomerAccountVariables["RegisterCount"] = "02";
            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(fakeAerData.CustomerAccountNumber, new[] { fakeAerData });

            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.SiteDetails.AccountNumber = fakeAerData.CustomerAccountNumber;
            customerAccount.CurrentTariff.Name = tariffName;
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                CustomerAccount = customerAccount
            });

            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultTariffChange();

            var controller = new ControllerFactory()
                .WithConfigManager(fakeConfigManager)
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .Build<AccountEligibilityController>();

            // Required to use ConfigurationManager for now as the DefaultRegistry uses this.
            string allowEconomyMultiRateMeters = ConfigurationManager.AppSettings["AllowEconomyMultiRateMeters"];

            if (!string.IsNullOrEmpty(allowEconomyMultiRateMeters) && allowEconomyMultiRateMeters == "false")
            {
                TestHelper.GetResultUrlString(controller.CheckEligibility()).ShouldEqual(isAcquisition ? "Acquisition" : "CustomerAccountIneligible");
            }
        }

        [TestCase("Standard Energy Economy 7", "Standard Energy", "02", "SWAL", true, "First account is economy 7 and should go to acquisition")]
        [TestCase("Standard Energy", "Standard Energy Economy 7", "02", "SWAL", true, "Second account is economy 7 and should go to acquisition")]
        [TestCase("Standard Energy", "Standard Energy", "02", "SWAL", false, "No economy 7 or domestic economy should go to ineligible based on number of registers")]
        [TestCase("Standard Energy Economy 7", "Standard Energy", "01", "ATGA", true, "First account is economy 7 and should go to acquisition, for Atlantic customer")]
        [TestCase("Standard Energy", "Standard Energy Economy 7", "01", "ATGA", true, "Second account is economy 7 and should go to acquisition, for Atlantic customer")]
        [TestCase("Standard Energy", "Standard Energy", "01", "ATGA", false, "No economy 7 or domestic economy should go to ineligible based on being an Atlantic customer")]
        public void AcquisitionJourneyFallout(string firstAccTariffName, string secondAccTariffName, string numberOfRegisters, string brandCode, bool isAcquisition, string description)
        {
            // Arrange
            // ReSharper disable once UseObjectOrCollectionInitializer
            var fakeAerData = new FakeAERData[2];
            fakeAerData[0] = new FakeAERData
            {
                CustomerAccountNumber = "8023047213",
                CustomerName = "Mr Fred Flintstone"
            };
            fakeAerData[1] = new FakeAERData
            {
                CustomerAccountNumber = "0375557211",
                CustomerName = "Mr Fred Flintstone"
            };

            fakeAerData[0].CustomerAccountVariables["RegisterCount"] = numberOfRegisters;

            var fakeMcisData = new FakeMCISData
            {
                CustomerAccountNumber = "0375557211",
                Postcode = "SO14 2FJ",
                BrandCode = brandCode,
                CustomerAccountStatus = CustomerAccountStatusType.Found,
                ServicePlanDescription = secondAccTariffName
            };

            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(fakeAerData[0].CustomerAccountNumber, fakeAerData);

            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.SiteDetails.AccountNumber = fakeAerData[0].CustomerAccountNumber;
            customerAccount.CurrentTariff.Name = firstAccTariffName;
            customerAccount.CurrentTariff.FuelType = FuelType.Gas;
            customerAccount.CurrentTariff.BrandCode = brandCode;
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                CustomerAccount = customerAccount
            });

            var fakeManageCustomerInformationServiceWrapper = new FakeManageCustomerInformationServiceWrapper(new[] { fakeMcisData });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithManageCustomerInformationServiceWrapper(fakeManageCustomerInformationServiceWrapper)
                .Build<AccountEligibilityController>();

            // Required to use ConfigurationManager for now as the DefaultRegistry uses this.
            string allowEconomyMultiRateMeters = ConfigurationManager.AppSettings["AllowEconomyMultiRateMeters"];

            if (!string.IsNullOrEmpty(allowEconomyMultiRateMeters) && allowEconomyMultiRateMeters == "false")
            {
                TestHelper.GetResultUrlString(controller.CheckEligibility())
                    .ShouldEqual(isAcquisition ? "Acquisition" : "CustomerAccountIneligible");
            }
        }
    }
}