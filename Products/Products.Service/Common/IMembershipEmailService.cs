namespace Products.Service.Common
{
    public interface IMembershipEmailService
    {
        void SendMembershipEmail(string emailAddress, string campaignId, string name, string address, string email, string phoneNumber, string membershipId);
    }
}