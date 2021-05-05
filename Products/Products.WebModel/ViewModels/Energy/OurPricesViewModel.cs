namespace Products.WebModel.ViewModels.Energy
{
    using System.Collections.Generic;
    using Core;

    public class OurPricesViewModel
    {
        public string Postcode { get; set; }

        public FuelCategory FuelCategory { get; set; }

        public TariffStatus TariffStatus { get; set; }

        public List<OurPriceProductViewModel> Products { get; set; }
    }
}
