namespace Products.WebModel.ViewModels.TariffChange
{
    using Core;

    public class ConfirmAddressViewModel
    {
        public string Greeting { get; set; }

        public string FormattedAddress { get; set; }

        public bool IsCustomerAccountSet { get; set; }

        public CTCJourneyType CTCJourneyType { get; set; }
    }
}
