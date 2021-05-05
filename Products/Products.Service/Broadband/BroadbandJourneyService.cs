using Products.Infrastructure;
using Products.Model.Broadband;
using Products.Model.Constants;

namespace Products.Service.Broadband
{
    public class BroadbandJourneyService : IBroadbandJourneyService
    {
        private readonly ISessionManager _sessionManager;

        public BroadbandJourneyService(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public void SetBroadbandJourneyDetails(BroadbandJourneyDetails journeyDetails)
        {
            _sessionManager.SetSessionDetails(SessionKeys.BroadbandJourney, journeyDetails);
        }

        public BroadbandJourneyDetails GetBroadbandJourneyDetails()
        {
            return _sessionManager.GetSessionDetails<BroadbandJourneyDetails>(SessionKeys.BroadbandJourney) ??
                   new BroadbandJourneyDetails();
        }

        public void ClearBroadbandJourneyDetails()
        {
            _sessionManager.ClearSession();
        }
    }
}