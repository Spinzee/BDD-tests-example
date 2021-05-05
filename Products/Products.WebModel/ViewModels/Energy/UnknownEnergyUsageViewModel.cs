namespace Products.WebModel.ViewModels.Energy
{
    using System.ComponentModel.DataAnnotations;
    using Common;
    using Resources.Common;

    public class UnknownEnergyUsageViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = nameof(Form_Resources.PropertyTypeRequiredError))]
        public ButtonList PropertyType { get; set; } = new ButtonList();

        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = nameof(Form_Resources.PropertyTypeRequiredError))]
        public string SelectedPropertyTypeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = nameof(Form_Resources.NumberOfBedroomsRequiredError))]
        public ButtonList NumberOfBedrooms { get; set; } = new ButtonList();

        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = nameof(Form_Resources.NumberOfBedroomsRequiredError))]
        public string SelectedNumberOfBedroomsId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = nameof(Form_Resources.NumberOfAdultsRequiredError))]
        public ButtonList NumberOfAdults { get; set; } = new ButtonList();

        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = nameof(Form_Resources.NumberOfAdultsRequiredError))]
        public string SelectedNumberOfAdultsId { get; set; }
    }
}