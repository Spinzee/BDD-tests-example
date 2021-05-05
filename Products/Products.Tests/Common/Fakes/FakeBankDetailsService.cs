namespace Products.Tests.Common.Fakes
{
    using System;
    using ServiceWrapper.BankDetailsService;

    public class FakeBankDetailsService : IBankDetailsServiceWrapper
    {
        public getBankDetailsResponse GetBankDetailsResponse { get; set; } = PositiveBankServiceResponse();

        public bool ServiceCalled { get; set; }

        public bool ThrowException { get; set; } = false;

        public Exception Exception { get; set; } = new Exception();

        public FakeBankDetailsService()
        {
        }

        public FakeBankDetailsService(getBankDetailsResponse getBankDetailsResponse)
        {
            GetBankDetailsResponse = getBankDetailsResponse;
        }

        public getBankDetailsResponse GetBankDetails(string sortCode, string accountNumber)
        {
            if (ThrowException)
            {
                throw Exception;
            }

            ServiceCalled = true;

            return GetBankDetailsResponse;
        }

        private static getBankDetailsResponse PositiveBankServiceResponse()
        {
            return new getBankDetailsResponse
            {
                sortCodeAccountNumberValid = true,
                corporateAccountValid = false,
                sortCodeValid = true,
                bankName = "Big Bank",
                bankFormattedAddress = new bankFormattedAddressType
                {
                    bankAddressLine1 = "20",
                    bankAddressLine2 = "London Street",
                    bankAddressLine3 = "Reading",
                    bankPostcode = "RG1 3BS"
                }
            };
        }
    }
}