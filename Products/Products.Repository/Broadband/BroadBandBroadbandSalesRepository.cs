using Dapper;
using Products.Infrastructure;
using Products.Model.Broadband;
using Products.Model.Common;
using Products.Model.Constants;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Products.Model.Enums;

namespace Products.Repository.Broadband
{
    public class BroadBandSalesRepository : IBroadbandSalesRepository
    {
        private readonly IDatabaseHelper _dbHelper;

        private readonly string _redevProjConnectionString;
        private readonly string _ecomAuditConnectionString;
        private readonly string _profileConnectionString;

        private const string getSubProductByLoyaltyProc = "GetSubProductByLoyalty";
        private const string getSubProductParentProc = "GetSubProductParent";
        private const string productSignupProc = "ProductSignupSaveApplication";
        private const string getMasusReferenceProc = "Redev_Proj_ApplicationSignup_GetMasusReferenceNumber";
        private const string productAuditProc = "InsertBroadbandApplicationAudit";
        private const string insertMasusReferenceProc = "InsertMasusReference";
        private const string updateLastUserProc = "SetLastUpdatedUserId";
        private const string openreachAuditProc = "InsertOpenreachAuditdata";

        public BroadBandSalesRepository(IDatabaseHelper dbHelper, IConfigManager configManager)
        {
            Guard.Against<ArgumentNullException>(dbHelper == null, "dbHelper is null");
            Guard.Against<ArgumentNullException>(configManager == null, "configManager is null");

            _dbHelper = dbHelper;
            _redevProjConnectionString = configManager.GetConnectionString("ReDevProj.DbConnection");
            _ecomAuditConnectionString = configManager.GetConnectionString("eComAudit.DbConnection");
            _profileConnectionString = configManager.GetConnectionString("CSGatewayProfiles.DbConnection");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(_redevProjConnectionString), "ReDevProj.DbConnection");
        }

        private async Task<SubProductResult> GetSubProductByLoyalty(string productCode, int siteDetailsID)
        {
            var result = await _dbHelper.ExecuteStoredProcAsync<SubProductResult>(_redevProjConnectionString, getSubProductByLoyaltyProc, new
            {
                loyaltyProd = productCode,
                siteDetailsID = siteDetailsID
            });

            return result.FirstOrDefault();
        }

        private async Task<SubProductResult> GetSubProductParent(int subProductId)
        {
            var result = await _dbHelper.ExecuteStoredProcAsync<SubProductResult>(_redevProjConnectionString, getSubProductParentProc, new
            {
                subProductId = subProductId
            });

            return result.FirstOrDefault();
        }

        public async Task<int> SaveApplication(ApplicationData application)
        {
            // TODO: opening and closing the connection 3 times, use the same connection!!
            int paymentMethodID = 6, dayInMonthPaymentMade = 28, siteDetailsId = SalesData.SiteId;

            var broadbandSubProductResult = await GetSubProductByLoyalty(application.ProductCode, siteDetailsId);
            if (broadbandSubProductResult == null)
                throw new ArgumentException($"{getSubProductByLoyaltyProc} returned no results for product code: {application.ProductCode}");

            int broadbandSubProduct = broadbandSubProductResult.SubProductID;

            var telephoneSubProductResult = await GetSubProductParent(broadbandSubProduct);
            if (telephoneSubProductResult == null)
                throw new ArgumentException($"{getSubProductParentProc} returned no results for product code: {application.ProductCode}");

            int telSubProduct = telephoneSubProductResult.SubProductID;

            var param = new DynamicParameters();
            param.Add("@Postcode", application.Postcode);
            param.Add("@AddressTypeID", AddressTypes.Supply);
            param.Add("@HouseName", application.HouseName);
            param.Add("@HouseNumber", application.HouseNumber);
            param.Add("@AddressLine1", application.AddressLine1 ?? string.Empty);
            param.Add("@AddressLine2", application.AddressLine2 ?? string.Empty);
            param.Add("@AddressLine3", application.AddressLine3 ?? string.Empty);
            param.Add("@BaseProductID ", null);
            param.Add("@PaymentMethodID ", paymentMethodID);
            param.Add("@SubProductID ", null);
            param.Add("@TariffTypeID ", 1);
            param.Add("@SiteID ", siteDetailsId);
            param.Add("@AccountName", application.AccountName); ;
            param.Add("@SortCode", application.SortCode); ;
            param.Add("@AccountNumber", application.AccountNumber); ;
            param.Add("@DayInMonthPaymentMade ", dayInMonthPaymentMade);
            param.Add("@MonthlyAmount ", 0);
            param.Add("@CsUserGUID", application.UserID);
            param.Add("@Email", application.Email);
            param.Add("@ApplicationStatus", "Default");
            param.Add("@Title", application.Title);
            param.Add("@FName", application.Firstname);
            param.Add("@SName", application.Surname);
            param.Add("@DayPhone", application.DayPhone);
            param.Add("@MobilePhone", application.Mobile);
            param.Add("@MarketingConsent ", application.MarketingConsent);
            param.Add("@PromotionCode", "");
            param.Add("@Brand", "SSE");
            param.Add("@CampaignCode", application.CampaignCode);
            param.Add("@LoyaltyProduct", "");
            param.Add("@ReferenceNumbers", "");
            param.Add("@BaseProductIDsLinkedToReferenceNumbers", "");
            param.Add("@BillingPostcode", application.Postcode);
            param.Add("@BillingAddressTypeID ", AddressTypes.Billing);
            param.Add("@BillingHouseName", application.HouseName);
            param.Add("@BillingHouseNumber", application.HouseNumber);
            param.Add("@BillingAddressLine1", application.AddressLine1 ?? string.Empty);
            param.Add("@BillingAddressLine2", application.AddressLine2 ?? string.Empty);
            param.Add("@BillingAddressLine3", application.AddressLine3 ?? string.Empty);
            param.Add("@UseBillAddForMail ", 0);
            param.Add("@Town", application.Town);
            param.Add("@Country", "");
            param.Add("@BillingTown", application.Town);
            param.Add("@BillingCountry", "");
            param.Add("@ElecSubProductID ", 0);
            param.Add("@GasSubProductID ", 0);
            param.Add("@TelSubProductID ", telSubProduct);
            param.Add("@DualSubProductID ", 0);
            param.Add("@ElecSubProductRef", null);
            param.Add("@GasSubProductRef", null);
            param.Add("@TelSubProductRef", application.DayPhone);
            param.Add("@RugbyTeamName", "");
            param.Add("@ElecProductPresentButNoDual", false);
            param.Add("@GasProductPresentButNoDual ", false);
            param.Add("@TelProductPresent", true);
            param.Add("@DualFuelProductPresent", false);
            param.Add("@WantToReceivePaperBills", false);
            param.Add("@SingleBankAccChosen", true);
            param.Add("@GenericPaymentMethodID", null);
            param.Add("@ElecPaymentMethodID", null);
            param.Add("@GasPaymentMethodID", null);
            param.Add("@TelPaymentMethodID", paymentMethodID);
            param.Add("@GenericAccountName", null);
            param.Add("@GenericSortCode", null);
            param.Add("@GenericAccountNumber", null);
            param.Add("@ElecAccountName", null);
            param.Add("@ElecSortCode", null);
            param.Add("@ElecAccountNumber", null);
            param.Add("@ElecDayInMonthPaymentMade", null);
            param.Add("@ElecMonthlyAmount", null);
            param.Add("@GasAccountName", null);
            param.Add("@GasSortCode", null);
            param.Add("@GasAccountNumber", null);
            param.Add("@GasDayInMonthPaymentMade", null);
            param.Add("@GasMonthlyAmount", null);
            param.Add("@TelAccountName", application.AccountName); ;
            param.Add("@TelSortCode", application.SortCode); ;
            param.Add("@TelAccountNumber", application.AccountNumber); ;
            param.Add("@TelDayInMonthPaymentMade", dayInMonthPaymentMade);
            param.Add("@TelMonthlyAmount", 0);
            param.Add("@ConsultantID", "");
            param.Add("@IsNsc", false);
            param.Add("@DateOfBirth", application.DateOfBirth);
            param.Add("@MACRequired", false);
            param.Add("@BroadbandSubProductRef", application.DayPhone);
            param.Add("@BroadbandProductPresent", true);
            param.Add("@BroadbandSubProductId", broadbandSubProduct);
            param.Add("@BroadbandPaymentMethodId", paymentMethodID);
            param.Add("@BroadbandAccountName", application.AccountName); ;
            param.Add("@BroadbandSortCode", application.SortCode); ;
            param.Add("@BroadbandAccountNumber", application.AccountNumber); ;
            param.Add("@Linespeed", application.LineSpeed);
            param.Add("@AntivirusRequested", false);

            param.Add("@IsIGT", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            param.Add("@ApplicationIdOutput", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _dbHelper.ExecuteAsync(_redevProjConnectionString, productSignupProc, param);
            return param.Get<int>("@ApplicationIdOutput");
        }

        public async Task InsertApplicationAudit(ApplicationAudit auditData)
        {
            await _dbHelper.ExecuteAsync(_ecomAuditConnectionString, productAuditProc, auditData);
        }

        public async Task InsertMasusReference(int applicationId, string email, Guid userId)
        {
            var masusRefs = await _dbHelper.ExecuteStoredProcAsync<string>(_redevProjConnectionString, getMasusReferenceProc, new
            {
                applicationID = applicationId,
                email = email
            });

            var now = DateTime.Now;
            foreach (var MasusRef in masusRefs)
            {
                // use the same connection!!
                await _dbHelper.ExecuteAsync(_profileConnectionString, insertMasusReferenceProc, new
                {
                    userId = userId.ToString("B"),
                    masusReference = MasusRef,
                    masusDate = now
                });
            }
        }

        public async Task SetLastUpdated(Guid userid)
        {
            await _dbHelper.ExecuteAsync(_profileConnectionString, updateLastUserProc, new
            {
                UserProfileGUID = userid.ToString("B"),
                UserID = ""
            });
        }

        public async Task InsertOpenReachAudit(OpenreachAuditData auditData)
        {
            await _dbHelper.ExecuteAsync(_ecomAuditConnectionString, openreachAuditProc, auditData);
        }
    }
}
