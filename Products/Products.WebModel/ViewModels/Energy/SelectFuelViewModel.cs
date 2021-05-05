namespace Products.WebModel.ViewModels.Energy
{
    using System.ComponentModel.DataAnnotations;
    using Core;
    using Model.Enums;


    public class SelectFuelViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Fuel type is required")]
        [EnumDataType(typeof(FuelType), ErrorMessage = "Invalid fuel type")]

        public FuelType? FuelType { get; set; }

        public CAndCRedirectRoute CAndCRedirectRoute { get; set; }
    }
}
