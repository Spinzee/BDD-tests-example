namespace Products.Tests.Energy
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Broadband.Fakes.Repository;
    using Broadband.Model;
    using Common.Fakes;
    using Common.Helpers;
    using Core;
    using Fakes.Repositories;
    using Helpers;
    using HomeServices.Fakes;
    using NUnit.Framework;
    using Products.Infrastructure.Extensions;
    using Products.Model.Broadband;
    using Products.Model.Common;
    using Products.Model.Constants;
    using Products.Model.Energy;
    using Products.Model.Enums;
    using Service.Security;
    using Should;
    using Web.Areas.Energy.Controllers;
    using WebModel.ViewModels.Energy;
    using ControllerFactory = Helpers.ControllerFactory;

    public class InsertEnergySalesTests
    {
        [TestCase(FuelType.Dual, ElectricityMeterType.Standard)]
        public async Task ApplicationObjectShouldPopulateWithValidDataWhenInsertingEnergySale(FuelType fuelType, ElectricityMeterType meterType)
        {
            // Arrange
            var cookieCollection = new HttpCookieCollection
            {
                new HttpCookie("migrateMemberid", "123456"), new HttpCookie("migrateAffiliateid", "affiliateCode1"),
                new HttpCookie("migrateCampaignid", "1410789843095")
            };
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(cookieCollection))
                .Build();
            FakeEnergySalesRepository fakeEnergyRepository = new FakeEnergySalesRepository().WithSubProducts();

            EnergyCustomer customer = CustomerFactory.GetCustomerWithFullDetails(fuelType, meterType);
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, FakeProductsStub.GetTariffs("Dual", "Standard"));

            var fakeSalesRepository = new FakeSalesRepository();
            var controller = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager)
                .WithFakeSalesRepository(fakeSalesRepository)
                .WithFakeEnergySalesRepository(fakeEnergyRepository)
                .Build<SignUpController>();

            var summaryViewModel = new SummaryViewModel();

            // Act
            ActionResult result = await controller.ViewSummary(summaryViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("Confirmation");
            routeResult.RouteValues["controller"].ShouldEqual(null);

            fakeSalesRepository.InsertSaleCount.ShouldEqual(1);
            fakeSalesRepository.InsertSaleSqlParameters["@Postcode"].ShouldEqual(customer.Postcode);
            fakeSalesRepository.InsertSaleSqlParameters["@AddressTypeID"].ShouldEqual(((int) AddressTypes.Supply).ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@HouseName"].ShouldEqual(null);
            fakeSalesRepository.InsertSaleSqlParameters["@HouseNumber"].ShouldEqual(customer.SelectedAddress.HouseName);
            fakeSalesRepository.InsertSaleSqlParameters["@PaymentMethodID"].ShouldEqual(((int) customer.SelectedPaymentMethod).ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@TariffTypeID"].ShouldEqual(SalesData.StandardTariffTypeId.ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@AccountName"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@SortCode"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@AccountNumber"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@DayInMonthPaymentMade"].ShouldEqual("0");
            fakeSalesRepository.InsertSaleSqlParameters["@MonthlyAmount"].ShouldEqual("0");
            fakeSalesRepository.InsertSaleSqlParameters["@BillingAddressTypeID"].ShouldEqual(((int) AddressTypes.Billing).ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@ElecSubProductID"].ShouldEqual("0");
            fakeSalesRepository.InsertSaleSqlParameters["@GasSubProductID"].ShouldEqual("0");
            fakeSalesRepository.InsertSaleSqlParameters["@DualSubProductID"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@ElecProductPresentButNoDual"].ShouldEqual("False");
            fakeSalesRepository.InsertSaleSqlParameters["@GasProductPresentButNoDual"].ShouldEqual("False");
            fakeSalesRepository.InsertSaleSqlParameters["@DualFuelProductPresent"].ShouldEqual("True");
            fakeSalesRepository.InsertSaleSqlParameters["@SingleBankAccChosen"].ShouldEqual("True");
            fakeSalesRepository.InsertSaleSqlParameters["@GenericPaymentMethodID"].ShouldEqual(((int) customer.SelectedPaymentMethod).ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@ElecPaymentMethodID"].ShouldEqual(((int) customer.SelectedPaymentMethod).ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@GasPaymentMethodID"].ShouldEqual(((int) customer.SelectedPaymentMethod).ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@GenericAccountName"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@GenericSortCode"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@GenericAccountNumber"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@ElecAccountName"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@ElecSortCode"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@ElecAccountNumber"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@ElecDayInMonthPaymentMade"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@ElecMonthlyAmount"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@GasAccountName"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@GasSortCode"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@GasAccountNumber"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@GasDayInMonthPaymentMade"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@GasMonthlyAmount"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@ElectricDayUsage"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@ElectricNightUsage"].ShouldEqual("0");
            fakeSalesRepository.InsertSaleSqlParameters["@GasUsage"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@ElectricMonetaryAmount"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@GasMonetaryAmount"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@CampaignCode"].ShouldEqual("US00CI");
            fakeSalesRepository.InsertSaleSqlParameters["@ConsultantID"].ShouldEqual("123456");
        }

        [TestCase(FuelType.Dual, ElectricityMeterType.Standard)]
        public async Task ApplicationObjectShouldPopulateWithValidDataWhenInsertingBroadbandBundleSale(FuelType fuelType, ElectricityMeterType meterType)
        {
            // Arrange
            var cookieCollection = new HttpCookieCollection
            {
                new HttpCookie("migrateMemberid", "123456"), new HttpCookie("migrateAffiliateid", "affiliateCode1"),
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

            var controller = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager)
                .WithFakeSalesRepository(fakeSalesRepository)
                .WithFakeEnergySalesRepository(fakeEnergyRepository)
                .WithBroadbandSalesRepository(fakeBroadbandRepository)
                .Build<SignUpController>();

            var summaryViewModel = new SummaryViewModel();

            // Act
            ActionResult result = await controller.ViewSummary(summaryViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("Confirmation");
            routeResult.RouteValues["controller"].ShouldEqual(null);

            var energyCustomer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            var openReachData = fakeSessionManager.GetSessionDetails<OpenReachData>(SessionKeys.OpenReachResponse);

            fakeSalesRepository.InsertSaleCount.ShouldEqual(1);
            fakeSalesRepository.InsertSaleSqlParameters["@Postcode"].ShouldEqual(customer.Postcode);
            fakeSalesRepository.InsertSaleSqlParameters["@AddressTypeID"].ShouldEqual(((int) AddressTypes.Supply).ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@HouseName"].ShouldEqual(energyCustomer.SelectedAddress.GetHouseName());
            fakeSalesRepository.InsertSaleSqlParameters["@HouseNumber"].ShouldEqual(customer.SelectedAddress.GetHouseNumber());
            fakeSalesRepository.InsertSaleSqlParameters["@PaymentMethodID"].ShouldEqual(((int) customer.SelectedPaymentMethod).ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@TariffTypeID"].ShouldEqual(SalesData.StandardTariffTypeId.ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@AccountName"].ShouldEqual("eV+9QQ8WpXY=");
            fakeSalesRepository.InsertSaleSqlParameters["@SortCode"].ShouldEqual("mEFaqn4htXs=");
            fakeSalesRepository.InsertSaleSqlParameters["@AccountNumber"].ShouldEqual("9VArcOUBcVB1ZYqUlagdYA==");
            fakeSalesRepository.InsertSaleSqlParameters["@DayInMonthPaymentMade"].ShouldEqual("0");
            fakeSalesRepository.InsertSaleSqlParameters["@MonthlyAmount"].ShouldEqual("0");
            fakeSalesRepository.InsertSaleSqlParameters["@BillingAddressTypeID"].ShouldEqual(((int) AddressTypes.Billing).ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@ElecSubProductID"].ShouldEqual("0");
            fakeSalesRepository.InsertSaleSqlParameters["@GasSubProductID"].ShouldEqual("0");
            fakeSalesRepository.InsertSaleSqlParameters["@DualSubProductID"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@ElecProductPresentButNoDual"].ShouldEqual("False");
            fakeSalesRepository.InsertSaleSqlParameters["@GasProductPresentButNoDual"].ShouldEqual("False");
            fakeSalesRepository.InsertSaleSqlParameters["@DualFuelProductPresent"].ShouldEqual("True");
            fakeSalesRepository.InsertSaleSqlParameters["@SingleBankAccChosen"].ShouldEqual("True");
            fakeSalesRepository.InsertSaleSqlParameters["@GenericPaymentMethodID"].ShouldEqual(((int) customer.SelectedPaymentMethod).ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@ElecPaymentMethodID"].ShouldEqual(((int) customer.SelectedPaymentMethod).ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@GasPaymentMethodID"].ShouldEqual(((int) customer.SelectedPaymentMethod).ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@GenericAccountName"].ShouldEqual("eV+9QQ8WpXY=");
            fakeSalesRepository.InsertSaleSqlParameters["@GenericSortCode"].ShouldEqual("mEFaqn4htXs=");
            fakeSalesRepository.InsertSaleSqlParameters["@GenericAccountNumber"].ShouldEqual("9VArcOUBcVB1ZYqUlagdYA==");
            fakeSalesRepository.InsertSaleSqlParameters["@ElecAccountName"].ShouldEqual("eV+9QQ8WpXY=");
            fakeSalesRepository.InsertSaleSqlParameters["@ElecSortCode"].ShouldEqual("mEFaqn4htXs=");
            fakeSalesRepository.InsertSaleSqlParameters["@ElecAccountNumber"].ShouldEqual("9VArcOUBcVB1ZYqUlagdYA==");
            fakeSalesRepository.InsertSaleSqlParameters["@ElecDayInMonthPaymentMade"]
                .ShouldEqual(energyCustomer.DirectDebitDetails.DirectDebitPaymentDate.ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@ElecMonthlyAmount"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@GasAccountName"].ShouldEqual("eV+9QQ8WpXY=");
            fakeSalesRepository.InsertSaleSqlParameters["@GasSortCode"].ShouldEqual("mEFaqn4htXs=");
            fakeSalesRepository.InsertSaleSqlParameters["@GasAccountNumber"].ShouldEqual("9VArcOUBcVB1ZYqUlagdYA==");
            fakeSalesRepository.InsertSaleSqlParameters["@GasDayInMonthPaymentMade"]
                .ShouldEqual(energyCustomer.DirectDebitDetails.DirectDebitPaymentDate.ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@GasMonthlyAmount"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@ElectricDayUsage"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@ElectricNightUsage"].ShouldEqual("0");
            fakeSalesRepository.InsertSaleSqlParameters["@GasUsage"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@ElectricMonetaryAmount"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@GasMonetaryAmount"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@CampaignCode"].ShouldEqual(energyCustomer.CampaignCode);
            fakeSalesRepository.InsertSaleSqlParameters["@CsUserGUID"].ShouldEqual(energyCustomer.UserId.ToString());

            fakeBroadbandRepository.InsertSaleCount.ShouldEqual(1);
            fakeBroadbandRepository.InsertSaleSqlParameters["AccountName"].ShouldEqual("eV+9QQ8WpXY=");
            fakeBroadbandRepository.InsertSaleSqlParameters["AccountNumber"].ShouldEqual("9VArcOUBcVB1ZYqUlagdYA==");
            fakeBroadbandRepository.InsertSaleSqlParameters["SortCode"].ShouldEqual("mEFaqn4htXs=");
            fakeBroadbandRepository.InsertSaleSqlParameters["AddressLine1"].ShouldEqual("London Road");
            fakeBroadbandRepository.InsertSaleSqlParameters["AddressLine2"].ShouldEqual("Earley");
            fakeBroadbandRepository.InsertSaleSqlParameters["AddressLine3"].ShouldEqual("Berkshire");
            fakeBroadbandRepository.InsertSaleSqlParameters["DateOfBirth"].ShouldEqual(DateTime.Parse(energyCustomer.PersonalDetails.DateOfBirth).ToString(CultureInfo.InvariantCulture));
            fakeBroadbandRepository.InsertSaleSqlParameters["DayPhone"].ShouldEqual(energyCustomer.CLIChoice.OpenReachProvidedCLI);
            fakeBroadbandRepository.InsertSaleSqlParameters["Email"].ShouldEqual(energyCustomer.ContactDetails.EmailAddress);
            fakeBroadbandRepository.InsertSaleSqlParameters["FirstName"].ShouldEqual(energyCustomer.PersonalDetails.FirstName);
            fakeBroadbandRepository.InsertSaleSqlParameters["HouseName"].ShouldBeNull();
            fakeBroadbandRepository.InsertSaleSqlParameters["HouseNumber"].ShouldEqual("123");
            fakeBroadbandRepository.InsertSaleSqlParameters["LineSpeed"].ShouldEqual("0");
            fakeBroadbandRepository.InsertSaleSqlParameters["MarketingConsent"].ShouldEqual(energyCustomer.ContactDetails.MarketingConsent.ToString());
            fakeBroadbandRepository.InsertSaleSqlParameters["Mobile"].ShouldEqual(energyCustomer.ContactDetails.ContactNumber);
            fakeBroadbandRepository.InsertSaleSqlParameters["Postcode"].ShouldEqual(energyCustomer.Postcode);
            fakeBroadbandRepository.InsertSaleSqlParameters["ProductCode"].ShouldEqual(energyCustomer.SelectedBroadbandProductCode);
            fakeBroadbandRepository.InsertSaleSqlParameters["Surname"].ShouldEqual(energyCustomer.PersonalDetails.LastName);
            fakeBroadbandRepository.InsertSaleSqlParameters["Title"].ShouldEqual(energyCustomer.PersonalDetails.Title);
            fakeBroadbandRepository.InsertSaleSqlParameters["Town"].ShouldEqual("Reading");
            fakeBroadbandRepository.InsertSaleSqlParameters["CampaignCode"].ShouldEqual(energyCustomer.CampaignCode);
            fakeBroadbandRepository.InsertSaleSqlParameters["CsUserGUID"].ShouldEqual(energyCustomer.UserId.ToString());

            fakeBroadbandRepository.InsertOpenreachAuditSqlParameters["ApplicationId"].ShouldEqual(energyCustomer.BroadbandApplicationId.ToString());
            fakeBroadbandRepository.InsertOpenreachAuditSqlParameters["LineStatus"].ShouldEqual(openReachData.LineStatus.GetDescription());
            fakeBroadbandRepository.InsertOpenreachAuditSqlParameters["Postode"].ShouldEqual(energyCustomer.Postcode);
            fakeBroadbandRepository.InsertOpenreachAuditSqlParameters["HouseName"].ShouldEqual(energyCustomer.SelectedBTAddress.FormattedAddressLine1);
            fakeBroadbandRepository.InsertOpenreachAuditSqlParameters["CLI"].ShouldEqual(energyCustomer.CLIChoice.FinalCLI);
            fakeBroadbandRepository.InsertOpenreachAuditSqlParameters["AddressLineKey"].ShouldEqual(openReachData.AddressLineKey);


            fakeBroadbandRepository.InsertAuditSqlParameters["CLIProvided"].ShouldEqual((!string.IsNullOrEmpty(energyCustomer.CLIChoice.FinalCLI)).ToString());
            fakeBroadbandRepository.InsertAuditSqlParameters["ProductCode"].ShouldEqual(energyCustomer.SelectedBroadbandProductCode);
            fakeBroadbandRepository.InsertAuditSqlParameters["ConnectionCharge"].ShouldEqual("0");
            fakeBroadbandRepository.InsertAuditSqlParameters["InstallationCharge"].ShouldEqual("0");
            fakeBroadbandRepository.InsertAuditSqlParameters["IsSSECustomer"].ShouldEqual(energyCustomer.IsSSECustomerCLI.ToString());
            fakeBroadbandRepository.InsertSaleCount.ShouldEqual(1);
            fakeBroadbandRepository.InsertAuditCount.ShouldEqual(1);
            fakeBroadbandRepository.InsertOpenreachAuditCount.ShouldEqual(1);
            
        }


        [TestCase(FuelType.Dual, ElectricityMeterType.Standard)]
        public async Task ApplicationObjectShouldPopulateWithValidDataWhenInsertingHesBundleSale(FuelType fuelType, ElectricityMeterType meterType)
        {
            // Arrange
            var cookieCollection = new HttpCookieCollection
            {
                new HttpCookie("migrateMemberid", "123456"), new HttpCookie("migrateAffiliateid", "affiliateCode1"),
                new HttpCookie("migrateCampaignid", "1410789843095")
            };
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(cookieCollection))
                .Build();
            FakeEnergySalesRepository fakeEnergyRepository = new FakeEnergySalesRepository().WithSubProducts();

            EnergyCustomer customer = CustomerFactory.CustomerDetailsWithFixAndProtectWithExtraTariffChosen(fuelType, meterType);

            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.OpenReachResponse, FakeOpenReachData.GetOpenReachData());

            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, FakeProductsStub.GetTariffs("Dual", "Standard"));

            var fakeSalesRepository = new FakeSalesRepository();
            var fakeHomeServicesSalesRepository = new FakeHomeServicesSalesRepository();
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultHomeServices();
            ICryptographyService cryptographyService = new CryptographyService(fakeConfigManager);

            var controller = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager)
                .WithFakeSalesRepository(fakeSalesRepository)
                .WithFakeEnergySalesRepository(fakeEnergyRepository)
                .WithHomeServicesSalesRepository(fakeHomeServicesSalesRepository)
                .Build<SignUpController>();

            var summaryViewModel = new SummaryViewModel();

            // Act
            ActionResult result = await controller.ViewSummary(summaryViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("Confirmation");
            routeResult.RouteValues["controller"].ShouldEqual(null);

            var energyCustomer = fakeSessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);

            fakeSalesRepository.InsertSaleCount.ShouldEqual(1);
            fakeSalesRepository.InsertSaleSqlParameters["@Postcode"].ShouldEqual(customer.Postcode);
            fakeSalesRepository.InsertSaleSqlParameters["@AddressTypeID"].ShouldEqual(((int)AddressTypes.Supply).ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@HouseName"].ShouldEqual(energyCustomer.SelectedAddress.GetHouseName());
            fakeSalesRepository.InsertSaleSqlParameters["@HouseNumber"].ShouldEqual(customer.SelectedAddress.GetHouseNumber());
            fakeSalesRepository.InsertSaleSqlParameters["@PaymentMethodID"].ShouldEqual(((int)customer.SelectedPaymentMethod).ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@TariffTypeID"].ShouldEqual(SalesData.StandardTariffTypeId.ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@AccountName"].ShouldEqual("eV+9QQ8WpXY=");
            fakeSalesRepository.InsertSaleSqlParameters["@SortCode"].ShouldEqual("mEFaqn4htXs=");
            fakeSalesRepository.InsertSaleSqlParameters["@AccountNumber"].ShouldEqual("9VArcOUBcVB1ZYqUlagdYA==");
            fakeSalesRepository.InsertSaleSqlParameters["@DayInMonthPaymentMade"].ShouldEqual("0");
            fakeSalesRepository.InsertSaleSqlParameters["@MonthlyAmount"].ShouldEqual("0");
            fakeSalesRepository.InsertSaleSqlParameters["@BillingAddressTypeID"].ShouldEqual(((int)AddressTypes.Billing).ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@ElecSubProductID"].ShouldEqual("0");
            fakeSalesRepository.InsertSaleSqlParameters["@GasSubProductID"].ShouldEqual("0");
            fakeSalesRepository.InsertSaleSqlParameters["@DualSubProductID"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@ElecProductPresentButNoDual"].ShouldEqual("False");
            fakeSalesRepository.InsertSaleSqlParameters["@GasProductPresentButNoDual"].ShouldEqual("False");
            fakeSalesRepository.InsertSaleSqlParameters["@DualFuelProductPresent"].ShouldEqual("True");
            fakeSalesRepository.InsertSaleSqlParameters["@SingleBankAccChosen"].ShouldEqual("True");
            fakeSalesRepository.InsertSaleSqlParameters["@GenericPaymentMethodID"].ShouldEqual(((int)customer.SelectedPaymentMethod).ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@ElecPaymentMethodID"].ShouldEqual(((int)customer.SelectedPaymentMethod).ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@GasPaymentMethodID"].ShouldEqual(((int)customer.SelectedPaymentMethod).ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@GenericAccountName"].ShouldEqual("eV+9QQ8WpXY=");
            fakeSalesRepository.InsertSaleSqlParameters["@GenericSortCode"].ShouldEqual("mEFaqn4htXs=");
            fakeSalesRepository.InsertSaleSqlParameters["@GenericAccountNumber"].ShouldEqual("9VArcOUBcVB1ZYqUlagdYA==");
            fakeSalesRepository.InsertSaleSqlParameters["@ElecAccountName"].ShouldEqual("eV+9QQ8WpXY=");
            fakeSalesRepository.InsertSaleSqlParameters["@ElecSortCode"].ShouldEqual("mEFaqn4htXs=");
            fakeSalesRepository.InsertSaleSqlParameters["@ElecAccountNumber"].ShouldEqual("9VArcOUBcVB1ZYqUlagdYA==");
            fakeSalesRepository.InsertSaleSqlParameters["@ElecDayInMonthPaymentMade"]
                .ShouldEqual(energyCustomer.DirectDebitDetails.DirectDebitPaymentDate.ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@ElecMonthlyAmount"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@GasAccountName"].ShouldEqual("eV+9QQ8WpXY=");
            fakeSalesRepository.InsertSaleSqlParameters["@GasSortCode"].ShouldEqual("mEFaqn4htXs=");
            fakeSalesRepository.InsertSaleSqlParameters["@GasAccountNumber"].ShouldEqual("9VArcOUBcVB1ZYqUlagdYA==");
            fakeSalesRepository.InsertSaleSqlParameters["@GasDayInMonthPaymentMade"]
                .ShouldEqual(energyCustomer.DirectDebitDetails.DirectDebitPaymentDate.ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@GasMonthlyAmount"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@ElectricDayUsage"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@ElectricNightUsage"].ShouldEqual("0");
            fakeSalesRepository.InsertSaleSqlParameters["@GasUsage"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@ElectricMonetaryAmount"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@GasMonetaryAmount"].ShouldNotBeNull();
            fakeSalesRepository.InsertSaleSqlParameters["@CampaignCode"].ShouldEqual(energyCustomer.CampaignCode);
            fakeSalesRepository.InsertSaleSqlParameters["@CsUserGUID"].ShouldEqual(energyCustomer.UserId.ToString());
            fakeSalesRepository.InsertSaleSqlParameters["@CsUserGUID"].ShouldNotEqual(Guid.Empty.ToString());

            fakeHomeServicesSalesRepository.ApplicationData.BankName.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.DirectDebitDetails.BankName));
            fakeHomeServicesSalesRepository.ApplicationData.AccountHolder.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.DirectDebitDetails.AccountName));
            fakeHomeServicesSalesRepository.ApplicationData.AccountNo.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.DirectDebitDetails.AccountNumber));
            fakeHomeServicesSalesRepository.ApplicationData.SortCode.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.DirectDebitDetails.SortCode));
            fakeHomeServicesSalesRepository.ApplicationData.PaymentDay.ShouldEqual(customer.DirectDebitDetails.DirectDebitPaymentDate.ToString());
           
            fakeHomeServicesSalesRepository.ApplicationData.HouseNameNumber.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.SelectedAddress.HouseName));
            fakeHomeServicesSalesRepository.ApplicationData.AddressLine1.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.SelectedAddress.AddressLine1));
            fakeHomeServicesSalesRepository.ApplicationData.AddressLine2.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.SelectedAddress.AddressLine2));
            fakeHomeServicesSalesRepository.ApplicationData.Town.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.SelectedAddress.Town));
            fakeHomeServicesSalesRepository.ApplicationData.County.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.SelectedAddress.County));
            fakeHomeServicesSalesRepository.ApplicationData.Postcode.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.Postcode));
            
            fakeHomeServicesSalesRepository.ApplicationData.Title.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.PersonalDetails.Title));
            fakeHomeServicesSalesRepository.ApplicationData.FirstName.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.PersonalDetails.FirstName));
            fakeHomeServicesSalesRepository.ApplicationData.Surname.ShouldEqual(customer.PersonalDetails.LastName);
            
            fakeHomeServicesSalesRepository.ApplicationData.DaytimePhoneNo.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.ContactDetails.ContactNumber));
            fakeHomeServicesSalesRepository.ApplicationData.EmailAddress.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.ContactDetails.EmailAddress));
            fakeHomeServicesSalesRepository.ApplicationData.MobilePhoneNo.ShouldBeNull();
            fakeHomeServicesSalesRepository.ApplicationData.NoMarketing.ShouldEqual(customer.ContactDetails.MarketingConsent ? "N" : "Y");
            
            fakeHomeServicesSalesRepository.ApplicationData.IsSignupWithEnergy.ShouldBeFalse();
            fakeHomeServicesSalesRepository.ApplicationData.PromoCodes.ShouldEqual("Fix and Protect Bundle Upsell");
            fakeHomeServicesSalesRepository.ApplicationData.AccountNumber.ShouldBeNull();
            fakeHomeServicesSalesRepository.ApplicationData.ProductData.Count.ShouldEqual(1);
            fakeHomeServicesSalesRepository.InsertSaleCount.ShouldEqual(1);
        }




        [Test]
        public void ShouldRedirectToErrorAndLogErrorMessageWhenExceptionThrownByDatabase()
        {
            // Arrange 
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(new HttpCookieCollection()))
                .Build();

            EnergyCustomer customer = CustomerFactory.GetCustomerWithFullDetails(FuelType.Dual, ElectricityMeterType.Standard);
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);
            fakeSessionManager.SessionObject.Add(SessionKeys.AvailableEnergyTariffs, FakeProductsStub.GetTariffs("Dual", "Standard"));

            // Act
            var controller = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager)
                .WithFakeSalesRepository(new FakeSalesRepository { Exception = new Exception("Database Exception Error") })
                .Build<SignUpController>();

            var summaryViewModel = new SummaryViewModel();

            // Act
            var ex = Assert.ThrowsAsync<Exception>(() => controller.ViewSummary(summaryViewModel));
            ex.Message.ShouldNotBeNull();
            ex.Message.ShouldNotBeEmpty();
        }
    }
}