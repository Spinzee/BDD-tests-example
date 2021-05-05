namespace Products.WebModel.ViewModels.Energy
{
    using System.ComponentModel.DataAnnotations;
    using Common;
    using Core;
    using Resources.Common;

    public class SelectMeterTypeViewModel : BaseViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "ElectricityMeterRequiredError")]
        public RadioButtonList MeterTypes { get; set; } = new RadioButtonList();

        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "ElectricityMeterRequiredError")]
        public ElectricityMeterType? SelectedElectricityMeterType { get; set; }
    }
}