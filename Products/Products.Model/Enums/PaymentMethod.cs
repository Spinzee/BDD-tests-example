namespace Products.Model.Enums
{
    using System.ComponentModel;

    public enum PaymentMethod
    {
        [Description("")]
        None,
        [Description("DD")]
        MonthlyDirectDebit = 1,
        [Description("QC")]
        Quarterly = 4,
        [Description("PP")]
        PayAsYouGo = 5
    }
}
