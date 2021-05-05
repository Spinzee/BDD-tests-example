namespace Products.Repository.Energy
{
    using System;
    using System.Data;
    using System.Threading.Tasks;
    using Core.Configuration.Settings;
    using Dapper;
    using Infrastructure;
    using Model.Constants;

    public class EnergySalesRepository : IEnergySalesRepository
    {
        private const string GetSubProductIdForSiteAndBaseProductId = "GetSubProductIdforSiteAndBaseProductId";
        private readonly IDatabaseHelper _dbHelper;
        private readonly string _redevProjConnectionString;

        public EnergySalesRepository(IDatabaseHelper dbHelper, IConfigurationSettings settings)
        {
            Guard.Against<ArgumentException>(dbHelper == null, $"{nameof(dbHelper)} is null");
            Guard.Against<ArgumentException>(settings == null, $"{nameof(settings)} is null");

            _dbHelper = dbHelper;
            // ReSharper disable once PossibleNullReferenceException
            _redevProjConnectionString = settings.ConnectionStringSettings.RedevProj;

            Guard.Against<ArgumentException>(string.IsNullOrEmpty(_redevProjConnectionString), $"{nameof(_redevProjConnectionString)} is null or empty");
        }

        public async Task<int> GetSubProductIdForFuelType(string product, int baseProductId, bool isPrePay)
        {
            var param = new DynamicParameters();
            param.Add("@SiteID", SalesData.SiteId);
            param.Add("@Product", product);
            param.Add("@BaseProductId", baseProductId);
            param.Add("@IsPrePay", isPrePay);
            param.Add("@SPID", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _dbHelper.ExecuteAsync(_redevProjConnectionString, GetSubProductIdForSiteAndBaseProductId, param);
            return param.Get<int>("@SPID");
        }
    }
}