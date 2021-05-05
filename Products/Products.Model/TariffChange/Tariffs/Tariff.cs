namespace Products.Model.TariffChange.Tariffs
{
    using Core;
    using Infrastructure.Extensions;

    public class Tariff
    {
        public string Name { get; set; }

        public string DisplayName => Name.TrimEconomyWording();

        public string Type { get; set; }

        public TariffForFuel ElectricityDetails { get; set; }

        public TariffForFuel GasDetails { get; set; }

        public bool IsFollowOnTariff { get; set; }

        public TariffGroup TariffGroup { get; set; }
    }
}