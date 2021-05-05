namespace Products.Model.Energy
{
    public class EnergyMultiplier
    {
        public string MultiplierType { get; set; }

        public int? Order { get; set; }

        public string Value { get; set; }

        public double? MultiplierElec { get; set; }

        public double? MultiplierGas { get; set; }
    }
}