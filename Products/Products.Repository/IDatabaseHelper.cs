namespace Products.Repository
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;

    public interface IDatabaseHelper
    {
        Task<IEnumerable<T>> ExecuteStoredProcAsync<T>(string connectionString, string storedProc, object param, int commandTimeout = 10);

        Task<int> ExecuteAsync(string connectionString, string procName, object param, CommandType commandType = CommandType.StoredProcedure, int commandTimeout = 10);
    }
}
