namespace Products.WebModel.ViewModels.Broadband
{
    using System.Collections.Generic;

    public class AvailablePackagesViewModel
    {
        public string PostCode { get; set; }

        public List<AvailableProductViewModel> Products { get; set; }

        public string BroadbandChargesParagraph { get; set; }

        public string CancellationChargesParagraph2 { get; set; }
    }
}