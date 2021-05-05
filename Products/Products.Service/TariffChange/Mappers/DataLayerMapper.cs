namespace Products.Service.TariffChange.Mappers
{
    using System.Collections.Generic;
    using System.Globalization;
    using Infrastructure.Extensions;
    using Products.Model.TariffChange.Customers;
    using Products.WebModel.Resources.TariffChange;

    public static class DataLayerMapper
    {
        public static Dictionary<string, string> GetDataLayerDictionary(Customer customer, CustomerAccount customerAccount)
        {
            return new Dictionary<string, string>
            {
                { DataLayer_Resources.EnergyProduct, customer?.CustomerSelectedTariff?.DisplayName ?? string.Empty },
                { DataLayer_Resources.FuelType, customer?.GetCustomerFuelType().ToDescription() },
                { DataLayer_Resources.NewTariffStart, customer?.CustomerSelectedTariff?.EffectiveDate == null ? string.Empty : customer.CustomerSelectedTariff?.EffectiveDate.ToShortDateString()},
                { DataLayer_Resources.ExpiryDays, customer?.NumberOfDaysToTariffExpire()?.ToString() ?? string.Empty },
                { DataLayer_Resources.MonthlyCost, customer?.CustomerSelectedTariff?.ProjectedMonthlyCostValue.ToString(CultureInfo.InvariantCulture) ?? string.Empty },
                { DataLayer_Resources.YearlyCost, customer?.CustomerSelectedTariff?.ProjectedAnnualCostValue.ToString(CultureInfo.InvariantCulture) ?? string.Empty },
                { DataLayer_Resources.TariffCards, customer?.CountOfTariffs },
                { DataLayer_Resources.ExitFee, customer?.CustomerSelectedTariff?.ExitFee.ToString(CultureInfo.InvariantCulture) ?? string.Empty },
                { DataLayer_Resources.SmartEligible, customerAccount?.IsSmartEligible.ToString() ?? string.Empty}
            };
        }
    }
}
