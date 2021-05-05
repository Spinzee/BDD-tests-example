namespace Products.ControllerHelpers
{
    using System.Collections.Generic;
    using Model.Energy;

    public interface IContentManagementControllerHelper
    {
        List<CMSEnergyContent> GetCMSEnergyContentList();
    }
}
