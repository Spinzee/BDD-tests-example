using Products.WebModel.ViewModels.Broadband;
using System.Collections.Generic;

namespace Products.Service.Broadband
{
    public interface IConfirmationService
    {
        ConfirmationViewModel ConfirmationViewModel(Dictionary<string, string> dataLayerDictionary);
    }
}