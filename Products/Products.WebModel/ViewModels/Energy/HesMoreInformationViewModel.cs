namespace Products.WebModel.ViewModels.Energy
{
    using System.Collections.Generic;

    public class HesMoreInformationViewModel
    {
        public string ExcessAmount { get; set; }

        public List<string> WhatsIncluded { get; set; }
        
        public List<string> WhatsExcluded { get; set; }
        
        public string ProjectedHesPackageMonthlyCost { get; set; }
        
        public string OriginalFixNProtectMonthlyCost { get; set; }
        
        public string ProjectedMonthlySavingsAmount { get; set; }
        
        public string BundleDisclaimerModalText { get; set; }
        
        public string ExcessText { get; set; }
    }
}