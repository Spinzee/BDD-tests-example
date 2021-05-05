namespace Products.Model.Energy
{
    using Core;
    using Enums;

    public class MeterInformation
    {
        public bool IsInstallerSSE { get; set; }

        public string MeterNumber { get; set; }

        public bool IsPrePay { get; set; }

        public ElectricityMeterType ElectricityMeterType { get; set; }

        public SmartMeterType SmartType { get; set; }

        public FuelType FuelType { get; set; }

        public string MeterSerialNumber { get; set; }
    }
}