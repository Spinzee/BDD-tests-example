namespace Products.Tests.Broadband.Fakes.Services
{
    using System.Data;
    using System.Threading.Tasks;
    using Products.Repository;
    using System.Collections.Generic;

    public class FakeDatabaseHelper : IDatabaseHelper
    {
        public async Task<int> ExecuteAsync(string connectionString, string procName, object param, CommandType commandType = CommandType.StoredProcedure, int commandTimeout = 10)
        {
            await Task.Delay(1);
            return 0;
        }

        public async Task<IEnumerable<T>> ExecuteStoredProcAsync<T>(string connectionString, string storedProc, object param, int commandTimeout = 10)
        {
            await Task.Delay(1);
            return new List<T>();
        }
    }
}
