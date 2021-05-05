namespace Products.Web.Areas.TariffChange.Controllers
{
    using System.Web.Mvc;
    using Infrastructure;
    using Model.Enums;
    using Service.Common;
    using Products.Service.TariffChange;
    using Products.WebModel.ViewModels.TariffChange;
    using System.Threading.Tasks;

    [RouteArea("TariffChange", AreaPrefix = "tariff-change")]
    public class SummaryController : Controller
    {
        private readonly ICustomerAccountService _customerAccountService;
        private readonly IProfileService _profileService;
        private readonly ICustomerService _customerService;
        private readonly IContextManager _contextManager;
        private readonly ITariffChangeService _tariffChangeService;
        private readonly IJourneyDetailsService _journeyDetailsService;

        public SummaryController(ICustomerAccountService customerAccountService, 
            ICustomerService customerService, 
            IContextManager contextManager, 
            ITariffChangeService tariffChangeService,
            IProfileService profileService,
            IJourneyDetailsService journeyDetailsService)
        {
            _customerAccountService = customerAccountService;
            _customerService = customerService;
            _contextManager = contextManager;
            _tariffChangeService = tariffChangeService;
            _profileService = profileService;
            _journeyDetailsService = journeyDetailsService;
        }

        [HttpGet]
        [Route("summary")]
        public ActionResult TariffSummary()
        {
            if (_customerService.CheckEmailAddressIsNotNull())
            {
                TariffSummaryViewModel tariffSummaryViewModel = _tariffChangeService.GetTariffSummaryViewModel();
                tariffSummaryViewModel.FuelTypeIconSvg = GetIconSvg(tariffSummaryViewModel.FuelType);
                return View("TariffSummary", tariffSummaryViewModel);
            }

            return RedirectToAction("IdentifyCustomer", "CustomerIdentification");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("summary")]
        public async Task<ActionResult> TariffSummary(TariffSummaryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TariffSummaryViewModel tariffSummaryViewModel = _tariffChangeService.GetTariffSummaryViewModel();
                tariffSummaryViewModel.FuelTypeIconSvg = GetIconSvg(tariffSummaryViewModel.FuelType);
                return View("TariffSummary", tariffSummaryViewModel);
            }            

            if (_customerService.CheckCustomerHasBeenSet())
            {
                var customer = _journeyDetailsService.GetCustomer();
                var customerExists = await _profileService.CheckProfileExists(customer.EmailAddress);
                if (customerExists.HasValue && !customerExists.Value && !string.IsNullOrEmpty(customer.Password))
                {
                    await _profileService.CreateOnlineProfile(customer.Password);
                }

                if (_customerAccountService.UpdateCustomerTariffSelection())
                {
                    return RedirectToAction("ShowConfirmation", "Confirmation");
                }
            }

            return RedirectToAction("IdentifyCustomer", "CustomerIdentification");
        }

        private string GetIconSvg(FuelType fuelType)
        {
            string iconPath = _contextManager.HttpContext.Server.MapPath($"~/Content/Svgs/icons/fuel/{fuelType}.svg");
            if (System.IO.File.Exists(iconPath))
            {
                return System.IO.File.ReadAllText(iconPath);
            }

            return string.Empty;
        }
    }
}