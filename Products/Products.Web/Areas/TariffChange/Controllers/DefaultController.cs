using System.Configuration;
using System.Web.Mvc;

namespace Products.Web.Areas.TariffChange.Controllers
{
    [RouteArea("TariffChange", AreaPrefix = "tariff-change")]
    public class DefaultController : Controller
    {
        public ActionResult Index()
        {
            return RedirectPermanent(ConfigurationManager.AppSettings["LinkBackToHomeURL"]);
        }
    }
}
