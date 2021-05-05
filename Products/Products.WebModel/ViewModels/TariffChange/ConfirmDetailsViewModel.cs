namespace Products.WebModel.ViewModels.TariffChange
{
    using Common;
    using Core;
    using Products.WebModel.Resources.TariffChange;

    public class ConfirmDetailsViewModel : BaseViewModel
    {
        public bool CustomerAlertActive { get; set; }

        public bool HasMultipleServices { get; set; }

        public bool IsValidForPostCode { get; set; }

        public CTCJourneyType CTCJourneyType { get; set; }

        public MultiSiteAddressesViewModel MultiSiteAddressesViewModel { get; set; }

        public ConfirmAddressViewModel ConfirmAddressViewModel { get; set; }

        public ModalTitleAndBody GetLoadingModal()
        {
            return new ModalTitleAndBody
            {
                Title = AvailableTariffs_Resources.LoadingPopupTitle,
                Message = AvailableTariffs_Resources.LoadingPopupBody
            };
        }
    }
}