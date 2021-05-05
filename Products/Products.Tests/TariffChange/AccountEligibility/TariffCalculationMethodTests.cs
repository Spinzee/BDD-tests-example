namespace Products.Tests.TariffChange.AccountEligibility
{
    using System;
    using System.Globalization;
    using System.Web.Mvc;
    using Common.Fakes;
    using Fakes.Models;
    using Fakes.Services;
    using Helpers;
    using Model.TariffChange;
    using Model.TariffChange.Customers;
    using Model.TariffChange.Enums;
    using Model.TariffChange.Tariffs;
    using NUnit.Framework;
    using ServiceWrapper.AnnualEnergyReviewService;
    using ServiceWrapper.ManageCustomerInformationService;
    using Should;
    using Web.Areas.TariffChange.Controllers;
    using FuelType = Model.Enums.FuelType;
    using ServiceStatusType = Model.TariffChange.Customers.ServiceStatusType;

    [TestFixture]
    public class TariffCalculationMethodTests
    {
        [Test]
        public void ShouldReturnCorrectTariffCalculationMethodWhenConsumptionDetailsTypeIsSetToPreviousSAForecastIsInAERResponse()
        {
            //Arrange
            var fakeAerData = new FakeAERData
            {
                ConsumptionRuleDescription = consumptionDetailsTypeConsumptionRuleDescription.previoussaforecast
            };

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

            //Act
            controller.CheckEligibility();

            //Assert
            TariffCalculationMethod tariffCalculationMethodResult = fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.TariffCalculationMethod;
            tariffCalculationMethodResult.ShouldEqual(TariffCalculationMethod.CurrentRate);
        }

        [Test]
        [TestCase(5, 5, TariffCalculationMethod.Original)]
        [TestCase(0, 1, TariffCalculationMethod.Original)]
        public void ShouldReturnCorrectTariffCalculationMethodEnumWhenAnnouncementDateHasNotPassed(int futureDays, int futureMins,
            TariffCalculationMethod tariffCalculationMethod)
        {
            //Arrange
            var journeyDetails = new JourneyDetails
            {
                CustomerAccount = new CustomerAccount
                {
                    CurrentTariff = new CurrentTariffForFuel
                    {
                        Name = "Fixed",
                        BrandCode = "SSE",
                        FuelType = FuelType.Electricity
                    },
                    SiteDetails = new SiteDetails
                    {
                        HasSingleActiveEnergyServiceAccount = true,
                        AccountNumber = "7158604317",
                        ServiceStatusType = ServiceStatusType.Active
                    },
                    LastBilledDate = DateTime.MinValue
                }
            };

            string accountNo = journeyDetails.CustomerAccount.SiteDetails.AccountNumber;

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(journeyDetails);
            var fakeAnnualEnergyReviewServiceWrapper =
                new FakeAnnualEnergyReviewServiceWrapper(accountNo, new[] { new FakeAERData { CustomerAccountNumber = accountNo }, new FakeAERData() });

            var fakeMcisData = new FakeMCISData
            {
                CustomerAccountNumber = "1111111113",
                Postcode = "SO14 2FJ",
                ServicePlanDescription = "Fixed",
                ServiceStatus = ServiceWrapper.ManageCustomerInformationService.ServiceStatusType.Active
            };

            var fakeManageCustomerInformationServiceWrapper = new FakeManageCustomerInformationServiceWrapper(new[] { fakeMcisData });

            DateTime futureAnnouncementDate = DateTime.Now.AddDays(futureDays).AddMinutes(futureMins);

            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AtlasAnnouncementDate", futureAnnouncementDate.ToString(CultureInfo.CurrentCulture));

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithManageCustomerInformationServiceWrapper(fakeManageCustomerInformationServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<AccountEligibilityController>();

            ActionResult result = controller.CheckEligibility();

            result.ShouldNotBeNull();

            TariffCalculationMethod tariffCalculationMethodResult = fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.TariffCalculationMethod;
            tariffCalculationMethodResult.ShouldEqual(tariffCalculationMethod);
        }

        [Test]
        [TestCase(5, TariffCalculationMethod.Original)]
        [TestCase(0, TariffCalculationMethod.CurrentRate)]
        [TestCase(-5, TariffCalculationMethod.CurrentRate)]
        public void ShouldReturnCorrectTariffCalculationMethodEnumWhenAnnouncementDateHasPassedAndCustomerHasBeenBilled(int numberOfDaysDifferenceFromAtlasDate, TariffCalculationMethod tariffCalculationMethod)
        {
            //Arrange
            DateTime atlasAnnouncementDate = DateTime.Today.AddDays(-5);
            DateTime lastBilledDate = atlasAnnouncementDate.AddDays(numberOfDaysDifferenceFromAtlasDate);

            var journeyDetails = new JourneyDetails
            {
                CustomerAccount = new CustomerAccount
                {
                    CurrentTariff = new CurrentTariffForFuel
                    {
                        Name = "Fixed",
                        BrandCode = "SSE",
                        FuelType = FuelType.Electricity
                    },
                    SiteDetails = new SiteDetails
                    {
                        HasSingleActiveEnergyServiceAccount = true,
                        AccountNumber = "7158604317",
                        ServiceStatusType = ServiceStatusType.Active
                    },
                    LastBilledDate = lastBilledDate
                }
            };

            string accountNo = journeyDetails.CustomerAccount.SiteDetails.AccountNumber;

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(journeyDetails);
            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(accountNo, new[]
            {
                new FakeAERData
                    { CustomerAccountNumber = accountNo, EndDate = DateTime.Today },
                new FakeAERData { CustomerAccountNumber = "9111111113", EndDate = DateTime.Today }
            });

            var fakeMcisData = new FakeMCISData
            {
                CustomerAccountNumber = "9111111113",
                Postcode = "SO14 2FJ",
                ServicePlanDescription = "Fixed",
                ServiceStatus = ServiceWrapper.ManageCustomerInformationService.ServiceStatusType.Active,
                Service = ServiceTypeType.gas
            };

            var fakeManageCustomerInformationServiceWrapper = new FakeManageCustomerInformationServiceWrapper(new[] { fakeMcisData });

            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AtlasAnnouncementDate", atlasAnnouncementDate.ToString(CultureInfo.CurrentCulture));

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithManageCustomerInformationServiceWrapper(fakeManageCustomerInformationServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<AccountEligibilityController>();

            ActionResult result = controller.CheckEligibility();

            result.ShouldNotBeNull();

            TariffCalculationMethod tariffCalculationMethodResult = fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.TariffCalculationMethod;
            tariffCalculationMethodResult.ShouldEqual(tariffCalculationMethod);
        }

        [Test]
        public void ShouldReturnCorrectTariffCalculationMethodEnumWhenAnnouncementDateHasPassedAndCustomerHasNotBeenBilled()
        {
            //Arrange
            DateTime atlasAnnouncementDate = DateTime.Today.AddDays(-5);
            DateTime lastBilledDate = DateTime.MaxValue;

            var journeyDetails = new JourneyDetails
            {
                CustomerAccount = new CustomerAccount
                {
                    CurrentTariff = new CurrentTariffForFuel
                    {
                        Name = "Fixed",
                        BrandCode = "SSE",
                        FuelType = FuelType.Electricity
                    },
                    SiteDetails = new SiteDetails
                    {
                        HasSingleActiveEnergyServiceAccount = true,
                        AccountNumber = "7158604317",
                        ServiceStatusType = ServiceStatusType.Active
                    },
                    LastBilledDate = lastBilledDate
                }
            };

            string accountNo = journeyDetails.CustomerAccount.SiteDetails.AccountNumber;

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(journeyDetails);
            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(accountNo, new[]
            {
                new FakeAERData
                    { CustomerAccountNumber = accountNo, EndDate = DateTime.Today },
                new FakeAERData { CustomerAccountNumber = "9111111113", EndDate = DateTime.Today }
            });

            var fakeMcisData = new FakeMCISData
            {
                CustomerAccountNumber = "9111111113",
                Postcode = "SO14 2FJ",
                ServicePlanDescription = "Fixed",
                ServiceStatus = ServiceWrapper.ManageCustomerInformationService.ServiceStatusType.Active,
                Service = ServiceTypeType.gas
            };

            var fakeManageCustomerInformationServiceWrapper = new FakeManageCustomerInformationServiceWrapper(new[] { fakeMcisData });

            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AtlasAnnouncementDate", atlasAnnouncementDate.ToString(CultureInfo.CurrentCulture));

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithManageCustomerInformationServiceWrapper(fakeManageCustomerInformationServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<AccountEligibilityController>();

            ActionResult result = controller.CheckEligibility();

            result.ShouldNotBeNull();

            TariffCalculationMethod tariffCalculationMethodResult = fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.TariffCalculationMethod;
            tariffCalculationMethodResult.ShouldEqual(TariffCalculationMethod.Original);
        }
    }
}