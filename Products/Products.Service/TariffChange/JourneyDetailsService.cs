namespace Products.Service.TariffChange
{
    using System;
    using System.Collections.Generic;
    using Core;
    using Infrastructure;
    using Products.Model.TariffChange;
    using Products.Model.TariffChange.Customers;

    public class JourneyDetailsService : IJourneyDetailsService
    {
        private readonly ITariffChangeSessionService _tariffChangeSessionService;

        public JourneyDetailsService(ITariffChangeSessionService tariffChangeSessionService)
        {
            Guard.Against<ArgumentNullException>(tariffChangeSessionService == null, "tariffChangeSessionService is null");
            _tariffChangeSessionService = tariffChangeSessionService;
        }

        public void SetCustomerAccount(CustomerAccount customerAccount)
        {
            JourneyDetails journeyDetails = _tariffChangeSessionService.GetJourneyDetails();
            journeyDetails.CustomerAccount = customerAccount;
            _tariffChangeSessionService.SetJourneyDetails(journeyDetails);
        }

        public CustomerAccount GetCustomerAccount()
        {
            return _tariffChangeSessionService.GetJourneyDetails().CustomerAccount;
        }

        public void SetCustomer(Customer customer)
        {
            JourneyDetails journeyDetails = _tariffChangeSessionService.GetJourneyDetails();
            journeyDetails.Customer = customer;
            _tariffChangeSessionService.SetJourneyDetails(journeyDetails);
        }

        public Customer GetCustomer()
        {
            return _tariffChangeSessionService.GetJourneyDetails().Customer;
        }

        public void SavePassword(string password)
        {
            var journeyDetails = _tariffChangeSessionService.GetJourneyDetails();
            journeyDetails.Customer.Password = password;
        }

        public void ClearJourneyDetails()
        {
            _tariffChangeSessionService.RemoveJourneyDetails();
        }

        public void SetCustomerJourney(CTCJourneyType ctcJourneyType)
        {
            JourneyDetails journeyDetails = _tariffChangeSessionService.GetJourneyDetails();
            journeyDetails.CTCJourneyType = ctcJourneyType;
            _tariffChangeSessionService.SetJourneyDetails(journeyDetails);
        }

        public CTCJourneyType GetCustomerJourney()
        {
            return _tariffChangeSessionService.GetJourneyDetails().CTCJourneyType;
        }

        public void SetMultipleCustomerAccounts(IList<CustomerAccount> customerAccounts)
        {
            JourneyDetails journeyDetails = _tariffChangeSessionService.GetJourneyDetails();
            journeyDetails.MultipleAccounts = customerAccounts;
            _tariffChangeSessionService.SetJourneyDetails(journeyDetails);
        }

        public void ClearMultipleCustomerAccounts()
        {
            JourneyDetails journeyDetails = _tariffChangeSessionService.GetJourneyDetails();
            journeyDetails.MultipleAccounts = null;
            _tariffChangeSessionService.SetJourneyDetails(journeyDetails);
        }

        public IList<CustomerAccount> GetCustomerAccounts()
        {
            JourneyDetails journeyDetails = _tariffChangeSessionService.GetJourneyDetails();
            return journeyDetails.MultipleAccounts;
        }
    }
}