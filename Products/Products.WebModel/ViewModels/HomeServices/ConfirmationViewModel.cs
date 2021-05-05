using System.Collections.Generic;

namespace Products.WebModel.ViewModels.HomeServices
{
    public class ConfirmationViewModel
    {
        public string ProductName { get; set; }
        public List<string> ProductExtras { get; set; } = new List<string>();
        public bool IsLandLord { get; set; }
        public Dictionary<string, string> DataLayer { get; set; }
    }
}