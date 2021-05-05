namespace Products.Infrastructure.Extensions
{
    using System;

    // ReSharper disable once UnusedMember.Global
    public static class DoubleExtensions
    {
        public static string ToNumber(this double asDouble)
        {
            string asString = $"{asDouble}";
            int decimalIndex = asString.IndexOf(".", StringComparison.InvariantCulture);
            if (decimalIndex < 1)
            {
                if (asString.Length > 4)
                {
                    string doubleResultString = asDouble.ToString("N");
                    int pointPos = doubleResultString.IndexOf('.');
                    return doubleResultString.Substring(0, pointPos);
                }

                return asString;
            }

            if (decimalIndex != 4)
            {
                return asDouble.ToString("N");
            }

            return $"{asString.Substring(0, 1)}{asDouble.ToString("N").Substring(2, 6)}";
        }

        public static string ToPounds(this double pounds)
        {
            return $"£{ToNumber(pounds)}";
        }

        public static string RoundUpToNearestPoundWithPoundSign(this double number)
        {
            return $"£{RoundUpToNearestPound(number)}";
        }

        public static double RoundUpToNearestPound(this double number)
        {
            return number.ToString("C").Contains(".00") ? Math.Floor(number) : Math.Ceiling(number);
        }

        public static string FromPenceToPound(this double number)
        {
            return ToPounds(number / 100);
        }

        public static decimal ToDecimalPoints(this double? number)
        {
            if (number.HasValue)
            {
                double num = Math.Round(number.Value, 2);
                return Convert.ToDecimal(num);
            }

            return 0;
        }

        public static string ToTwoDecimalPlacesString(this double? number) => (number ?? 0).ToString("0.00");

        public static string ToCurrency(this double myNumber) => $"{myNumber:C}".Replace(".00", "");

        public static string AmountSplitInPounds(this double amount) => NumberFormatter.GetSplitCostArray(amount)[0];

        public static string AmountSplitInPence(this double amount)
        {
            string[] costArray = NumberFormatter.GetSplitCostArray(amount);
            return costArray.Length > 1 ? $".{costArray[1]}" : string.Empty;
        }
    }
}
