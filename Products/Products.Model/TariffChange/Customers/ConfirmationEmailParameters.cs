namespace Products.Model.TariffChange.Customers
{
    using System;

    public class ConfirmationEmailParameters
    {
        public string AccountHolderName { get; set; }

        public string TariffName { get; set; }

        public string EmailAddress { get; set; }

        public DateTime EffectiveDate { get; set; }

        public bool IncludeBroadbandRedeemContent { get; set; }
    }
}