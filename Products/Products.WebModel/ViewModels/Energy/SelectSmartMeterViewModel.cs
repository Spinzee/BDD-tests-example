using Products.WebModel.Attributes;
using Products.WebModel.Resources.Common;
using Products.WebModel.ViewModels.Common;
using System.ComponentModel.DataAnnotations;

namespace Products.WebModel.ViewModels.Energy
{
    public class SelectSmartMeterViewModel : BaseViewModel
    {
        [ProgressiveCheckbox]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SmartMeterRequiredError")]
        public RadioButtonList SmartMeter { get; set; } = new RadioButtonList();


        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SmartMeterRequiredError")]
        public bool? HasSmartMeter { get; set; }
    }
}
