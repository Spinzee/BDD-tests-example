using Products.Model.Enums;
using Products.Model.TariffChange;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.Repository.TariffChange
{
    public interface ITariffRepository
    {
        Task<List<AvailableTariffResult>> GetAvailableSingleRateTariffsAsync(string postcode, string brand, FuelType fuelType, int rateCode);
        Task<List<AvailableTariffResult>> GetAvailableMultiRateTariffsAsync(string postcode, string brand, string tariffType, int rateCode);
        Task<List<AvailableTariffResult>> GetPreservedSingleRateTariffsByServicePlanIdAsync(string postcode, string brand, string servicePlanId, FuelType fuelType, int rateCode);
        Task<List<AvailableTariffResult>> GetPreservedMultiRateTariffsByServicePlanIdAsync(string postcode, string brand, string servicePlanId, string tariffType, int rateCode);
    }
}