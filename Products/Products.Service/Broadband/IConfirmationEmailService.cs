using Products.Model.Broadband;

namespace Products.Service.Broadband
{
    public interface IConfirmationEmailService
    {
        void SendConfirmationEmail(string baseUrl, BroadbandConfirmationEmailParameters emailParameters);

    }
}