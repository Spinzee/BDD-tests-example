namespace Products.Web.Areas.HomeServices.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.UI;
    using Attributes;
    using ControllerHelpers.HomeServices;
    using Infrastructure;
    using Model.Enums;
    using WebModel.ViewModels.HomeServices;

    [RouteArea("HomeServices", AreaPrefix = "home-services-signup")]
    [OutputCache(NoStore = true, Duration = 0, Location = OutputCacheLocation.None)]
    [HomeServicesRouteCheck]
    public class HomeServicesController : Controller
    {
        private readonly IHomeServicesControllerHelper _homeServicesControllerHelper;

        public HomeServicesController(IHomeServicesControllerHelper homeServicesControllerHelper)
        {
            Guard.Against<ArgumentException>(homeServicesControllerHelper == null, $"{nameof(homeServicesControllerHelper)} is null");
            _homeServicesControllerHelper = homeServicesControllerHelper;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.ActionName;
            ViewBag.StepCounter = _homeServicesControllerHelper.GetStepCounter(actionName);
            base.OnActionExecuted(filterContext);
        }

        [HttpGet]
        [Route("enter-postcode")]
        public async Task<ActionResult> Postcode(string productCode)
        {
            bool alertStatus = await _homeServicesControllerHelper.IsCustomerAlert();
            if (alertStatus)
            {
                return RedirectToAction("HomeServicesUnavailable");
            }

            PostcodeViewModel model = _homeServicesControllerHelper.GetEnterPostcodeViewModel(HomeServicesCustomerType.Residential, AddressTypes.Cover);
            return View("Postcode", model);
        }

        [HttpGet]
        [Route("enter-cover-postcode")]
        public async Task<ActionResult> LandlordPostcode(string productCode)
        {
            bool alertStatus = await _homeServicesControllerHelper.IsCustomerAlert();
            if (alertStatus)
            {
                return RedirectToAction("HomeServicesUnavailable");
            }

            PostcodeViewModel model = _homeServicesControllerHelper.GetEnterPostcodeViewModel(HomeServicesCustomerType.Landlord, AddressTypes.Cover);
            return View("Postcode", model);
        }

        [HttpGet]
        [Route("enter-billing-postcode")]
        public ActionResult LandlordBillingPostcode()
        {
            PostcodeViewModel model = _homeServicesControllerHelper.GetEnterPostcodeViewModel(HomeServicesCustomerType.Landlord, AddressTypes.Billing);
            return View("Postcode", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("enter-postcode")]
        public async Task<ActionResult> Postcode(PostcodeViewModel viewModel)
        {
            bool alertStatus = await _homeServicesControllerHelper.IsCustomerAlert();
            if (alertStatus)
            {
                return RedirectToAction("HomeServicesUnavailable");
            }

            if (ModelState.IsValid)
            {
                if (_homeServicesControllerHelper.IsNorthernIrelandPostcode(viewModel.Postcode))
                {
                    return RedirectToAction("AreaNotCovered");
                }

                if (viewModel.AddressTypes == AddressTypes.Cover)
                {
                    bool? response = await _homeServicesControllerHelper.IsProductAvailable(viewModel);

                    if (response.HasValue)
                    {
                        if (response.Value)
                        {
                            _homeServicesControllerHelper.SavePostcode(viewModel);
                            _homeServicesControllerHelper.SaveProductCode(viewModel.ProductCode);
                            return RedirectToAction("CoverDetails");
                        }
                        else
                        {
                            return RedirectToAction("ExcludedPostcode");
                        }
                    }
                }
                else if (viewModel.AddressTypes == AddressTypes.Billing)
                {
                    _homeServicesControllerHelper.SavePostcode(viewModel);
                    return RedirectToAction("SelectBillingAddress");
                }

                return RedirectToAction("UnableToComplete");
            }

            PostcodeViewModel model = _homeServicesControllerHelper.GetEnterPostcodeViewModel(viewModel.CustomerType, viewModel.AddressTypes);
            viewModel.HeaderText = model.HeaderText;
            viewModel.ParagraphText = model.ParagraphText;

            return View("Postcode", viewModel);
        }

        [HttpGet]
        [Route("cover-details")]
        public ActionResult CoverDetails()
        {
            return View("CoverDetails", _homeServicesControllerHelper.GetCoverDetailsViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("cover-details")]
        public ActionResult CoverDetails(CoverDetailsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("PersonalDetails");
            }

            return View("CoverDetails", _homeServicesControllerHelper.GetCoverDetailsViewModel());
        }

        [HttpGet]
        [Route("personal-details")]
        public ActionResult PersonalDetails()
        {
            return View("PersonalDetails", _homeServicesControllerHelper.GetPersonalDetailsViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("personal-details")]
        public ActionResult PersonalDetails(PersonalDetailsViewModel viewModel)
        {
            PersonalDetailsViewModel personalDetailsViewModel = _homeServicesControllerHelper.GetPersonalDetailsViewModel();
            viewModel.IsScottishPostcode = personalDetailsViewModel.IsScottishPostcode;
            TryValidateModel(viewModel);

            if (ModelState.IsValid)
            {
                _homeServicesControllerHelper.SavePersonalDetailsViewModel(viewModel);
                return RedirectToAction("SelectAddress");
            }

            viewModel.BackChevronViewModel = _homeServicesControllerHelper.GetPersonalDetailsViewModel().BackChevronViewModel;
            return View("PersonalDetails", viewModel);
        }

        [HttpGet]
        [Route("select-address")]
        public async Task<ActionResult> SelectAddress()
        {
            SelectAddressViewModel selectAddressViewModel = await _homeServicesControllerHelper.GetSelectAddressViewModel(AddressTypes.Cover);
            if (!selectAddressViewModel.Addresses.Any() && selectAddressViewModel.QASEnabled)
            {
                return RedirectToAction("UnableToComplete");
            }

            return View(selectAddressViewModel);
        }

        [HttpGet]
        [Route("select-billing-address")]
        public async Task<ActionResult> SelectBillingAddress()
        {
            SelectAddressViewModel selectAddressViewModel = await _homeServicesControllerHelper.GetSelectAddressViewModel(AddressTypes.Billing);
            if (!selectAddressViewModel.Addresses.Any() && selectAddressViewModel.QASEnabled)
            {
                return RedirectToAction("UnableToComplete");
            }
            return View("SelectAddress", selectAddressViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("select-address")]
        public async Task<ActionResult> SelectAddress(SelectAddressViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.IsManual)
                {
                    _homeServicesControllerHelper.SetManualAddress(viewModel);
                }
                else
                {
                    bool isSet = await _homeServicesControllerHelper.SetSelectedAddressByMoniker(viewModel);

                    if (!isSet)
                    {
                        return RedirectToAction("UnableToComplete");
                    }
                }

                if (_homeServicesControllerHelper.IsLandlord() && viewModel.AddressType == AddressTypes.Cover)
                {
                    return RedirectToAction("LandlordBillingPostcode");
                }

                return RedirectToAction("ContactDetails");
            }

            SelectAddressViewModel selectAddressViewModel = await _homeServicesControllerHelper.GetSelectAddressViewModel(viewModel.AddressType);
            viewModel.BackChevronViewModel = selectAddressViewModel.BackChevronViewModel;
            viewModel.Addresses = selectAddressViewModel.Addresses;
            viewModel.Postcode = selectAddressViewModel.Postcode;
            viewModel.HeaderText = selectAddressViewModel.HeaderText;
            viewModel.ParaText = selectAddressViewModel.ParaText;
            viewModel.SubHeaderText = selectAddressViewModel.SubHeaderText;
            return View(viewModel);
        }

        [HttpGet]
        [Route("contact-details")]
        public ActionResult ContactDetails()
        {
            ContactDetailsViewModel contactDetailsViewModel = _homeServicesControllerHelper.GetContactDetailsViewModel();
            return View(contactDetailsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("contact-details")]
        public ActionResult ContactDetails(ContactDetailsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _homeServicesControllerHelper.SetContactDetails(viewModel);
                return RedirectToAction("BankDetails");
            }

            viewModel.BackChevronViewModel = _homeServicesControllerHelper.GetContactDetailsViewModel().BackChevronViewModel;
            return View(viewModel);
        }

        [HttpGet]
        [Route("bank-details")]
        public ActionResult BankDetails()
        {
            return View(_homeServicesControllerHelper.GetBankDetailsViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("bank-details")]
        public ActionResult BankDetails(BankDetailsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _homeServicesControllerHelper.ProcessBankDetails(viewModel);
                if (viewModel.BankDetailsIsValid)
                {
                    return RedirectToAction("Summary");
                }

                if (viewModel.IsRetryExceeded)
                {
                    return RedirectToAction("InvalidBankDetails");
                }
            }

            BankDetailsViewModel model = _homeServicesControllerHelper.GetBankDetailsViewModel();
            viewModel.BackChevronViewModel = model.BackChevronViewModel;
            viewModel.ProductData = model.ProductData;
            return View(viewModel);
        }

        [HttpGet]
        [Route("order-summary")]
        public ActionResult Summary()
        {
            return View("Summary", _homeServicesControllerHelper.GetSummaryViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("order-summary")]
        public async Task<ActionResult> Summary(SummaryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                await _homeServicesControllerHelper.ConfirmSale();
                return RedirectToAction("Confirmation");
            }

            SummaryViewModel model = _homeServicesControllerHelper.GetSummaryViewModel();
            viewModel.BackChevronViewModel = model.BackChevronViewModel;
            return View(viewModel);
        }

        [HttpGet]
        [Route("confirmation")]
        public ActionResult Confirmation()
        {
            return View(_homeServicesControllerHelper.ConfirmationViewModel());
        }

        [HttpGet]
        [Route("print-mandate")]
        public ActionResult PrintMandate()
        {
            return View("PrintMandate", _homeServicesControllerHelper.GetPrintMandateViewModel());
        }

        [ChildActionOnly]
        public ActionResult YourCoverBasket()
        {
            YourCoverBasketViewModel viewModel = _homeServicesControllerHelper.GetYourCoverBasket();
            return PartialView("_YourCoverBasket", viewModel);
        }

        [Route("your-cover-basket")]
        [AjaxOnly]
        public ActionResult YourCoverBasketAjax()
        {
            YourCoverBasketViewModel viewModelAjax = _homeServicesControllerHelper.GetYourCoverBasketAjax();
            if (viewModelAjax != null)
            {
                return PartialView("_YourCoverBasket", viewModelAjax);
            }

            return new HttpStatusCodeResult(500, "can't process request");
        }

        [HttpPost]
        [AjaxOnly]
        [ValidateAntiForgeryToken]
        [Route("update-selected-product")]
        public ActionResult UpdateSelectedProductExcess(string productCode)
        {
            bool status = _homeServicesControllerHelper.UpdateExcessProductCode(productCode);
            if (status)
            {
#pragma warning disable IDE0037 // Use inferred member name
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                // ReSharper disable once RedundantAnonymousTypePropertyName
                return Json(new { status = status });
#pragma warning restore IDE0037 // Use inferred member name
            }

            return new HttpStatusCodeResult(400, "product code not found");
        }

        [HttpGet]
        [AjaxOnly]
        [Route("your-cover-header")]
        public ActionResult YourCoverHeader()
        {
            CoverDetailsHeaderViewModel viewModel = _homeServicesControllerHelper.GetYourCoverHeader();
            if (viewModel != null)
            {
                return PartialView("_CoverDetailsHeader", viewModel);
            }

            return new HttpStatusCodeResult(500, "can't process request");
        }

        [HttpPost]
        [AjaxOnly]
        [ValidateAntiForgeryToken]
        [Route("update-selected-product-extra")]
        public ActionResult UpdateSelectedProductExtra(string productCode)
        {
            bool status = _homeServicesControllerHelper.UpdateExtraProductCode(productCode);
            if (status)
            {
#pragma warning disable IDE0037 // Use inferred member name
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                // ReSharper disable once RedundantAnonymousTypePropertyName
                return Json(new { status = status });
#pragma warning restore IDE0037 // Use inferred member name
            }

            return new HttpStatusCodeResult(400, "product code not found");
        }

        [ChildActionOnly]
        public ActionResult GetDataLayer()
        {
            Dictionary<string, string> dataLayerModel = _homeServicesControllerHelper.GetDataLayer();
            return PartialView("_DataLayer", dataLayerModel);
        }

        [HttpGet]
        [Route("invalid-bank-details")]
        public ActionResult InvalidBankDetails()
        {
            return View();
        }

        [Route("area-not-covered")]
        public ActionResult AreaNotCovered()
        {
            return View();
        }

        [Route("unable-to-complete")]
        public ActionResult UnableToComplete()
        {
            return View();
        }

        [HttpGet]
        [Route("postcode-not-covered")]
        public ActionResult ExcludedPostcode()
        {
            return View();
        }

        [HttpGet]
        [Route("unavailable")]
        public ActionResult HomeServicesUnavailable()
        {
            return View("Unavailable");
        }
    }
}