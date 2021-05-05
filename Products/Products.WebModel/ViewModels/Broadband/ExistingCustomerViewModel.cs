using Products.WebModel.Attributes;
using System.ComponentModel.DataAnnotations;
using Products.WebModel.ViewModels.Common;

namespace Products.WebModel.ViewModels.Broadband
{
    public class ExistingCustomerViewModel : BaseViewModel
    {
        public ExistingCustomerViewModel()
        {
            IsExistingCustomer = YesNoNotSetOptions.NotSet;
        }

        [Display(ResourceType = typeof(Resources.Common.Resources), Name = "RadioYesNoName")]
        [LocalisedDescription("YesNoChoice")]
        public YesNoNotSetOptions IsExistingCustomer { get; set; }
    }
}