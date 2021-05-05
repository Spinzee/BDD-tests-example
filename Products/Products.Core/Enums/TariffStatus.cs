namespace Products.Core
{
    using System.ComponentModel.DataAnnotations;

    public enum TariffStatus
    {
        [Display(Name = "available for sale")]
        ForSale,
        [Display(Name = "not available for sale")]
        // ReSharper disable once UnusedMember.Global
        Preserved
    }
}
