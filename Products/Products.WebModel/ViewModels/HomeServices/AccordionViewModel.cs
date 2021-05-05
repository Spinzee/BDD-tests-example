using System.Collections.Generic;

namespace Products.WebModel.ViewModels.HomeServices
{
    public class AccordionViewModel
    {
        public List<ProductPDFViewModel> ExtraProductPDFs { get; set; } = new List<ProductPDFViewModel>();

        public bool IsLandLord { get; set; }

        public List<ProductPDFViewModel> ProductPDFs { get; set; } = new List<ProductPDFViewModel>();
    }
}
