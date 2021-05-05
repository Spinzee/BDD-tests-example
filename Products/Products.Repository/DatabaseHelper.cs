namespace Products.Repository
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Dapper;

    public class DatabaseHelper : IDatabaseHelper
    {
        public async Task<IEnumerable<T>> ExecuteStoredProcAsync<T>(string connectionString, string procName, object param, int commandTimeout = 10)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                return await conn.QueryAsync<T>(procName, param, commandType: CommandType.StoredProcedure, commandTimeout: commandTimeout);
            }
        }

        public async Task<int> ExecuteAsync(string connectionString, string procName, object param, CommandType commandType = CommandType.StoredProcedure, int commandTimeout = 10)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                return await conn.ExecuteAsync(procName, param, commandType: commandType, commandTimeout: commandTimeout);
            }
        }
    }
}
