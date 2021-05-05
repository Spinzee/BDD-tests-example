namespace Products.Tests.TariffChange.Fakes.Services
{
    using System.Collections.Generic;
    using Service.TariffChange;
    using WebModel.ViewModels.TariffChange;

    public class FakeAvailableTariffService : IAvailableTariffService
    {
        public List<AvailableTariff> AvailableTariffs { get; set; }

        public void ClearAvailableTariffs()
        {
            AvailableTariffs = null;
        }

        public List<AvailableTariff> GetAvailableTariffs()
        {
            return AvailableTariffs;
        }

        public void SetAvailableTariffs(List<AvailableTariff> availableTariffs)
        {
            AvailableTariffs = availableTariffs;
        }
    }
}