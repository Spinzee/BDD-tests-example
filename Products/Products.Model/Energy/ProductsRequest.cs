using System;

namespace Products.Model.Energy
{
    [Serializable]
    public class ProductsRequest
    {
        public string PostCode { get; set; }
        public string MeterType { get; set; }
        public string AccountType { get; set; }
        public string BillingPreference { get; set; }
        public string PaymentType { get; set; }
        public string FuelType { get; set; }
        public double? StandardGasKwh { get; set; }
        public double? StandardElectricityKwh { get; set; }
        public double? Economy7ElectricityDayKwh { get; set; }
        public double? Economy7ElectricityNightKwh { get; set; }
    }
}
