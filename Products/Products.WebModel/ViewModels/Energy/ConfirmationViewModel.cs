namespace Products.WebModel.ViewModels.Energy
{
    using System.Collections.Generic;

    public class ConfirmationViewModel
    {
        public string ProductName { get; set; }

        public DataLayerViewModel DataLayer { get; set; }

        public bool IsABundle { get; set; }

        public ConfirmationCrossSellBannerViewModel CrossSellBanner { get; set; }

        public string BoxText { get; set; }

        public string ThankYouText { get; set; }

        public string HelpPageUrlAltText { get; set; }

        public string HelpPageUrlText { get; set; }

        public bool IsSmartMessageVisible { get; set; }

        public List<string> WhatHappensNext { get; set; }
    }
}