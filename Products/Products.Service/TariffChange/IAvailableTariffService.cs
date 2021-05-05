namespace Products.Service.TariffChange
{
    using System.Collections.Generic;
    using Products.WebModel.ViewModels.TariffChange;

    public interface IAvailableTariffService
    {
        void SetAvailableTariffs(List<AvailableTariff> availableTariffs);

        List<AvailableTariff> GetAvailableTariffs();

        void ClearAvailableTariffs();
    }
}