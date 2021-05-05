using System.ComponentModel;

namespace Products.Model.Enums
{
    public enum TariffType
    {
        None,
        [Description("Evergreen")]
        Evergreen,
        [Description("Fixed")]
        Fixed
    }

    public static class TariffTypeExtensions
    {
        public static TariffType GetTariffType(string tariffType)
        {
            switch (tariffType)
            {
                case "Evergreen":
                    return TariffType.Evergreen;
                case "Fixed":
                    return TariffType.Fixed;
                default:
                    return TariffType.None;
            }
        }
    }
}