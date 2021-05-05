namespace Products.WebModel.ViewModels.Energy
{
    using Common;

    public class PhonePackageViewModel : BaseViewModel
    {
        public BroadbandPackageSpeedViewModel BroadbandPackageSpeedViewModel { get; set; }

        public KeepYourNumberViewModel KeepYourNumberViewModel { get; set; }

        public SelectTalkPackageViewModel SelectTalkPackageViewModel { get; set; }

        public YourPriceViewModel ShoppingBasketViewModel { get; set; }

        public NewInstallationViewModel NewInstallationViewModel { get; set; }

        public bool ApplyInstallationFee { get; set; }

        public string HowSavingsCalculatedText { get; set; }
    }
}