using Products.Model.Constants;
using Products.Model.HomeServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Products.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HomeServicesRouteCheckAttribute : ActionFilterAttribute
    {
        private readonly string HomeServicesLinkBackToHubURL = ConfigurationManager.AppSettings["HomeServicesHubUrl"];
        private RouteValidationStrategy _routeValidationStrategy;

        private readonly string[] _noSessionCheckRoutes =
        {
            "/home-services-signup/enter-postcode",
            "/home-services-signup/enter-cover-postcode",
            "/home-services-signup/area-not-covered",
            "/home-services-signup/unable-to-complete",
            "/home-services-signup/unavailable",
            "/home-services-signup/postcode-not-covered"
        };

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContextBase context = filterContext.HttpContext;
            string requestPath = context.Request.Path.ToLower();

            var homeServicesCustomer = (HomeServicesCustomer)context.Session[SessionKeys.HomeServicesCustomer];

            if (homeServicesCustomer == null)
            {
                if (!_noSessionCheckRoutes.Contains(requestPath))
                {
                    ClearSessionAndReturnToHome(filterContext);
                    return;
                }
            }

            _routeValidationStrategy = new RouteValidationStrategy(homeServicesCustomer, context);

            var actionName = filterContext.ActionDescriptor.ActionName;

            if (!_routeValidationStrategy.RouteIsValid(actionName))
            {
                ClearSessionAndReturnToHome(filterContext);
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        private void ClearSessionAndReturnToHome(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Session.Clear();
            filterContext.HttpContext.Session.Abandon();
            filterContext.Result = new RedirectResult(HomeServicesLinkBackToHubURL);
        }
    }

    public class RouteValidationStrategy
    {
        private readonly List<RouteValidRule> _routeValidationRules = new List<RouteValidRule>();
        public RouteValidationStrategy(HomeServicesCustomer homeServicesCustomer, HttpContextBase context)
        {
            _routeValidationRules.Add(new PostcodeRouteValidRule("Postcode", context, homeServicesCustomer));
            _routeValidationRules.Add(new PostcodeRouteValidRule("LandlordPostcode", context, homeServicesCustomer));
            _routeValidationRules.Add(new CoverDetailsRouteValidRule("CoverDetails", homeServicesCustomer));
            _routeValidationRules.Add(new PersonalDetailsRouteValidRule("PersonalDetails", homeServicesCustomer));
            _routeValidationRules.Add(new EnterBillingPostcodeRouteValid("LandlordBillingPostcode", homeServicesCustomer));
            _routeValidationRules.Add(new SelectAddressDetailsRouteValidRule("SelectAddress", homeServicesCustomer));
            _routeValidationRules.Add(new SelectBillingAddressRouteValidRule("SelectBillingAddress", homeServicesCustomer));
            _routeValidationRules.Add(new ContactDetailsRouteValidRule("ContactDetails", homeServicesCustomer));
            _routeValidationRules.Add(new BankDetailsRouteValidRule("BankDetails", homeServicesCustomer));
            _routeValidationRules.Add(new OrderSummaryRouteValidRule("Summary", homeServicesCustomer));
            _routeValidationRules.Add(new ConfirmationRouteValidRule("Confirmation", homeServicesCustomer));
        }

        public bool RouteIsValid(string actionName)
        {
            var routeRule = _routeValidationRules.FirstOrDefault(r => r.ActionName == actionName);

            return routeRule == null || routeRule.IsValid();
        }
    }

    internal class ConfirmationRouteValidRule : RouteValidRule
    {
        private HomeServicesCustomer _homeServicesCustomer;

        public ConfirmationRouteValidRule(string actionName, HomeServicesCustomer homeServicesCustomer) : base(actionName)
        {
            _homeServicesCustomer = homeServicesCustomer;
        }

        public override bool IsValid()
        {
            return _homeServicesCustomer != null && _homeServicesCustomer.ApplicationIds != null && _homeServicesCustomer.ApplicationIds.Any();
        }
    }

    internal class BankDetailsRouteValidRule : RouteValidRule
    {
        private HomeServicesCustomer _homeServicesCustomer;

        public BankDetailsRouteValidRule(string actionName, HomeServicesCustomer homeServicesCustomer) : base(actionName)
        {
            _homeServicesCustomer = homeServicesCustomer;
        }

        public override bool IsValid()
        {
            return _homeServicesCustomer != null && _homeServicesCustomer.ContactDetails != null;
        }
    }

    internal class EnterBillingPostcodeRouteValid : RouteValidRule
    {
        private HomeServicesCustomer _homeServicesCustomer;

        public EnterBillingPostcodeRouteValid(string actionName, HomeServicesCustomer homeServicesCustomer) : base(actionName)
        {
            _homeServicesCustomer = homeServicesCustomer;
        }

        public override bool IsValid()
        {
            return _homeServicesCustomer != null && _homeServicesCustomer.SelectedCoverAddress != null;
        }
    }

    internal class PostcodeRouteValidRule : RouteValidRule
    {
        private HttpContextBase _context;
        private HomeServicesCustomer _homeServicesCustomer;

        public PostcodeRouteValidRule(string actionName, HttpContextBase context, HomeServicesCustomer homeServicesCustomer) : base(actionName)
        {
            _context = context;
            _homeServicesCustomer = homeServicesCustomer;
        }

        public override bool IsValid()
        {
            var productCode = _context.Request.QueryString["productcode"];
            var urlRefererProductCode = _context?.Request?.UrlReferrer?.Query;

            var postHadProductCode = !string.IsNullOrEmpty(urlRefererProductCode) && urlRefererProductCode.ToLower().Contains("productcode");

            var isBillingPostCode = _homeServicesCustomer != null
                                    && _homeServicesCustomer.IsLandlord
                                    && !string.IsNullOrEmpty(_homeServicesCustomer.SelectedProductCode)
                                    && _homeServicesCustomer.SelectedCoverAddress != null;

            return !string.IsNullOrEmpty(productCode) || postHadProductCode == true || isBillingPostCode == true;
        }
    }

    internal class OrderSummaryRouteValidRule : RouteValidRule
    {
        private HomeServicesCustomer _homeServicesCustomer;

        public OrderSummaryRouteValidRule(string actionName, HomeServicesCustomer homeServicesCustomer) : base(actionName)
        {
            _homeServicesCustomer = homeServicesCustomer;
        }

        public override bool IsValid()
        {
            return _homeServicesCustomer != null && _homeServicesCustomer.DirectDebitDetails != null;
        }
    }

    internal class ContactDetailsRouteValidRule : RouteValidRule
    {
        private HomeServicesCustomer _homeServicesCustomer;

        public ContactDetailsRouteValidRule(string actionName, HomeServicesCustomer homeServicesCustomer) : base(actionName)
        {
            _homeServicesCustomer = homeServicesCustomer;
        }

        public override bool IsValid()
        {
            if (_homeServicesCustomer.IsLandlord)
            {
                return _homeServicesCustomer.SelectedBillingAddress != null && _homeServicesCustomer.SelectedCoverAddress != null;
            }
            else
            {
                return _homeServicesCustomer.SelectedCoverAddress != null;
            }
        }
    }

    internal class SelectBillingAddressRouteValidRule : RouteValidRule
    {
        private HomeServicesCustomer _homeServicesCustomer;
        public SelectBillingAddressRouteValidRule(string actionName, HomeServicesCustomer homeServicesCustomer) : base(actionName)
        {
            _homeServicesCustomer = homeServicesCustomer;
        }

        public override bool IsValid()
        {
            return _homeServicesCustomer != null && _homeServicesCustomer.IsLandlord == true
                && _homeServicesCustomer.SelectedCoverAddress != null
                && !string.IsNullOrEmpty(_homeServicesCustomer.BillingPostcode);
        }
    }

    internal class SelectAddressDetailsRouteValidRule : RouteValidRule
    {
        private HomeServicesCustomer _homeServicesCustomer;

        public SelectAddressDetailsRouteValidRule(string actionName, HomeServicesCustomer homeServicesCustomer) : base(actionName)
        {
            _homeServicesCustomer = homeServicesCustomer;
        }

        public override bool IsValid()
        {
            return _homeServicesCustomer != null && _homeServicesCustomer.PersonalDetails != null;
        }
    }

    internal class CoverDetailsRouteValidRule : RouteValidRule
    {
        private readonly HomeServicesCustomer _homeServicesCustomer;

        public CoverDetailsRouteValidRule(string actionName, HomeServicesCustomer homeServicesCustomer) : base(actionName)
        {
            _homeServicesCustomer = homeServicesCustomer;
        }

        public override bool IsValid()
        {
            return _homeServicesCustomer != null && !string.IsNullOrEmpty(_homeServicesCustomer.CoverPostcode);
        }
    }

    internal class PersonalDetailsRouteValidRule : RouteValidRule
    {
        private readonly HomeServicesCustomer _homeServicesCustomer;

        public PersonalDetailsRouteValidRule(string actionName, HomeServicesCustomer homeServicesCustomer) : base(actionName)
        {
            _homeServicesCustomer = homeServicesCustomer;
        }

        public override bool IsValid()
        {
            return _homeServicesCustomer != null && !string.IsNullOrEmpty(_homeServicesCustomer.SelectedProductCode);
        }
    }

    internal class SelectAddressDetailsValidRule : RouteValidRule
    {
        private readonly HomeServicesCustomer _homeServicesCustomer;


        public SelectAddressDetailsValidRule(string actionName, HomeServicesCustomer homeServicesCustomer) : base(actionName)
        {
            _homeServicesCustomer = homeServicesCustomer;
        }

        public override bool IsValid()
        {
            return _homeServicesCustomer != null && !string.IsNullOrEmpty(_homeServicesCustomer.SelectedProductCode);
        }
    }

    internal abstract class RouteValidRule
    {

        public RouteValidRule(string actionName)
        {
            ActionName = actionName;
        }

        public string ActionName
        {
            get;
        }


        public abstract bool IsValid();
    }
}