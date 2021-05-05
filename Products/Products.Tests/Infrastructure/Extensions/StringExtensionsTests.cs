namespace Products.Tests.Infrastructure.Extensions
{
    using System;
    using NUnit.Framework;
    using Products.Infrastructure.Extensions;
    using Should;

    [TestFixture]
    public class StringExtensionsTests
    {
        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase(" ", "")]
        [TestCase("\n\r\f\n\t\v", "")]
        [TestCase("ABC", "ABC")]
        [TestCase("abc", "ABC")]
        [TestCase("A b C", "ABC")]
        [TestCase("a B c", "ABC")]
        public void RemoveSpacesAndConvertToUpperReturnsCorrectValues(string input, string expectedResult)
        {
            // Arrange/Act
            string result = input.RemoveSpacesAndConvertToUpper();

            // Assert
            result.ShouldEqual(expectedResult);
        }

        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase("A", "A")]
        [TestCase("a", "A")]
        [TestCase("ab", "Ab")]
        [TestCase("abc", "Abc")]
        [TestCase("abC", "AbC")]
        public void ToCapitalLetterReturnsCorrectValues(string input, string expectedResult)
        {
            // Arrange/Act
            string result = input.ToCapitalLetter();

            // Assert
            result.ShouldEqual(expectedResult);
        }

        [TestCase("No Replace Tariff", "No Replace Tariff")]
        [TestCase("Replace Tariff Economy 7", "Replace Tariff")]
        [TestCase("Economy 7 Replace Tariff", "Replace Tariff")]
        [TestCase("Replace Tariff Economy 10", "Replace Tariff")]
        [TestCase("Economy 10 Replace Tariff", "Replace Tariff")]
        [TestCase("Replace Tariff Domestic Economy", "Replace Tariff")]
        [TestCase("Domestic Economy Replace Tariff", "Replace Tariff")]
        [TestCase("Replace Tariff Economy 7 Economy 10 Domestic Economy", "Replace Tariff")]
        [TestCase("Economy 7 Economy 10 Domestic Economy Replace Tariff", "Replace Tariff")]
        public void TrimEconomyWordingReturnsCorrectValues(string input, string expectedResult)
        {
            // Arrange/Act
            string result = input.TrimEconomyWording();

            // Assert
            result.ShouldEqual(expectedResult);
        }

        [TestCase("", "null")]
        [TestCase(null, "null")]
        [TestCase("1 dec 1985", "1 dec 1985")]
        [TestCase("29 feb 1985", "null")]
        [TestCase("1 fab 1985", "null")]
        [TestCase("32 jan 2005", "null")]
        public void TryParseDateTimeReturnsCorrectValues(string input, string expectedResult)
        {
            // Arrange
            DateTime? expectedDateTime = null;
            if (expectedResult != "null")
            {
                expectedDateTime = Convert.ToDateTime(expectedResult);
            }

            // Act
            DateTime? result = input.TryParseDateTime();
            
            // Assert
            result.ShouldEqual(expectedDateTime);
        }

        [TestCase("", "Unknown")]
        [TestCase(null, "Unknown")]
        [TestCase("blah", "Unknown")]
        [TestCase("0", "Unknown")]
        [TestCase("-1", "Unknown")]
        [TestCase("1", "1st")]
        [TestCase("2", "2nd")]
        [TestCase("3", "3rd")]
        [TestCase("4", "4th")]
        [TestCase("5", "5th")]
        [TestCase("6", "6th")]
        [TestCase("7", "7th")]
        [TestCase("8", "8th")]
        [TestCase("9", "9th")]
        [TestCase("10", "10th")]
        [TestCase("11", "11th")]
        [TestCase("12", "12th")]
        [TestCase("13", "13th")]
        [TestCase("14", "14th")]
        [TestCase("15", "15th")]
        [TestCase("16", "16th")]
        [TestCase("17", "17th")]
        [TestCase("18", "18th")]
        [TestCase("19", "19th")]
        [TestCase("20", "20th")]
        [TestCase("21", "21st")]
        [TestCase("22", "22nd")]
        [TestCase("23", "23rd")]
        [TestCase("24", "24th")]
        [TestCase("25", "25th")]
        [TestCase("26", "26th")]
        [TestCase("27", "27th")]
        [TestCase("28", "28th")]
        [TestCase("29", "Unknown")]
        [TestCase("30", "Unknown")]
        [TestCase("31", "Unknown")]
        [TestCase("32", "Unknown")]
        public void AppendPaymentDaySuffixReturnsCorrectValues(string input, string expectedResult)
        {
            // Arrange/Act
            string result = input.AppendPaymentDaySuffix();

            // Assert
            result.ShouldEqual(expectedResult);
        }

        [TestCase(" ", "_")]
        [TestCase("Fish & Chips", "Fish__amp__Chips")]
        [TestCase("(Fish & Chips", "_ob_Fish__amp__Chips")]
        [TestCase("(Fish & Chips)", "_ob_Fish__amp__Chips_cb_")]
        [TestCase("- (Fish & Chips)", "_dash___ob_Fish__amp__Chips_cb_")]
        [TestCase("/Fish & Chips", "_fs_Fish__amp__Chips")]
        [TestCase(@"/Fish & Chips\", "_fs_Fish__amp__Chips_bs_")]
        [TestCase("\"Fish & Chips", "_dq_Fish__amp__Chips")]
        [TestCase("\"Fish & Chips\"", "_dq_Fish__amp__Chips_dq_")]
        [TestCase("'Fish & Chips'", "_sq_Fish__amp__Chips_sq_")]
        [TestCase("'Fish & Chips'\r", "_sq_Fish__amp__Chips_sq__")]
        [TestCase("'Fish & Chips'\n", "_sq_Fish__amp__Chips_sq__")]
        [TestCase("'Fish & Chips'\t", "_sq_Fish__amp__Chips_sq__")]
        public void GetHTMLSafeNameReturnsCorrectValues(string input, string expectedResult)
        {
            // Arrange/Act
            string result = input.GetHTMLSafeName();

            // Assert
            result.ShouldEqual(expectedResult);
        }
    }
}
