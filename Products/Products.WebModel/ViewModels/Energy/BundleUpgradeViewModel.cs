namespace Products.WebModel.ViewModels.Energy
{
    using System.Collections.Generic;
    using Common;
    using Core.Enums;
    using Infrastructure.Extensions;
    using Resources.Energy;

    public class BundleUpgradeViewModel : BaseViewModel
    {
        public BundleUpgradeViewModel(
            YourPriceViewModel shoppingBasketViewModel,
            string modalProductName,
            string modalTagline,
            string header,
            string upgradeDescription,
            string productCode,
            string nextButtonAction,
            BundlePackageType bundlePackageType,
            double discountedPrice,
            double originalPrice,
            bool isAddedToBasket,
            string baseUrl,
            string howSavingsCalculatedText,
            string postcode = "",
            BroadbandPackageSpeedViewModel broadbandPackageSpeedViewModel = null,
            List<string> whatsIncluded = null,
            List<string> whatsExcluded = null)
        {
            ShoppingBasketViewModel = shoppingBasketViewModel;
            NextButtonAction = nextButtonAction;
            Header = header;
            UpgradeDescription = upgradeDescription;
            ModalProductName = modalProductName;
            ModalTagLine = modalTagline;
            DiscountedPrice = discountedPrice.ToCurrency();
            Price = originalPrice.ToCurrency();
            DiscountAmount = (originalPrice - discountedPrice).ToCurrency();
            ProductCode = productCode;
            IsAddedToBasket = isAddedToBasket;
            IsAddedToBasketStr = isAddedToBasket.ToString().ToLowerInvariant();
            AddRemoveButtonText = isAddedToBasket ? Upgrades_Resources.RemoveButtonText : Upgrades_Resources.AddButtonText;
            AddRemoveButtonAltText = isAddedToBasket ? Upgrades_Resources.RemoveButtonAlt : Upgrades_Resources.AddButtonTextAlt;
            RemoveButtonAltText = Upgrades_Resources.RemoveButtonAlt;
            AddButtonAltText = Upgrades_Resources.AddButtonTextAlt;
            RemoveButtonIconUrl = $"{baseUrl}/Content/Svgs/icons/trashcan-white.svg";
            AddedCssClass = isAddedToBasket ? "added" : "";
            MoreInformationModalId = $"extraMoreInformation-{productCode}";
            MoreInformationModalDataTargetId = $"#{MoreInformationModalId}";
            ModalAddRemoveButtonText = isAddedToBasket ? Extras_Resources.ModalRemoveButtonText : Extras_Resources.ModalAddButtonText;
            ModalAddRemoveButtonCssClass = isAddedToBasket ? "button-secondary" : "button-primary";
            ExtraContainerId = $"extra-container-{productCode}";
            ButtonGroup = $"button-extra-{productCode}";
            ModalExtraPriceHeaderId = $"extra-modal-priceheader-{productCode}";
            SavingsText = $"{Upgrades_Resources.SaveTxt} {DiscountAmount} {Upgrades_Resources.PerMonthTxt}";
            OriginalPriceText = $"{Upgrades_Resources.WasTxt} {Price}";
            BannerImageCssClass = bundlePackageType == BundlePackageType.FixAndFibre
                ? "bundle-upgrade-fixnfibreplus-sales-banner"
                : "bundle-upgrade-fixnprotectplus-sales-banner";
            ModalBannerImageCssClass = bundlePackageType == BundlePackageType.FixAndFibre ? "bundle-upgrade-fixnfibreplus-modal-banner" : "bundle-upgrade-fixnprotectplus-modal-banner";
            BundlePackageType = bundlePackageType;
            AccordionSavingsTextId = $"savings-text-{productCode}";
            ModalSavingsText = bundlePackageType == BundlePackageType.FixAndFibre
                ? Upgrades_Resources.FixNFibrePlusModalSavingsText
                : Upgrades_Resources.FixNProtectPlusModalSavingsText;
            BroadbandPackageSpeedViewModel = broadbandPackageSpeedViewModel;
            Postcode = postcode;
            WhatYouNeedToKnowPartial = bundlePackageType == BundlePackageType.FixAndFibre 
                ? "_FixNFiberWhatYouNeedToKnow" 
                : "_FixNProtectWhatYouNeedToKnow";
            LearnMoreModalPartial = bundlePackageType == BundlePackageType.FixAndFibre
                ? "_UpgradeFixNFibrePlusLearnMoreModal"
                : "_UpgradeFixNProtectPlusLearnMoreModal";
            WhatsExcluded = whatsExcluded ?? new List<string>();
            WhatsIncluded = whatsIncluded ?? new List<string>();
            HowSavingsCalculatedText = howSavingsCalculatedText;
        }

        public string HowSavingsCalculatedText { get; }

        public BroadbandPackageSpeedViewModel BroadbandPackageSpeedViewModel { get; }

        public List<string> WhatsExcluded { get; set; }

        public List<string> WhatsIncluded { get; set; }

        public string WhatYouNeedToKnowPartial { get; }

        public string LearnMoreModalPartial { get; }

        public string Postcode { get; }

        public string Header { get; }

        public string UpgradeDescription { get; }

        public string ModalProductName { get; }

        public string ProductCode { get; }

        public string AccordionSavingsTextId { get; }

        public YourPriceViewModel ShoppingBasketViewModel { get; }

        public string ModalTagLine { get; }

        public string Price { get; }

        public string DiscountedPrice { get; }

        public string DiscountAmount { get; }
        
        public string NextButtonAction { get; }

        public bool IsAddedToBasket { get; }

        public string IsAddedToBasketStr { get; }

        public string AddRemoveButtonText { get; }

        public string ModalAddRemoveButtonText { get; }

        public string ModalAddRemoveButtonCssClass { get; }

        public string ModalSavingsText { get; }

        public string ModalBannerImageCssClass { get; }

        public string AddRemoveButtonAltText { get; }

        public string RemoveButtonAltText { get; }

        public string AddButtonAltText { get; }

        public string RemoveButtonIconUrl { get; }

        public string AddedCssClass { get; }

        public string MoreInformationModalId { get; }

        public string MoreInformationModalDataTargetId { get; }

        public string ExtraContainerId { get; }

        public string ModalExtraPriceHeaderId { get; }

        public string ButtonGroup { get; }

        public string SavingsText { get; }

        public string OriginalPriceText { get; }

        public string BannerImageCssClass { get; }

        public BundlePackageType BundlePackageType { get; }
    }
}