using Products.Service.TariffChange;
using System.Web.Mvc;
using System.Web.UI;

namespace Products.Web.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, Location = OutputCacheLocation.None)]
    public class ErrorController : Controller
    {
        private readonly IJourneyDetailsService _journeyDetailsService;

        public ErrorController(IJourneyDetailsService journeyDetailsService)
        {
            _journeyDetailsService = journeyDetailsService;
        }

        // GET: Error
        [HttpGet]
        [Route("error/technical-fault")]
        public ActionResult Error()
        {
            _journeyDetailsService.ClearJourneyDetails();
            return View();
        }

        [HttpGet]
        [Route("error/unhandled-error")]
        public ActionResult UnhandledError()
        {
            _journeyDetailsService.ClearJourneyDetails();
            return View();
        }

        [HttpGet]
        [Route("error/page-not-found")]
        public ActionResult PageNotFound()
        {
            _journeyDetailsService.ClearJourneyDetails();
            return View("PageNotFound");
        }

        [HttpGet]
        [Route("error/javascript-disabled")]
        public ActionResult JavascriptDisabled()
        {
            return View();
        }
    }
}