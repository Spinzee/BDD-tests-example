using System;

namespace Products.Model.Common
{
    [Serializable]
    public class ContactDetails
    {
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; }
        public bool MarketingConsent { get; set; }
    }
}
