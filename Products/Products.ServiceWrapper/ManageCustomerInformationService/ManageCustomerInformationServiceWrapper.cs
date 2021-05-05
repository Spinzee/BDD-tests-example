namespace Products.ServiceWrapper.ManageCustomerInformationService
{
    using System.Linq;

    public interface IManageCustomerInformationServiceWrapper
    {
        GetCustomerAccountsResponse GetCustomerAccounts(string[] customerAccountNumbers);
    }

    public class ManageCustomerInformationServiceWrapper : IManageCustomerInformationServiceWrapper
    {
        private readonly IManageCustomerInformationServiceClientFactory _manageCustomerInformationServiceClientFactory;
        public ManageCustomerInformationServiceWrapper(IManageCustomerInformationServiceClientFactory manageCustomerInformationServiceClientFactory)
        {
            _manageCustomerInformationServiceClientFactory = manageCustomerInformationServiceClientFactory;
        }

        public GetCustomerAccountsResponse GetCustomerAccounts(string[] customerAccountNumbers)
        {
            GetCustomerAccountsResponse response;

            using (var client = _manageCustomerInformationServiceClientFactory.Create())
            {
                GetCustomerAccountsRequest request = new GetCustomerAccountsRequest
                {
                    MessageHeader = _manageCustomerInformationServiceClientFactory.CreateMessageHeader(),
                    CustomerAccountNumber = customerAccountNumbers.ToList()
                };

                response = ((IManageCustomerInformationService)client).GetCustomerAccounts(request);
            }

            return response;
        }
    }
}
