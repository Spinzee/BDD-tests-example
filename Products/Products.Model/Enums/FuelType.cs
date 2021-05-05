namespace Products.Model.Enums
{
    using System.ComponentModel;

    public enum FuelType
    {
        [Description("")]
        None,
        [Description("Electricity")]
        Electricity,
        [Description("Gas")]
        Gas,
        [Description("Dual")]
        Dual
    }
}