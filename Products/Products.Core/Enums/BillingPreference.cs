namespace Products.Core
{
    using System.ComponentModel;

    public enum BillingPreference
    {
        [Description("")]
        None,
        [Description("Paperless")]
        Paperless,
        [Description("Paper")]
        Paper
    }
}
