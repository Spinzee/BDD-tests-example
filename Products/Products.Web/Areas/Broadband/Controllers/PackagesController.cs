using Products.Infrastructure;
using Products.Service.Broadband;
using Products.WebModel.ViewModels.Broadband;
using System;
using System.Linq;
using System.Web.Mvc;
using Products.Web.Attributes;

namespace Products.Web.Areas.Broadband.Controllers
{
    public class PackagesController : BaseController
    {
        private readonly IPackageService _packageService;

        public PackagesController(IPackageService packageService)
        {
            Guard.Against<ArgumentNullException>(packageService == null, "packageService is null");
            _packageService = packageService;
        }

        [HttpGet]
        [Route("available-packages")]
        public ActionResult AvailablePackages()
        {
            SetSpeedCapViewbag();
            var model = _packageService.GetAvailablePackagesViewModel();
            if (model == null)
            {
                return RedirectToAction("CannotCompleteOnline", "LineChecker");
            }

            return View("~/Areas/Broadband/Views/CustomerPackages/AvailablePackages.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("available-packages")]
        public ActionResult AvailablePackage(string talkCode, string selectedTalkProductCode)
        {
            SetSpeedCapViewbag();

            bool? result = _packageService.GetBroadbandProductsForAvailablePackages(selectedTalkProductCode, talkCode);

            if (result == null)
            {
                return RedirectToAction("CannotCompleteOnline", "LineChecker");
            }

            if (result.Value)
            {
                return RedirectToAction("AvailablePackages");                
            }

            return RedirectToAction("SelectedPackage");
        }


        [HttpGet]
        [Route("selected-package")]
        public ActionResult SelectedPackage()
        {
            SetSpeedCapViewbag();

            return View("~/Areas/Broadband/Views/CustomerPackages/SelectedPackage.cshtml", _packageService.GetSelectedPackageViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("selected-package")]
        public ActionResult SelectedPackage(SelectedPackageViewModel model, string TalkCode)
        {
            SetSpeedCapViewbag();

            SelectedPackageViewModel viewModel = _packageService.SetSelectedPackageViewModel(model, TalkCode);

            if (viewModel == null)
            {
                return RedirectToAction("CannotCompleteOnline", "LineChecker");
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("~/Areas/Broadband/Views/shared/_YourPriceDynamic.cshtml", viewModel.YourPriceViewModel);
            }

            return RedirectToAction("TransferYourNumber", "CustomerDetails");
        }

        [HttpGet]
        [Route("available-packages-add-calls")]
        [AjaxOnly]
        public ActionResult GetAddCallsAjax()
        {
            var model = _packageService.GetAvailableTalkPackageViewModel();
            return PartialView("~/Areas/Broadband/Views/CustomerPackages/_AddCalls.cshtml", model);
        }

        private void SetSpeedCapViewbag()
        {
            ViewBag.SpeedCap = _packageService.GetSpeedCap();
        }
    }
}