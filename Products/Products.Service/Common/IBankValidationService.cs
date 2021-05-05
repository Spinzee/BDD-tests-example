namespace Products.Service.Common
{
    using Products.Model.Common;

    public interface IBankValidationService
    {
        BankDetails GetBankDetails(string suppliedSortCode, string suppliedBankAccountNumber);
    }
}