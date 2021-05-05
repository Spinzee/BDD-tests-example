namespace Products.WebModel.ViewModels.Broadband
{
    using System.Collections.Generic;
    using Common;
    using Core;

    public class SelectedPackageViewModel : BaseViewModel
    {
        public YourPriceViewModel YourPriceViewModel { get; set; }

        public BroadbandType BroadbandType { get; set; }

        public string PostCode { get; set; }

        public string SelectedProductName { get; set; }

        public string SelectedProductDescription { get; set; }

        public string SelectedTalkProductCode { get; set; }

        public string MaxDownload { get; set; }

        public string MaxUpload { get; set; }

        public string MinDownload { get; set; }

        public string MinUpload { get; set; }

        public List<TalkProductViewModel> TalkProducts { get; set; }

        public string CliNumber { get; set; }      

        public string InstallationFeeText { get; set; }

        public string CancellationChargesParagraph2 { get; set; }       
        
        public bool AvailablePackageLinkIsHidden { get; set; }

        public List<TermsAndConditionsPdfLink> TermsAndConditionsPdfLinks { get; set; }

        public string TermsAndConditionsParagraph1 { get; set; }

        public string TermsAndConditionsParagraph2 { get; set; }

        public string TermsAndConditionsParagraph3 { get; set; }

        public string TermsAndConditionsParagraph4 { get; set; }

        public string CancellationParagraph3 { get; set; }

        public bool ShowCancellationParagraph4 { get; set; }

        public string ZeroPriceBullet1 { get; set; }

        public string LineFeatureDisclaimer { get; set; }
    }
}