namespace Products.WebModel.ViewModels.Energy
{
    using Common;
    using Resources.Common;

    public class BaseSummaryExtraWithModal : BaseSummaryExtra
    {
        public string ModalLinkAltText { get; set; } = ProductFeatures_Resources.ViewFullDetailsAltText;

        public string ModalLinkText { get; set; } = ProductFeatures_Resources.ViewFullDetailsLink;

        public string ProductCode { get; set; }

        public string SectionId { get; set; }

        public ConfirmationModalViewModel RemoveExtraModalViewModel { get; set; }

        public ExtrasItemViewModel ExtrasItemViewModel { get; set; }
    }
}
