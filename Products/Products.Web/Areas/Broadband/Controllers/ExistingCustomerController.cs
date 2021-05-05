using System.Web.Mvc;
using System.Web.UI;

namespace Products.Web.Areas.Broadband.Controllers
{
    [RouteArea("Broadband", AreaPrefix = "broadband")]
    [OutputCache(NoStore = true, Duration = 0, Location = OutputCacheLocation.None)]
    public class ExistingCustomerController : Controller
    {

        [HttpGet]
        [Route("existing-customer")]
        public ActionResult ExistingCustomer(string productCode = "", string migrateAffiliateid = "", string migrateCampaignid = "", string migrateMemberid = "")
        {
            return RedirectToAction("LineChecker", "LineChecker",
                new { productCode, migrateAffiliateid, migrateCampaignid, migrateMemberid });
        }
    }
}