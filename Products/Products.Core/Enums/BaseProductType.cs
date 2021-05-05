namespace Products.Core
{
    using System.ComponentModel;

    public enum BaseProductType
    {
        StandardElectricOnly = 1,
        GasOnly = 2,
        // ReSharper disable once UnusedMember.Global
        Telephone = 3,
        DualFuel = 4,
        [Description("All Electric supply (with heating element) and no mains gas")]
        Economy7 = 5,
        Broadband = 6
    }
}
