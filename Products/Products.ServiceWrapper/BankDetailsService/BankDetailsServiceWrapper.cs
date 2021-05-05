using Products.Infrastructure;
using System;

namespace Products.ServiceWrapper.BankDetailsService
{
    public class BankDetailsServiceWrapper : IBankDetailsServiceWrapper
    {
        private readonly IConfigManager _configManager;

        public BankDetailsServiceWrapper(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        private messageHeader CreateMessageHeader()
        {
            string csEnvironment = _configManager.GetAppSetting("CSenvironment");
            return new messageHeader
            {
                channelID = new messageHeaderChannelID(),
                clientID = "MKTWEB",
                correlationID = Guid.NewGuid().ToString(),
                creationTimeStamp = DateTime.UtcNow,
                environment = csEnvironment,
                messageID = Guid.NewGuid().ToString()
            };
        }

        public getBankDetailsResponse GetBankDetails(string sortCode, string accountNumber)
        {
            using (var client = new BankDetailsServiceInterfaceClient("BankDetailsService"))
            {
                var messageHeader = CreateMessageHeader();
                var bankDetailsRequest = new getBankDetailsRequest
                {
                    messageHeader = messageHeader,
                    bankAccountNumber = accountNumber,
                    brandLineOfBusinessCollection = new brandDetailsType[0],
                    sortCode = sortCode
                };

                var bankDetailsResponse = new getBankDetailsResponse
                {
                    messageHeader = messageHeader,
                    bankFormattedAddress = new bankFormattedAddressType(),
                    bankName = string.Empty,
                    corporateAccountValid = false,
                    serviceDetailsCollection = new serviceDetailsType[0],
                    sortCodeAccountNumberValid = false,
                    sortCodeValid = false,
                    suppliedBankAccountNumber = string.Empty,
                    suppliedSortCode = string.Empty
                };

                client.Open();
                client.getBankDetails(ref bankDetailsRequest.messageHeader, bankDetailsRequest.sortCode,
                    bankDetailsRequest.bankAccountNumber, bankDetailsRequest.brandLineOfBusinessCollection,
                    out bankDetailsResponse.suppliedBankAccountNumber, out bankDetailsResponse.bankName,
                    out bankDetailsResponse.bankFormattedAddress, out bankDetailsResponse.sortCodeValid,
                    out bankDetailsResponse.sortCodeAccountNumberValid, out bankDetailsResponse.corporateAccountValid,
                    out bankDetailsResponse.serviceDetailsCollection);

                return bankDetailsResponse;
            }
        }
        
    }
}