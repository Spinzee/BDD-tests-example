namespace Products.WebModel.ViewModels.Energy
{
    using Attributes;
    using Products.WebModel.Resources.Common;
    using Products.WebModel.Resources.Energy;
    using System.ComponentModel.DataAnnotations;

    public class PostcodeViewModel
    {
        [AriaDescription(ResourceType = typeof(Postcode_Resources), Name = "PostcodeAriaDescription")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "PostcodeRequiredError")]
        [RegularExpression(RegularExpressionConstants.Postcode, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "PostCodeRegexErrorMessage")]
        [Display(ResourceType = typeof(Form_Resources), Name = "PostcodeTitle")]
        [DataType(DataType.PostalCode)]
        [LocalisedDescription("PostCode")]
        public string PostCode { get; set; }

        [AriaDescription(ResourceType = typeof(Postcode_Resources), Name = "LandlineAriaDescription")]
        [RegularExpression(RegularExpressionConstants.Cli, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "LandlineRegexErrorMessage")]
        [Display(ResourceType = typeof(Form_Resources), Name = "LandlineTitle")]
        [DataType(DataType.PhoneNumber)]
        [LocalisedDescription("PhoneNumber")]
        public string Landline { get; set; }

        public string ProductCode { get; set; }

        public bool ShowLandline { get; set; }

        public string Header { get; set; }

        public string ParagraphText { get; set; }

        public string SubmitButtonText { get; set; }

        public string SubmitButtonTitle { get; set; }

        public bool OurPrices { get; set; }

        public bool IsBundle { get; set; }
    }
}