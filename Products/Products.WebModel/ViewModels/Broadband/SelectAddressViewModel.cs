using Products.WebModel.Attributes;
using Products.WebModel.Resources.Broadband;
using Products.WebModel.Resources.Common;
using Products.WebModel.ViewModels.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Products.WebModel.ViewModels.Broadband
{
    public class SelectAddressViewModel : BaseViewModel
    {
        [Display(ResourceType = typeof(Form_Resources), Name = "AddressDropdownTitle")]
        [LocalisedDescription("Select Address")]
        public List<AddressViewModel> Addresses { get; set; }

        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "AddressDropdownRequiredError")]
        [AriaDescription(ResourceType = typeof(SelectAddress_Resources), Name = "SelectAddressAriaDescription")]
        [Display(ResourceType = typeof(Form_Resources), Name = "AddressDropdownTitle")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "AddressDropdownTitle")]
        [LocalisedDescription("Select Address")]
        [UIHint("AriaDropDownList")]
        public int SelectedAddressId { get; set; }

        public ModalTitleAndBody LoadingModal { get; set; }
    }
}