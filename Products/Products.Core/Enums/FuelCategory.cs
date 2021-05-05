namespace Products.Core
{
    using System.ComponentModel.DataAnnotations;

    public enum FuelCategory
    {
        [Display(Name = "standard electricity tariffs")]
        Standard,
        [Display(Name = "multi-rate electricity tariffs")]
        MultiRate,
        [Display(Name = "gas tariffs")]
        Gas
    }
}
