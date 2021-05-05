namespace Products.Web.Areas.Broadband.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Infrastructure;
    using Service.Broadband;
    using WebModel.Resources.Broadband;
    using WebModel.ViewModels.Broadband;

    public class LineCheckerController : BaseController
    {
        private readonly ILineCheckerService _lineCheckerService;

        public LineCheckerController(ILineCheckerService lineCheckerService)
        {
            Guard.Against<ArgumentNullException>(lineCheckerService == null, "lineCheckerService is null");
            _lineCheckerService = lineCheckerService;
        }


        [Route("line-check")]
        public async Task<ActionResult> LineChecker(string productCode = "")
        {
            bool hasAlert = await _lineCheckerService.IsCustomerAlert();
            if (hasAlert)
            {
                return RedirectToAction("BroadbandUnavailable");
            }

            return View(_lineCheckerService.GetLineCheckerViewModel(productCode));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Submit(LineCheckerViewModel lineCheckerViewModel)
        {
            bool hasAlert = await _lineCheckerService.IsCustomerAlert();
            if (hasAlert)
            {
                return RedirectToAction("BroadbandUnavailable");
            }

            if (ModelState.IsValid)
            {
                _lineCheckerService.SetInformationPassedByHub(lineCheckerViewModel.ProductCode);

                if (_lineCheckerService.IsNorthernIrelandPostcode(lineCheckerViewModel.PostCode))
                {
                    return RedirectToAction("AreaNotCovered", "LineChecker");
                }

                _lineCheckerService.SetLineCheckerDetails(lineCheckerViewModel);

                return RedirectToAction("SelectAddress", "LineChecker");
            }

            lineCheckerViewModel.BackChevronViewModel = _lineCheckerService.GetLineCheckerViewModel(lineCheckerViewModel.ProductCode).BackChevronViewModel;
            return View("~/Areas/Broadband/Views/LineChecker/LineChecker.cshtml", lineCheckerViewModel);
        }

        [HttpGet]
        [Route("select-address")]
        public async Task<ActionResult> SelectAddress()
        {
            List<AddressViewModel> addresses = await _lineCheckerService.GetAddresses();
            if (!addresses.Any())
            {
                return RedirectToAction("CannotCompleteOnline", "LineChecker");
            }

            TempData["AddressList"] = addresses;

            return View("~/Areas/Broadband/Views/LineChecker/SelectAddress.cshtml", _lineCheckerService.GetSelectAddressViewModel(addresses));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("select-address")]
        public async Task<JsonResult> SelectAddress(SelectAddressViewModel viewModel)
        {
            var addresses = (List<AddressViewModel>) TempData["AddressList"];
            TempData.Keep("AddressList");

            if (ModelState.IsValid)
            {
                bool openReachFallout = _lineCheckerService.IsOpenReachFallout(viewModel.SelectedAddressId, addresses);

                if (openReachFallout)
                {
                    return Json(new {Status = "CannotCompleteOnline"});                    
                }

                bool? result = await _lineCheckerService.IsProductAvailable(viewModel, addresses);

                if (!result.HasValue)
                {
                    return Json(new { Status = "CannotCompleteOnline" });                    
                }

                if (result.Value)
                {
                    return Json(new { Status = "SelectedPackage" });                    
                }

                return Json(new { Status = "AvailablePackages" });                
            }

            return Json(new {Status = "SelectAddress"});
        }

        [Route("unable-to-complete")]
        public ActionResult CannotCompleteOnline()
        {
            CannotCompleteOnlineViewModel cannotCompleteOnlineViewModel = _lineCheckerService.GetCannotCompleteOnlineViewModel();
            return View("~/Areas/Broadband/Views/LineChecker/CannotCompleteOnline.cshtml", cannotCompleteOnlineViewModel);
        }

        [Route("address-not-listed")]
        public ActionResult AddressNotListed()
        {
            return View("~/Areas/Broadband/Views/LineChecker/AddressNotListed.cshtml");
        }

        [Route("area-not-covered")]
        public ActionResult AreaNotCovered()
        {
            ViewBag.Title = Fallout_Resources.AreaNotCoveredTitle;
            return View();
        }

        [HttpGet]
        [Route("unavailable")]
        public ActionResult BroadbandUnavailable()
        {
            return View("Unavailable");
        }
    }
}