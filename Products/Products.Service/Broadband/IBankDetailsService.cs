using Products.WebModel.ViewModels.Broadband;

namespace Products.Service.Broadband
{
    public interface IBankDetailsService
    {
        BankDetailsViewModel GetBankDetailsViewModel();

        BankDetailsViewModel SetBankDetailsViewModel(BankDetailsViewModel model);
    }
}