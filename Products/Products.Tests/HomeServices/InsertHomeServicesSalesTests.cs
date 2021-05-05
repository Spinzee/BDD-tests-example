namespace Products.Tests.HomeServices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Fakes;
    using Helpers;
    using Model.Constants;
    using NUnit.Framework;
    using Products.Model.HomeServices;
    using Products.Tests.Common.Fakes;
    using Products.Tests.Common.Helpers;
    using Products.Web.Areas.HomeServices.Controllers;
    using Products.WebModel.ViewModels.HomeServices;
    using Service.Security;
    using Should;

    public class InsertHomeServicesSalesTests
    {
        [TestCaseSource(nameof(GetHomeServicesCustomerObject))]
        public async Task ApplicationObjectShouldPopulateWithValidDataWhenInsertingHomeServicesSale(HomeServicesCustomer customer, int expectedNumberOfSales)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, customer);
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultHomeServices();
            ICryptographyService cryptographyService = new CryptographyService(fakeConfigManager);

            var fakeSalesRepository = new FakeHomeServicesSalesRepository();
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithConfigManager(fakeConfigManager)
                .WithFakeSalesRepository(fakeSalesRepository)
                .Build<HomeServicesController>();

            var summaryViewModel = new SummaryViewModel();

            // Act
            ActionResult result = await controller.Summary(summaryViewModel);

            // Assert
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("Confirmation");

            fakeSalesRepository.ApplicationData.BankName.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.DirectDebitDetails.BankName));
            fakeSalesRepository.ApplicationData.AccountHolder.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.DirectDebitDetails.AccountName));
            fakeSalesRepository.ApplicationData.AccountNo.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.DirectDebitDetails.AccountNumber));
            fakeSalesRepository.ApplicationData.SortCode.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.DirectDebitDetails.SortCode));
            fakeSalesRepository.ApplicationData.PaymentDay.ShouldEqual(customer.DirectDebitDetails.DirectDebitPaymentDate.ToString());

            fakeSalesRepository.ApplicationData.HouseNameNumber.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.SelectedCoverAddress.HouseName));
            fakeSalesRepository.ApplicationData.AddressLine1.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.SelectedCoverAddress.AddressLine1));
            fakeSalesRepository.ApplicationData.AddressLine2.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.SelectedCoverAddress.AddressLine2));
            fakeSalesRepository.ApplicationData.Town.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.SelectedCoverAddress.Town));
            fakeSalesRepository.ApplicationData.County.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.SelectedCoverAddress.County));
            fakeSalesRepository.ApplicationData.Postcode.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.CoverPostcode));
            
            fakeSalesRepository.ApplicationData.BillingAddressLine1.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.SelectedBillingAddress.AddressLine1));
            fakeSalesRepository.ApplicationData.BillingAddressLine2.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.SelectedBillingAddress.AddressLine2));
            fakeSalesRepository.ApplicationData.BillingCounty.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.SelectedBillingAddress.County));
            fakeSalesRepository.ApplicationData.BillingHouseNameOrNumber.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.SelectedBillingAddress.HouseName));
            fakeSalesRepository.ApplicationData.BillingPostcode.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.BillingPostcode));
            fakeSalesRepository.ApplicationData.BillingTown.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.SelectedBillingAddress.Town));

            fakeSalesRepository.ApplicationData.Title.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.PersonalDetails.Title));
            fakeSalesRepository.ApplicationData.FirstName.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.PersonalDetails.FirstName));
            fakeSalesRepository.ApplicationData.Surname.ShouldEqual(customer.PersonalDetails.LastName);

            fakeSalesRepository.ApplicationData.DaytimePhoneNo.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.ContactDetails.ContactNumber));
            fakeSalesRepository.ApplicationData.EmailAddress.ShouldEqual(cryptographyService.EncryptHomeServicesValue(customer.ContactDetails.EmailAddress));
            fakeSalesRepository.ApplicationData.MobilePhoneNo.ShouldBeNull();
            fakeSalesRepository.ApplicationData.NoMarketing.ShouldEqual(customer.ContactDetails.MarketingConsent ? "N" : "Y");
            
            fakeSalesRepository.ApplicationData.IsSignupWithEnergy.ShouldBeFalse();
            fakeSalesRepository.ApplicationData.PromoCodes.ShouldBeNull();
            fakeSalesRepository.ApplicationData.AccountNumber.ShouldBeNull();
            fakeSalesRepository.ApplicationData.ProductData.Count.ShouldEqual(FakeCustomerFactory.GetProductDataFromHomeServicesCustomer(customer).Count);
            fakeSalesRepository.InsertSaleCount.ShouldEqual(expectedNumberOfSales);
        }

        private static IEnumerable<TestCaseData> GetHomeServicesCustomerObject()
        {
            yield return new TestCaseData(FakeCustomerFactory.GetHomeservicesCustomerWithExtras(), 2);
            yield return new TestCaseData(FakeCustomerFactory.GetHomeservicesCustomerWithoutExtras(), 1);
        }
    }
}
