using Products.WebModel.Attributes;
using Products.WebModel.Resources.Common;
using Products.WebModel.ViewModels.Common;
using System.ComponentModel.DataAnnotations;

namespace Products.WebModel.ViewModels.HomeServices
{
    public class PersonalDetailsViewModel : BaseViewModel
    {
        [AriaDescription(ResourceType = typeof(PersonalDetailsCommon_Resources), Name = "TitleAriaDescription")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "TitlesError")]
        [Display(ResourceType = typeof(Form_Resources), Name = "TitlesTitle")]
        [LocalisedDescription("Title")]
        [EnumDataType(typeof(Titles), ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "TitlesError")]
        public Titles? Titles { get; set; }

        [AriaDescription(ResourceType = typeof(PersonalDetailsCommon_Resources), Name = "FirstNameAriaDescription")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "FirstNameRequiredError")]
        [RegularExpression(RegularExpressionConstants.AcquisitionFirstName, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "FirstNameRegExError")]
        [Display(ResourceType = typeof(Form_Resources), Name = "FirstNameTitle")]
        [LocalisedDescription("FirstName")]
        [StringLength(20, MinimumLength = 1, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "FirstNameRegExError")]
        public string FirstName { get; set; }

        [AriaDescription(ResourceType = typeof(PersonalDetailsCommon_Resources), Name = "LastNameAriaDescription")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "LastNameRequiredError")]
        [Display(ResourceType = typeof(Form_Resources), Name = "LastNameTitle")]
        [LocalisedDescription("LastName")]
        [RegularExpression(RegularExpressionConstants.AcquisitionLastName, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "LastNameRegExError")]
        [StringLength(30, MinimumLength = 1, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "LastNameRegExError")]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "DateOfBirthRequiredError")]
        [RegularExpression(RegularExpressionConstants.DateOfBirth, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "DateOfBirthRegExError")]
        [Display(ResourceType = typeof(Form_Resources), Name = "DateOfBirthTitle")]
        [ValidDateOfBirth]
        public string DateOfBirth { get; set; }

        [AriaDescription(ResourceType = typeof(PersonalDetailsCommon_Resources), Name = "DateOfBirthDayAriaDescription")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "DateOfBirthRequiredError")]
        [LocalisedDescription("DateOfBirth")]
        [RegularExpression(RegularExpressionConstants.DateOfBirthDay, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "DateOfBirthRegExError")]
        [MaxLength(2, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "DateOfBirthRegExError")]
        public string DateOfBirthDay { get; set; }

        [AriaDescription(ResourceType = typeof(PersonalDetailsCommon_Resources), Name = "DateOfBirthMonthAriaDescription")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "DateOfBirthRequiredError")]
        [LocalisedDescription("DateOfBirth")]
        [RegularExpression(RegularExpressionConstants.DateOfBirthMonth, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "DateOfBirthRegExError")]
        [MaxLength(2, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "DateOfBirthRegExError")]
        public string DateOfBirthMonth { get; set; }

        [AriaDescription(ResourceType = typeof(PersonalDetailsCommon_Resources), Name = "DateOfBirthYearAriaDescription")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "DateOfBirthRequiredError")]
        [LocalisedDescription("DateOfBirth")]
        [RegularExpression(RegularExpressionConstants.DateOfBirthYear, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "DateOfBirthRegExError")]
        [MaxLength(4, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "DateOfBirthRegExError")]
        [ValidDateOfBirth]
        public string DateOfBirthYear { get; set; }

        public bool IsScottishPostcode { get; set; }
    }
}
