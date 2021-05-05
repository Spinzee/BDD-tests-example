using Products.Infrastructure;
using Products.Service.Broadband;
using Products.Web.Areas.Broadband.Enums;
using Products.WebModel.ViewModels.Broadband;
using System;
using System.Web.Mvc;

namespace Products.Web.Areas.Broadband.Controllers
{
    public class BankDetailsController : BaseStepCounterController
    {
        private readonly IBankDetailsService _bankDetailsService;

        public BankDetailsController(IBankDetailsService bankDetailsService,
            IBroadbandJourneyService broadbandJourneyService) : base(broadbandJourneyService)
        {
            Guard.Against<ArgumentNullException>(bankDetailsService == null, $"{nameof(bankDetailsService)} is null");
            _bankDetailsService = bankDetailsService;
        }

        [Route("bank-details")]
        public ActionResult BankDetails()
        {
            SetStepCounter(PageName.BankDetails);
            return View(_bankDetailsService.GetBankDetailsViewModel());
        }

        [Route("bank-details")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submit(BankDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var viewModel = _bankDetailsService.SetBankDetailsViewModel(model);

                if (viewModel == null)
                    return RedirectToAction("Fallout", "BankDetails");

                if (viewModel.IsRetry == false)
                    return RedirectToAction("Summary", "Summary");
            }

            SetStepCounter(PageName.BankDetails);
            model.BackChevronViewModel = _bankDetailsService.GetBankDetailsViewModel().BackChevronViewModel;
            return View("~/Areas/Broadband/Views/BankDetails/BankDetails.cshtml", model);
        }

        [Route("invalid-bank-details")]
        [HttpGet]
        public ActionResult Fallout()
        {
            return View("~/Areas/Broadband/Views/BankDetails/InvalidBankDetails.cshtml");
        }
    }
}