namespace Products.Core.Enums
{
    using System.ComponentModel;

    public enum BundlePackageType
    {
        [Description("Fix & Protect")]
        FixAndProtect,
        [Description("Fix & Fibre")]
        FixAndFibre,
        [Description("None")] None
    }
}