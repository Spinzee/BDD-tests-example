namespace Products.Web.Areas.TariffChange.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.UI;
    using Core;
    using Helpers;
    using Infrastructure;
    using Products.Model.TariffChange.Customers;
    using Products.Model.TariffChange.Enums;
    using Products.Service.TariffChange;
    using Products.WebModel.Resources.TariffChange;
    using Products.WebModel.ViewModels.TariffChange;

    [RouteArea("TariffChange", AreaPrefix = "tariff-change")]
    [OutputCache(NoStore = true, Duration = 0, Location = OutputCacheLocation.None)]
    public class CustomerIdentificationController : Controller
    {
        private readonly ITariffChangeService _tariffChangeService;

        public CustomerIdentificationController(ITariffChangeService tariffChangeService)
        {
            Guard.Against<ArgumentNullException>(tariffChangeService == null, "tariffChangeService is null");
            _tariffChangeService = tariffChangeService;
        }

        [HttpGet]
        [Route("identify")]
        public async Task<ActionResult> IdentifyCustomer()
        {
            ConfirmDetailsViewModel model = await _tariffChangeService.GetConfirmDetailsViewModel();

            TempData["IsPostLogin"] = model.CTCJourneyType != CTCJourneyType.PreLogIn;

            if (model.CustomerAlertActive)
            {
                return RedirectToAction("CtcUnavailable");
            }

            if (model.CTCJourneyType == CTCJourneyType.PostLogInWithMultipleSites)
            {
                return View("ConfirmDetails", model);
            }

            if (model.CTCJourneyType == CTCJourneyType.PostLogInWithSingleSite)
            {
                if (model.HasMultipleServices)
                {
                    TempData["DataLayer"] = new List<FalloutReasonResult>
                    {
                        new FalloutReasonResult { FalloutReason = FalloutReason.Indeterminable, FalloutDescription = "Customer has multiple service accounts." }
                    };
                    return RedirectToAction("CustomerAccountIneligible", "AccountEligibility");
                }

                if (!model.IsValidForPostCode)
                {
                    return RedirectToAction("AccountDetailsNotMatched");
                }

                return RedirectToAction("ConfirmDetails");
            }

            return View("IdentifyCustomer", _tariffChangeService.GetUnAuthenticatedIdentifyViewModel());

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("identify")]
        public ActionResult IdentifyCustomer(IdentifyCustomerViewModel identifyCustomerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("IdentifyCustomer", identifyCustomerViewModel);
            }

            GoogleCaptchaViewModel googleCaptchaViewModel = _tariffChangeService.CheckGoogleCaptchaViewModel();

            if (!googleCaptchaViewModel.IsValidReCaptcha)
            {
                return View("IdentifyCustomer");
            }

            ConfirmDetailsViewModel model = _tariffChangeService.ValidateCustomer(identifyCustomerViewModel);
            if (model.HasMultipleServices)
            {
                TempData["DataLayer"] = new List<FalloutReasonResult>
                {
                    new FalloutReasonResult { FalloutReason = FalloutReason.Indeterminable, FalloutDescription = "Customer has multiple service accounts." }
                };
                return RedirectToAction("CustomerAccountIneligible", "AccountEligibility");
            }

            if (!model.IsValidForPostCode)
            {
                return RedirectToAction("AccountDetailsNotMatched");
            }

            return RedirectToAction("ConfirmDetails");
        }

        [HttpGet]
        [Route("account-not-found")]
        public ActionResult AccountDetailsNotMatched()
        {
            return View("AccountDetailsNotMatched");
        }

        [HttpGet]
        [Route("confirm-details")]
        public ActionResult ConfirmDetails()
        {
            var viewModel = new ConfirmDetailsViewModel();
            ConfirmAddressViewModel confirmAddressViewModel = _tariffChangeService.GetConfirmAddressViewModel();

            if (!confirmAddressViewModel.IsCustomerAccountSet)
            {
                return RedirectToAction("IdentifyCustomer");
            }

            viewModel.CTCJourneyType = confirmAddressViewModel.CTCJourneyType;
            viewModel.ConfirmAddressViewModel = confirmAddressViewModel;

            switch (viewModel.CTCJourneyType)
            {
                case CTCJourneyType.PostLogInWithMultipleSites:
                    viewModel.ProgressBarViewModel = new ProgressBarViewModel
                    {
                        Sections = new List<ProgressBarSection>
                        {
                            new ProgressBarSection
                            {
                                Text = ProgressBar_Resource.SelectAddressSectionHeader,
                                Status = ProgressBarStatus.Active
                            },
                            new ProgressBarSection
                            {
                                Text = ProgressBar_Resource.TariffChoiceSectionHeader,
                                Status = ProgressBarStatus.Awaiting
                            },
                            new ProgressBarSection
                            {
                                Text = ProgressBar_Resource.SummarySectionHeader,
                                Status = ProgressBarStatus.Awaiting
                            }
                        }
                    };
                    break;

                case CTCJourneyType.PostLogInWithSingleSite:
                    viewModel.ProgressBarViewModel = new ProgressBarViewModel
                    {
                        Sections = new List<ProgressBarSection>
                        {
                            new ProgressBarSection
                            {
                                Text = ProgressBar_Resource.TariffChoiceSectionHeader,
                                Status = ProgressBarStatus.Active
                            },
                            new ProgressBarSection
                            {
                                Text = ProgressBar_Resource.SummarySectionHeader,
                                Status = ProgressBarStatus.Awaiting
                            }
                        }
                    };
                    break;
                case CTCJourneyType.PostLogInWithNoAccounts:
                    viewModel.ProgressBarViewModel = new ProgressBarViewModel
                    {
                        Sections = new List<ProgressBarSection>
                        {
                            new ProgressBarSection
                            {
                                Text = ProgressBar_Resource.UserDetailsSectionHeader,
                                Status = ProgressBarStatus.Active
                            },
                            new ProgressBarSection
                            {
                                Text = ProgressBar_Resource.TariffChoiceSectionHeader,
                                Status = ProgressBarStatus.Awaiting
                            },
                            new ProgressBarSection
                            {
                                Text = ProgressBar_Resource.SummarySectionHeader,
                                Status = ProgressBarStatus.Awaiting
                            }
                        }
                    };
                    break;
                default:
                    viewModel.ProgressBarViewModel = new ProgressBarViewModel
                    {
                        Sections = new List<ProgressBarSection>
                        {
                            new ProgressBarSection
                            {
                                Text = ProgressBar_Resource.UserDetailsSectionHeader,
                                Status = ProgressBarStatus.Active
                            },
                            new ProgressBarSection
                            {
                                Text = ProgressBar_Resource.TariffChoiceSectionHeader,
                                Status = ProgressBarStatus.Awaiting
                            },
                            new ProgressBarSection
                            {
                                Text = ProgressBar_Resource.EmailAddressSectionHeader,
                                Status = ProgressBarStatus.Awaiting
                            },
                            new ProgressBarSection
                            {
                                Text = ProgressBar_Resource.SummarySectionHeader,
                                Status = ProgressBarStatus.Awaiting
                            }
                        }
                    };
                    break;

            }

            return View("ConfirmDetails", viewModel);
        }

        [HttpGet]
        [Route("unavailable")]
        public ActionResult CtcUnavailable()
        {
            ViewBag.IsPostLogin = HtmlHelpers.IsPostLogin(TempData["IsPostLogin"]);

            return View("Unavailable");
        }

        [HttpPost]
        [Route("multiple-address")]
        [ValidateAntiForgeryToken]
        public ActionResult SelectAddress(MultiSiteAddressesViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                int selectedSiteId = Convert.ToInt32(viewModel.SelectedSiteId);
                ConfirmDetailsViewModel validatedModel = _tariffChangeService.ValidateMultiSiteCustomer(selectedSiteId);

                if (validatedModel.HasMultipleServices)
                {
                    TempData["DataLayer"] = new List<FalloutReasonResult>
                    {
                        new FalloutReasonResult { FalloutReason = FalloutReason.Indeterminable, FalloutDescription = "Customer has multiple service accounts." }
                    };
                    return RedirectToAction("CustomerAccountIneligible", "AccountEligibility");
                }

                return RedirectToAction("CheckEligibility", "AccountEligibility");
            }

            return RedirectToAction("IdentifyCustomer");
        }
    }
}