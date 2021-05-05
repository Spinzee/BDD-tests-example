namespace Products.Tests.TariffChange.AccountEligibility
{
    using System;
    using Common.Fakes;
    using Common.Helpers;
    using Fakes.Models;
    using Fakes.Services;
    using Helpers;
    using Model.Enums;
    using Model.TariffChange;
    using Model.TariffChange.Customers;
    using NUnit.Framework;
    using Should;
    using Web.Areas.TariffChange.Controllers;

    public class ServicePlanValidationTests
    {
        [TestCase("Y", false, "When service plan is MAndS account is ineligible")]
        [TestCase("N", true, "When service plan is not MAndS account is eligible")]
        public void ServicePlanIneligibleWhenMAndS(string isMAndS, bool isValid, string description)
        {
            // Arrange
            // ReSharper disable once UseObjectOrCollectionInitializer
            var fakeAerData = new FakeAERData();
            fakeAerData.CustomerAccountVariables["M&SBrand"] = isMAndS;
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

        [Test]
        public void WhenMAndSGoToGenericFalloutRatherThanRenewalsOrAcquisition()
        {
            // Arrange
            // ReSharper disable once UseObjectOrCollectionInitializer
            var fakeAerData = new FakeAERData
            {
                EndDate = DateTime.Today.AddDays(59)
            };
            fakeAerData.CustomerAccountVariables["M&SBrand"] = "Y";
            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(fakeAerData.CustomerAccountNumber, new[] { fakeAerData });

            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.SiteDetails.AccountNumber = fakeAerData.CustomerAccountNumber;
            customerAccount.CurrentTariff.Name = "M&S Standard Economy 7";
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
            TestHelper.GetResultUrlString(controller.CheckEligibility()).ShouldEqual("CustomerAccountIneligible");
        }

        [Test]
        [TestCase("Y", false, "When service plan is Combined Heat + Power account is ineligible")]
        [TestCase("N", true, "When service plan is not Combined Heat + Power account is eligible")]
        public void ServicePlanIneligibleWhenCombinedHeatAndPower(string isCombinedHeatingAndPower, bool isValid, string description)
        {
            // Arrange
            // ReSharper disable once UseObjectOrCollectionInitializer
            var fakeAerData = new FakeAERData();
            fakeAerData.CustomerAccountVariables["CHPAccount"] = isCombinedHeatingAndPower;
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

        [Test, Description("Dual Fuel account with different tariffs on each fuel should be ineligible.")]
        [TestCase("Standard", "Standard", true)]
        [TestCase("Standard", "SSE 1 Year Fixed", false)]
        public void DualFuelWithDifferentTariffPerFuelShouldBeIneligible(string tariffOne, string tariffTwo, bool isValid)
        {
            // Arrange
            var fakeAerData1 = new FakeAERData();
            var fakeAerData2 = new FakeAERData
            {
                CustomerAccountNumber = "2222222226"
            };
            var fakeAnnualEnergyReviewServiceWrapper =
                new FakeAnnualEnergyReviewServiceWrapper(fakeAerData1.CustomerAccountNumber, new[] { fakeAerData1, fakeAerData2 });
            var fakeMcisData = new FakeMCISData
            {
                CustomerAccountNumber = "2222222226",
                ServicePlanDescription = tariffTwo
            };
            var fakeManageCustomerInformationServiceWrapper = new FakeManageCustomerInformationServiceWrapper(new[] { fakeMcisData });

            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.SiteDetails.AccountNumber = fakeAerData1.CustomerAccountNumber;
            customerAccount.CurrentTariff.Name = tariffOne;
            customerAccount.CurrentTariff.FuelType = FuelType.Gas;
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                CustomerAccount = customerAccount
            });
            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AtlasAnnouncementDate", "30/04/2018 9:00");

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithManageCustomerInformationServiceWrapper(fakeManageCustomerInformationServiceWrapper)
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
    }
}