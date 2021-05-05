using Products.WebModel.ViewModels.TariffChange;
using System.Threading.Tasks;

namespace Products.Service.TariffChange
{
    using System.Collections.Generic;
    using Model.Energy;

    public interface ITariffService
    {
        Task<TariffsViewModel> GetCurrentAndAvailableTariffsAsync(List<CMSEnergyContent> cmsEnergyContents);
        SelectedTariffsViewModel GetCurrentSelectedTariff(string tariffName, bool isImmediateRenewal);
    }
}