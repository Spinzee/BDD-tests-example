using System.Collections.Generic;

namespace Products.WebModel.ViewModels.Common
{
    public class BaseViewModel
    {
        public BackChevronViewModel BackChevronViewModel { get; set; }

        public Dictionary<string, string> DataLayerDictionary { get; set; }
    }
}