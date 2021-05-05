namespace Products.Tests.Infrastructure.Extensions
{
    using System;
    using NUnit.Framework;
    using Products.Infrastructure.Extensions;
    using Should;

    [TestFixture]
    public class DateTimeExtensionsTests
    {
        [TestCase("1 jan 2000", "1st January 2000")]
        [TestCase("2 feb 2001", "2nd February 2001")]
        [TestCase("3 mar 2002", "3rd March 2002")]
        [TestCase("4 apr 2003", "4th April 2003")]
        [TestCase("5 may 2004", "5th May 2004")]
        [TestCase("10 jun 2005", "10th June 2005")]
        [TestCase("21 jul 2006", "21st July 2006")]
        [TestCase("22 aug 2007", "22nd August 2007")]
        [TestCase("23 sep 2008", "23rd September 2008")]
        [TestCase("24 oct 2009", "24th October 2009")]
        [TestCase("30 nov 2010", "30th November 2010")]
        [TestCase("31 dec 2011", "31st December 2011")]
        public void ToSseStringReturnsCorrectValues(string date, string expectedString)
        {
            // Arrange/Act
            DateTime dateValue = Convert.ToDateTime(date);
            string result = dateValue.ToSseString();

            // Assert
            result.ShouldEqual(expectedString);
        }

        [TestCase("", "")]
        [TestCase("1 jan 2000", "Expired")]
        [TestCase("1 jan 2100", "Expires on 1st January 2100")]
        [TestCase("today", "Expires today on ")]
        [TestCase("today+1", "Expires tomorrow on ")]
        [TestCase("today+5", "Expires in 5 days on ")]
        public void ToSseExpiryDateStringReturnsCorrectValues(string date, string expectedStartString)
        {
            // Arrange/Act
            DateTime dateValue = DateTime.MinValue;
            if (date.Length > 0)
            {
                if (date.Contains("today"))
                {
                    // ReSharper disable once SwitchStatementMissingSomeCases
                    switch (date)
                    {
                        case "today":
                            dateValue = DateTime.Today;
                            break;
                        case "today+1":
                            dateValue = DateTime.Today.AddDays(1);
                            break;
                        case "today+5":
                            dateValue = DateTime.Today.AddDays(5);
                            break;
                    }
                }
                else
                {
                    dateValue = Convert.ToDateTime(date);
                }
            }

            string result = dateValue.ToSseExpiryDateString();

            // Assert
            result.ShouldStartWith(expectedStartString);
        }
    }
}
