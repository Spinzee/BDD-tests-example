using Products.Infrastructure;
using Products.Service.Broadband;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Products.Web.Areas.Broadband.Controllers
{
    public class ConfirmationController : BaseController
    {
        private readonly IConfirmationService _confirmationService;

        public ConfirmationController(IConfirmationService confirmationService)
        {
            Guard.Against<ArgumentNullException>(confirmationService == null, "confirmationService is null");
            _confirmationService = confirmationService;
        }

        [Route("confirmation")]
        public ActionResult Confirmation()
        {
            var confirmationViewModel = _confirmationService.ConfirmationViewModel((Dictionary<string, string>)TempData["DataLayerDictionary"]);
            return View("~/Areas/Broadband/Views/Confirmation/Confirmation.cshtml", confirmationViewModel);
        }
    }
}