namespace Products.WebModel.ViewModels.Energy
{
    public class EnergyUsageViewModel : BaseViewModel
    {
        public int ActiveTabIndex { get; set; }
        public KnownEnergyUsageViewModel KnownEnergyUsageViewModel { get; set; }
        public UnknownEnergyUsageViewModel UnknownEnergyUsageViewModel { get; set; }
    }
}
