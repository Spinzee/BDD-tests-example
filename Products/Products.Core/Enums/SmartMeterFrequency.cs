namespace Products.Core
{
    using System.ComponentModel;

    public enum SmartMeterFrequency
    {
        [Description("Unknown")]
        None,
        [Description("Half hourly")]
        HalfHourly,
        [Description("Daily")]
        Daily,
        [Description("Monthly")]
        Monthly

    }
}
