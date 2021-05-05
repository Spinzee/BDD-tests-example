using System;

namespace Products.Model.TariffChange.Customers.Profile
{
    [Serializable]
    public class MarketingConsent
    {

        public MarketingConsent()
        {
            ConventionalMarketing = true;
        }

        public bool EmailNewsletter { get; set; }

        public bool EmailMarketing { get; set; }

        public bool ConventionalMarketing { get; set; }
    }
}