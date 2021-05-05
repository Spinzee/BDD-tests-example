namespace Products.Tests.Energy.Model
{
    using System;
    using Products.Model.Energy;

    public static class FakeProjectionData
    {
        public static Projection GetProjection()
        {
            return new Projection
            {
                EnergyAveStandardElecKwh = 11614.0,
                EnergyAveStandardGasKwh = 46737.0,
                EnergyAveEcon7ElecKwh = 16792.0,
                MSOA = "E02004775",
                LSOA = "E01022953",
                EnergyEconomy7DayElecKwh = Math.Round(16792.0 * 0.58),
                EnergyEconomy7NightElecKwh = Math.Round(16792.0 * 0.42)
            };
        }
    }
}