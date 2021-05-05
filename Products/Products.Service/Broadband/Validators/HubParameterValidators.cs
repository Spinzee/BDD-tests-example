namespace Products.Service.Broadband.Validators
{
    using System.Text.RegularExpressions;
    using Products.WebModel.Resources.Common;

    public static class HubParametersValidator
    {
        public static bool ValidateMembershipId(string membershipid)
        {
            Match match = Regex.Match(membershipid, RegularExpressionConstants.MembershipId, RegexOptions.IgnoreCase);
            return match.Success;
        }
    }
}
