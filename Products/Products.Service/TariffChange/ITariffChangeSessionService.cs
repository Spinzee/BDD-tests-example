using Products.Model.TariffChange;

namespace Products.Service.TariffChange
{
    public interface ITariffChangeSessionService
    {
        void SetJourneyDetails(JourneyDetails value);

        JourneyDetails GetJourneyDetails();

        void RemoveJourneyDetails();
    }
}