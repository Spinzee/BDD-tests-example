namespace Products.Web.Areas.Energy.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Attributes;
    using ControllerHelpers.Energy;
    using Infrastructure;
    using WebModel.ViewModels.Energy;

    [EnergyCheckSession]
    public class TariffsController : BaseStepCounterController
    {
        private readonly ITariffsControllerHelper _tariffsControllerHelper;

        public TariffsController(ITariffsControllerHelper tariffsControllerHelper) : base(tariffsControllerHelper)
        {
            Guard.Against<ArgumentException>(tariffsControllerHelper == null,
                $"{nameof(tariffsControllerHelper)} is null");
            _tariffsControllerHelper = tariffsControllerHelper;
        }

        [HttpGet]
        [Route("quote-details")]
        public async Task<ActionResult> AvailableTariffs()
        {
            AvailableTariffsViewModel viewModel = await _tariffsControllerHelper.GetAvailableTariffsViewModel();

            if (viewModel != null)
            {
                return View("AvailableTariffs", viewModel);
            }

            return RedirectToAction("UnableToComplete", "SignUp");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("quote-details")]
        public async Task<ActionResult> AvailableTariffs(string selectedTariffId)
        {
            if (!_tariffsControllerHelper.SetSelectedTariff(selectedTariffId))
            {
                return Json(new { RedirectUrl = Url.Action("AvailableTariffs") });
            }

            /* Tariffs other than broadband selected must use RedirectToAction since the client side doesn't. */
            if (_tariffsControllerHelper.IsBroadbandBundleSelected(selectedTariffId))
            {
                return await ToFixNFibreBundleJourney();
            }

            return ToEnergyTariffFixNProtectBundleJourney();
        }

        /// <summary>
        /// Selected Tariffs other than broadband bundle must follow this route.
        /// </summary>
        /// <returns></returns>
        private ActionResult ToEnergyTariffFixNProtectBundleJourney()
        {
            if (_tariffsControllerHelper.SelectedTariffHasUpgrades())
            {
                _tariffsControllerHelper.SetAvailableBundleUpgrade();
                return RedirectToAction("Upgrades", "SignUp");
            }

            if (_tariffsControllerHelper.SelectedTariffHasExtras())
            {
                return RedirectToAction("Extras", "SignUp");
            }

            return RedirectToAction("PersonalDetails", "SignUp");
        }

        /// <summary>
        /// Selected tariff is a Broadband follow this route. Note: returns JsonResult since the redirection
        /// is done on the client side by javascript.
        /// </summary>
        private async Task<JsonResult> ToFixNFibreBundleJourney()
        {
            /* Fix n Fibre tariff selected. */
            if (!await _tariffsControllerHelper.HasMatchingBTAddressForCustomer())
            {
                return Json(new { RedirectUrl = Url.Action("ConfirmAddress", "SignUp") });
            }

            await _tariffsControllerHelper.SetAvailableBroadbandProduct();
            if (!_tariffsControllerHelper.IsBroadbandPackageAvailable())
            {
                _tariffsControllerHelper.MarkBundleAsUnavailable();
                return Json(new { RedirectUrl = Url.Action("AvailableTariffs") });
            }

            _tariffsControllerHelper.UpdateYourPriceViewModel();
            if (_tariffsControllerHelper.SelectedTariffHasUpgrades())
            {
                _tariffsControllerHelper.SetAvailableBundleUpgrade();
                return Json(new { RedirectUrl = Url.Action("Upgrades", "SignUp") });
            }
            
            return Json(new { RedirectUrl = Url.Action("PhonePackage", "SignUp") });
        }

        public async Task<ActionResult> CheckBroadbandPackage()
        {
            await _tariffsControllerHelper.SetAvailableBroadbandProduct();
            if (!_tariffsControllerHelper.IsBroadbandPackageAvailable())
            {
                _tariffsControllerHelper.MarkBundleAsUnavailable();
                return RedirectToAction("AvailableTariffs");
            }

            if (_tariffsControllerHelper.SelectedTariffHasUpgrades())
            {
                _tariffsControllerHelper.SetAvailableBundleUpgrade();
                return RedirectToAction("Upgrades", "SignUp");
            }

            return RedirectToAction("PhonePackage", "SignUp");
        }

        [HttpGet]
        [Route("energy-usage")]
        public ActionResult EnergyUsage()
        {
            return View(_tariffsControllerHelper.GetEnergyUsageViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("energy-usage")]
        public async Task<ActionResult> EnergyUsage(EnergyUsageViewModel model)
        {
            EnergyUsageViewModel viewModel;

            if (model.ActiveTabIndex == 1)
            {
                model.UnknownEnergyUsageViewModel = model.UnknownEnergyUsageViewModel ?? new UnknownEnergyUsageViewModel();
                if (!TryValidateModel(model.UnknownEnergyUsageViewModel))
                {
                    viewModel = _tariffsControllerHelper.GetEnergyUsageViewModel(model.UnknownEnergyUsageViewModel);
                    viewModel.ActiveTabIndex = 1;
                    return View(viewModel);
                }

                if (!await _tariffsControllerHelper.SetUnknownUsage(model.UnknownEnergyUsageViewModel))
                {
                    return RedirectToAction("UnableToComplete", "SignUp");
                }
            }
            else
            {
                if (!TryValidateModel(model.KnownEnergyUsageViewModel))
                {
                    viewModel = _tariffsControllerHelper.GetEnergyUsageViewModel();
                    viewModel.KnownEnergyUsageViewModel = model.KnownEnergyUsageViewModel;
                    return View(viewModel);
                }

                _tariffsControllerHelper.SetKnownUsage(model.KnownEnergyUsageViewModel);
            }

            return RedirectToAction("AvailableTariffs", "Tariffs");
        }
    }
}