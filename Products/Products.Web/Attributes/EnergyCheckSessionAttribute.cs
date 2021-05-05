namespace Products.Web.Attributes
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Core;
    using Model.Constants;
    using Model.Energy;
    using Model.Enums;
    using WebModel.ViewModels.Energy;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EnergyCheckSessionAttribute : ActionFilterAttribute
    {
        private readonly string _energyLinkBackToHubURL = ConfigurationManager.AppSettings["EnergyLinkBackToHubURL"];
        private readonly string[] _noSessionCheckRoutes = 
        {
            "/energy-signup/existing-customer",
            "/energy-signup/enter-postcode",
            "/energy-signup/area-not-covered",
            "/energy-signup/unavailable"
        };

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContextBase context = filterContext.HttpContext;
            string requestPath = context.Request.Path.ToLower();

            var energyCustomer = (EnergyCustomer)context.Session[SessionKeys.EnergyCustomer];

            if (energyCustomer == null)
            {
                if (!_noSessionCheckRoutes.Contains(requestPath))
                {
                    ClearSessionAndReturnToHome(filterContext);
                    return;
                }
            }
            else
            {                
                bool isCAndCJourney = energyCustomer.IsCAndCJourney();

                YourPriceViewModel yourPrice;
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (requestPath)
                {
                    case "/energy-signup/select-address":
                        if (string.IsNullOrEmpty(energyCustomer.Postcode))
                        {
                            ClearSessionAndReturnToHome(filterContext);
                            return;
                        }
                        break;
                    case "/energy-signup/select-fuel":
                        if (energyCustomer.SelectedAddress == null)
                        {
                            ClearSessionAndReturnToHome(filterContext);
                            return;
                        }
                        break;
                    case "/energy-signup/payment-method":
                        if (isCAndCJourney && energyCustomer.IsMeterDetailsPayGo() && !energyCustomer.IsSmartMeterSmets2() || energyCustomer.SelectedFuelType == FuelType.None)
                        {
                            ClearSessionAndReturnToHome(filterContext);
                            return;
                        }
                        break;
                    case "/energy-signup/meter-type":
                        if (isCAndCJourney || energyCustomer.SelectedPaymentMethod == PaymentMethod.None || energyCustomer.SelectedFuelType == FuelType.Gas || energyCustomer.SelectedBillingPreference == BillingPreference.None)
                        {
                            ClearSessionAndReturnToHome(filterContext);
                            return;
                        }
                        break;
                    case "/energy-signup/smart-meter":
                        if (isCAndCJourney || energyCustomer.SelectedFuelType != FuelType.Gas && energyCustomer.SelectedElectricityMeterType == ElectricityMeterType.None ||
                            energyCustomer.SelectedFuelType == FuelType.Gas && energyCustomer.SelectedPaymentMethod == PaymentMethod.None)
                        {
                            ClearSessionAndReturnToHome(filterContext);
                            return;
                        }
                        break;
                    case "/energy-signup/smart-meter-frequency":
                        if (!isCAndCJourney || energyCustomer.SelectedPaymentMethod == PaymentMethod.None || !energyCustomer.IsSmartMeter())
                        {
                            ClearSessionAndReturnToHome(filterContext);
                            return;
                        }
                        break;

                    case "/energy-signup/energy-usage":
                        if (!isCAndCJourney && energyCustomer.HasSmartMeter == null ||
                            isCAndCJourney && energyCustomer.SelectedPaymentMethod == PaymentMethod.None ||
                            isCAndCJourney && energyCustomer.IsSmartMeter() && energyCustomer.AskSmartMeterFrequency() && energyCustomer.SmartMeterFrequency == SmartMeterFrequency.None)
                        {
                            ClearSessionAndReturnToHome(filterContext);
                            return;
                        }
                        break;
                    case "/energy-signup/quote-details":
                        if (energyCustomer.Projection == null)
                        {
                            ClearSessionAndReturnToHome(filterContext);
                            return;
                        }
                        break;
                    case "/energy-signup/extras":
                        yourPrice = (YourPriceViewModel)context.Session[SessionKeys.EnergyYourPriceDetails];
                        if (energyCustomer.SelectedTariff == null || yourPrice == null || !energyCustomer.SelectedTariff.IsHesBundle())
                        {
                            ClearSessionAndReturnToHome(filterContext);
                            return;
                        }
                        break;
                    case "/energy-signup/confirm-address":
                        yourPrice = (YourPriceViewModel)context.Session[SessionKeys.EnergyYourPriceDetails];
                        if (energyCustomer.SelectedTariff?.BundlePackage == null || yourPrice == null)
                        {
                            ClearSessionAndReturnToHome(filterContext);
                            return;
                        }
                        break;
                    case "/energy-signup/phone-package":
                        yourPrice = (YourPriceViewModel)context.Session[SessionKeys.EnergyYourPriceDetails];
                        if (energyCustomer.SelectedTariff?.BundlePackage == null || yourPrice == null || energyCustomer.SelectedBTAddress == null)
                        {
                            ClearSessionAndReturnToHome(filterContext);
                            return;
                        }
                        break;

                    case "/energy-signup/upgrades":
                        yourPrice = (YourPriceViewModel)context.Session[SessionKeys.EnergyYourPriceDetails];
                        if (energyCustomer.SelectedTariff?.BundlePackage == null || yourPrice == null)
                        {
                            ClearSessionAndReturnToHome(filterContext);
                            return;
                        }
                        else if  (energyCustomer.SelectedTariff.IsBroadbandBundle() && energyCustomer.SelectedBTAddress == null)
                        {
                            ClearSessionAndReturnToHome(filterContext);
                            return;
                        }
                        break;
                    case "/energy-signup/personal-details":
                        yourPrice = (YourPriceViewModel)context.Session[SessionKeys.EnergyYourPriceDetails];
                        if (energyCustomer.SelectedTariff == null || yourPrice == null)
                        {
                            ClearSessionAndReturnToHome(filterContext);
                            return;
                        }
                        break;

                    case "/energy-signup/contact-details":
                        yourPrice = (YourPriceViewModel)context.Session[SessionKeys.EnergyYourPriceDetails];
                        if (energyCustomer.PersonalDetails == null || yourPrice == null)
                        {
                            ClearSessionAndReturnToHome(filterContext);
                            return;
                        }
                        break;
                    case "/energy-signup/online-account":
                        yourPrice = (YourPriceViewModel)context.Session[SessionKeys.EnergyYourPriceDetails];
                        if (energyCustomer.ContactDetails == null || energyCustomer.ProfileExists == true || yourPrice == null)
                        {
                            ClearSessionAndReturnToHome(filterContext);
                            return;
                        }
                        break;

                    case "/energy-signup/bank-details":
                        yourPrice = (YourPriceViewModel)context.Session[SessionKeys.EnergyYourPriceDetails];
                        if (energyCustomer.ContactDetails == null || yourPrice == null || energyCustomer.SelectedPaymentMethod != PaymentMethod.MonthlyDirectDebit)
                        {
                            ClearSessionAndReturnToHome(filterContext);
                            return;
                        }
                        break;

                    case "/energy-signup/order-summary":
                    case "/energy-signup/print-mandate":
                        yourPrice = (YourPriceViewModel)context.Session[SessionKeys.EnergyYourPriceDetails];
                        if (
                            energyCustomer.ContactDetails == null
                            || energyCustomer.SelectedPaymentMethod == PaymentMethod.MonthlyDirectDebit && energyCustomer.DirectDebitDetails == null
                            || yourPrice == null)
                        {
                            ClearSessionAndReturnToHome(filterContext);
                            return;
                        }
                        break;

                    case "/energy-signup/confirmation":
                        yourPrice = (YourPriceViewModel)context.Session[SessionKeys.EnergyYourPriceDetails];
                        if (energyCustomer.EnergyApplicationId == 0 || yourPrice == null)
                        {
                            ClearSessionAndReturnToHome(filterContext);
                            return;
                        }
                        break;

                    case "/energy/common/yourpricedetails":
                        yourPrice = (YourPriceViewModel)context.Session[SessionKeys.EnergyYourPriceDetails];
                        if (energyCustomer.SelectedTariff == null || yourPrice == null)
                        {
                            ClearSessionAndReturnToHome(filterContext);
                            return;
                        }
                        break;
                }
            }

            base.OnActionExecuting(filterContext);
        }

        private void ClearSessionAndReturnToHome(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Session.Clear();
            filterContext.HttpContext.Session.Abandon();
            filterContext.Result = new RedirectResult(_energyLinkBackToHubURL);
        }
    }
}