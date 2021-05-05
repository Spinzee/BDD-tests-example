namespace Products.Infrastructure
{
    public static class StringHelper
    {
        public static string GetFormattedPostcode(string postcode)
        {
            string formattedPostCode = string.IsNullOrEmpty(postcode) ? string.Empty : postcode.Replace(" ", "");
            return formattedPostCode.Length > 3 ? formattedPostCode.Insert(formattedPostCode.Length - 3, " ") : postcode;
        }

        public static string GetFormattedSortCode(string sixDigitSortCode)
        {
            string retVal = sixDigitSortCode;
            if (sixDigitSortCode?.Length == 6)
            {
                string segmentOne = sixDigitSortCode.Substring(0, 2);
                string segmentTwo = sixDigitSortCode.Substring(2, 2);
                string segmentThree = sixDigitSortCode.Substring(4, 2);
                retVal = GetFormattedSortCode(segmentOne, segmentTwo, segmentThree);
            }
            
            return retVal;
        }

        private static string GetFormattedSortCode(string segmentOne, string segmentTwo, string segmentThree)
        {
            return $"{segmentOne}-{segmentTwo}-{segmentThree}";
        }
    }
}
