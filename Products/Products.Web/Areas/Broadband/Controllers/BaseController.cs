using Products.Web.Attributes;
using System.Web.Mvc;
using System.Web.UI;

namespace Products.Web.Areas.Broadband.Controllers
{
    [BroadbandCheckSession]
    [RouteArea("Broadband", AreaPrefix = "broadband")]
    [OutputCache(NoStore = true, Duration = 0, Location = OutputCacheLocation.None)]
    public partial class BaseController : Controller
    {

    }
}
