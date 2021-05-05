namespace Products.Model.TariffChange.Customers
{
    public enum FalloutReason
    {
        None = 0,
        Indeterminable = 1,
        Ineligible = 2,
        PaymentMethodIneligible = 3,
        AtlanticIneligible = 4,
        Acquisition = 5,
        Renewals = 6,
        MandS = 7,
        ZeroAnnualCost = 8
    }
}