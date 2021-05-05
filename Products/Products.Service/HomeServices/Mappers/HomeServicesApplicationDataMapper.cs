using Products.Infrastructure;
using Products.Model.HomeServices;
using Products.Service.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Products.Service.HomeServices.Mappers
{
    public class HomeServicesApplicationDataMapper
    {
        private readonly ICryptographyService _cryptographyService;

        public HomeServicesApplicationDataMapper(ICryptographyService cryptographyService)
        {
            Guard.Against<ArgumentException>(cryptographyService == null, $"{nameof(cryptographyService)} is null");

            _cryptographyService = cryptographyService;
        }

        private ApplicationData GetApplicationData(HomeServicesCustomer customer)
        {
            return new ApplicationData()
            {
                Title = _cryptographyService.EncryptHomeServicesValue(customer.PersonalDetails.Title),
                Surname = customer.PersonalDetails.LastName,
                FirstName = _cryptographyService.EncryptHomeServicesValue(customer.PersonalDetails.FirstName),
                HouseNameNumber = _cryptographyService.EncryptHomeServicesValue(customer.SelectedCoverAddress.HouseName),
                AddressLine1 =
                    string.IsNullOrEmpty(customer.SelectedCoverAddress.AddressLine1)
                        ? null
                        : _cryptographyService.EncryptHomeServicesValue(customer.SelectedCoverAddress.AddressLine1),
                AddressLine2 =
                    string.IsNullOrEmpty(customer.SelectedCoverAddress.AddressLine2)
                        ? null
                        : _cryptographyService.EncryptHomeServicesValue(customer.SelectedCoverAddress.AddressLine2),
                Town =
                    string.IsNullOrEmpty(customer.SelectedCoverAddress.Town)
                        ? null
                        : _cryptographyService.EncryptHomeServicesValue(customer.SelectedCoverAddress.Town),
                County =
                    string.IsNullOrEmpty(customer.SelectedCoverAddress.County)
                        ? null
                        : _cryptographyService.EncryptHomeServicesValue(customer.SelectedCoverAddress.County),
                Postcode = _cryptographyService.EncryptHomeServicesValue(customer.CoverPostcode),
                DaytimePhoneNo =
                    _cryptographyService.EncryptHomeServicesValue(customer.ContactDetails.ContactNumber),
                EmailAddress = _cryptographyService.EncryptHomeServicesValue(customer.ContactDetails.EmailAddress),
                BankName = _cryptographyService.EncryptHomeServicesValue(customer.DirectDebitDetails.BankName),
                AccountHolder =
                    _cryptographyService.EncryptHomeServicesValue(customer.DirectDebitDetails.AccountName),
                SortCode = _cryptographyService.EncryptHomeServicesValue(customer.DirectDebitDetails.SortCode),
                AccountNo = _cryptographyService.EncryptHomeServicesValue(customer.DirectDebitDetails.AccountNumber),
                PaymentDay = customer.DirectDebitDetails.DirectDebitPaymentDate.ToString(),
                NoMarketing = customer.ContactDetails.MarketingConsent ? "N" : "Y",
                BillingHouseNameOrNumber =
                (customer.SelectedBillingAddress == null ||
                 string.IsNullOrEmpty(customer.SelectedBillingAddress.HouseName))
                    ? null
                    : _cryptographyService.EncryptHomeServicesValue(customer.SelectedBillingAddress.HouseName),
                BillingAddressLine1 =
                (customer.SelectedBillingAddress == null ||
                 string.IsNullOrEmpty(customer.SelectedBillingAddress.AddressLine1))
                    ? null
                    : _cryptographyService.EncryptHomeServicesValue(customer.SelectedBillingAddress.AddressLine1),
                BillingAddressLine2 =
                (customer.SelectedBillingAddress == null ||
                 string.IsNullOrEmpty(customer.SelectedBillingAddress.AddressLine2))
                    ? null
                    : _cryptographyService.EncryptHomeServicesValue(customer.SelectedBillingAddress.AddressLine2),
                BillingTown =
                (customer.SelectedBillingAddress == null ||
                 string.IsNullOrEmpty(customer.SelectedBillingAddress.Town))
                    ? null
                    : _cryptographyService.EncryptHomeServicesValue(customer.SelectedBillingAddress.Town),
                BillingCounty =
                (customer.SelectedBillingAddress == null ||
                 string.IsNullOrEmpty(customer.SelectedBillingAddress.County))
                    ? null
                    : _cryptographyService.EncryptHomeServicesValue(customer.SelectedBillingAddress.County),
                BillingPostcode =
                    string.IsNullOrEmpty(customer.BillingPostcode)
                        ? null
                        : _cryptographyService.EncryptHomeServicesValue(customer.BillingPostcode),
                PromoCodes = null,
                MobilePhoneNo = null,
                AccountNumber = null,
                IsSignupWithEnergy = false,
                Affiliate = customer.CampaignCode
            };
        }

        public ApplicationData GetHomeServicesDataModel(HomeServicesCustomer homeServicesCustomer)
        {
            var applicationData = GetApplicationData(homeServicesCustomer);
            var selectedProduct = homeServicesCustomer.GetSelectedProduct();
            var selectedExtras = homeServicesCustomer.GetSelectedExtras();

            applicationData.ProductData = new List<ProductData>() {
                new ProductData()
                {
                    Products = selectedProduct.Id.ToString(),
                    InitialCost = selectedProduct.MonthlyCost,
                    TotalCost = selectedProduct.MonthlyCost,
                    Discount = 0
                }
            };

            applicationData.ProductData.AddRange(selectedExtras.Select(p => new ProductData()
            {
                Products = p.Id.ToString(),
                InitialCost = p.Cost,
                TotalCost = p.Cost,
                Discount = 0
            }).ToList());

            return applicationData;
        }
    }
}
