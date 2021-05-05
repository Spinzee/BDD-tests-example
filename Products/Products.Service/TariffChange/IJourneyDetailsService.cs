namespace Products.Service.TariffChange
{
    using System.Collections.Generic;
    using Core;
    using Products.Model.TariffChange.Customers;

    public interface IJourneyDetailsService
    {
        void SetCustomerAccount(CustomerAccount customerAccount);

        CustomerAccount GetCustomerAccount();

        void SetCustomer(Customer customer);

        Customer GetCustomer();

        void ClearJourneyDetails();

        void SetCustomerJourney(CTCJourneyType customerJourney);

        CTCJourneyType GetCustomerJourney();

        void SetMultipleCustomerAccounts(IList<CustomerAccount> customerAccounts);

        void ClearMultipleCustomerAccounts();

        IList<CustomerAccount> GetCustomerAccounts();

        void SavePassword(string password);
    }
}