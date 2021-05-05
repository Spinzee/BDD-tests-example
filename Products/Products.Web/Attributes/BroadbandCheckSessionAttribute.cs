using Products.Model.Broadband;
using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Products.Model.Constants;
using Products.WebModel.ViewModels.Broadband;

namespace Products.Web.Attributes
{
    /// <summary>
    /// Session ExpireFilter Attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class BroadbandCheckSessionAttribute : ActionFilterAttribute
    {
        private readonly string LinkBackToHubURL = ConfigurationManager.AppSettings["BroadbandLinkBackToHubURL"];

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContextBase context = filterContext.HttpContext;
            var requestPath = context.Request.Path.ToLower();

            var broadbandJourneyDetails = (BroadbandJourneyDetails)context.Session["broadband_journey"];

            switch (requestPath)
            {
                case "/broadband/select-address":
                    if (string.IsNullOrEmpty(broadbandJourneyDetails?.Customer?.PostcodeEntered))
                    {
                        ClearSessionAndReturnToHome(filterContext);
                        return;
                    }
                    break;
                case "/broadband/available-packages":
                case "/broadband/selected-package":
                    if (broadbandJourneyDetails?.Customer?.SelectedAddress == null)
                    {
                        ClearSessionAndReturnToHome(filterContext);
                        return;
                    }
                    break;
                case "/broadband/transfer-your-number":
                    var yourPrice = (YourPriceViewModel)context.Session[SessionKeys.YourPriceDetails];
                    if (yourPrice == null)
                    {
                        ClearSessionAndReturnToHome(filterContext);
                        return;
                    }
                    break;
                case "/broadband/personal-details":
                    if (!(broadbandJourneyDetails?.Customer?.TransferYourNumberIsSet ?? false))
                    {
                        ClearSessionAndReturnToHome(filterContext);
                        return;
                    }
                    break;
                case "/broadband/contact-details":
                    if (broadbandJourneyDetails?.Customer?.PersonalDetails == null)
                    {
                        ClearSessionAndReturnToHome(filterContext);
                        return;
                    }
                    break;
                case "/broadband/online-account":
                    if (broadbandJourneyDetails?.Customer?.ContactDetails == null) 
                    {
                        ClearSessionAndReturnToHome(filterContext);
                        return;
                    }
                    break;
                case "/broadband/bank-details":
                    if (broadbandJourneyDetails?.Customer?.ContactDetails == null)
                    {
                        ClearSessionAndReturnToHome(filterContext);
                        return;
                    }
                    break;
                case "/broadband/summary":
                    if (broadbandJourneyDetails?.Customer?.DirectDebitDetails == null)
                    {
                        ClearSessionAndReturnToHome(filterContext);
                        return;
                    }
                    break;
                case "/broadband/confirmation":
                    if (broadbandJourneyDetails?.Customer?.ApplicationId == null)
                    {
                        ClearSessionAndReturnToHome(filterContext);
                        return;
                    }
                    break;
                case "/broadband/printmandate":
                    if (broadbandJourneyDetails?.Customer?.DirectDebitDetails == null)
                    {
                        ClearSessionAndReturnToHome(filterContext);
                        return;
                    }
                    break;
                case "/broadband/unable-to-complete":
                    if (broadbandJourneyDetails?.Customer == null)
                    {
                        ClearSessionAndReturnToHome(filterContext);
                        return;
                    }
                    break;
            }

            base.OnActionExecuting(filterContext);
        }

        private void ClearSessionAndReturnToHome(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Session.Clear();
            filterContext.HttpContext.Session.Abandon();
            filterContext.Result = new RedirectResult(LinkBackToHubURL);
        }
    }
}