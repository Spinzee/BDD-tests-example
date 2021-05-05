using System.Collections.Generic;

namespace Products.WebModel.ViewModels.HomeServices
{
    public class CoverSummaryViewModel
    {
        public string SelectedProductName { get; set; }
        public string ContractStartDate { get; set; }
        public string ContractLength { get; set; }
        public string ExcessAmount { get; set; }
        public List<ExtraViewModel> Extras { get; set; }
        public string TotalMonthlyCost { get; set; }
        public string TotalYearlyProductCost { get; set; }
        public string YearlyTotalCost { get; set; }
        public bool HasOffers { get; set; }
        public string CoverMonthlyPaymentAmount { get; set; }
        public string OfferParagraph { get; set; }
        public string OfferHeader { get; set; }
    }

    public class ExtraViewModel
    {
        public string ExtraName { get; set; }
        public string ExtraMonthlyCost { get; set; }
        public string ExtraYearlyCost { get; set; }
    }
}
