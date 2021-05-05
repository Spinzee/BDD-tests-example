#nullable enable
using System.Data;
// dapper best for high performance, entity framework good for speed of development.  
// dapper uses ADO.net with a thin layer of abstraction
using Dapper;
using System.Data.SqlClient;
using System.Linq;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Model;

namespace Data
{
    public class ProfileRepository : IProfileRepository
    {
        private const string GetAccountStatusProc = "GetAccountStatus";
        private readonly DatabaseOptions _databaseOptions;

        public ProfileRepository(IOptions<DatabaseOptions> databaseOptions)
        {
            _databaseOptions = databaseOptions.Value;
        }

        public LoginStatus? GetAccountStatus(string email)
        {
            LoginStatus loginStatus;
            var spParameters = new DynamicParameters();
            spParameters.Add("@email", email);
            
            using (var conn = new SqlConnection(_databaseOptions.BookAppointment))
            {
                loginStatus =
                    conn.Query<LoginStatus>(GetAccountStatusProc, spParameters, commandType: CommandType.StoredProcedure)
                        .FirstOrDefault();
            }

            return loginStatus;
        }
    }
}
