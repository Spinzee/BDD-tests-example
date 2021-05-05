namespace Products.Web.Areas.Energy.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core;
    using Infrastructure;
    using Products.ControllerHelpers.Energy;
    using Products.WebModel.Resources.Energy;
    using Products.WebModel.ViewModels.Energy;

    [RouteArea("Energy", AreaPrefix = "our-prices")]
    public class OurPricesController : Controller
    {
        private readonly IQuoteControllerHelper _quoteControllerHelper;
        private readonly ITariffsControllerHelper _tariffsControllerHelper;

        public OurPricesController(
            IQuoteControllerHelper quoteControllerHelper,
            ITariffsControllerHelper tariffsControllerHelper)
        {
            Guard.Against<ArgumentException>(quoteControllerHelper == null, $"{nameof(quoteControllerHelper)} is null");
            Guard.Against<ArgumentException>(tariffsControllerHelper == null, $"{nameof(tariffsControllerHelper)} is null");

            _quoteControllerHelper = quoteControllerHelper;
            _tariffsControllerHelper = tariffsControllerHelper;
        }

        [HttpGet]
        [Route("enter-postcode")]
        public async Task<ActionResult> EnterPostcode()
        {
            bool alertStatus = await _quoteControllerHelper.IsOurPricesCustomerAlert();
            if (alertStatus)
            {
                return RedirectToAction("OurPriceUnavailable");
            }

            return View(_quoteControllerHelper.GetOurPricesPostcodeViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("enter-postcode")]
        public async Task<ActionResult> EnterPostcode(PostcodeViewModel model)
        {
            bool alertStatus = await _quoteControllerHelper.IsOurPricesCustomerAlert();
            if (alertStatus)
            {
                return RedirectToAction("OurPriceUnavailable");
            }

            if (ModelState.IsValid)
            {
                if (_quoteControllerHelper.IsNorthernIrelandPostcode(model.PostCode))
                {
                    return RedirectToAction("AreaNotCovered");
                }

                bool? isValid = await _quoteControllerHelper.IsValidPostCode(model.PostCode);
                if (isValid == true)
                {
                    return RedirectToAction("OurPrices", new { postcode = model.PostCode });
                }

                return RedirectToAction("CannotFindPrices");
            }

            return View(model);
        }

        [Route("view-tariffs-and-prices")]
        public async Task<ActionResult> OurPrices(string postcode, FuelCategory fuelCategory = FuelCategory.Standard, TariffStatus tariffStatus = TariffStatus.ForSale)
        {
            return View(await _tariffsControllerHelper.GetOurPriceViewModel(postcode, tariffStatus, fuelCategory));
        }

        [Route("cannot-find-prices")]
        public ActionResult CannotFindPrices()
        {
            return View("UnableToComplete", _quoteControllerHelper.GetUnableToCompleteViewModel(false));
        }

        [Route("area-not-covered")]
        public ActionResult AreaNotCovered()
        {
            ViewBag.Title = OurPrices_Resources.AreaNotCoveredTitle;
            return View();
        }

        [Route("unavailable")]
        public ActionResult OurPriceUnavailable()
        {
            return View("Unavailable");
        }
    }
}