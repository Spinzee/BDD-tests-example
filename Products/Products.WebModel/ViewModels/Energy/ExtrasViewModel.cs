namespace Products.WebModel.ViewModels.Energy
{
    using System.Collections.Generic;

    public class ExtrasViewModel : BaseViewModel
    {
        public YourPriceViewModel ShoppingBasketViewModel { get; set; }

        public List<ExtrasItemViewModel> Extras { get; set; }

        public string HowSavingsCalculatedText { get; set; }
    }
}