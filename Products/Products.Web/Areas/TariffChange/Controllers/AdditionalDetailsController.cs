using Products.Infrastructure;
using Products.Service.TariffChange;
using Products.WebModel.ViewModels.TariffChange;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;

namespace Products.Web.Areas.TariffChange.Controllers
{
    [RouteArea("TariffChange", AreaPrefix = "tariff-change")]
    [OutputCache(NoStore = true, Duration = 0, Location = OutputCacheLocation.None)]
    public class AdditionalDetailsController : Controller
    {
        private readonly ITariffChangeService _tariffChangeService;
        private readonly IProfileService _profileService;

        public AdditionalDetailsController(IProfileService profileService, ITariffChangeService tariffChangeService)
        {
            Guard.Against<ArgumentNullException>(profileService == null, "profileService is null");
            Guard.Against<ArgumentNullException>(tariffChangeService == null, "tariffChangeService is null");

            _profileService = profileService;
            _tariffChangeService = tariffChangeService;
        }

        [HttpGet]
        [Route("customer-email")]
        public ActionResult GetCustomerEmail()
        {

            var customerEmailViewModel = _tariffChangeService.GetCustomerEmailViewModel();

            if (customerEmailViewModel == null)
            {
                return RedirectToAction("IdentifyCustomer", "CustomerIdentification");
            }

            return View("GetCustomerEmail", customerEmailViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("customer-email")]
        public async Task<ActionResult> CheckProfileExists(GetCustomerEmailViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                bool? profileExists = await _profileService.CheckProfileExists(viewModel.EmailAddress);

                if (profileExists == null)
                {
                    return RedirectToAction("IdentifyCustomer", "CustomerIdentification");
                }

                if ((bool)profileExists)
                {
                    return RedirectToAction("TariffSummary", "Summary");
                }

                return RedirectToAction("CreatePassword", "CreatePassword");
            }

            return View("GetCustomerEmail", _tariffChangeService.GetCustomerEmailViewModel());
        }
    }
}