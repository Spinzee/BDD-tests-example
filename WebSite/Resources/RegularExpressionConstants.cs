namespace WebSite.Resources
{
    public static class RegularExpressionConstants
    {
        public const string Email = @"^([a-zA-Z0-9_\'\-\+\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,24}|[0-9]{1,3})(\]?)$";

        public const string Password = @"^[a-zA-Z0-9#~@]{7,14}$";
    }
}
