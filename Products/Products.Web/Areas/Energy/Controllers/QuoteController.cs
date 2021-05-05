namespace Products.Web.Areas.Energy.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Infrastructure;
    using Products.ControllerHelpers.Energy;
    using Attributes;
    using Core;
    using Products.WebModel.Resources.Energy;
    using Products.WebModel.ViewModels.Energy;

    [EnergyCheckSession]
    public class QuoteController : BaseStepCounterController
    {
        private readonly IQuoteControllerHelper _quoteControllerHelper;

        public QuoteController(IQuoteControllerHelper quoteControllerHelper) : base(quoteControllerHelper)
        {
            Guard.Against<ArgumentException>(quoteControllerHelper == null, $"{nameof(quoteControllerHelper)} is null");
            _quoteControllerHelper = quoteControllerHelper;
        }

        [HttpGet]
        [Route("existing-customer")]
        public ActionResult ExistingCustomer(string productCode, string cli, bool isBundle = false)
        {
            var existingCustomerViewModel = new ExistingCustomerViewModel
            {
                IsBundlingJourney = isBundle,
                ProductCode = productCode,
                ShowCli = cli == "1"
            };

            return View("ExistingCustomer", existingCustomerViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("existing-customer")]
        public ActionResult ExistingCustomer(ExistingCustomerViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (!viewModel.IsSseCustomer)
                {
                    _quoteControllerHelper.SaveDetailsFromHub(viewModel);
                    return RedirectToAction("EnterPostcode");
                }

                return RedirectToAction("IdentifyCustomer", "CustomerIdentification", new { area = "TariffChange" });
            }

            return View("ExistingCustomer", viewModel);
        }

        [HttpGet]
        [Route("enter-postcode")]
        public async Task<ActionResult> EnterPostcode(string productCode, string ourPrices, bool isBundle = false, string postcode = "")
        {
            bool alertStatus = await _quoteControllerHelper.IsEnergyCustomerAlert();
            if (alertStatus)
            {
                return RedirectToAction("EnergyUnavailable");
            }

            bool.TryParse(ourPrices, out bool ourPricesValue);
            if (!string.IsNullOrEmpty(postcode))
            {
                return CheckAndSavePostcode(new PostcodeViewModel { ProductCode = productCode, PostCode = postcode, OurPrices = ourPricesValue, IsBundle = isBundle });
            }

            return View(_quoteControllerHelper.GetEnergyPostcodeViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("enter-postcode")]
        public async Task<ActionResult> EnterPostcode(PostcodeViewModel model)
        {
            bool alertStatus = await _quoteControllerHelper.IsEnergyCustomerAlert();
            if (alertStatus)
            {
                return RedirectToAction("EnergyUnavailable");
            }

            if (ModelState.IsValid)
            {
                return CheckAndSavePostcode(model);
            }

            return View(model);
        }

        [Route("area-not-covered")]
        public ActionResult AreaNotCovered()
        {
            ViewBag.Title = Fallout_Resources.AreaNotCoveredTitle;
            return View();
        }

        [HttpGet]
        [Route("select-address")]
        public async Task<ActionResult> SelectAddress()
        {
            SelectAddressViewModel selectAddressViewModel = await _quoteControllerHelper.GetSelectAddressViewModel();
            if (!selectAddressViewModel.Addresses.Any() && selectAddressViewModel.QASEnabled)
            {
                return RedirectToAction("UnableToComplete", "SignUp");
            }

            return View(selectAddressViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("select-address")]
        public async Task<ActionResult> SelectAddress(SelectAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.IsManual)
                {
                    _quoteControllerHelper.SetManualAddress(model);
                }
                else
                {
                    string[] addressArray = model.SelectedAddressId.Split(new[] { "~~" }, StringSplitOptions.None);

                    bool isSet = false;
                    if (addressArray.Length == 2)
                    {
                        isSet = await _quoteControllerHelper.SetSelectedAddress(addressArray[0], addressArray[1]);
                    }

                    if (!isSet)
                    {
                        return RedirectToAction("UnableToComplete", "SignUp");
                    }
                }

                bool cAndCResult = await _quoteControllerHelper.ProcessAddress();
                if (!cAndCResult)
                {
                    return RedirectToAction("UnableToComplete", "SignUp");
                }

                _quoteControllerHelper.ResetCustomer();

                return RedirectToAction("SelectFuel");
            }

            SelectAddressViewModel selectAddressViewModel = await _quoteControllerHelper.GetSelectAddressViewModel();
            model.BackChevronViewModel = selectAddressViewModel.BackChevronViewModel;
            model.Addresses = selectAddressViewModel.Addresses;
            model.Postcode = selectAddressViewModel.Postcode;
            model.HeaderText = selectAddressViewModel.HeaderText;
            model.ParaText = selectAddressViewModel.ParaText;
            model.SubHeaderText = selectAddressViewModel.SubHeaderText;
            return View(model);
        }

        [Route("cannot-find-prices")]
        public ActionResult CannotFindPrices()
        {
            return View("UnableToComplete", _quoteControllerHelper.GetUnableToCompleteViewModel(true));
        }

        [HttpGet]
        [Route("unavailable")]
        public ActionResult EnergyUnavailable()
        {
            return View("Unavailable");
        }

        [HttpGet]
        [Route("select-fuel")]
        public ActionResult SelectFuel()
        {
            return View(_quoteControllerHelper.GetSelectFuelViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("select-fuel")]
        public ActionResult SelectFuel(SelectFuelViewModel model)
        {
            if (ModelState.IsValid)
            {
                SelectFuelViewModel viewModel = _quoteControllerHelper.SetSelectedFuel(model);
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (viewModel.CAndCRedirectRoute)
                {
                    case CAndCRedirectRoute.Usage:
                        return RedirectToAction("EnergyUsage", "Tariffs");
                    case CAndCRedirectRoute.SmartPaygoFallout:
                        return RedirectToAction("SmartPayGo");
                    case CAndCRedirectRoute.MultiRateMeterFallout:
                        return RedirectToAction("OtherMeterType");
                    case CAndCRedirectRoute.PaymentMethod:
                        return RedirectToAction("PaymentMethod");
                }
            }

            model.BackChevronViewModel = _quoteControllerHelper.GetSelectFuelViewModel().BackChevronViewModel;
            return View(model);
        }

        [HttpGet]
        [Route("payment-method")]
        public ActionResult PaymentMethod()
        {
            return View(_quoteControllerHelper.GetSelectPaymentMethodViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("payment-method")]
        public ActionResult PaymentMethod(SelectPaymentMethodViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _quoteControllerHelper.SaveSelectedPaymentMethod(viewModel);
                _quoteControllerHelper.SetBillingPreference();

                if (_quoteControllerHelper.ShowSmartFrequency())
                {
                    return RedirectToAction("SmartMeterFrequency");
                }

                if (_quoteControllerHelper.ShowUsageScreenForCAndCJourneyNonSmartCustomer())
                {
                    return RedirectToAction("EnergyUsage", "Tariffs");
                }

                // ReSharper disable once ConvertIfStatementToReturnStatement
                if (_quoteControllerHelper.ShowSmartMeterTypeQuestionInNonCAndCJourney())
                {
                    return RedirectToAction("SmartMeter");
                }

                return RedirectToAction("MeterType");
            }

            SelectPaymentMethodViewModel selectedPaymentMethodViewModel = _quoteControllerHelper.GetSelectPaymentMethodViewModel();
            viewModel.PaymentMethods = selectedPaymentMethodViewModel.PaymentMethods;
            viewModel.BackChevronViewModel = selectedPaymentMethodViewModel.BackChevronViewModel;
            return View(viewModel);
        }

        [HttpGet]
        [Route("meter-type")]
        public ActionResult MeterType()
        {
            return View(_quoteControllerHelper.GetSelectMeterTypeViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("meter-type")]
        public ActionResult MeterType(SelectMeterTypeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (_quoteControllerHelper.IsAMeterTypeFallout(viewModel))
                {
                    return RedirectToAction("OtherMeterType");
                }

                _quoteControllerHelper.SetSelectedMeterType(viewModel);
                return RedirectToAction("SmartMeter");
            }

            SelectMeterTypeViewModel selectMeterTypeViewModel = _quoteControllerHelper.GetSelectMeterTypeViewModel();
            viewModel.BackChevronViewModel = selectMeterTypeViewModel.BackChevronViewModel;
            viewModel.MeterTypes = selectMeterTypeViewModel.MeterTypes;

            return View(viewModel);
        }

        [HttpGet]
        [Route("smart-meter")]
        public ActionResult SmartMeter()
        {
            return View(_quoteControllerHelper.GetSelectSmartMeterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("smart-meter")]
        public ActionResult SmartMeter(SelectSmartMeterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _quoteControllerHelper.SetHasSmartMeter(viewModel);
                return RedirectToAction("EnergyUsage", "Tariffs");
            }

            SelectSmartMeterViewModel selectSmartMeterViewModel = _quoteControllerHelper.GetSelectSmartMeterViewModel();
            viewModel.SmartMeter = selectSmartMeterViewModel.SmartMeter;
            viewModel.BackChevronViewModel = selectSmartMeterViewModel.BackChevronViewModel;

            return View(viewModel);
        }

        [Route("other-meter-type")]
        public ActionResult OtherMeterType()
        {
            return View("~/Areas/Energy/Views/Quote/OtherMeterType.cshtml");
        }

        [Route("smart-paygo")]
        public ActionResult SmartPayGo()
        {
            return View("~/Areas/Energy/Views/Quote/SmartPayGoFallout.cshtml");
        }

        [HttpGet]
        [Route("smart-meter-frequency")]
        public ActionResult SmartMeterFrequency()
        {
            SmartMeterFrequencyViewModel viewModel = _quoteControllerHelper.GetSmartMeterFrequencyViewModel();

            return View("~/Areas/Energy/Views/Quote/SmartMeterConsent.cshtml", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("smart-meter-frequency")]
        public ActionResult SmartMeterFrequency(SmartMeterFrequencyViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _quoteControllerHelper.SaveSelectedSmartMeterFrequency(viewModel);
                return RedirectToAction("EnergyUsage", "Tariffs");
            }

            SmartMeterFrequencyViewModel smartMeterFrequencyViewModel = _quoteControllerHelper.GetSmartMeterFrequencyViewModel();
            viewModel.SmartMeterReadingFrequency = smartMeterFrequencyViewModel.SmartMeterReadingFrequency;
            viewModel.BackChevronViewModel = smartMeterFrequencyViewModel.BackChevronViewModel;
            return View("~/Areas/Energy/Views/Quote/SmartMeterConsent.cshtml", viewModel);
        }

        private ActionResult CheckAndSavePostcode(PostcodeViewModel model)
        {
            if (_quoteControllerHelper.IsNorthernIrelandPostcode(model.PostCode))
            {
                return RedirectToAction("AreaNotCovered");
            }

            _quoteControllerHelper.SaveCustomer(model);
            return RedirectToAction("SelectAddress");
        }
    }
}