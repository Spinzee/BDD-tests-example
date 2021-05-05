using Products.Infrastructure;
using Products.Model.Enums;
using Products.Model.TariffChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Products.Repository.TariffChange
{
    public class TariffRepository : ITariffRepository
    {
        private readonly IDatabaseHelper _databaseHelper;
        private readonly string _connectionString;

        public TariffRepository(IConfigManager configManager, IDatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper;
            _connectionString = configManager.GetConnectionString("tcData.DbConnection");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(_connectionString), "Missing tcData.DbConnection connection string in web.config file.");
        }


        public async Task<List<AvailableTariffResult>> GetAvailableSingleRateTariffsAsync(string postcode, string brand, FuelType fuelType, int rateCode)
        {
            var result = await _databaseHelper.ExecuteStoredProcAsync<AvailableTariffResult>(_connectionString, "proc_GetAvailableSingleRateTariffs", new
            {
                postcode = postcode,
                ratecode = rateCode,
                businessUseCode = fuelType == FuelType.Gas ? "GSS" : "ESU",
                brandName = brand
            },
            30);

            if (result != null && !result.Any())
            {
                throw new Exception(
                    $"Tariff Service Exception: Postcode: {postcode}, Brand: {brand}, Fuel Type: {fuelType} returned no rows for method GetAvailableSingleRateTariffs");
            }

            return result?.ToList();
        }

        public async Task<List<AvailableTariffResult>> GetAvailableMultiRateTariffsAsync(string postcode, string brand, string tariffType, int rateCode)
        {
            var result = await _databaseHelper.ExecuteStoredProcAsync<AvailableTariffResult>(_connectionString, "proc_GetAvailableMultiRateTariffs", new
            {
                tariffType = tariffType,
                postcode = postcode,
                ratecode = rateCode,
                businessUseCode = "ESU",
                brandName = brand
            },
            30);

            if (result != null && !result.Any())
            {
                throw new Exception(
                    $"Tariff Service Exception: Postcode: {postcode}, Brand: {brand}, Tariff Type: {tariffType} returned no rows for method GetAvailableMultiRateTariffs");
            }

            return result.ToList();
        }

        public async Task<List<AvailableTariffResult>> GetPreservedSingleRateTariffsByServicePlanIdAsync(string postcode, string brand, string servicePlanId, FuelType fuelType, int rateCode)
        {
            var result = await _databaseHelper.ExecuteStoredProcAsync<AvailableTariffResult>(_connectionString, "proc_GetPreservedSingleRateTariffsByServicePlanId", new
            {
                postcode = postcode,
                ratecode = rateCode,
                businessUseCode = fuelType == FuelType.Gas ? "GSS" : "ESU",
                brandName = brand,
                servicePlanId = servicePlanId
            },
            30);

            if (result != null && !result.Any())
            {
                throw new Exception(
                    $"Tariff Service Exception: Postcode: {postcode}, Brand: {brand}, Tariff Service plan Id: {servicePlanId} Fuel Type: {fuelType} returned no rows for method GetPreservedSingleRateTariffsByServicePlanIdAsync");
            }

            return result.ToList();
        }

        public async Task<List<AvailableTariffResult>> GetPreservedMultiRateTariffsByServicePlanIdAsync(string postcode, string brand, string servicePlanId, string tariffType, int rateCode)
        {
            var result = await _databaseHelper.ExecuteStoredProcAsync<AvailableTariffResult>(_connectionString, "proc_GetPreservedMultiRateTariffsByServicePlanId", new
            {
                postcode = postcode,
                ratecode = rateCode,
                businessUseCode = "ESU",
                brandName = brand,
                servicePlanId = servicePlanId
            },
            30);

            if (result != null && !result.Any())
            {
                throw new Exception(
                    $"Tariff Service Exception: Postcode: {postcode}, Brand: {brand}, Tariff Service Plan Id: {servicePlanId} returned no rows for method GetPreservedMultiRateTariffsByServicePlanIdAsync");
            }

            return result.ToList();
        }
    }
}