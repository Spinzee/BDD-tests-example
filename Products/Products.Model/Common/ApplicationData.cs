namespace Products.Model.Common
{
    using System;

    public class ApplicationData
    {
        public int? GenericPaymentMethodID { get; set; }

        public int? ElecPaymentMethodID { get; set; }

        public int? GasPaymentMethodID { get; set; }

        public int? TelPaymentMethodID { get; set; }

        public string GenericAccountName { get; set; }

        public string GenericSortCode { get; set; }

        public string GenericAccountNumber { get; set; }

        public string ElecAccountName { get; set; }

        public string ElecSortCode { get; set; }

        public string ElecAccountNumber { get; set; }

        public int? ElecDayInMonthPaymentMade { get; set; }

        public int? ElecMonthlyAmount { get; set; }

        public string GasAccountName { get; set; }

        public string GasSortCode { get; set; }

        public string GasAccountNumber { get; set; }

        public string ConsultantID { get; set; }

        public bool IsNsc { get; set; }

        public bool? MACRequired { get; set; }

        public bool? BroadbandProductPresent { get; set; }

        public int? BroadbandSubProductId { get; set; }

        public int? BroadbandPaymentMethodId { get; set; }

        public int? GasDayInMonthPaymentMade { get; set; }

        public int? GasMonthlyAmount { get; set; }

        public int? TelMonthlyAmount { get; set; }

        public int? TelDayInMonthPaymentMade { get; set; }

        public int? DualSubProductID { get; set; }

        public int? TelSubProductID { get; set; }

        public int BillingAddressTypeID { get; set; }

        public int? ElecSubProductID { get; set; }

        public int? GasSubProductID { get; set; }

        public string ProductCode { get; set; }

        public string Title { get; set; }

        public string Firstname { get; set; }

        public string Surname { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string DayPhone { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string HouseName { get; set; }

        public string HouseNumber { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public string Town { get; set; }

        public string Postcode { get; set; }

        public string AccountName { get; set; }

        public string SortCode { get; set; }

        public string AccountNumber { get; set; }

        public bool MarketingConsent { get; set; }

        public int? LineSpeed { get; set; }

        public string CampaignCode { get; set; }

        public Guid UserID { get; set; }

        public string ProductName { get; set; }

        public int? BaseProductId { get; set; }

        public int AddressTypeId { get; set; }

        public int PaymentMethodId { get; set; }

        public string SubProductId { get; set; }

        public int TariffTypeId { get; set; }

        public int SiteDetailsId { get; set; }

        public int? DayInMonthPaymentMade { get; set; }

        public int MonthlyAmount { get; set; }

        public string ApplicationStatus { get; set; }

        public string PromotionCode { get; set; }

        public string Brand { get; set; }

        public string LoyaltyProduct { get; set; }

        public string ReferenceNumbers { get; set; }

        public string BaseProductIDsLinkedToReferenceNumbers { get; set; }

        public bool UseBillAddForMail { get; set; }

        public string Country { get; set; }

        public string BillingTown { get; set; }

        public string BillingCountry { get; set; }

        public string ElecSubProductRef { get; set; }

        public string GasSubProductRef { get; set; }

        public string TelSubProductRef { get; set; }

        public string RugbyTeamName { get; set; }

        public bool ElecProductPresentButNoDual { get; set; }

        public bool GasProductPresentButNoDual { get; set; }

        public bool TelProductPresent { get; set; }

        public bool DualFuelProductPresent { get; set; }

        public bool WantToReceivePaperBills { get; set; }

        public bool? SingleBankAccChosen { get; set; }

        public bool? AntivirusRequested { get; set; }

        public bool? SelectedBenefits { get; set; }

        public int ElectricDayUsage { get; set; }

        public int ElectricNightUsage { get; set; }

        public int GasUsage { get; set; }

        public decimal? ElectricMonetaryAmount { get; set; }

        public decimal? GasMonetaryAmount { get; set; }

        public int? LifeStylePropertyType { get; set; }

        public int? LifeStylePropertySize { get; set; }

        public int? LifeStyleOccupancy { get; set; }

        public string BillingPostCode { get; set; }

        public string BillingHouseName { get; set; }

        public string BillingHouseNumber { get; set; }

        public string BillingAddressLine1 { get; set; }

        public string BillingAddressLine2 { get; set; }

        public string BillingAddressLine3 { get; set; }

        public string TelAccountName { get; set; }

        public string TelSortCode { get; set; }

        public string TelAccountNumber { get; set; }

        public string BroadbandSubProductRef { get; set; }

        public string BroadbandAccountName { get; set; }

        public string BroadbandSortCode { get; set; }

        public string BroadbandAccountNumber { get; set; }

        public string Linespeed { get; set; }

        public string ReadFrequency { get; set; }

        public string SmartProducts { get; set; }

        public string SmartServices { get; set; }

        public string SmartMeterType { get; set; }
    }
}
