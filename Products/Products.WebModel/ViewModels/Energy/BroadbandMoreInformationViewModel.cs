namespace Products.WebModel.ViewModels.Energy
{
    using Common;

    public class BroadbandMoreInformationViewModel
    {
        public string ProjectedBroadbandMonthlyCost { get; set; }

        public string OriginalBroadbandMonthlyCost { get; set; }

        public string BundleDisclaimerModalText { get; set; }

        public string ProjectedBroadbandMonthlySavingsAmount { get; set; }

        public BroadbandPackageSpeedViewModel BroadbandPackageSpeed { get; set; }
    }
}
