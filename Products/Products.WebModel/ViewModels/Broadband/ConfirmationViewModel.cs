using Products.WebModel.ViewModels.Common;

namespace Products.WebModel.ViewModels.Broadband
{
    public class ConfirmationViewModel : BaseViewModel
    {
        public string ProductName { get; set; }

        public bool IsSSECustomer { get; set; }
    }
}