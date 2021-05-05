namespace Products.Tests.TariffChange.AccountEligibility
{
    using System.Web.Mvc;
    using Common.Fakes;
    using Fakes.Models;
    using Fakes.Services;
    using Helpers;
    using Model.TariffChange;
    using Model.TariffChange.Customers;
    using Model.TariffChange.Tariffs;
    using NUnit.Framework;
    using ServiceWrapper.ManageCustomerInformationService;
    using Should;
    using Web.Areas.TariffChange.Controllers;
    using FuelType = Model.Enums.FuelType;
    using ServiceStatusType = Model.TariffChange.Customers.ServiceStatusType;

    public class CheckEligibilityTests
    {
        [TestCase(ServiceStatusType.Active, ServiceStatusType.Active, "AvailableTariffs")]
        [TestCase(ServiceStatusType.Active, ServiceStatusType.Pending, "CustomerAccountIneligible")]
        [TestCase(ServiceStatusType.Active, ServiceStatusType.ProvisionallyFinalled, "CustomerAccountIneligible")]
        [TestCase(ServiceStatusType.Active, ServiceStatusType.Finalled, "CustomerAccountIneligible")]
        [TestCase(ServiceStatusType.Pending, ServiceStatusType.Pending, "CustomerAccountIneligible")]
        [TestCase(ServiceStatusType.Pending, ServiceStatusType.Active, "CustomerAccountIneligible")]
        [TestCase(ServiceStatusType.Pending, ServiceStatusType.ProvisionallyFinalled, "CustomerAccountIneligible")]
        [TestCase(ServiceStatusType.Pending, ServiceStatusType.Finalled, "CustomerAccountIneligible")]
        [TestCase(ServiceStatusType.ProvisionallyFinalled, ServiceStatusType.ProvisionallyFinalled, "CustomerAccountIneligible")]
        [TestCase(ServiceStatusType.ProvisionallyFinalled, ServiceStatusType.Active, "CustomerAccountIneligible")]
        [TestCase(ServiceStatusType.ProvisionallyFinalled, ServiceStatusType.Pending, "CustomerAccountIneligible")]
        [TestCase(ServiceStatusType.ProvisionallyFinalled, ServiceStatusType.Finalled, "CustomerAccountIneligible")]
        [TestCase(ServiceStatusType.Finalled, ServiceStatusType.Finalled, "CustomerAccountIneligible")]
        [TestCase(ServiceStatusType.Finalled, ServiceStatusType.Active, "CustomerAccountIneligible")]
        [TestCase(ServiceStatusType.Finalled, ServiceStatusType.Pending, "CustomerAccountIneligible")]
        [TestCase(ServiceStatusType.Finalled, ServiceStatusType.ProvisionallyFinalled, "CustomerAccountIneligible")]
        public void ShouldRedirectToCustomerAccountIneligiblePageForDuelFuelCustomersWhenBothAccountsAreNotActive(ServiceStatusType firstAccountStatusType, ServiceStatusType secondServiceStatusType, string expectedRedirectResult)
        {
            // Arrange
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
                        HasSingleActiveEnergyServiceAccount =
                            HasSingleActiveEnergyServiceAccount(true, firstAccountStatusType, CustomerAccountStatusType.Found, FuelType.Electricity),
                        AccountNumber = "7158604317",
                        ServiceStatusType = firstAccountStatusType
                    }
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
                ServiceStatus = (ServiceWrapper.ManageCustomerInformationService.ServiceStatusType) secondServiceStatusType
            };

            var fakeManageCustomerInformationServiceWrapper = new FakeManageCustomerInformationServiceWrapper(new[] { fakeMcisData });
            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AtlasAnnouncementDate", "30/04/2018 9:00");

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithManageCustomerInformationServiceWrapper(fakeManageCustomerInformationServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<AccountEligibilityController>();

            // Act
            ActionResult result = controller.CheckEligibility();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) result).RouteValues["action"].ShouldEqual(expectedRedirectResult);
        }

        private static bool HasSingleActiveEnergyServiceAccount(bool hasService, ServiceStatusType firstAccountStatusType, CustomerAccountStatusType customerAccountStatusType, FuelType fuelType)
        {
            return hasService && customerAccountStatusType == CustomerAccountStatusType.Found && firstAccountStatusType == ServiceStatusType.Active && fuelType != FuelType.None;
        }
    }
}