namespace Products.Web.Areas.Energy
{
    using System.Web.Mvc;

    // ReSharper disable once UnusedMember.Global
    public class EnergyAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Energy";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Energy_default",
                "Energy/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}