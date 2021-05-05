namespace Products.Service.Broadband
{
    using System;
    using System.Collections.Generic;
    using Infrastructure;
    using Mappers;
    using Model.Broadband;
    using WebModel.ViewModels.Broadband;
    using WebModel.ViewModels.Broadband.Extensions;

    public class ConfirmationService : IConfirmationService
    {
        private readonly IBroadbandJourneyService _broadbandJourneyService;

        public ConfirmationService(IBroadbandJourneyService broadbandJourneyService)
        {
            Guard.Against<ArgumentException>(broadbandJourneyService == null, $"{nameof(broadbandJourneyService)} is null");
            _broadbandJourneyService = broadbandJourneyService;
        }

        public ConfirmationViewModel ConfirmationViewModel(Dictionary<string, string> dataLayerDictionary)
        {
            Customer customer = _broadbandJourneyService.GetBroadbandJourneyDetails().Customer;
            Guard.Against<ArgumentException>(customer == null, $"{nameof(customer)} is null");
            _broadbandJourneyService.ClearBroadbandJourneyDetails();

            return new ConfirmationViewModel
            {
                // ReSharper disable once PossibleNullReferenceException
                ProductName = customer.SelectedProduct.BroadbandType.GetTitle(customer.SelectedProduct.GetSelectedTalkProduct(customer.SelectedProductCode).BroadbandProductGroup),
                IsSSECustomer = customer.IsSSECustomer,
                DataLayerDictionary = dataLayerDictionary ?? new Dictionary<string, string>()
            };
        }
    }
}