namespace Products.Web
{
    using System;
    using System.Net;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using App_Start;
    using Service.Common;

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            MvcHandler.DisableMvcResponseHeader = true;
            PreSendRequestHeaders += Application_PreSendRequestHeaders;
            AntiForgeryConfig.SuppressIdentityHeuristicChecks = true; // This is to fix anti-forgery token validation issue, where a token is meant to be for a different user.
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        private void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Headers.Remove("Server");
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            IAuthenticationService httpAuthenticationService = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<HttpAuthenticationService>();
            httpAuthenticationService.CreateGenericPrincipalWithRole();
        }
    }
}
