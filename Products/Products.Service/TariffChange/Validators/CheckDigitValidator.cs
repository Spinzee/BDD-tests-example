using System;
using System.Globalization;

namespace Products.Service.TariffChange.Validators
{
    public interface ICheckDigitValidator
    {
        bool IsValid(string value);
    }

    public class CheckDigitValidator : ICheckDigitValidator
    {
        private const int AccountNumberLength = 10;

        private const int AccountNumberLengthWithoutCheckDigit = 9;

        public bool IsValid(string value)
        {
            if (value != null && value.Length != AccountNumberLength)
            {
                return false;
            }

            if (value != null)
            {
                string accountNumberWithoutCheckDigit = value.Substring(0, AccountNumberLengthWithoutCheckDigit);
                char valueCheckDigit = value[AccountNumberLengthWithoutCheckDigit];
                char calculatedCheckDigit = GetCheckDigit(accountNumberWithoutCheckDigit);

                if (valueCheckDigit != calculatedCheckDigit)
                {
                    return false;
                }
            }

            return true;
        }

        private static char GetCheckDigit(string value)
        {
            if (value != null && value.Length != AccountNumberLengthWithoutCheckDigit)
            {
                throw new ArgumentException(
                                            string.Format(
                                            CultureInfo.InvariantCulture,
                                            "The account number value is an incorrect length.  Expected:{0}. Actual:{1}",
                                            AccountNumberLengthWithoutCheckDigit,
                                            value.Length));
            }

            bool isDigits = value != null && Array.TrueForAll(value.ToCharArray(), char.IsDigit);

            if (!isDigits)
            {
                throw new ArgumentException(
                                            string.Format(
                                            CultureInfo.InvariantCulture,
                                            "The account number value is not numeric. Value:{0}",
                                            value));
            }

            return CheckDigit(value);
        }

        private static char CheckDigit(string value)
        {
            int a = int.Parse(value[0].ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
            int b = int.Parse(value[1].ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture) * 7;
            int c = int.Parse(value[2].ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture) * 3;
            int d = int.Parse(value[3].ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
            int e = int.Parse(value[4].ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture) * 7;
            int f = int.Parse(value[5].ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture) * 3;
            int g = int.Parse(value[6].ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
            int h = int.Parse(value[7].ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture) * 7;
            int i = int.Parse(value[8].ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture) * 3;

            int checkDigit = (a + b + c + d + e + f + g + h + i) % 10;

            return Convert.ToChar(checkDigit.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
        }
    }
}
