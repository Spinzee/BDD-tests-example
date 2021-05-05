namespace Products.Model.TariffChange
{
    using System.Collections.Generic;
    using Core;
    using Customers;

    public class JourneyDetails
    {
        public CustomerAccount CustomerAccount { get; set; }

        public Customer Customer { get; set; }

        public CTCJourneyType CTCJourneyType { get; set; }

        public IList<CustomerAccount> MultipleAccounts { get; set; }
    }
}