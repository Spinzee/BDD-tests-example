namespace Products.Infrastructure.Extensions
{
    using System;
    using System.Text.RegularExpressions;

    // ReSharper disable once UnusedMember.Global
    public static class StringExtensions
    {
        public static string ToCapitalLetter(this string input) => input?.Length > 0 ? input[0].ToString().ToUpper() + input.Substring(1) : string.Empty;

        public static string TrimEconomyWording(this string tariffName) =>
            tariffName
                .Replace("Economy 7", string.Empty)
                .Replace("Economy 10", string.Empty)
                .Replace("Domestic Economy", string.Empty)
                .Trim();

        public static string RemoveSpacesAndConvertToUpper(this string input)
        {
            return string.IsNullOrWhiteSpace(input) ? string.Empty : Regex.Replace(input.ToUpper(), @"\s+", "");
        }

        public static DateTime? TryParseDateTime(this string text)
        {
            return DateTime.TryParse(text, out DateTime validDate) ? validDate : (DateTime?)null;
        }

        public static string AppendPaymentDaySuffix(this string dayOfMonth)
        {
            int.TryParse(dayOfMonth, out int dayOfMonthValue);
            if (dayOfMonthValue < 1 || dayOfMonthValue > 28)
            {
                return "Unknown";
            }
            return NumberFormatter.ToDayOfMonthOrdinal(dayOfMonthValue);
        }

        public static string GetHTMLSafeName(this string name) =>
            name.Replace(" ", "_")
                .Replace("&", "_amp_")
                .Replace("(", "_ob_")
                .Replace(")", "_cb_")
                .Replace("-", "_dash_")
                .Replace("/", "_fs_")
                .Replace(@"\", "_bs_")
                .Replace("\"", "_dq_")
                .Replace("'", "_sq_")
                .Replace("\r", "_")
                .Replace("\n", "_")
                .Replace("\t", "_");
    }
}
