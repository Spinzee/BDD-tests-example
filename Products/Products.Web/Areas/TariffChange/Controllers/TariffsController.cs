namespace Products.Web.Areas.TariffChange.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.UI;
    using ControllerHelpers;
    using Infrastructure;
    using Model.Energy;
    using Service.TariffChange;
    using WebModel.ViewModels.TariffChange;

    [RouteArea("TariffChange", AreaPrefix = "tariff-change")]
    [OutputCache(NoStore = true, Duration = 0, Location = OutputCacheLocation.None)]
    public class TariffsController : Controller
    {
        private readonly ITariffChangeService _tariffChangeService;
        private readonly IContentManagementControllerHelper _contentManagementControllerHelper;
        private readonly ITariffService _tariffService;

        public TariffsController(ITariffService tariffService, ITariffChangeService tariffChangeService, IContentManagementControllerHelper contentManagementControllerHelper)
        {
            Guard.Against<ArgumentNullException>(tariffService == null, "tariffService is null");
            Guard.Against<ArgumentNullException>(tariffChangeService == null, "tariffChangeService is null");
            Guard.Against<ArgumentNullException>(contentManagementControllerHelper == null, "contentManagementControllerHelper is null");
            _tariffService = tariffService;
            _tariffChangeService = tariffChangeService;
            _contentManagementControllerHelper = contentManagementControllerHelper;
        }

        [HttpGet]
        [Route("available-tariffs")]
        public async Task<ActionResult> AvailableTariffs()
        {
            List<CMSEnergyContent> cmsContent = _contentManagementControllerHelper.GetCMSEnergyContentList();
            TariffsViewModel tariffsViewModel = await _tariffService.GetCurrentAndAvailableTariffsAsync(cmsContent);
            if (tariffsViewModel.IsCustomerFallout)
            {
                return RedirectToAction("IdentifyCustomer", "CustomerIdentification");
            }

            if (tariffsViewModel.AvailableTariffs == null || tariffsViewModel.AvailableTariffs.Count == 0)
            {
                return RedirectToAction("CallUs", "Tariffs");
            }

            return View("AvailableTariffs", tariffsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("available-tariffs")]
        public ActionResult SelectTariff(string tariffName, bool isImmediateRenewal)
        {
            SelectedTariffsViewModel tariffsViewModel = _tariffService.GetCurrentSelectedTariff(tariffName, isImmediateRenewal);
            if (tariffsViewModel.IsCustomerFallout)
            {
                return RedirectToAction("IdentifyCustomer", "CustomerIdentification");
            }

            if (tariffsViewModel.IsTariffSelected)
            {
                if (tariffsViewModel.ShowEmailConfirmation)
                {
                    return RedirectToAction("GetCustomerEmail", "AdditionalDetails");
                }

                return RedirectToAction("TariffSummary", "Summary");
            }

            return RedirectToAction("AvailableTariffs");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectFollowOnTariff()
        {
            _tariffChangeService.SetFollowOnAsSelectedTariff();
            return RedirectToAction("ShowConfirmation", "Confirmation");
        }

        [HttpGet]
        [Route("call-us")]
        public ActionResult CallUs()
        {
            _tariffChangeService.ClearJourneyDetails();
            return View();
        }
    }
}