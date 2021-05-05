namespace Products.WebModel.ViewModels.Energy
{
    using Infrastructure.Extensions;
    using Resources.Energy;
    using Products.WebModel.Resources.Common;

    public class PhonePackageUpgradeViewModel
    {
        public PhonePackageUpgradeViewModel(string name, double price, string baseUrl)
        {
            Name = name;
            Price = price.Equals(0.00) ? Resources.LineRentalTalkPriceText : price.ToCurrency();
            RemoveUpgradeButtonAltText = YourPrice_Resources.RemoveUpgradeButtonAltText;
            RemoveUpgradeButtonIconUrl = $"{baseUrl}/Content/Svgs/icons/trashcan-white.svg";
            RemovePhonePackageAltText = YourPrice_Resources.RemovePhonePackageAltText;
        }

        public string Name { get; }

        public string Price { get; }

        public string RemoveUpgradeButtonAltText { get; }

        public string RemoveUpgradeButtonIconUrl { get; }

        public string RemovePhonePackageAltText { get; }
    }
}