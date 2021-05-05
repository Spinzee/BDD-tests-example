namespace Products.Model.Energy
{
    using System;
    using Core;

    [Serializable]
    public class Projection
    {
        // ReSharper disable once InconsistentNaming
        public string MSOA { get; set; }

        // ReSharper disable once InconsistentNaming
        public string LSOA { get; set; }

        public double? EnergyAveStandardElecKwh { get; set; }

        public double? EnergyAveEcon7ElecKwh { get; set; }

        public double? EnergyAveStandardGasKwh { get; set; }

        public double? EnergyEconomy7DayElecKwh { get; set; }

        public double? EnergyEconomy7NightElecKwh { get; set; }

        public UsageFrequency Frequency { get; set; } = UsageFrequency.Annual;
    }
}
