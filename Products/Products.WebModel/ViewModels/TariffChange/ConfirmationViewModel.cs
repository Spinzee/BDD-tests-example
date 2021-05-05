namespace Products.WebModel.ViewModels.TariffChange
{
    using System.Collections.Generic;
    using Core;

    public class ConfirmationViewModel
    {
        public string CustomerEmailAddress { get; set; }

        public CTCJourneyType CTCJourneyType { get; set; }

        public string Header { get; set; }

        public string Paragraph { get; set; }

        public List<string> BulletList { get; set; }

        public Dictionary<string, string> DataLayer { get; set; }

        public bool ShowSmartBookingLink { get; set; }

        public bool IsSmartCustomer { get; set; }

        public bool ShowTelcoLink { get; set; }

        public bool IsPreLoginJourney => CTCJourneyType == CTCJourneyType.PreLogIn;
    }
}
