using Products.Model.Broadband;

namespace Products.Service.Broadband
{
    public interface IBroadbandJourneyService
    {
        void SetBroadbandJourneyDetails(BroadbandJourneyDetails journeyDetails);
        BroadbandJourneyDetails GetBroadbandJourneyDetails();
        void ClearBroadbandJourneyDetails();
    }
}