namespace Products.Web.Areas.TariffChange
{
    using System.Web.Mvc;

    // ReSharper disable once UnusedMember.Global
    public class TariffChangeAreaRegistration : AreaRegistration 
    {
        public override string AreaName => "TariffChange";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //    #region IdentifyCustomerController

            //    context.MapRoute(
            //        "identify",
            //        "tariff-change/identify",
            //        new { action = "IdentifyCustomer", controller = "CustomerIdentification" }
            //    );

            //    context.MapRoute(
            //        "confirmdetails",
            //        "tariff-change/confirm-details",
            //        new { action = "ConfirmDetails", controller = "CustomerIdentification" }
            //    );

            //    context.MapRoute(
            //        "accountnotfound",
            //        "tariff-change/account-not-found",
            //        new { action = "AccountDetailsNotMatched", controller = "CustomerIdentification" }
            //    );

            //    context.MapRoute(
            //        "ctcunavailable",
            //        "tariff-change/unavailable",
            //        new { action = "CtcUnavailable", controller = "CustomerIdentification" }
            //    );

            //    #endregion

            //    #region AdditionalDetailsController

            //    context.MapRoute(
            //        "getcustomeremail",
            //        "tariff-change/customer-email",
            //        new { action = "GetCustomerEmail", controller = "AdditionalDetails" }
            //    );

            //    context.MapRoute(
            //        "checkprofile",
            //        "tariff-change/customer-email",
            //        new { action = "CheckProfileExists", controller = "AdditionalDetails" }
            //    );

            //    #endregion

            context.MapRoute(
                "TariffChange_default",
                "TariffChange/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}