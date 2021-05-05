using Products.Infrastructure;
using Products.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Products.Repository.Common
{
    public class CustomerAlertRepository : ICustomerAlertRepository
    {
        private const string GetActiveCustomerAlertsProc = "GetActiveCustomerAlerts";
        private readonly string _connectionString;
        private readonly IDatabaseHelper _databaseHelper;

        public CustomerAlertRepository(IConfigManager configManager, IDatabaseHelper dbHelper)
        {
            _connectionString = configManager.GetConnectionString("ReDevProj.DbConnection");
            _databaseHelper = dbHelper;

            Guard.Against<ArgumentNullException>(dbHelper == null, "dbHelper is null");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(_connectionString), "Missing ReDevProj.DbConnection in web.config.");
        }

        public async Task<CustomerAlertResult> IsCustomerAlertActive(string customerAlertName)
        {
            var result = await _databaseHelper.ExecuteStoredProcAsync<CustomerAlertResult>(_connectionString, GetActiveCustomerAlertsProc, new
            {
                Type = customerAlertName
            });

            return result.FirstOrDefault();
        }
    }
}
