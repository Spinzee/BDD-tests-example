using Products.Infrastructure.Logging;
using Products.Web.App_Start;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Products.Web.Attributes
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomHandleErrorAttribute"/> class.
        /// </summary>
        public CustomHandleErrorAttribute()
        {
            _logger = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<Log4NetLogger>();
        }

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            filterContext.ExceptionHandled = true;

            if (filterContext.Exception is HttpRequestValidationException)
            {
                return;
            }

            string controllerName = (string)filterContext.RouteData.Values["controller"];
            string actionName = (string)filterContext.RouteData.Values["action"];
            string area = (string)filterContext.RouteData.DataTokens["area"];

            _logger.Error($"Unhandled Exception, Area: {area}, Controller: {controllerName}, Action: {actionName}", filterContext.Exception);

            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Error" }, { "controller", "Error" }, { "area", "" } });

            base.OnException(filterContext);
        }
    }
}