using Products.WebModel.Attributes;
using Products.WebModel.Resources.Energy;
using Products.WebModel.ViewModels.Common;

namespace Products.WebModel.ViewModels.Energy
{
    public class SmartMeterFrequencyViewModel : BaseViewModel
    {
        public RadioButtonList SmartMeterReadingFrequency { get; set; } = new RadioButtonList();

        public bool SmartMeterFrequencyEnabled { get; set; }

        [RequiredIf("SmartMeterFrequencyEnabled", true, ErrorMessageResourceType = typeof(SmartMeter_Resources), ErrorMessageResourceName = "SmartMeterFrequencyRequiredError")]
        public string SmartMeterFrequencyId { get; set; }
    }
}
