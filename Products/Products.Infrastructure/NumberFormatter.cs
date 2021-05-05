namespace Products.Infrastructure
{
    using System.Linq;
    using Extensions;

    public static class NumberFormatter
    {
        public static string ToPence(double pence)
        {
            return $"{pence.ToNumber()}p";
        }

        public static string ToDigitsOnly(string numberText)
        {
            return new string(numberText.Where(char.IsDigit).ToArray());
        }

        public static string ToDayOfMonthOrdinal(int dayValue)
        {
            string suffix = string.Empty;

            if (dayValue > 0 && dayValue < 32)
            {
                switch (dayValue)
                {
                    case 1:
                    case 21:
                    case 31:
                        suffix = "st";
                        break;
                    case 2:
                    case 22:
                        suffix = "nd";
                        break;
                    case 3:
                    case 23:
                        suffix = "rd";
                        break;
                    default:
                        suffix = "th";
                        break;
                }
            }

            return $"{dayValue}{suffix}";
        }

        public static string[] GetSplitCostArray(double cost) => cost.ToString("C").Split('.');
    }
}