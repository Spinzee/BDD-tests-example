using Products.Infrastructure;
using Products.Model.Constants;
using Products.Model.HomeServices;
using Products.WebModel.Resources.HomeServices;
using System;

namespace Products.Service.HomeServices
{
    public class StepCounterService : IStepCounterService
    {
        private readonly ISessionManager _sessionManager;

        public StepCounterService(ISessionManager sessionManager)
        {
            Guard.Against<ArgumentException>(sessionManager == null, $"{nameof(sessionManager)} is null");
            _sessionManager = sessionManager;
        }

        public string GetStepCounter(string pagename)
        {
            var homeServicesCustomer = _sessionManager.GetOrDefaultSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            var finalStepSegment = (pagename == "LandlordPostcode" || homeServicesCustomer.IsLandlord) ? 9 : 7;

            switch (pagename)
            {
                case "Postcode":
                    return string.Format(Common_Resources.StepCounter, 1, finalStepSegment);
                case "LandlordPostcode":
                    return string.Format(Common_Resources.StepCounter, 1, finalStepSegment); 
                case "CoverDetails":
                    return string.Format(Common_Resources.StepCounter, 2, finalStepSegment);
                case "PersonalDetails":
                    return string.Format(Common_Resources.StepCounter, 3, finalStepSegment);
                case "SelectAddress":
                    return string.Format(Common_Resources.StepCounter, 4, finalStepSegment);
                case "LandlordBillingPostcode":
                    return string.Format(Common_Resources.StepCounter, 5, finalStepSegment);
                case "SelectBillingAddress":
                    return string.Format(Common_Resources.StepCounter, 6, finalStepSegment);
                case "ContactDetails":
                    return string.Format(Common_Resources.StepCounter, homeServicesCustomer.IsLandlord ? 7 : 5, finalStepSegment);
                case "BankDetails":
                    return string.Format(Common_Resources.StepCounter, homeServicesCustomer.IsLandlord ? 8 : 6, finalStepSegment);
                case "Summary":
                    return string.Format(Common_Resources.StepCounter, homeServicesCustomer.IsLandlord ? 9 : 7, finalStepSegment);
            }

            return string.Empty;
        }
    }
}
