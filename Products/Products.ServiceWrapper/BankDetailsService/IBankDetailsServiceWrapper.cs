namespace Products.ServiceWrapper.BankDetailsService
{
    public interface IBankDetailsServiceWrapper
    {
        getBankDetailsResponse GetBankDetails(string sortCode, string accountNumber);
    }
}