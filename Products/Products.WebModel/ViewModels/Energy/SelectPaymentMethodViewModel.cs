namespace Products.WebModel.ViewModels.Energy
{
    using System.ComponentModel.DataAnnotations;
    using Common;
    using Model.Enums;
    using Resources.Common;

    public class SelectPaymentMethodViewModel : BaseViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "PaymentMethodRequiredError")]
        public RadioButtonList PaymentMethods { get; set; } = new RadioButtonList();

        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "PaymentMethodRequiredError")]
        public PaymentMethod? SelectedPaymentMethodId { get; set; }
    }
}