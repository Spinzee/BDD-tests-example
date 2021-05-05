namespace Products.Tests.Broadband.Fakes.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Products.Model.Broadband;
    using Products.Model.Common;
    using Products.Repository.Broadband;
    
    public class FakeBroadbandSalesRepository : IBroadbandSalesRepository
    {
        public int InsertSaleCount { get; set; }

        public int InsertAuditCount { get; set; }

        public int InsertOpenreachAuditCount { get; set; }

        public int UpdateLastUserCount { get; set; }

        public int InsertMasusReferenceCount { get; set; }

        public Exception Exception { get; set; }

        public Exception AuditException { get; set; }

        public Exception SetLastUpdatedException { get; set; }

        public ArgumentException ArgumentException { get; set; }

        public Exception OpenreachAuditException { get; set; }

        public Dictionary<string, string> InsertAuditSqlParameters = new Dictionary<string, string>();

        public Dictionary<string, string> InsertSaleSqlParameters = new Dictionary<string, string>();

        public Dictionary<string, string> UpdateLastUserParameters = new Dictionary<string, string>();

        public Dictionary<string, string> InsertOpenreachAuditSqlParameters = new Dictionary<string, string>();

        public async Task<int> SaveApplication(ApplicationData application)
        {
            if (Exception != null)
            {
                throw Exception;
            }

            if (ArgumentException != null)
            {
                throw ArgumentException;
            }

            InsertSaleSqlParameters.Add("AccountName", application.AccountName);
            InsertSaleSqlParameters.Add("AccountNumber", application.AccountNumber);
            InsertSaleSqlParameters.Add("AddressLine1", application.AddressLine1);
            InsertSaleSqlParameters.Add("AddressLine2", application.AddressLine2);
            InsertSaleSqlParameters.Add("AddressLine3", application.AddressLine3);
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            InsertSaleSqlParameters.Add("DateOfBirth", application.DateOfBirth.ToString());
            InsertSaleSqlParameters.Add("DayPhone", application.DayPhone);
            InsertSaleSqlParameters.Add("Email", application.Email);
            InsertSaleSqlParameters.Add("FirstName", application.Firstname);
            InsertSaleSqlParameters.Add("HouseName", application.HouseName);
            InsertSaleSqlParameters.Add("HouseNumber", application.HouseNumber);
            InsertSaleSqlParameters.Add("LineSpeed", application.LineSpeed.ToString());
            InsertSaleSqlParameters.Add("MarketingConsent", application.MarketingConsent.ToString());
            InsertSaleSqlParameters.Add("Mobile", application.Mobile);
            InsertSaleSqlParameters.Add("Postcode", application.Postcode);
            InsertSaleSqlParameters.Add("ProductCode", application.ProductCode);
            InsertSaleSqlParameters.Add("SortCode", application.SortCode);
            InsertSaleSqlParameters.Add("Surname", application.Surname);
            InsertSaleSqlParameters.Add("Title", application.Title);
            InsertSaleSqlParameters.Add("Town", application.Town);
            InsertSaleSqlParameters.Add("CampaignCode", application.CampaignCode);
            InsertSaleSqlParameters.Add("CsUserGUID", application.UserID.ToString());

            InsertSaleCount++;

            return await Task.FromResult(1265);
        }

        public async Task InsertApplicationAudit(ApplicationAudit auditData)
        {
            if (AuditException != null)
            {
                throw AuditException;
            }

            InsertAuditSqlParameters.Add("CLIProvided", auditData.CLIProvided.ToString());
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            InsertAuditSqlParameters.Add("MonthlyDDPrice", auditData.MonthlyDDPrice.ToString());
            InsertAuditSqlParameters.Add("ProductCode", auditData.ProductCode);
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            InsertAuditSqlParameters.Add("ConnectionCharge", auditData.ConnectionCharge.ToString());
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            InsertAuditSqlParameters.Add("InstallationCharge", auditData.InstallationCharge.ToString());
            InsertAuditSqlParameters.Add("IsSSECustomer", auditData.IsSSECustomer.ToString());

            InsertAuditCount++;

            await Task.CompletedTask;
        }

        public async Task InsertMasusReference(int applicationId, string email, Guid userId)
        {
            if (Exception != null)
            {
                throw new Exception();
            }

            InsertAuditSqlParameters.Add("ApplicationId", applicationId.ToString());
            InsertAuditSqlParameters.Add("Email", email);
            InsertAuditSqlParameters.Add("UserId", userId.ToString());

            InsertMasusReferenceCount++;

            await Task.CompletedTask;
        }

        public async Task SetLastUpdated(Guid userid)
        {
            if (SetLastUpdatedException != null)
            {
                throw SetLastUpdatedException;
            }

            UpdateLastUserParameters.Add("UserId", userid.ToString());

            UpdateLastUserCount++;

            await Task.CompletedTask;
        }

        public async Task InsertOpenReachAudit(OpenreachAuditData auditData)
        {
            if (OpenreachAuditException != null)
            {
                throw OpenreachAuditException;
            }

            InsertOpenreachAuditSqlParameters.Add("ApplicationId", auditData.ApplicationId.ToString());
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            InsertOpenreachAuditSqlParameters.Add("SaleDate", auditData.SaleDate.ToString());
            InsertOpenreachAuditSqlParameters.Add("LineStatus", auditData.LineStatus);
            InsertOpenreachAuditSqlParameters.Add("Postode", auditData.Postcode);
            InsertOpenreachAuditSqlParameters.Add("HouseName", auditData.AddressLine1);
            InsertOpenreachAuditSqlParameters.Add("CLI", auditData.CLI);
            InsertOpenreachAuditSqlParameters.Add("AddressLineKey", auditData.AddressLineKey);

            InsertOpenreachAuditCount++;

            await Task.CompletedTask;
        }
    }
}
