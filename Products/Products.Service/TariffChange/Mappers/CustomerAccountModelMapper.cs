namespace Products.Service.TariffChange.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Model.TariffChange.Customers;
    using Model.TariffChange.Tariffs;
    using ServiceWrapper.ManageCustomerInformationService;
    using FuelType = Model.Enums.FuelType;
    using ServiceStatusType = ServiceWrapper.ManageCustomerInformationService.ServiceStatusType;

    public static class CustomerAccountModelMapper
    {
        public const string Fixed = "Fixed";
        public const string Evergreen = "Evergreen";
        public const string Standard = "Standard";

        public static CustomerAccount MapCustomerAccountsResponseToCustomerAccount(GetCustomerAccountsResponse response, string accountNumber)
        {
            bool hasCustomerAccount = response.CustomerAccounts.Count > 0;
            CustomerAccountsType customerAccount = hasCustomerAccount ? response.CustomerAccounts[0] : null;
            bool hasSite = hasCustomerAccount && customerAccount.Sites.Count > 0;
            SitesType site = hasSite ? customerAccount.Sites[0] : null;
            bool hasService = hasSite && site.Services.Count > 0;
            ServiceType service = hasService ? site.Services[0] : null;
            bool isBillingException = customerAccount != null && customerAccount.BillingException;

            FuelType fuelType;
            if (hasService && service.Servicetype == ServiceTypeType.electricity)
            {
                fuelType = FuelType.Electricity;
            }
            else if (hasService && service.Servicetype == ServiceTypeType.gas)
            {
                fuelType = FuelType.Gas;
            }
            else
            {
                fuelType = FuelType.None;
            }

            return new CustomerAccount
            {
                IsBillingException = isBillingException,
                LastBilledDate = response.CustomerAccounts[0].LastBillDate,
                SiteDetails = new SiteDetails
                {
                    AccountNumber = accountNumber,
                    Address = hasSite ? FormattedAddress(site.SiteAddress) : string.Empty,
                    Found = hasCustomerAccount && customerAccount.CustomerAccountStatus != CustomerAccountStatusType.NotFound,
                    HasMultipleServices = hasCustomerAccount && customerAccount.CustomerAccountStatus == CustomerAccountStatusType.MultipleServices,
                    HasSingleActiveEnergyServiceAccount = hasService && customerAccount.CustomerAccountStatus == CustomerAccountStatusType.Found &&
                                                          service.ServiceStatus == ServiceStatusType.Active && fuelType != FuelType.None,
                    PostCode = hasSite ? site.SiteAddress.PostCode : string.Empty,
                    SiteId = hasSite ? Convert.ToInt32(site.SiteAddress.AddressID) : 0,
                    ServiceStatusType = hasService
                        ? (Model.TariffChange.Customers.ServiceStatusType) service.ServiceStatus
                        : Model.TariffChange.Customers.ServiceStatusType.Active
                },
                CurrentTariff = new CurrentTariffForFuel
                {
                    Name = hasService ? service.ServicePlanDescription : string.Empty,
                    BrandCode = hasCustomerAccount ? customerAccount.BrandCode : string.Empty,
                    FuelType = fuelType,
                    TariffType = GetTariffTypeForService(service)
                },
                IsSmart = MapCharacteristic(response.CustomerAccounts[0].AccountCharacteristicsCollection, "IsSmart"),
                IsSmartEligible = MapCharacteristic(response.CustomerAccounts[0].AccountCharacteristicsCollection, "IsSmartElgb"),
                IsWHD = MapCharacteristic(response.CustomerAccounts[0].AccountCharacteristicsCollection, "IsWHDBenElg")
            };
        }

        public static CustomerAccount MapServiceCustomerAccountToCustomerAccount(CustomerAccountsType customerAccount)
        {
            bool hasSite = customerAccount.Sites.Count > 0;
            SitesType site = hasSite ? customerAccount.Sites[0] : null;
            bool hasService = hasSite && site.Services.Count > 0;
            ServiceType service = hasService ? site.Services[0] : null;
            bool isBillingException = customerAccount.BillingException;

            FuelType fuelType;
            if (hasService && service.Servicetype == ServiceTypeType.electricity)
            {
                fuelType = FuelType.Electricity;
            }
            else if (hasService && service.Servicetype == ServiceTypeType.gas)
            {
                fuelType = FuelType.Gas;
            }
            else
            {
                fuelType = FuelType.None;
            }

            return new CustomerAccount
            {
                IsBillingException = isBillingException,
                LastBilledDate = customerAccount.LastBillDate,
                SiteDetails = new SiteDetails
                {
                    AccountNumber = customerAccount.CustomerAccountNumber,
                    Address = hasSite ? FormattedAddress(site.SiteAddress) : string.Empty,
                    Found = customerAccount.CustomerAccountStatus != CustomerAccountStatusType.NotFound,
                    HasMultipleServices = customerAccount.CustomerAccountStatus == CustomerAccountStatusType.MultipleServices,
                    HasSingleActiveEnergyServiceAccount = hasService && customerAccount.CustomerAccountStatus == CustomerAccountStatusType.Found &&
                                                          service.ServiceStatus == ServiceStatusType.Active && fuelType != FuelType.None,
                    PostCode = hasSite ? site.SiteAddress.PostCode : string.Empty,
                    SiteId = hasSite ? Convert.ToInt32(site.SiteAddress.AddressID) : 0,
                    ServiceStatusType = hasService
                        ? (Model.TariffChange.Customers.ServiceStatusType) service.ServiceStatus
                        : Model.TariffChange.Customers.ServiceStatusType.Active
                },
                CurrentTariff = new CurrentTariffForFuel
                {
                    Name = hasService ? service.ServicePlanDescription : string.Empty,
                    BrandCode = customerAccount.BrandCode,
                    FuelType = fuelType,
                    TariffType = GetTariffTypeForService(service)
                },
                IsSmart = MapCharacteristic(customerAccount.AccountCharacteristicsCollection, "IsSmart"),
                IsSmartEligible = MapCharacteristic(customerAccount.AccountCharacteristicsCollection, "IsSmartElgb"),
                IsWHD = MapCharacteristic(customerAccount.AccountCharacteristicsCollection, "IsWHDBenElg")
            };
        }

        private static string GetTariffTypeForService(ServiceType service)
        {
            return service == null ? Evergreen : service.ServicePlanDescription.ToLower().Contains(Standard.ToLower()) ? Evergreen : Fixed;
        }

        private static bool MapCharacteristic(IEnumerable<CustomerAccountsTypeAccountCharacteristics> characteristicsList, string key)
        {
            return characteristicsList != null && (from c in characteristicsList where c.Key == key select bool.Parse(c.Value)).FirstOrDefault();
        }

        private static string FormattedAddress(AddressType address)
        {
            if (address == null)
            {
                return string.Empty;
            }

            string[] lines = { address.AddressLine1, address.AddressLine2, address.AddressLine3, address.AddressLine4, address.PostCode };

            return string.Join(", " + Environment.NewLine, lines.Where(x => !string.IsNullOrEmpty(x)));
        }
    }
}