namespace Products.Tests.Energy.Fakes.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Products.Model.Common;
    using Repository.Common;

    public class FakeSalesRepository : ISalesRepository
    {
        public Dictionary<string, string> InsertSaleSqlParameters = new Dictionary<string, string>();

        public int InsertSaleCount { get; set; }

        public Exception Exception { get; set; }

        public async Task<int> SaveApplication(ApplicationData application)
        {
            await Task.Delay(1);
            if (Exception != null)
            {
                throw Exception;
            }

            InsertSaleSqlParameters.Add("@Postcode", application.Postcode);
            InsertSaleSqlParameters.Add("@AddressTypeID", application.AddressTypeId.ToString());
            InsertSaleSqlParameters.Add("@HouseName", application.HouseName);
            InsertSaleSqlParameters.Add("@HouseNumber", application.HouseNumber);
            InsertSaleSqlParameters.Add("@AddressLine1", application.AddressLine1);
            InsertSaleSqlParameters.Add("@AddressLine2", application.AddressLine2);
            InsertSaleSqlParameters.Add("@AddressLine3", application.AddressLine3);
            InsertSaleSqlParameters.Add("@BaseProductID", application.BaseProductId.ToString());
            InsertSaleSqlParameters.Add("@PaymentMethodID", application.PaymentMethodId.ToString());
            InsertSaleSqlParameters.Add("@SubProductID", application.SubProductId);
            InsertSaleSqlParameters.Add("@TariffTypeID", application.TariffTypeId.ToString());
            InsertSaleSqlParameters.Add("@SiteID", application.SiteDetailsId.ToString());
            InsertSaleSqlParameters.Add("@AccountName", application.AccountName);
            InsertSaleSqlParameters.Add("@SortCode", application.SortCode);
            InsertSaleSqlParameters.Add("@AccountNumber", application.AccountNumber);
            InsertSaleSqlParameters.Add("@DayInMonthPaymentMade", application.DayInMonthPaymentMade.ToString());
            InsertSaleSqlParameters.Add("@MonthlyAmount", application.MonthlyAmount.ToString());
            InsertSaleSqlParameters.Add("@CsUserGUID", application.UserID.ToString());
            InsertSaleSqlParameters.Add("@Email", application.Email);
            InsertSaleSqlParameters.Add("@ApplicationStatus", application.ApplicationStatus);
            InsertSaleSqlParameters.Add("@Title", application.Title);
            InsertSaleSqlParameters.Add("@FName", application.Firstname);
            InsertSaleSqlParameters.Add("@SName", application.Surname);
            InsertSaleSqlParameters.Add("@DayPhone", application.DayPhone);
            InsertSaleSqlParameters.Add("@MobilePhone", application.Mobile);
            InsertSaleSqlParameters.Add("@MarketingConsent", application.MarketingConsent.ToString());
            InsertSaleSqlParameters.Add("@PromotionCode", application.PromotionCode);
            InsertSaleSqlParameters.Add("@Brand", application.Brand);
            InsertSaleSqlParameters.Add("@CampaignCode", application.CampaignCode);
            InsertSaleSqlParameters.Add("@LoyaltyProduct", application.LoyaltyProduct);
            InsertSaleSqlParameters.Add("@ReferenceNumbers", application.ReferenceNumbers);
            InsertSaleSqlParameters.Add("@BaseProductIDsLinkedToReferenceNumbers", application.BaseProductIDsLinkedToReferenceNumbers);
            InsertSaleSqlParameters.Add("@BillingPostcode", application.Postcode);
            InsertSaleSqlParameters.Add("@BillingAddressTypeID", application.BillingAddressTypeID.ToString());
            InsertSaleSqlParameters.Add("@BillingHouseName", application.HouseName);
            InsertSaleSqlParameters.Add("@BillingHouseNumber", application.HouseNumber);
            InsertSaleSqlParameters.Add("@BillingAddressLine1", application.AddressLine1);
            InsertSaleSqlParameters.Add("@BillingAddressLine2", application.AddressLine2);
            InsertSaleSqlParameters.Add("@BillingAddressLine3", application.AddressLine3);
            InsertSaleSqlParameters.Add("@UseBillAddForMail", application.UseBillAddForMail.ToString());
            InsertSaleSqlParameters.Add("@Town", application.Town);
            InsertSaleSqlParameters.Add("@Country", application.Country);
            InsertSaleSqlParameters.Add("@BillingTown", application.BillingTown);
            InsertSaleSqlParameters.Add("@BillingCountry", application.BillingCountry);
            InsertSaleSqlParameters.Add("@ElecSubProductID", application.ElecSubProductID.ToString());
            InsertSaleSqlParameters.Add("@GasSubProductID", application.GasSubProductID.ToString());
            InsertSaleSqlParameters.Add("@TelSubProductID", application.TelSubProductID.ToString());
            InsertSaleSqlParameters.Add("@DualSubProductID", application.DualSubProductID.ToString());
            InsertSaleSqlParameters.Add("@ElecSubProductRef", application.ElecSubProductRef);
            InsertSaleSqlParameters.Add("@GasSubProductRef", application.GasSubProductRef);
            InsertSaleSqlParameters.Add("@TelSubProductRef", application.TelSubProductRef);
            InsertSaleSqlParameters.Add("@RugbyTeamName", application.RugbyTeamName);
            InsertSaleSqlParameters.Add("@ElecProductPresentButNoDual", application.ElecProductPresentButNoDual.ToString());
            InsertSaleSqlParameters.Add("@GasProductPresentButNoDual", application.GasProductPresentButNoDual.ToString());
            InsertSaleSqlParameters.Add("@TelProductPresent", application.TelProductPresent.ToString());
            InsertSaleSqlParameters.Add("@DualFuelProductPresent", application.DualFuelProductPresent.ToString());
            InsertSaleSqlParameters.Add("@WantToReceivePaperBills", application.WantToReceivePaperBills.ToString());
            InsertSaleSqlParameters.Add("@SingleBankAccChosen", application.SingleBankAccChosen.ToString());
            InsertSaleSqlParameters.Add("@GenericPaymentMethodID", application.GenericPaymentMethodID.ToString());
            InsertSaleSqlParameters.Add("@ElecPaymentMethodID", application.ElecPaymentMethodID.ToString());
            InsertSaleSqlParameters.Add("@GasPaymentMethodID", application.GasPaymentMethodID.ToString());
            InsertSaleSqlParameters.Add("@TelPaymentMethodID", application.TelPaymentMethodID.ToString());
            InsertSaleSqlParameters.Add("@GenericAccountName", application.GenericAccountName);
            InsertSaleSqlParameters.Add("@GenericSortCode", application.GenericSortCode);
            InsertSaleSqlParameters.Add("@GenericAccountNumber", application.GenericAccountNumber);
            InsertSaleSqlParameters.Add("@ElecAccountName", application.ElecAccountName);
            InsertSaleSqlParameters.Add("@ElecSortCode", application.ElecSortCode);
            InsertSaleSqlParameters.Add("@ElecAccountNumber", application.ElecAccountNumber);
            InsertSaleSqlParameters.Add("@ElecDayInMonthPaymentMade", application.ElecDayInMonthPaymentMade.ToString());
            InsertSaleSqlParameters.Add("@ElecMonthlyAmount", application.ElecMonthlyAmount.ToString());
            InsertSaleSqlParameters.Add("@GasAccountName", application.GasAccountName);
            InsertSaleSqlParameters.Add("@GasSortCode", application.GasSortCode);
            InsertSaleSqlParameters.Add("@GasAccountNumber", application.GasAccountNumber);
            InsertSaleSqlParameters.Add("@GasDayInMonthPaymentMade", application.GasDayInMonthPaymentMade.ToString());
            InsertSaleSqlParameters.Add("@GasMonthlyAmount", application.GasMonthlyAmount.ToString());
            InsertSaleSqlParameters.Add("@TelAccountName", application.TelAccountName);
            InsertSaleSqlParameters.Add("@TelSortCode", application.TelSortCode);
            InsertSaleSqlParameters.Add("@TelAccountNumber", application.TelAccountNumber);
            InsertSaleSqlParameters.Add("@TelDayInMonthPaymentMade", application.TelDayInMonthPaymentMade.ToString());
            InsertSaleSqlParameters.Add("@TelMonthlyAmount", application.TelMonthlyAmount.ToString());
            InsertSaleSqlParameters.Add("@ConsultantID", application.ConsultantID);
            InsertSaleSqlParameters.Add("@IsNsc", application.IsNsc.ToString());
            InsertSaleSqlParameters.Add("@DateOfBirth", application.DateOfBirth.ToString(CultureInfo.InvariantCulture));
            InsertSaleSqlParameters.Add("@MACRequired", application.MACRequired.ToString());
            InsertSaleSqlParameters.Add("@BroadbandSubProductRef", application.BroadbandSubProductRef);
            InsertSaleSqlParameters.Add("@BroadbandProductPresent", application.BroadbandProductPresent.ToString());
            InsertSaleSqlParameters.Add("@BroadbandSubProductId", application.BroadbandSubProductId.ToString());
            InsertSaleSqlParameters.Add("@BroadbandPaymentMethodId", application.BroadbandPaymentMethodId.ToString());
            InsertSaleSqlParameters.Add("@BroadbandAccountName", application.BroadbandAccountName);
            InsertSaleSqlParameters.Add("@BroadbandSortCode", application.BroadbandSortCode);
            InsertSaleSqlParameters.Add("@BroadbandAccountNumber", application.BroadbandAccountNumber);
            InsertSaleSqlParameters.Add("@Linespeed", application.LineSpeed.ToString());
            InsertSaleSqlParameters.Add("@AntivirusRequested", application.AntivirusRequested.ToString());
            InsertSaleSqlParameters.Add("@selectedBenefits", application.SelectedBenefits.ToString());
            InsertSaleSqlParameters.Add("@ElectricDayUsage", application.ElectricDayUsage.ToString());
            InsertSaleSqlParameters.Add("@ElectricNightUsage", application.ElectricNightUsage.ToString());
            InsertSaleSqlParameters.Add("@GasUsage", application.GasUsage.ToString());
            InsertSaleSqlParameters.Add("@ElectricMonetaryAmount", application.ElectricMonetaryAmount.ToString());
            InsertSaleSqlParameters.Add("@GasMonetaryAmount", application.GasMonetaryAmount.ToString());
            InsertSaleSqlParameters.Add("@LifeStylePropertyType", application.LifeStylePropertyType.ToString());
            InsertSaleSqlParameters.Add("@LifeStylePropertySize", application.LifeStylePropertySize.ToString());
            InsertSaleSqlParameters.Add("@LifeStyleOccupancy", application.LifeStyleOccupancy.ToString());
            InsertSaleSqlParameters.Add("@ReadFrequency", application.ReadFrequency);
            InsertSaleSqlParameters.Add("@SmartProducts", application.SmartProducts);
            InsertSaleSqlParameters.Add("@SmartServices", application.SmartServices);
            InsertSaleSqlParameters.Add("@SmartMeterType", application.SmartMeterType);

            return InsertSaleCount++;
        }

        public async Task InsertMasusReference(int applicationId, string email, Guid userId)
        {
            await Task.Delay(1);
            if (Exception != null)
            {
                throw Exception;
            }
        }
    }
}