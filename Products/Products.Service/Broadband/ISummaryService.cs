namespace Products.Service.Broadband
{
    using Products.WebModel.ViewModels.Broadband;

    public interface ISummaryService
    {
        SummaryViewModel GetSummaryViewModel();
        WebModel.ViewModels.Common.DirectDebitMandateViewModel GetPrintMandateViewModel();
        SummaryViewModel PopulateSummaryViewModel(SummaryViewModel model);
    }
}