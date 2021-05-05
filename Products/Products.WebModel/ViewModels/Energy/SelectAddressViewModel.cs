using Products.WebModel.Attributes;
using Products.WebModel.Resources.Common;
using Products.WebModel.Resources.Energy;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Products.WebModel.ViewModels.Energy
{
    public class SelectAddressViewModel : BaseViewModel
    {
        public List<KeyValuePair<string, string>> Addresses { get; set; }

        [RequiredIf("IsManual", false, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "AddressDropdownRequiredError")]
        [AriaDescription(ResourceType = typeof(SelectAddress_Resources), Name = "SelectAddressAriaDescription")]
        [Display(ResourceType = typeof(Form_Resources), Name = "AddressDropdownTitle")]
        [UIHint("AriaDropDownList")]
        public string SelectedAddressId { get; set; }

        public string Postcode { get; set; }


        // Manual Address
        [AriaDescription(ResourceType = typeof(SelectAddress_Resources), Name = "PropertyNumberDescription")]
        [RequiredIf("IsManual", true, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "PropertyNumberRequiredError")]
        [RegularExpression(RegularExpressionConstants.PropertyNumber, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "PropertyNumberRegExError")]
        [Display(ResourceType = typeof(Form_Resources), Name = "PropertyNumberTitle")]
        [LocalisedDescription("PropertyNumber")]
        public string PropertyNumber { get; set; }

        [AriaDescription(ResourceType = typeof(SelectAddress_Resources), Name = "AddressLine1Description")]
        [RequiredIf("IsManual", true, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "AddressLine1RequiredError")]
        [RegularExpression(RegularExpressionConstants.AddressLine1, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "AddressLine1RegExError")]
        [Display(ResourceType = typeof(Form_Resources), Name = "AddressLine1Title")]
        [LocalisedDescription("AddressLine1")]
        public string AddressLine1 { get; set; }

        [AriaDescription(ResourceType = typeof(SelectAddress_Resources), Name = "AddressLine2Description")]
        [RegularExpression(RegularExpressionConstants.AddressLine2, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "AddressLine2RegExError")]
        [Display(ResourceType = typeof(Form_Resources), Name = "AddressLine2Title")]
        [LocalisedDescription("PropertyNumber")]
        public string AddressLine2 { get; set; }

        [AriaDescription(ResourceType = typeof(SelectAddress_Resources), Name = "TownDescription")]
        [RequiredIf("IsManual", true, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "TownRequiredError")]
        [RegularExpression(RegularExpressionConstants.Town, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "TownRegExError")]
        [Display(ResourceType = typeof(Form_Resources), Name = "TownTitle")]
        [LocalisedDescription("Town")]
        public string Town { get; set; }

        [AriaDescription(ResourceType = typeof(SelectAddress_Resources), Name = "CountyDescription")]
        [RegularExpression(RegularExpressionConstants.County, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "CountyRegExError")]
        [Display(ResourceType = typeof(Form_Resources), Name = "CountyTitle")]
        [LocalisedDescription("County")]
        public string County { get; set; }

        public bool IsManual { get; set; }
        public string HeaderText { get; set; }
        public string ParaText { get; set; }
        public string SubHeaderText { get; set; }
        public bool HasValidAddress { get; set; }
        public bool QASEnabled { get; set; }
    }
}
