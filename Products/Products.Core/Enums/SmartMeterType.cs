namespace Products.Core
{
    using System.ComponentModel;

    public enum SmartMeterType
    {
        None,
        [Description("SMETS1")]
        Smets1,
        [Description("SMETS2")]
        Smets2
    }
}