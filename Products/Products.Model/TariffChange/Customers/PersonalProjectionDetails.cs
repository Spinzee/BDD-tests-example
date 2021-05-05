namespace Products.Model.TariffChange.Customers
{
    public class PersonalProjectionDetails
    {
        public int SiteId { get; set; }
        public double GasUsage { get; set; }
        public double GasSpend { get; set; }
        public double ElectricityUsage { get; set; }
        public double ElectricitySpend { get; set; }
    }
}