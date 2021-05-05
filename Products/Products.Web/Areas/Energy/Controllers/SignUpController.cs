namespace Products.Web.Areas.Energy.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Attributes;
    using ControllerHelpers.Energy;
    using Infrastructure;
    using Model.Enums;
    using WebModel.ViewModels.Common;
    using WebModel.ViewModels.Energy;

    [EnergyCheckSession]
    public class SignUpController : BaseStepCounterController
    {
        private readonly ISignUpControllerHelper _signUpControllerHelper;

        public SignUpController(ISignUpControllerHelper signUpControllerHelper)
            : base(signUpControllerHelper)
        {
            Guard.Against<ArgumentException>(signUpControllerHelper == null, $"{nameof(signUpControllerHelper)} is null");
            _signUpControllerHelper = signUpControllerHelper;
        }

        [HttpGet]
        [Route("personal-details")]
        public ActionResult PersonalDetails()
        {
            return View(_signUpControllerHelper.GetPersonalDetailsViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("personal-details")]
        public ActionResult PersonalDetails(PersonalDetailsViewModel personalDetailsViewModel)
        {
            PersonalDetailsViewModel viewModel = _signUpControllerHelper.GetPersonalDetailsViewModel();
            personalDetailsViewModel.IsScottishPostcode = viewModel.IsScottishPostcode;
            TryValidateModel(personalDetailsViewModel);
            if (ModelState.IsValid)
            {
                _signUpControllerHelper.SavePersonalDetailsVieModel(personalDetailsViewModel);
                return RedirectToAction("ContactDetails");
            }

            personalDetailsViewModel.BackChevronViewModel = viewModel.BackChevronViewModel;
            return View(personalDetailsViewModel);
        }

        [HttpGet]
        [Route("contact-details")]
        public ActionResult ContactDetails()
        {
            return View(_signUpControllerHelper.GetContactDetailsViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("contact-details")]
        public async Task<ActionResult> ContactDetails(ContactDetailsViewModel contactDetailsViewModel)
        {
            if (ModelState.IsValid)
            {
                _signUpControllerHelper.SetContactDetails(contactDetailsViewModel);

                if (await _signUpControllerHelper.DoesProfileExist())
                {
                    return RedirectToAction(_signUpControllerHelper.HasSelectedDirectDebit() ? "BankDetails" : "ViewSummary");
                }

                return RedirectToAction("OnlineAccount");
            }

            contactDetailsViewModel.BackChevronViewModel =
                _signUpControllerHelper.GetContactDetailsViewModel().BackChevronViewModel;
            return View(contactDetailsViewModel);
        }

        [HttpGet]
        [Route("bank-details")]
        public ActionResult BankDetails()
        {
            return View(_signUpControllerHelper.GetBankDetailsViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("bank-details")]
        public ActionResult BankDetails(BankDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool? isRetry = _signUpControllerHelper.ValidateBankDetails(model);
                switch (isRetry)
                {
                    case null:
                        return RedirectToAction("ViewSummary");
                    case false:
                        return RedirectToAction("InvalidBankDetails");
                    default:
                        model.IsRetry = true;
                        break;
                }
            }

            model = _signUpControllerHelper.GetUpdatedBankDetailsViewModel(model);

            return View(model);
        }

        [HttpGet]
        [Route("invalid-bank-details")]
        public ActionResult InvalidBankDetails()
        {
            return View();
        }

        [Route("online-account")]
        public async Task<ActionResult> OnlineAccount()
        {
            if (await _signUpControllerHelper.DoesProfileExist())
            {
                return RedirectToAction(_signUpControllerHelper.HasSelectedDirectDebit() ? "BankDetails" : "ViewSummary");
            }

            return View(_signUpControllerHelper.GetOnlineAccountViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("online-account")]
        public ActionResult OnlineAccount(OnlineAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                _signUpControllerHelper.SaveOnlineAccountPassword(model.Password);
                return RedirectToAction(_signUpControllerHelper.HasSelectedDirectDebit() ? "BankDetails" : "ViewSummary");
            }

            model.BackChevronViewModel = _signUpControllerHelper.GetOnlineAccountViewModel().BackChevronViewModel;
            return View(model);
        }

        [HttpGet]
        [Route("order-summary")]
        public ActionResult ViewSummary()
        {
            return View("Summary", _signUpControllerHelper.GetSummaryViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("order-summary")]
        public async Task<ActionResult> ViewSummary(SummaryViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Summary", _signUpControllerHelper.GetSummaryViewModel());
            }
            
            if (!await _signUpControllerHelper.DoesProfileExist())
            {
                await _signUpControllerHelper.CreateOnlineProfile();
            }
            await _signUpControllerHelper.ConfirmSale();            
            return RedirectToAction("Confirmation");
        }

        [HttpGet]
        [Route("print-mandate")]
        public ActionResult PrintMandate(ProductType productType)
        {
            return View("PrintMandate", _signUpControllerHelper.GetPrintMandateViewModel(productType));
        }

        [HttpGet]
        [Route("phone-package")]
        public ActionResult PhonePackage()
        {
            return View("PhonePackage", _signUpControllerHelper.GetPhonePackageViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("phone-package")]
        public ActionResult PhonePackage(string talkCode, PhonePackageViewModel viewModel)
        {
            _signUpControllerHelper.SetPhonePackageInformation(talkCode, false);
            _signUpControllerHelper.UpdateCLIFromSession(viewModel.KeepYourNumberViewModel);
            ModelState.Clear();
            TryValidateModel(viewModel);

            if (!ModelState.IsValid)
            {
                return View("PhonePackage", viewModel);
            }

            _signUpControllerHelper.UpdateCustomerKeepYourNumber(viewModel.KeepYourNumberViewModel);

            if (_signUpControllerHelper.SelectedTariffHasExtras())
            {
                return RedirectToAction("Extras");
            }

            return RedirectToAction("PersonalDetails");
        }

        [HttpGet]
        [Route("extras")]
        public ActionResult Extras()
        {
            return View(_signUpControllerHelper.GetExtrasViewModel());
        }

        [AjaxOnly]
        [HttpGet]
        [Route("add-extra")]
        public PartialViewResult AddExtra(string productCode)
        {
            _signUpControllerHelper.AddExtra(productCode);
            return PartialView("_YourPriceDynamic", _signUpControllerHelper.GetYourPriceViewModel());
        }

        [AjaxOnly]
        [HttpGet]
        [Route("remove-extra")]
        public PartialViewResult RemoveExtra(string productCode)
        {
            _signUpControllerHelper.RemoveExtra(productCode);
            return PartialView("_YourPriceDynamic", _signUpControllerHelper.GetYourPriceViewModel());
        }

        [AjaxOnly]
        [HttpGet]
        [Route("remove-extra-summary")]
        public PartialViewResult RemoveExtraSummary(string productCode)
        {
            _signUpControllerHelper.RemoveExtra(productCode);
            return PartialView("_SummaryAccordion", _signUpControllerHelper.GetSummaryViewModel());
        }

        [Route("non-matching-meter-details")]
        public ActionResult IncorrectMeter()
        {
            return View("IncorrectMeterFallout");
        }

        [HttpGet]
        [Route("confirmation")]
        public ActionResult Confirmation()
        {
            return View(_signUpControllerHelper.ConfirmationViewModel());
        }

        [HttpGet]
        [Route("unable-to-complete")]
        public ActionResult UnableToComplete()
        {
            return View(_signUpControllerHelper.GetUnableToCompleteViewModel());
        }

        [AjaxOnly]
        [HttpGet]
        [Route("update-your-price")]
        public ActionResult UpdateYourPrice(string talkCode, bool setToDefault)
        {
            _signUpControllerHelper.SetPhonePackageInformation(talkCode, setToDefault);

            YourPriceViewModel viewModel = _signUpControllerHelper.GetYourPriceViewModel();
            if (viewModel != null)
            {
                return PartialView("_YourPriceDynamic", viewModel);
            }

            return new HttpStatusCodeResult(500, "can't process request");
        }

        [AjaxOnly]
        [HttpGet]
        [Route("update-summary")]
        public ActionResult UpdateSummary(string talkCode, bool setToDefault)
        {
            _signUpControllerHelper.SetPhonePackageInformation(talkCode, setToDefault);

            SummaryViewModel viewModel = _signUpControllerHelper.GetSummaryViewModel();
            if (viewModel != null)
            {
                return View("Summary", viewModel);
            }

            return new HttpStatusCodeResult(500, "can't process request");
        }

        [AjaxOnly]
        [HttpGet]
        [Route("update-talk-package")]
        public ActionResult UpdateSelectTalkPackage()
        {
            SelectTalkPackageViewModel viewModel = _signUpControllerHelper.GetPhonePackageViewModel().SelectTalkPackageViewModel;
            if (viewModel != null)
            {
                return PartialView("_SelectTalkPackage", viewModel);
            }

            return new HttpStatusCodeResult(500, "can't process request");
        }

        [AjaxOnly]
        [HttpGet]
        [Route("update-direct-debit-amounts")]
        public ActionResult UpdateDirectDebitAmounts()
        {
            BankDetailsViewModel viewModel = _signUpControllerHelper.GetBankDetailsViewModel();
            if (viewModel != null)
            {
                return View("BankDetails", viewModel);
            }

            return new HttpStatusCodeResult(500, "can't process request");
        }

        [HttpGet]
        [Route("confirm-address")]
        public async Task<ActionResult> ConfirmAddress()
        {
            ConfirmAddressViewModel viewModel = await _signUpControllerHelper.GetConfirmAddressViewModel();
            TempData["AddressList"] = viewModel.Addresses;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("confirm-address")]
        public ActionResult ConfirmAddress(ConfirmAddressViewModel viewModel)
        {
            var addresses = (List<BTAddressViewModel>)TempData["AddressList"];
            TempData.Keep("AddressList");

            if (ModelState.IsValid)
            {
                _signUpControllerHelper.SetSelectedBTAddress(viewModel, addresses);
                return RedirectToAction("CheckBroadbandPackage", "Tariffs");
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("upgrades")]
        public ActionResult Upgrades() 
        {
            return View(_signUpControllerHelper.GetUpgradesViewModel());
        }

        [AjaxOnly]
        [HttpGet]
        [Route("add-bundle-upgrade")]
        public PartialViewResult AddBundleUpgrade(string productCode)
        {
            _signUpControllerHelper.AddBundleUpgrade(productCode);
            return PartialView("_AddBundleUpgrade", _signUpControllerHelper.GetUpgradesViewModel());
        }

        [AjaxOnly]
        [HttpGet]
        [Route("remove-bundle-upgrade")]
        public PartialViewResult RemoveBundleUpgrade(string productCode)
        {
            _signUpControllerHelper.RemoveBundleUpgrade(productCode);
            return PartialView("_AddBundleUpgrade", _signUpControllerHelper.GetUpgradesViewModel());
        }
    }
}