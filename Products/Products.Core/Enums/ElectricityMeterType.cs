namespace Products.Core
{
    using System.ComponentModel;

    public enum ElectricityMeterType
    {
        [Description("")]
        None,
        [Description("Standard")]
        Standard,
        [Description("Economy 7")]
        Economy7,
        Other
    }
}