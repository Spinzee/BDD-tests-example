using Products.Model.Enums;
using Products.WebModel.Attributes;
using Products.WebModel.Resources.Common;
using Products.WebModel.Resources.HomeServices;
using Products.WebModel.ViewModels.Common;
using System.ComponentModel.DataAnnotations;

namespace Products.WebModel.ViewModels.HomeServices
{
    public class PostcodeViewModel : BaseViewModel
    {
        [AriaDescription(ResourceType = typeof(EnterPostcode_Resources), Name = "PostcodeAriaDescription")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "PostcodeRequiredError")]
        [RegularExpression(RegularExpressionConstants.Postcode, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "PostCodeRegexErrorMessage")]
        [Display(ResourceType = typeof(Form_Resources), Name = "PostcodeTitle")]
        [DataType(DataType.PostalCode)]
        [LocalisedDescription("PostCode")]
        public string Postcode { get; set; }

        public string ProductCode { get; set; }

        public HomeServicesCustomerType CustomerType { get; set; }

        public string HeaderText { get; set; }

        public string ParagraphText { get; set; }
        public AddressTypes AddressTypes { get; set; }

    }
}

