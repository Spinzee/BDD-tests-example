namespace Products.WebModel.ViewModels.Energy
{
    using Infrastructure.Extensions;

    public class SelectedExtraViewModel
    {
        public SelectedExtraViewModel(string productCode, string name, double monthlyDirectDebit)
        {
            ProductCode = productCode;
            MonthlyDirectDebit = monthlyDirectDebit.ToCurrency();
            Name = name;
        }

        public string ProductCode { get; }

        public string MonthlyDirectDebit { get; }

        public string Name { get; }
    }
}