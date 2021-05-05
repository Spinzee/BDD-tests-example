namespace Products.Service.Common
{
    using System;
    using System.ServiceModel;
    using Infrastructure;
    using Products.Model.Common;
    using ServiceWrapper.BankDetailsService;

    public class BankValidationService : IBankValidationService
    {
        private readonly IBankDetailsServiceWrapper _bankDetailsServiceWrapper;

        public BankValidationService(IBankDetailsServiceWrapper bankDetailsServiceWrapper)
        {
            Guard.Against<ArgumentException>(bankDetailsServiceWrapper == null, $"{nameof(bankDetailsServiceWrapper)} is null");
            _bankDetailsServiceWrapper = bankDetailsServiceWrapper;
        }

        public BankDetails GetBankDetails(string suppliedSortCode, string suppliedBankAccountNumber)
        {
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(suppliedSortCode), $"{nameof(suppliedSortCode)} is null or empty");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(suppliedBankAccountNumber), $"{nameof(suppliedBankAccountNumber)} is null or empty");

            try
            {
                getBankDetailsResponse bankDetailsResponse = _bankDetailsServiceWrapper.GetBankDetails(suppliedSortCode, suppliedBankAccountNumber);

                if (bankDetailsResponse == null || !bankDetailsResponse.sortCodeValid ||
                    !bankDetailsResponse.sortCodeAccountNumberValid || bankDetailsResponse.corporateAccountValid
                    || string.IsNullOrEmpty(bankDetailsResponse.bankName))
                {
                    return null;
                }

                return new BankDetails
                {
                    CorporateAccountValid = bankDetailsResponse.corporateAccountValid,
                    SortCodeValid = bankDetailsResponse.sortCodeValid,
                    SortCodeAccountNumberValid = bankDetailsResponse.sortCodeAccountNumberValid,
                    BankName = bankDetailsResponse.bankName ?? string.Empty,
                    BankAddress = new BankFormattedAddressType
                    {
                        BankAddressLine1Field = bankDetailsResponse.bankFormattedAddress?.bankAddressLine1 ?? string.Empty,
                        BankAddressLine2Field = bankDetailsResponse.bankFormattedAddress?.bankAddressLine2 ?? string.Empty,
                        BankAddressLine3Field = bankDetailsResponse.bankFormattedAddress?.bankAddressLine3 ?? string.Empty,
                        BankAddressLine4Field = bankDetailsResponse.bankFormattedAddress?.bankAddressLine4 ?? string.Empty,
                        BankPostcodeField = bankDetailsResponse.bankFormattedAddress?.bankPostcode ?? string.Empty
                    }
                };
            }
            catch (FaultException<invalidRequestFaultType>)
            {
                // We may want to log this: $"Error - {e.Detail.description}";
            }
            catch (Exception ex)
            {
                throw new Exception("Exception occured while calling Bank Details Service", ex);
            }

            return null;
        }
    }
}
