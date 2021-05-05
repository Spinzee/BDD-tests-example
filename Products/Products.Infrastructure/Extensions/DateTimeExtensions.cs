namespace Products.Infrastructure.Extensions
{
    using System;
    using System.Text;

    public static class DateTimeExtensions
    {
        public static string ToSseExpiryDateString(this DateTime dateTimeValue)
        {
            if (dateTimeValue == DateTime.MinValue)
            {
                return string.Empty;
            }

            int days = (dateTimeValue - DateTime.Today).Days;
            if (days < 0)
            {
                return "Expired";
            }

            var expiresOnString = new StringBuilder("Expires ");

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (days == 0)
            {
                expiresOnString.Append("today ");
            }
            else if (days == 1)
            {
                expiresOnString.Append("tomorrow ");
            }
            else if (days <= 60)
            {
                expiresOnString.Append($"in {days} days ");
            }

            expiresOnString.Append($"on {dateTimeValue.ToSseString()}");
            return expiresOnString.ToString();
        }

        public static string ToSseString(this DateTime dateTimeValue)
        {
            return string.Format("{1} {0:MMMM} {0:yyyy}", dateTimeValue, NumberFormatter.ToDayOfMonthOrdinal(dateTimeValue.Day));
        }
    }
}
