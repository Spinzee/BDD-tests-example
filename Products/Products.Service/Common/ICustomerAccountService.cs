namespace Products.Service.Common
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Products.Model.TariffChange.Customers;

    public interface ICustomerAccountService
    {
        CustomerAccount GetCustomerAccount(string accountNumber);

        string[] ActionTariffChange(List<CustomerAccount> customerAccounts, PersonalProjectionDetails personalProjectionDetails, string emailAddress);

        void SendConfirmationEmail(ConfirmationEmailParameters emailParameters);

        void AddPersonalProjection(PersonalProjectionDetails personalProjectionDetails);

        bool UpdateCustomerTariffSelection();

        Task<List<string>> GetUserAccountsByLoginName(string emailAddress);
    }
}