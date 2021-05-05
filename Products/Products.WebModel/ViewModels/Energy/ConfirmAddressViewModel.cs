namespace Products.WebModel.ViewModels.Energy
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Attributes;
    using Resources.Common;
    using Resources.Energy;

    public class ConfirmAddressViewModel : BaseViewModel
    {
        [Display(ResourceType = typeof(Form_Resources), Name = "AddressDropdownTitle")]
        [LocalisedDescription("Select Address")]
        public List<BTAddressViewModel> Addresses { get; set; }

        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "AddressDropdownRequiredError")]
        [AriaDescription(ResourceType = typeof(SelectBTAddress_Resources), Name = "SelectAddressAriaDescription")]
        [Display(ResourceType = typeof(Form_Resources), Name = "AddressDropdownTitle")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "AddressDropdownTitle")]
        [LocalisedDescription("Select Address")]
        [UIHint("AriaDropDownList")]
        public int SelectedAddressId { get; set; }

        public string NotListedAction => "UnableToComplete";

        public string NotListedController => "SignUp";
    }
}
