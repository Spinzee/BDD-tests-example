using Products.Model.Broadband;
using Products.Model.Common;
using Products.Service.Common.Managers;
using Products.Service.Security;
using System;

namespace Products.Service.Broadband.Mappers
{
    public interface IApplicationDataMapper
    {
        ApplicationData GetApplicationData(Customer customer);
    }

    public class ApplicationDataMapper : IApplicationDataMapper
    {
        private readonly ICryptographyService _cryptographyService;
        private readonly ICampaignManager _campaignManager;

        public ApplicationDataMapper(ICryptographyService cryptographyService, ICampaignManager campaignManager)
        {
            _cryptographyService = cryptographyService;
            _campaignManager = campaignManager;
        }

        public ApplicationData GetApplicationData(Customer customer)
        {
            var applicationData = new ApplicationData
            {
                UserID = customer.UserId ?? Guid.Empty,
                ProductCode = customer.SelectedProductCode,
                Title = customer.PersonalDetails.Title,
                Firstname = customer.PersonalDetails.FirstName,
                Surname = customer.PersonalDetails.LastName,
                DateOfBirth = DateTime.Parse(customer.PersonalDetails.DateOfBirth),
                DayPhone = customer.CliNumber?.Trim() ?? "",

                Email = customer.ContactDetails.EmailAddress,
                Mobile = customer.ContactDetails.ContactNumber,
                MarketingConsent = customer.ContactDetails.MarketingConsent,

                AddressLine1 = customer.SelectedAddress.ThoroughfareName,
                Town = customer.SelectedAddress.PostTown,
                Postcode = customer.SelectedAddress.Postcode,


                AccountName = _cryptographyService.EncryptBroadbandValue(customer.DirectDebitDetails.AccountName),
                AccountNumber = _cryptographyService.EncryptBroadbandValue(customer.DirectDebitDetails.AccountNumber),
                SortCode = _cryptographyService.EncryptBroadbandValue(customer.DirectDebitDetails.SortCode),
                LineSpeed = 0
            };

            // if sub-premises/premise is not empty, use "sub-premises, premise, thoroughfare number" (if present) as HouseName
            // if thoroughfare number is present without premise/sub-premises, use it as HouseNumber
            string houseName = customer.SelectedAddress.GetFormattedHouseName();
            if (!string.IsNullOrEmpty(houseName))
                applicationData.HouseName = $"{houseName}, {customer.SelectedAddress.ThoroughfareNumber}";
            else
                applicationData.HouseNumber = customer.SelectedAddress.ThoroughfareNumber;


            // Getting campaignCodeMapping from config
            applicationData.CampaignCode = _campaignManager.GetCampaignCodesMapping(customer.MigrateAffiliateId, customer.MigrateCampaignId);
            return applicationData;
        }
    }
}
