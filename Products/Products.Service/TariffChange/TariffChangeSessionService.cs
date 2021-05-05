using Products.Infrastructure;
using Products.Model.TariffChange;
using System;

namespace Products.Service.TariffChange
{
    public class TariffChangeSessionService : ITariffChangeSessionService
    {
        private readonly ISessionManager _sessionManager;
        private const string JourneyDetailsKey = "TariffChangeJourneyDetailsSessionKey";

        public TariffChangeSessionService(ISessionManager sessionManager)
        {
            Guard.Against<ArgumentNullException>(sessionManager == null, "sessionManager is null");
            _sessionManager = sessionManager;
        }

        public void SetJourneyDetails(JourneyDetails value)
        {
            _sessionManager.SetSessionDetails(JourneyDetailsKey, value);
        }

        public JourneyDetails GetJourneyDetails()
        {
            return _sessionManager.GetSessionDetails<JourneyDetails>(JourneyDetailsKey) ?? new JourneyDetails();
        }

        public void RemoveJourneyDetails()
        {
            _sessionManager.RemoveSessionDetails(JourneyDetailsKey);
        }
    }
}