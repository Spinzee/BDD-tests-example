using System.ComponentModel;

namespace Products.Model.TariffChange.Enums
{
    public enum ProgressBarStatus
    {
        [Description("active")]
        Active,
        [Description("done")]
        Done,
        [Description("awaiting")]
        Awaiting
    }
}
