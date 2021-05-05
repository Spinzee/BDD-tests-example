namespace Products.Web.Areas.Broadband.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Enums;
    using Infrastructure;
    using Service.Broadband;
    using Service.Common;
    using WebModel.ViewModels.Broadband;

    public class CustomerDetailsController : BaseStepCounterController
    {
        private readonly ICustomerDetailsService _customerDetailsService;
        private readonly ICustomerProfileService _profileService;

        public CustomerDetailsController(
            IBroadbandJourneyService broadbandJourneyService,
            ICustomerDetailsService customerDetailsService,
            ICustomerProfileService profileService) : base(broadbandJourneyService)
        {
            Guard.Against<ArgumentException>(customerDetailsService == null, $"{nameof(customerDetailsService)} is null");
            _customerDetailsService = customerDetailsService;
            _profileService = profileService;
        }

        [HttpGet]
        [Route("personal-details")]
        public ActionResult PersonalDetails()
        {
            SetStepCounter(PageName.PersonalDetails);
            return View("~/Areas/Broadband/Views/CustomerDetails/PersonalDetails.cshtml", _customerDetailsService.GetPersonalDetailsViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("personal-details")]
        public ActionResult Submit(PersonalDetailsViewModel personalDetailsViewModel)
        {
            PersonalDetailsViewModel viewModel = _customerDetailsService.GetPersonalDetailsViewModel();
            personalDetailsViewModel.IsScottishPostcode = viewModel.IsScottishPostcode;
            TryValidateModel(personalDetailsViewModel);

            if (ModelState.IsValid)
            {
                _customerDetailsService.SetPersonalDetailsViewModel(personalDetailsViewModel);
                return RedirectToAction("ContactDetails", "CustomerDetails");
            }

            SetStepCounter(PageName.PersonalDetails);
            personalDetailsViewModel.BackChevronViewModel = viewModel.BackChevronViewModel;
            personalDetailsViewModel.YourPriceViewModel = viewModel.YourPriceViewModel;
            return View("~/Areas/Broadband/Views/CustomerDetails/PersonalDetails.cshtml", personalDetailsViewModel);
        }

        [Route("contact-details")]
        public ActionResult ContactDetails()
        {
            SetStepCounter(PageName.ContactDetails);
            return View("~/Areas/Broadband/Views/CustomerDetails/ContactDetails.cshtml", _customerDetailsService.GetContactDetailsViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("contact-details")]
        public async Task<ActionResult> SubmitContactDetails(ContactDetailsViewModel contactDetailsViewModel)
        {
            if (ModelState.IsValid)
            {
                Guid profileId = await _profileService.GetProfileIdByEmail(contactDetailsViewModel.EmailAddress);
                _customerDetailsService.SetContactDetailsViewModel(contactDetailsViewModel, profileId);

                if (profileId == Guid.Empty)
                {
                    return RedirectToAction("OnlineAccount", "OnlineAccount");
                }

                return RedirectToAction("BankDetails", "BankDetails");
            }

            SetStepCounter(PageName.ContactDetails);
            ContactDetailsViewModel defaultContactDetailsViewModel = _customerDetailsService.GetContactDetailsViewModel();
            contactDetailsViewModel.BackChevronViewModel = defaultContactDetailsViewModel.BackChevronViewModel;
            contactDetailsViewModel.YourPriceViewModel = defaultContactDetailsViewModel.YourPriceViewModel;
            return View("~/Areas/Broadband/Views/CustomerDetails/ContactDetails.cshtml", contactDetailsViewModel);
        }

        [HttpGet]
        [Route("transfer-your-number")]
        public ActionResult TransferYourNumber()
        {
            SetStepCounter(PageName.KeepYourNumber);
            return View("~/Areas/Broadband/Views/CustomerDetails/TransferYourNumber.cshtml", _customerDetailsService.GetTransferYourNumberViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("transfer-your-number")]
        public ActionResult TransferYourNumber(TransferYourNumberViewModel model)
        {
            model = _customerDetailsService.GetUpdatedTransferYourNumberViewModel(model);
            ModelState.Clear();
            TryValidateModel(model);

            if (ModelState.IsValid)
            {
                _customerDetailsService.SetTransferYourNumberViewModel(model);
                return RedirectToAction("PersonalDetails", "CustomerDetails");
            }

            SetStepCounter(PageName.KeepYourNumber);
            return View("~/Areas/Broadband/Views/CustomerDetails/TransferYourNumber.cshtml", model);
        }
    }
}