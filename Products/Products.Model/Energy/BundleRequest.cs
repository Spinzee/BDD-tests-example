namespace Products.Model.Energy
{
    public class BundleRequest
    {
        public string PostCode { get; set; }
        public double StandardGasKwh { get; set; }
        public double StandardElectricityKwh { get; set; }
        public double Economy7ElectricityDayKwh { get; set; }
        public double Economy7ElectricityNightKwh { get; set; }
    }
}
