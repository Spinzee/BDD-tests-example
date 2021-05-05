namespace Products.Service.Energy
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Model.Energy;
    using Products.Model.Common;

    public interface IConfirmationEmailService
    {
        Task SendConfirmationEmail(ConfirmationEmailParameters emailParameters, string emailTemplateName, HashSet<Extra> selectedExtras);
    }
}