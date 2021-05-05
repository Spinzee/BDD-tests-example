namespace Products.Service.Broadband
{
    using Products.WebModel.ViewModels.Broadband;

    public interface IPackageService
    {
        AvailablePackagesViewModel GetAvailablePackagesViewModel();

        bool? GetBroadbandProductsForAvailablePackages(string selectedTalkProductCode, string talkCode);

        SelectedPackageViewModel GetSelectedPackageViewModel();

        SelectedPackageViewModel SetSelectedPackageViewModel(SelectedPackageViewModel model, string talkCode);

        int GetSpeedCap();

        AvailableTalkPackagesViewModel GetAvailableTalkPackageViewModel();
    }
}