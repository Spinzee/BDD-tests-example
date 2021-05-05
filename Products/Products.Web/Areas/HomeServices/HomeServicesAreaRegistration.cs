namespace Products.Web.Areas.HomeServices
{
    using System.Web.Mvc;

    // ReSharper disable once UnusedMember.Global
    public class HomeServicesAreaRegistration : AreaRegistration 
    {
        public override string AreaName => "HomeServices";

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "HomeServices_default",
                "HomeServices/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}