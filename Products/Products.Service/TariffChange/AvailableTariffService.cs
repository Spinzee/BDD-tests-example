using Products.Infrastructure;
using Products.WebModel.ViewModels.TariffChange;
using System;
using System.Collections.Generic;

namespace Products.Service.TariffChange
{
    public class AvailableTariffService : IAvailableTariffService
    {
        private const string AvailableTariffsKey = "TariffChangeAvailableTariffsSessionKey";
        private readonly ISessionManager _sessionManager;

        public AvailableTariffService(ISessionManager sessionManager)
        {
            Guard.Against<ArgumentNullException>(sessionManager == null, "sessionManager is null");
            _sessionManager = sessionManager;
        }

        public void ClearAvailableTariffs()
        {
            _sessionManager.RemoveSessionDetails(AvailableTariffsKey);
        }

        public List<AvailableTariff> GetAvailableTariffs()
        {
            return _sessionManager.GetSessionDetails<List<AvailableTariff>>(AvailableTariffsKey);
        }

        public void SetAvailableTariffs(List<AvailableTariff> availableTariffs)
        {
            _sessionManager.SetSessionDetails(AvailableTariffsKey, availableTariffs);
        }
    }
}
