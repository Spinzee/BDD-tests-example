namespace Products.Core
{
    using System.ComponentModel;

    public enum LineStatus
    {
        [Description("Start of stopped Line")]
        // ReSharper disable once IdentifierTypo
        StartofstoppedLine,

        [Description("MPF Conversion")]
        MPFConversion,

        [Description("New Connection")]
        NewConnection,

        [Description("Other")]
        Other
    }
}