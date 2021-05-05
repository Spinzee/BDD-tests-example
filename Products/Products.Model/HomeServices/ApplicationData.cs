using System.Collections.Generic;

namespace Products.Model.HomeServices
{
    public class ApplicationData
    {
        public string AccountHolder { get; set; }
        public string AccountNo { get; set; }
        public string AccountNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Affiliate { get; set; }
        public string BankName { get; set; }
        public string BillingAddressLine1 { get; set; }
        public string BillingAddressLine2 { get; set; }
        public string BillingCounty { get; set; }
        public string BillingHouseNameOrNumber { get; set; }
        public string BillingPostcode { get; set; }
        public string BillingTown { get; set; }
        public string County { get; set; }
        public string DaytimePhoneNo { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string HouseNameNumber { get; set; }
        public string MobilePhoneNo { get; set; }
        public string NoMarketing { get; set; }
        public string PaymentDay { get; set; }
        public string Postcode { get; set; }
        public string PromoCodes { get; set; }
        public string SortCode { get; set; }
        public string Surname { get; set; }
        public string Title { get; set; }
        public string Town { get; set; }
        public bool IsSignupWithEnergy {get; set;}
        public List<ProductData> ProductData { get; set; }
    }

    public class ProductData
    {
        public string Products { get; set; }
        public double InitialCost { get; set; }
        public double TotalCost { get; set; }
        public double Discount { get; set; } = 0;
    }
}
