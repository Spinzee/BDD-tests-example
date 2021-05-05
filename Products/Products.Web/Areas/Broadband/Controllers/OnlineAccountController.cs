using Products.Infrastructure;
using Products.Service.Broadband;
using Products.Web.Areas.Broadband.Enums;
using Products.WebModel.ViewModels.Broadband;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Products.Web.Areas.Broadband.Controllers
{
    public class OnlineAccountController : BaseStepCounterController
    {
        private readonly IOnlineCreationService _onlineCreationService;

        public OnlineAccountController(
            IBroadbandJourneyService broadbandJourneyService,
            IOnlineCreationService onlineCreationService) : base(broadbandJourneyService)
        {
            Guard.Against<ArgumentNullException>(onlineCreationService == null, "onlineCreationService is null");
            _onlineCreationService = onlineCreationService;
        }

        [Route("online-account")]
        public ActionResult OnlineAccount()
        {
            SetStepCounter(PageName.CreateOnlineAccount);
            return View("~/Areas/Broadband/Views/OnlineAccount/OnlineAccount.cshtml", _onlineCreationService.GetOnlineAccountViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("online-account")]
        public ActionResult OnlineAccount(OnlineAccountViewModel model)
        {
            if (ModelState.IsValid)
            {                
                _onlineCreationService.SaveOnlinePassword(model.Password);
                return RedirectToAction("BankDetails", "BankDetails");
            }

            SetStepCounter(PageName.CreateOnlineAccount);
            return View("~/Areas/Broadband/Views/OnlineAccount/OnlineAccount.cshtml", _onlineCreationService.SetOnlineAccountViewModel(model));
        }
    }
}