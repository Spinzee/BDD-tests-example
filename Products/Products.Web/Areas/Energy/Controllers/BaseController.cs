namespace Products.Web.Areas.Energy.Controllers
{
    using System.Web.Mvc;

    [RouteArea("Energy", AreaPrefix = "energy-signup")]
    [OutputCache(NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
    public class BaseController : Controller
    {        
    }
}