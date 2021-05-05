
namespace Products.Service.Common
{
    using System.Text.RegularExpressions;
    using Products.WebModel.Resources.Common;

    public class PostcodeCheckerService : IPostcodeCheckerService
    {

        public bool IsNorthernIrelandPostcode(string postcode)
        {
            return postcode.ToUpper().StartsWith("BT");
        }

        public bool IsScottishPostcode(string postcode)
        {
            return Regex.IsMatch(postcode.ToUpper(),
                RegularExpressionConstants.ScottishPostcodes);
        }
    }
}
