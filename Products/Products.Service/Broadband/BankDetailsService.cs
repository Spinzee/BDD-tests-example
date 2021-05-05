using System;
using Products.Infrastructure;
using Products.Model.Common;
using Products.Service.Common;
using Products.WebModel.Resources.Broadband;
using Products.WebModel.ViewModels.Broadband;
using Products.WebModel.ViewModels.Common;

namespace Products.Service.Broadband
{
    using Core;
    using Model.Broadband;

    public class BankDetailsService : IBankDetailsService
    {
        private readonly ISessionManager _sessionManager;
        private readonly IBroadbandJourneyService _broadbandJourneyService;
        private readonly IBankValidationService _bankValidationService;

        public BankDetailsService(ISessionManager sessionManager, IBroadbandJourneyService broadbandJourneyService, IBankValidationService bankValidationService)
        {
            _sessionManager = sessionManager;
            _broadbandJourneyService = broadbandJourneyService;
            _bankValidationService = bankValidationService;
        }

        public BankDetailsViewModel GetBankDetailsViewModel()
        {
            return new BankDetailsViewModel
            {
                YourPriceViewModel = _sessionManager.GetSessionDetails<YourPriceViewModel>("yourPriceDetails"),
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = "ContactDetails",
                    ControllerName = "CustomerDetails",
                    TitleAttributeText = Common_Resources.BackButtonAlt
                }
            };
        }

        public BankDetailsViewModel SetBankDetailsViewModel(BankDetailsViewModel viewModel)
        {
            Guard.Against<ArgumentException>(viewModel == null, $"{nameof(viewModel)} is null");
            BroadbandJourneyDetails broadbandJourneyDetails = _broadbandJourneyService.GetBroadbandJourneyDetails();
            Guard.Against<ArgumentException>(broadbandJourneyDetails.Customer == null, "session object is null");

            // ReSharper disable once PossibleNullReferenceException
            viewModel.YourPriceViewModel = _sessionManager.GetSessionDetails<YourPriceViewModel>("yourPriceDetails");
            viewModel.BackChevronViewModel = new BackChevronViewModel
            {
                ActionName = "ContactDetails",
                ControllerName = "CustomerDetails",
                TitleAttributeText = Common_Resources.BackButtonAlt
            };
            
            // ReSharper disable once PossibleNullReferenceException
            if (broadbandJourneyDetails.Customer.BankServiceRetryCount >= 3)
                return null;

            BankDetails bankDetails = _bankValidationService.GetBankDetails(NumberFormatter.ToDigitsOnly(viewModel.SortCode), viewModel.AccountNumber);

            if (bankDetails == null)
            {
                broadbandJourneyDetails.Customer.BankServiceRetryCount++;
                _broadbandJourneyService.SetBroadbandJourneyDetails(broadbandJourneyDetails);
                viewModel.IsRetry = true;
                return viewModel;
            }

            broadbandJourneyDetails.Customer.DirectDebitDetails =
                new DirectDebitDetails
                {
                    AccountNumber = viewModel.AccountNumber,
                    SortCode = viewModel.SortCode,
                    AccountName = viewModel.AccountHolder,
                    BankName = bankDetails.BankName,
                    BankAddressLine1 = bankDetails.BankAddress.BankAddressLine1Field,
                    BankAddressLine2 = bankDetails.BankAddress.BankAddressLine2Field,
                    BankAddressLine3 = bankDetails.BankAddress.BankAddressLine3Field,
                    Postcode = bankDetails.BankAddress.BankPostcodeField
                };
            broadbandJourneyDetails.Customer.BankServiceRetryCount = 0;

            _broadbandJourneyService.SetBroadbandJourneyDetails(broadbandJourneyDetails);

            viewModel.IsRetry = false;
            return viewModel;
        }
    }
}