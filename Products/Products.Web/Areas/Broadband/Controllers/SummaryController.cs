namespace Products.Web.Areas.Broadband.Controllers
{
    using Infrastructure;
    using Products.Service.Broadband;
    using Enums;
    using Products.WebModel.ViewModels.Broadband;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    public class SummaryController : BaseStepCounterController
    {
        private readonly IBroadbandApplicationService _broadbandApplicationService;
        private readonly ISummaryService _summaryService;
        private readonly IOnlineCreationService _onlineCreationService;

        public SummaryController(IBroadbandJourneyService broadbandJourneyService,
            IBroadbandApplicationService broadbandApplicationService, 
            ISummaryService summaryService,
            IOnlineCreationService onlineCreationService) : base(broadbandJourneyService)
        {
            Guard.Against<ArgumentNullException>(broadbandApplicationService == null, $"{nameof(broadbandApplicationService)} is null");
            Guard.Against<ArgumentNullException>(onlineCreationService == null, $"{nameof(onlineCreationService)} is null");
            Guard.Against<ArgumentNullException>(summaryService == null, $"{nameof(summaryService)} is null");

            _broadbandApplicationService = broadbandApplicationService;
            _onlineCreationService = onlineCreationService;
            _summaryService = summaryService;
        }

        [HttpGet]
        [Route("summary")]
        public ActionResult Summary()
        {
            SetStepCounter(PageName.OrderSummary);
            return View("~/Areas/Broadband/Views/Summary/Summary.cshtml", _summaryService.GetSummaryViewModel());
        }

        [HttpPost]
        [Route("summary")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Submit(SummaryViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _onlineCreationService.CreateUserProfile();
                TempData["DataLayerDictionary"] = await _broadbandApplicationService.SubmitApplication();

                return RedirectToAction("Confirmation", "Confirmation");
            }

            SetStepCounter(PageName.OrderSummary);
            return View("~/Areas/Broadband/Views/Summary/Summary.cshtml", _summaryService.PopulateSummaryViewModel(model));
        }

        [HttpGet]
        [Route("PrintMandate")]
        public ActionResult PrintMandate()
        {
            return View("PrintMandate", _summaryService.GetPrintMandateViewModel());
        }
    }
}