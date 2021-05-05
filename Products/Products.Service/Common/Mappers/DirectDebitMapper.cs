using Products.Model.Common;
using Products.Model.Enums;
using Products.WebModel.Resources.Common;
using Products.WebModel.ViewModels.Common;

namespace Products.Service.Common.Mappers
{
    public class DirectDebitMapper
    {
        public static DirectDebitMandateViewModel GetMandateViewModel(DirectDebitDetails directDebitDetails, ProductType productType)
        {
            var mandate = new DirectDebitMandateViewModel
            {
                Name = directDebitDetails.AccountName,
                Sortcode = directDebitDetails.SortCode,
                AccountNumber = directDebitDetails.AccountNumber,
                BankName = directDebitDetails.BankName,
                AddressLine1 = directDebitDetails.BankAddressLine1,
                AddressLine2 = directDebitDetails.BankAddressLine2,
                AddressLine3 = directDebitDetails.BankAddressLine3,
                Postcode = directDebitDetails.Postcode
            };

            switch (productType)
            {
                case ProductType.Electric:
                case ProductType.Gas:
                case ProductType.Phone:
                    mandate.ServiceUserNumber = DirectDebitMandate_Resources.ServiceUserNumber;
                    mandate.CompanyName = DirectDebitMandate_Resources.CompanyName;
                    break;
                case ProductType.HomeServices:
                    mandate.ServiceUserNumber = DirectDebitMandate_Resources.ServiceUserNumberHomeServices;
                    mandate.CompanyName = DirectDebitMandate_Resources.CompanyNameHomeServices;
                    break;
                default:
                    break;
            }
            return mandate;
        }
    }
}
