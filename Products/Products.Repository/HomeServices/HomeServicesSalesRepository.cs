namespace Products.Repository.HomeServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Core.Configuration.Settings;
    using Dapper;
    using Infrastructure;
    using Products.Model.HomeServices;

    public class HomeServicesSalesRepository : IHomeServicesSalesRepository
    {
        private readonly string _redevProjConnectionString;
        private const string SaveHomeServicesApplicationData = "SSEHomeServices_OnlineApplication_InsertSingle";

        public HomeServicesSalesRepository(IConfigurationSettings settings)
        {
            Guard.Against<ArgumentException>(settings == null, $"{nameof(settings)} is null");
            // ReSharper disable once PossibleNullReferenceException
            _redevProjConnectionString = settings.ConnectionStringSettings.RedevProj;

            Guard.Against<ArgumentException>(string.IsNullOrEmpty(_redevProjConnectionString), $"{nameof(_redevProjConnectionString)} is null or empty");
        }

        public async Task<List<int>> SaveApplication(ApplicationData applicationData)
        {
            var applicationIds = new List<int>();

            using (var conn = new SqlConnection(_redevProjConnectionString))
            {
                await conn.OpenAsync();

                SqlTransaction transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                    foreach (ProductData item in applicationData.ProductData)
                    {
                        DynamicParameters parameters = GetDynamicParametersObject(applicationData, item);
                        int applicationId = await conn.ExecuteScalarAsync<int>(SaveHomeServicesApplicationData, parameters, transaction, commandType: CommandType.StoredProcedure);
                        applicationIds.Add(applicationId); 
                    }

                    transaction.Commit();
                    return applicationIds;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // ReSharper disable once PossibleIntendedRethrow
                    throw ex;
                }
            }       
        }

        private static DynamicParameters GetDynamicParametersObject(ApplicationData data, ProductData productData)
        {
            var param = new DynamicParameters();

            param.Add("@Title", data.Title);
            param.Add("@Surname", data.Surname);
            param.Add("@FirstName", data.FirstName);
            param.Add("@HouseNameNumber", data.HouseNameNumber);
            param.Add("@AddressLine1", data.AddressLine1);
            param.Add("@AddressLine2", data.AddressLine2);
            param.Add("@Town", data.Town);
            param.Add("@County", data.County);
            param.Add("@Postcode", data.Postcode);
            param.Add("@DaytimePhoneNo", data.DaytimePhoneNo);
            param.Add("@EmailAddress", data.EmailAddress);
            param.Add("@MobilePhoneNo", data.MobilePhoneNo);
            param.Add("@EmailAddress", data.EmailAddress);
            param.Add("@BankName", data.BankName);
            param.Add("@AccountHolder", data.AccountHolder);
            param.Add("@SortCode", data.SortCode);
            param.Add("@AccountNo", data.AccountNo);
            param.Add("@PaymentDay", data.PaymentDay);
            param.Add("@NoMarketing", data.NoMarketing);
            param.Add("@Products", productData.Products);
            param.Add("@InitialCost", productData.InitialCost);
            param.Add("@Discount", productData.Discount);
            param.Add("@TotalCost", productData.TotalCost);
            param.Add("@Affiliate", data.Affiliate);
            param.Add("@BillingHouseNameOrNumber", data.BillingHouseNameOrNumber);
            param.Add("@BillingAddressLine1", data.BillingAddressLine1);
            param.Add("@BillingAddressLine2", data.BillingAddressLine2);
            param.Add("@BillingTown", data.BillingTown);
            param.Add("@BillingCounty", data.BillingCounty);
            param.Add("@BillingPostcode", data.BillingPostcode);
            param.Add("@PromoCodes", data.PromoCodes);
            param.Add("@MobilePhoneNo", data.MobilePhoneNo);
            param.Add("@AccountNumber", data.AccountNumber);
            param.Add("@IsSignupWithEnergy", data.IsSignupWithEnergy);
            return param;
        }
    }
}
