using System.Collections.Generic;

namespace Products.WebModel.ViewModels.Energy
{
    public class OurPriceProductViewModel
    {
        public string TariffName { get; set; }

        public bool DisplayCheckAvailabilityLink { get; set; }

        public List<TariffInformationLabelViewModel> TariffOptions { get; set; }
    }
}
