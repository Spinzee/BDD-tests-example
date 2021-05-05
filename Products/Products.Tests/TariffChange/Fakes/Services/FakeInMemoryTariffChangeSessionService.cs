namespace Products.Tests.TariffChange.Fakes.Services
{
    using Model.TariffChange;
    using Service.TariffChange;

    public class FakeInMemoryTariffChangeSessionService : ITariffChangeSessionService
    {
        public FakeInMemoryTariffChangeSessionService()
        {
            JourneyDetails = new JourneyDetails();
        }

        public FakeInMemoryTariffChangeSessionService(JourneyDetails journeyDetails)
        {
            JourneyDetails = journeyDetails;
        }

        public JourneyDetails JourneyDetails { get; private set; }

        public void SetJourneyDetails(JourneyDetails value)
        {
            JourneyDetails = value;
        }

        public JourneyDetails GetJourneyDetails()
        {
            return JourneyDetails;
        }

        public void RemoveJourneyDetails()
        {
            JourneyDetails = null;
        }
    }
}