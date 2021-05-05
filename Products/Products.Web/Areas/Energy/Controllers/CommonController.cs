namespace Products.Web.Areas.Energy.Controllers
{
    using Attributes;
    using System.Web.Mvc;
    using ControllerHelpers.Energy;
    using WebModel.ViewModels.Energy;

    [EnergyCheckSession]
    public class CommonController : BaseController
    {
        private readonly BaseEnergyControllerHelper _baseControllerHelper;

        public CommonController(BaseEnergyControllerHelper baseControllerHelper)
        {
            _baseControllerHelper = baseControllerHelper;
        }

        public PartialViewResult YourPriceDetails()
        {
            return PartialView("_YourPriceDynamic", _baseControllerHelper.GetYourPriceViewModel());
        }

        [ChildActionOnly]
        public ActionResult GetDataLayer()
        {
            DataLayerViewModel dataLayerModel = _baseControllerHelper.GetDataLayerViewModel();
            return PartialView("_DataLayer", dataLayerModel);
        }
    }
}
