namespace Products.Repository.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using Core.Configuration.Settings;
    using Dapper;
    using Infrastructure;
    using Products.Model.Common;

    public class SalesRepository : ISalesRepository
    {
        private readonly IDatabaseHelper _dbHelper;
        private readonly string _redevProjConnectionString;
        private readonly string _profileConnectionString;
        private const string ProductSignupProc = "ProductSignupSaveApplication";
        private const string GetMasusReferenceProc = "Redev_Proj_ApplicationSignup_GetMasusReferenceNumber";
        private const string InsertMasusReferenceProc = "InsertMasusReference";

        public SalesRepository(IDatabaseHelper dbHelper, IConfigurationSettings settings)
        {
            Guard.Against<ArgumentException>(dbHelper == null, $"{nameof(dbHelper)} is null");
            Guard.Against<ArgumentException>(settings == null, $"{nameof(settings)} is null");

            _dbHelper = dbHelper;
            // ReSharper disable once PossibleNullReferenceException
            _redevProjConnectionString = settings.ConnectionStringSettings.RedevProj;
            _profileConnectionString = settings.ConnectionStringSettings.CSGatewayProfiles;
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(_redevProjConnectionString), $"{nameof(_redevProjConnectionString)} is null or empty");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(_profileConnectionString), $"{nameof(_profileConnectionString)} is null or empty");
        }

        public async Task<int> SaveApplication(ApplicationData application)
        {
            var param = new DynamicParameters();
            param.Add("@Postcode", application.Postcode);
            param.Add("@AddressTypeID", application.AddressTypeId);
            param.Add("@HouseName", application.HouseName);
            param.Add("@HouseNumber", application.HouseNumber);
            param.Add("@AddressLine1", application.AddressLine1);
            param.Add("@AddressLine2", application.AddressLine2);
            param.Add("@AddressLine3", application.AddressLine3);
            param.Add("@BaseProductID ", application.BaseProductId);
            param.Add("@PaymentMethodID ", application.PaymentMethodId);
            param.Add("@SubProductID ", application.SubProductId);
            param.Add("@TariffTypeID ", application.TariffTypeId);
            param.Add("@SiteID ", application.SiteDetailsId);
            param.Add("@AccountName", application.AccountName);
            param.Add("@SortCode", application.SortCode);
            param.Add("@AccountNumber", application.AccountNumber);
            param.Add("@DayInMonthPaymentMade ", application.DayInMonthPaymentMade);
            param.Add("@MonthlyAmount ", application.MonthlyAmount);
            param.Add("@CsUserGUID", application.UserID);
            param.Add("@Email", application.Email);
            param.Add("@ApplicationStatus", application.ApplicationStatus);
            param.Add("@Title", application.Title);
            param.Add("@FName", application.Firstname);
            param.Add("@SName", application.Surname);
            param.Add("@DayPhone", application.DayPhone);
            param.Add("@MobilePhone", application.Mobile);
            param.Add("@MarketingConsent ", application.MarketingConsent);
            param.Add("@PromotionCode", application.PromotionCode);
            param.Add("@Brand", application.Brand);
            param.Add("@CampaignCode", application.CampaignCode);
            param.Add("@LoyaltyProduct", application.LoyaltyProduct);
            param.Add("@ReferenceNumbers", application.ReferenceNumbers);
            param.Add("@BaseProductIDsLinkedToReferenceNumbers", application.BaseProductIDsLinkedToReferenceNumbers);
            param.Add("@BillingPostcode", application.Postcode);
            param.Add("@BillingAddressTypeID ", application.BillingAddressTypeID);
            param.Add("@BillingHouseName", application.HouseName);
            param.Add("@BillingHouseNumber", application.HouseNumber);
            param.Add("@BillingAddressLine1", application.AddressLine1);
            param.Add("@BillingAddressLine2", application.AddressLine2);
            param.Add("@BillingAddressLine3", application.AddressLine3);
            param.Add("@UseBillAddForMail ", application.UseBillAddForMail);
            param.Add("@Town", application.Town);
            param.Add("@Country", application.Country);
            param.Add("@BillingTown", application.BillingTown);
            param.Add("@BillingCountry", application.BillingCountry);
            param.Add("@ElecSubProductID ", application.ElecSubProductID);
            param.Add("@GasSubProductID ", application.GasSubProductID);
            param.Add("@TelSubProductID ", application.TelSubProductID);
            param.Add("@DualSubProductID ", application.DualSubProductID);
            param.Add("@ElecSubProductRef", application.ElecSubProductRef);
            param.Add("@GasSubProductRef", application.GasSubProductRef);
            param.Add("@TelSubProductRef", application.TelSubProductRef);
            param.Add("@RugbyTeamName", application.RugbyTeamName);
            param.Add("@ElecProductPresentButNoDual", application.ElecProductPresentButNoDual);
            param.Add("@GasProductPresentButNoDual ", application.GasProductPresentButNoDual);
            param.Add("@TelProductPresent", application.TelProductPresent);
            param.Add("@DualFuelProductPresent", application.DualFuelProductPresent);
            param.Add("@WantToReceivePaperBills", application.WantToReceivePaperBills);
            param.Add("@SingleBankAccChosen", application.SingleBankAccChosen);
            param.Add("@GenericPaymentMethodID", application.GenericPaymentMethodID);
            param.Add("@ElecPaymentMethodID", application.ElecPaymentMethodID);
            param.Add("@GasPaymentMethodID", application.GasPaymentMethodID);
            param.Add("@TelPaymentMethodID", application.TelPaymentMethodID);
            param.Add("@GenericAccountName", application.GenericAccountName);
            param.Add("@GenericSortCode", application.GenericSortCode);
            param.Add("@GenericAccountNumber", application.GenericAccountNumber);
            param.Add("@ElecAccountName", application.ElecAccountName);
            param.Add("@ElecSortCode", application.ElecSortCode);
            param.Add("@ElecAccountNumber", application.ElecAccountNumber);
            param.Add("@ElecDayInMonthPaymentMade", application.ElecDayInMonthPaymentMade);
            param.Add("@ElecMonthlyAmount", application.ElecMonthlyAmount);
            param.Add("@GasAccountName", application.GasAccountName);
            param.Add("@GasSortCode", application.GasSortCode);
            param.Add("@GasAccountNumber", application.GasAccountNumber);
            param.Add("@GasDayInMonthPaymentMade", application.GasDayInMonthPaymentMade);
            param.Add("@GasMonthlyAmount", application.GasMonthlyAmount);
            param.Add("@TelAccountName", application.TelAccountName);
            param.Add("@TelSortCode", application.TelSortCode);
            param.Add("@TelAccountNumber", application.TelAccountNumber);
            param.Add("@TelDayInMonthPaymentMade", application.TelDayInMonthPaymentMade);
            param.Add("@TelMonthlyAmount", application.TelMonthlyAmount);
            param.Add("@ConsultantID", application.ConsultantID);
            param.Add("@IsNsc", application.IsNsc);
            param.Add("@DateOfBirth", application.DateOfBirth);
            param.Add("@MACRequired", application.MACRequired);
            param.Add("@BroadbandSubProductRef", application.BroadbandSubProductRef);
            param.Add("@BroadbandProductPresent", application.BroadbandProductPresent);
            param.Add("@BroadbandSubProductId", application.BroadbandSubProductId);
            param.Add("@BroadbandPaymentMethodId", application.BroadbandPaymentMethodId);
            param.Add("@BroadbandAccountName", application.BroadbandAccountName);
            param.Add("@BroadbandSortCode", application.BroadbandSortCode);
            param.Add("@BroadbandAccountNumber", application.BroadbandAccountNumber);
            param.Add("@Linespeed", application.LineSpeed);
            param.Add("@AntivirusRequested", application.AntivirusRequested);
            param.Add("@selectedBenefits", application.SelectedBenefits);
            param.Add("@ElectricDayUsage", application.ElectricDayUsage);
            param.Add("@ElectricNightUsage", application.ElectricNightUsage);
            param.Add("@GasUsage", application.GasUsage);
            param.Add("@ElectricMonetaryAmount", application.ElectricMonetaryAmount);
            param.Add("@GasMonetaryAmount", application.GasMonetaryAmount);
            param.Add("@LifeStylePropertyType", application.LifeStylePropertyType);
            param.Add("@LifeStylePropertySize", application.LifeStylePropertySize);
            param.Add("@LifeStyleOccupancy", application.LifeStyleOccupancy);
            param.Add("@ReadFrequency", application.ReadFrequency);
            param.Add("@SmartProducts", application.SmartProducts);
            param.Add("@SmartServices", application.SmartServices);
            param.Add("@SmartMeterType", application.SmartMeterType);

            param.Add("@IsIGT", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            param.Add("@ApplicationIdOutput", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbHelper.ExecuteAsync(_redevProjConnectionString, ProductSignupProc, param);
            return param.Get<int>("@ApplicationIdOutput");
        }

        public async Task InsertMasusReference(int applicationID, string email, Guid userId)
        {
            IEnumerable<string> masusRefs = await _dbHelper.ExecuteStoredProcAsync<string>(_redevProjConnectionString, GetMasusReferenceProc, new
            {
                applicationID,
                email
            });

            DateTime now = DateTime.Now;
            foreach (string masusRef in masusRefs)
            {
                // use the same connection!!
                await _dbHelper.ExecuteAsync(_profileConnectionString, InsertMasusReferenceProc, new
                {
                    userId = userId.ToString("B"),
                    masusReference = masusRef,
                    masusDate = now
                });
            }
        }
    }
}
