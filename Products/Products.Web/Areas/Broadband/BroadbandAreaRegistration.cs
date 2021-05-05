namespace Products.Web.Areas.Broadband
{
    using System.Web.Mvc;

    // ReSharper disable once UnusedMember.Global
    public class BroadbandAreaRegistration : AreaRegistration 
    {
        public override string AreaName => "Broadband";

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Broadband_default",
                "Broadband/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}