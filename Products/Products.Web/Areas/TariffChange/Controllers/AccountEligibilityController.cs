using Products.Infrastructure;
using Products.Model.TariffChange.Customers;
using Products.Service.TariffChange;
using Products.WebModel.ViewModels.TariffChange;
using System;
using System.Web.Mvc;
using System.Web.UI;

namespace Products.Web.Areas.TariffChange.Controllers
{
    [RouteArea("TariffChange", AreaPrefix = "tariff-change")]
    [OutputCache(NoStore = true, Duration = 0, Location = OutputCacheLocation.None)]
    public class AccountEligibilityController : Controller
    {
        private readonly ITariffChangeService _tariffChangeService;

        public AccountEligibilityController(ITariffChangeService tariffChangeService)
        {
            Guard.Against<ArgumentNullException>(tariffChangeService == null, "tariffChangeService is null");
            _tariffChangeService = tariffChangeService;
        }

        [HttpGet]
        [Route("account-eligibility")]
        public ActionResult CheckEligibility()
        {
            var customerEligibilityViewModel = _tariffChangeService.GetCustomerEligibilityViewModel();

            if (customerEligibilityViewModel.AccountNumber == null)
            {
                return RedirectToAction("IdentifyCustomer", "CustomerIdentification");
            }

            if (customerEligibilityViewModel.FalloutReasons == null)
            {
                return RedirectToAction("AvailableTariffs", "Tariffs");
            }

            TempData["DataLayer"] = customerEligibilityViewModel.FalloutReasons;

            TempData["IsPostLogin"] = customerEligibilityViewModel.IsPostLogin;

            switch (customerEligibilityViewModel.FalloutReasonResult.FalloutReason)
            {
                case FalloutReason.AtlanticIneligible:
                case FalloutReason.MandS:
                case FalloutReason.Ineligible:
                case FalloutReason.Indeterminable:
                case FalloutReason.ZeroAnnualCost:
                    return RedirectToAction("CustomerAccountIneligible");
                case FalloutReason.PaymentMethodIneligible:
                    return RedirectToAction("PaymentMethodIneligible");
                case FalloutReason.Acquisition:
                    return RedirectToAction("Acquisition");
                case FalloutReason.Renewals:
                    return RedirectToAction("FalloutRenewal");
                default:
                    return RedirectToAction("AvailableTariffs", "Tariffs");
            }
        }

        [HttpGet]
        [Route("account-ineligible")]
        public ActionResult CustomerAccountIneligible()
        {
            ViewBag.DataLayer = TempData["DataLayer"];
            ViewBag.IsPostLogin = User.Identity != null;

            _tariffChangeService.ClearJourneyDetails();
            return View("CustomerAccountIneligible");
        }

        [HttpGet]
        [Route("payment-method-ineligible")]
        public ActionResult PaymentMethodIneligible()
        {
            ViewBag.DataLayer = TempData["DataLayer"];

            var viewModel = new PaymentMethodIneligibleViewModel
            {
                IsCustomerLoggedIn = User.Identity != null
            };

            _tariffChangeService.ClearJourneyDetails();
            return View("PaymentMethodIneligible", viewModel);
        }

        [HttpGet]
        [Route("fallout-renewal")]
        public ActionResult FalloutRenewal()
        {
            ViewBag.DataLayer = TempData["DataLayer"];

            _tariffChangeService.ClearJourneyDetails();
            return View("FalloutRenewal");
        }

        [HttpGet]
        [Route("acquisition")]
        public ActionResult Acquisition()
        {
            var customerAcquisitionViewModel = _tariffChangeService.GetAcquisitionJourneyViewModel();
            return Redirect(string.Format(customerAcquisitionViewModel.Url, customerAcquisitionViewModel.PostCode));
        }
    }
}