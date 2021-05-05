namespace Products.WebModel.Resources.Common
{
    /// <summary>
    /// Regular Expression Constants
    /// </summary>
    public static class RegularExpressionConstants
    {
        /// <summary>
        /// The email regex
        /// </summary>
        public const string Email = @"^([a-zA-Z0-9_\'\-\+\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,24}|[0-9]{1,3})(\]?)$";

        /// <summary>
        /// The name regex
        /// </summary>
        public const string Name = @"[0-9a-zA-Z _'-]*$";

        /// <summary>
        /// The last name regex
        /// </summary>
        public const string LastName = @"[a-zA-Z _'-]*$";

        /// <summary>
        /// The address line 1 regex
        /// </summary>
        public const string AddressLine1 = @"^[A-Za-z0-9-',. ]{1,50}$";

        /// <summary>
        /// The address line 2 regex
        /// </summary>
        public const string AddressLine2 = @"^[A-Za-z0-9-',. ]{0,50}$";

        /// <summary>
        /// The house number regex
        /// </summary>
        public const string PropertyNumber = @"^[A-Za-z0-9-',. ]{1,30}$";

        /// <summary>
        /// The address line regex
        /// </summary>
        public const string AlternateAddress = @"^[A-Za-z0-9-:;@?!£%&'"",() *\[\]]{0,64}$";

        /// <summary>
        /// The Town regex
        /// </summary>
        public const string Town = @"^[A-Za-z-',. ]{1,30}$";

        /// <summary>
        /// The County regex
        /// </summary>
        public const string County = @"^[A-Za-z-',. ]{1,30}$";

        /// <summary>
        /// The Country line regex
        /// </summary>
        public const string Country = @"^[A-Za-z0-9-:;@?!£%&'"",() *\[\]]{0,64}$";

        /// <summary>
        /// The PostCode line regex
        /// </summary>
        public const string PostCodeFreeText = @"^[A-Za-z0-9-:;@?!£%&'"",() *\[\]]{1,8}$";

        /// <summary>
        /// The company name regex
        /// </summary>
        public const string CompanyName = @"^[a-zA-Z0-9 ,'.&\(\)-]+$";

        /// <summary>
        /// The telephone number regex
        /// </summary>
        public const string TelephoneNumber =
            @"^((\(?0\?+\d{3,4}\)?\s?\d{3}\s?\d{3})|(\(?0\d{3}\)?\s?\d{3}\s?\d{4})|(\(?0\d{2}\)?\s?\d{4}\s?\d{4})|(\(?0\d{2}\)?\s?\d{3}\s?\d{4})|(\(?0\d{4}\)?\s?\d{5}))?$";

        /// <summary>
        /// The Contact number regex
        /// </summary>
        public const string ContactNumber =
            @"^(\(?\+?[0-9]*\)?)?[0-9_\- \(\)]*$";

        /// <summary>
        /// The Contact number regex
        /// </summary>
        public const string BroadbandContactNumber =
            @"^[-+()_\/\s]*([0-9][-+()_\/\s]*){11,15}$";

        /// <summary>
        /// Free text but a restricted character set
        /// </summary>
        public const string FreeText = @"^[A-Za-z0-9-:;@/?£%&\s\$\^\*\(\).'""#,~! _+=\{\}\[\]]*$";

        /// <summary>
        /// Numeric value with Scale of 4 and Precision 0 greater than zero, e.g. 9999
        /// </summary>
        public const string NumericS4P0 = @"^[0-9]{1,4}$";

        /// <summary>
        /// Numeric value with Scale of 3 and Precision 0 greater than 99, e.g. 999
        /// </summary>
        public const string NumericS3P0 = @"^[0-9]{3,3}$";

        /// <summary>
        /// Numeric value with Scale of 2 and Precision 0 greater than one, e.g. 10
        /// </summary>
        public const string NumericS2P0 = @"^[0-9]{1,2}$";

        /// <summary>
        /// The issue number
        /// </summary>
        public const string IssueNumber = @"^[0-9]*[1-9][0-9]*$";

        /// <summary>
        /// The password regex
        /// </summary>
        public const string Password = @"^[a-zA-Z0-9#~@]{7,14}$";

        /// <summary>
        /// The login password this needs to cater for CSR generated passwords
        /// </summary>
        public const string LoginPassword = @"^[a-zA-Z0-9#~@]{7,32}$";

        /// <summary>
        /// The add product postcode
        /// </summary>
        public const string AddProductPostcode = @"^[a-zA-Z0-9 ]{2,8}$";

        /// <summary>
        /// The postcode regex
        /// </summary>
        public const string Postcode = @"\s*(?=.{5,10}$)([A-PR-UWYZa-pr-uwyz]([0-9]{1,2}|([A-HK-Ya-hk-y][0-9]|[A-HK-Ya-hk-y][0-9]([0-9]|[ABEHMNPRV-Yabehmnprvwx-y]))|[0-9][A-HJKPS-UWa-hjkps-uw]) {0,1}[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2})\s*$";

        /// <summary>
        /// Friendly name with free text but a restricted character set
        /// </summary>
        public const string FriendlyName = @"^[A-Za-z0-9-:;@/?£%&\s\$\^\*\(\).'""#,~! _+=\{\}\[\]]{0,16}$";

        /// <summary>
        /// The customer account number regex
        /// </summary>
        public const string CustomerAccountNumber = @"^\d{10}$";

        /// <summary>
        /// The meter reading
        /// </summary>
        public const string MeterReading = @"^\d{1,6}$";

        /// <summary>
        /// Represents either a whole number or a number with 2 decimal points
        /// </summary>
        public const string Currency = @"^\d+(\.?\d{2})?";

        /// <summary>
        /// The card number to make payment
        /// </summary>
        public const string CardNumber = @"^[0-9- ]{1,19}$";

        /// <summary>
        /// The account number to set up direct debit
        /// </summary>
        public const string AccountNumber = @"^\d{8}$";

        // the sort code to set up direct debit
        //public const string SortCode = @"^(?!(?:0{6}|00-00-00))(?:\d{6}|\d\d-\d\d-\d\d)$";
        public const string SortCode = @"^\d{6}$";

        public const string AccountHolderName = @"[a-zA-Z '-]{1,18}$";

        public const string AcquisitionFirstName = @"[a-zA-Z '-]*$";

        public const string AcquisitionLastName = @"[a-zA-Z '-]*$";

        public const string DateOfBirth =
            @"^(?:(?:31(\/)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/)(?:0?[1,3-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{4})$";

        public const string Cli = @"^\s*(0\s*[12])((?:\s*\d){8,9})\s*$";

        public const string DateOfBirthDay = @"^(0?[1-9]|[12]\d|3[01])$";

        public const string DateOfBirthMonth = @"^(0?[1-9]|1[012])$";

        public const string DateOfBirthYear = @"(19|[2-9][0-9])\d{2}$";

        public const string MembershipId = @"^[a-zA-Z0-9]{1,20}$";

        public const string ScottishPostcodes = @"^(ZE|KW|IV|HS|PH|AB|DD|PA|FK|G|KY|KA|DG|TD|EH|ML)";
    }
}