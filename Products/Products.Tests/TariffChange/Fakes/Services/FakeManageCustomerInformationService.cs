namespace Products.Tests.TariffChange.Fakes.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using ServiceWrapper.ManageCustomerInformationService;

    public class FakeManageCustomerInformationServiceWrapper : IManageCustomerInformationServiceWrapper
    {
        private readonly GetCustomerAccountsResponse _customerAccountsResponse;
        private readonly Exception _exception;
        private readonly GetCustomerAccountsResponse _fakeCustomerAccountsResponse;

        public FakeManageCustomerInformationServiceWrapper()
        {
        }

        public FakeManageCustomerInformationServiceWrapper(GetCustomerAccountsResponse fakeCustomerAccountsResponse)
        {
            _fakeCustomerAccountsResponse = fakeCustomerAccountsResponse;
        }

        public FakeManageCustomerInformationServiceWrapper(Dictionary<string, string> fakeCustomerInformationDictionary)
        {
            _customerAccountsResponse = new GetCustomerAccountsResponse { CustomerAccounts = new List<CustomerAccountsType>() };

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (CustomerAccountsType account in fakeCustomerInformationDictionary.Select(fakeCustomerInformation => new CustomerAccountsType
            {
                CustomerAccountNumber = fakeCustomerInformation.Key,
                Sites = new List<SitesType>
                {
                    new SitesType
                    {
                        Services = new List<ServiceType>
                        {
                            new ServiceType
                            {
                                ServiceStatus = ServiceStatusType.Active,
                                Servicetype = ServiceTypeType.electricity,
                                ServicePlanDescription = "Standard"
                            }
                        },
                        SiteAddress = new AddressType
                        {
                            PostCode = fakeCustomerInformation.Value
                        }
                    }
                }
            }))
            {
                _customerAccountsResponse.CustomerAccounts.Add(account);
            }
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        public FakeManageCustomerInformationServiceWrapper(FakeMCISData[] fakeMcisData)
        {
            _customerAccountsResponse = new GetCustomerAccountsResponse { CustomerAccounts = new List<CustomerAccountsType>() };

            foreach (FakeMCISData mcisData in fakeMcisData)
            {
                var account = new CustomerAccountsType
                {
                    CustomerAccountNumber = mcisData.CustomerAccountNumber,
                    CustomerAccountStatus = mcisData.CustomerAccountStatus,
                    BrandCode = mcisData.BrandCode,
                    Sites = new List<SitesType>
                    {
                        new SitesType
                        {
                            Services = new List<ServiceType>
                            {
                                new ServiceType
                                {
                                    ServicePlanDescription = mcisData.ServicePlanDescription,
                                    Servicetype = mcisData.Service,
                                    ServiceStatus = mcisData.ServiceStatus
                                }
                            },
                            SiteAddress = new AddressType
                            {
                                PostCode = mcisData.Postcode
                            }
                        }
                    }
                };

                _customerAccountsResponse.CustomerAccounts.Add(account);
            }
        }

        public FakeManageCustomerInformationServiceWrapper(Exception exception)
        {
            _exception = exception;
        }

        public GetCustomerAccountsResponse GetCustomerAccounts(string[] customerAccountNumbers)
        {
            if (_exception != null)
            {
                throw _exception;
            }

            if (_fakeCustomerAccountsResponse != null)
            {
                return _fakeCustomerAccountsResponse;
            }

            var customerAccountResponse = new GetCustomerAccountsResponse();
            List<CustomerAccountsType> customerAccounts =
                _customerAccountsResponse.CustomerAccounts.Where(c => c.CustomerAccountNumber == customerAccountNumbers[0]).ToList();
            if (customerAccounts.Count == 0)
            {
                customerAccounts = GetEmptyCustomerAccount();
            }

            customerAccountResponse.CustomerAccounts = customerAccounts;
            return customerAccountResponse;
        }

        private static List<CustomerAccountsType> GetEmptyCustomerAccount()
        {
            var emptyCustomerAccount = new List<CustomerAccountsType>
            {
                new CustomerAccountsType
                {
                    CustomerAccountNumber = string.Empty,
                    CustomerAccountStatus = CustomerAccountStatusType.NotFound,

                    Sites = new List<SitesType>()
                }
            };

            return emptyCustomerAccount;
        }
    }
}