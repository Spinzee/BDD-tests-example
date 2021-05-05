using Products.Infrastructure;
using Products.Service.TariffChange;
using System;
using System.Web.Mvc;
using System.Web.UI;

namespace Products.Web.Areas.TariffChange.Controllers
{
    [RouteArea("TariffChange", AreaPrefix = "tariff-change")]
    [OutputCache(NoStore = true, Duration = 0, Location = OutputCacheLocation.None)]
    public class ConfirmationController : Controller
    {
        private readonly ITariffChangeService _tariffChangeService;

        public ConfirmationController(ITariffChangeService tariffChangeService)
        {
            Guard.Against<ArgumentNullException>(tariffChangeService == null, "tariffChangeService is null");
            _tariffChangeService = tariffChangeService;
        }

        [HttpGet]
        [Route("confirmation")]
        public ActionResult ShowConfirmation()
        {
            var confirmationViewModel = _tariffChangeService.GetConfirmationViewModel();

            if (confirmationViewModel == null)
            {
                return RedirectToAction("IdentifyCustomer", "CustomerIdentification");
            }
            return View("ShowConfirmation", confirmationViewModel);
        }
    }
}