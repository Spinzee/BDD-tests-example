using Products.Infrastructure;
using Products.Service.Broadband;
using Products.Web.Areas.Broadband.Enums;
using Products.WebModel.Resources.Broadband;
using System;

namespace Products.Web.Areas.Broadband.Controllers
{
    public class BaseStepCounterController : BaseController
    {
        private readonly IBroadbandJourneyService _broadbandJourneyService;

        public BaseStepCounterController(IBroadbandJourneyService broadbandJourneyService)
        {
            Guard.Against<ArgumentException>(broadbandJourneyService == null, "BroadbandJourneyService is null");

            _broadbandJourneyService = broadbandJourneyService;
        }


        protected void SetStepCounter(PageName pageName)
        {
            var broadbandDetails = _broadbandJourneyService.GetBroadbandJourneyDetails();

            string finalStep =  StepCount_Resources.CliNotProvidedFinalStepCount;

            switch (pageName)
            {
                case PageName.KeepYourNumber:
                    ViewBag.StepCounter = string.Format(StepCount_Resources.Step1Format, finalStep);
                    break;
                case PageName.PersonalDetails:
                    ViewBag.StepCounter = string.Format(StepCount_Resources.Step2Format, finalStep);
                    break;
                case PageName.ContactDetails:
                    ViewBag.StepCounter = string.Format( StepCount_Resources.Step3Format, finalStep);
                    break;
                case PageName.CreateOnlineAccount:
                    ViewBag.StepCounter = string.Format(StepCount_Resources.Step3_1Format, finalStep);
                    break;
                case PageName.BankDetails:
                    ViewBag.StepCounter = string.Format( StepCount_Resources.Step4Format, finalStep);
                    break;
                case PageName.OrderSummary:
                    ViewBag.StepCounter = string.Format(StepCount_Resources.Step5Format, finalStep);
                    break;
                default:
                    ViewBag.StepCounter = string.Empty;
                    break;
            }
        }
    }
}
