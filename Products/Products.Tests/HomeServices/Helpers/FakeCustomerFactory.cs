namespace Products.Tests.HomeServices.Helpers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Model.Common;
    using Model.HomeServices;

    public static class FakeCustomerFactory
    {
        public static HomeServicesCustomer GetHomeservicesCustomerWithExtras()
        {
            return new HomeServicesCustomer
            {
                AvailableProduct = FakeHomeServicesProductStub.GetFakeProducts("BOBC"),
                BankServiceRetryCount = 0,
                BillingPostcode = "PO91BH",
                ContactDetails = new ContactDetails
                {
                    EmailAddress = "a@a.com",
                    ContactNumber = "012121212121",
                    MarketingConsent = true
                },
                DirectDebitDetails = GetFakeDirectDebitDetails(),
                PersonalDetails = GetFakePersonalDetails(),
                CoverPostcode = "RG249SS",
                SelectedCoverAddress = GetFakeAddress(),
                SelectedBillingAddress = GetFakeAddress(),
                SelectedExtraCodes = new List<string> { "EC" },
                SelectedProductCode = "BOBC",
                IsLandlord = true
            };
        }

        public static HomeServicesCustomer GetHomeservicesCustomerWithoutExtras()
        {
            return new HomeServicesCustomer
            {
                AvailableProduct = FakeHomeServicesProductStub.GetFakeProducts("BC"),
                BankServiceRetryCount = 0,
                BillingPostcode = "PO91BH",
                ContactDetails = new ContactDetails
                {
                    EmailAddress = "a@a.com",
                    ContactNumber = "012121212121",
                    MarketingConsent = true
                },
                DirectDebitDetails = GetFakeDirectDebitDetails(),
                PersonalDetails = GetFakePersonalDetails(),
                CoverPostcode = "RG249SS",
                SelectedCoverAddress = GetFakeAddress(),
                SelectedBillingAddress = GetFakeAddress(),
                SelectedExtraCodes = new List<string>(),
                SelectedProductCode = "BC50",
                IsLandlord = true
            };
        }

        public static List<ProductData> GetProductDataFromHomeServicesCustomer(HomeServicesCustomer customer)
        {
            Product selectedProduct = customer.GetSelectedProduct();
            List<ProductExtra> selectedExtras = customer.GetSelectedExtras();

            var productData = new List<ProductData>
            {
                new ProductData
                {
                    Products = selectedProduct.Id.ToString(CultureInfo.InvariantCulture),
                    InitialCost = selectedProduct.MonthlyCost,
                    TotalCost = selectedProduct.MonthlyCost,
                    Discount = 0
                }
            };

            productData.AddRange(selectedExtras.Select(p => new ProductData
            {
                Products = p.Id.ToString(CultureInfo.InvariantCulture),
                InitialCost = p.Cost,
                TotalCost = p.Cost,
                Discount = 0
            }).ToList());

            return productData;
        }

        private static DirectDebitDetails GetFakeDirectDebitDetails()
        {
            return new DirectDebitDetails
            {
                AccountName = "Mr Test",
                AccountNumber = "12345678",
                SortCode = "102030",
                DirectDebitPaymentDate = 28,
                BankName = "Test"
            };
        }

        private static QasAddress GetFakeAddress()
        {
            return new QasAddress
            {
                HouseName = "123",
                AddressLine1 = "London Road",
                Town = "Reading",
                AddressLine2 = "Line 2",
                County = "Test County"
            };
        }

        private static PersonalDetails GetFakePersonalDetails()
        {
            return new PersonalDetails
            {
                Title = "Mr",
                FirstName = "Joe",
                LastName = "Bloggs",
                DateOfBirth = "01 January 1990"
            };
        }
    }
}