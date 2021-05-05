using Products.Model.Common;
using System;

namespace Products.Model.Broadband
{
    [Serializable]
    public class Customer
    {                        
        public Guid? UserId { get; set; }
        public bool IsSSECustomer { get; set; }
        public string CliNumber { get; set; }
        public string PostcodeEntered { get; set; }
        public BTAddress SelectedAddress { get; set; }
        public string SelectedProductCode { get; set; }
        public string MigrateAffiliateId { get; set; }
        public string MigrateCampaignId { get; set; }
        public string MembershipId { get; set; }
        public bool ApplyInstallationFee { get; set; }
        public BroadbandProduct SelectedProduct { get; set; }
        public PersonalDetails PersonalDetails { get; set; }
        public ContactDetails ContactDetails { get; set; }
        public DirectDebitDetails DirectDebitDetails { get; set; }
        public int BankServiceRetryCount { get; set; }
        public BroadbandProductGroup SelectedProductGroup { get; set; } = BroadbandProductGroup.None;
        public bool KeepExistingNumber { get; set; } = true;
        public string OriginalCliEntered { get; set; }
        public bool TransferYourNumberIsSet { get; set; }
        public int? ApplicationId { get; set; }
        public string SelectedTalkCode { get; set; }
        public bool IsUserFromHubPage { get; set; }
        public bool IsSSECustomerCLI { get; set; }
    }
}