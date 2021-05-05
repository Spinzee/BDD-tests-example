using System.ComponentModel;

namespace Products.Model.Enums
{
    public enum Site
    {
        [Description("M&S Energy")]
        // ReSharper disable once UnusedMember.Global
        MandSEnergy = 8,
        [Description("SSE")]
        SSE = 10,
        [Description("All")]
        All = 999
    }
}