namespace Products.Service.Broadband
{
    using System;
    using Common.Mappers;
    using Infrastructure;
    using Infrastructure.Logging;
    using Managers;
    using Model.Broadband;
    using Model.Common;
    using Model.Constants;
    using Model.Enums;
    using WebModel.Resources.Broadband;
    using WebModel.ViewModels.Broadband;
    using WebModel.ViewModels.Common;

    public class SummaryService : ISummaryService
    {
        private readonly IBroadbandJourneyService _broadbandJourneyService;
        private readonly IBroadbandManager _broadbandManager;
        private readonly ILogger _logger;
        private readonly ISessionManager _sessionManager;

        public SummaryService(
            IBroadbandJourneyService broadbandJourneyService,
            ILogger logger,
            ISessionManager sessionManager,
            IBroadbandManager broadbandManager)
        {
            Guard.Against<ArgumentException>(broadbandJourneyService == null, $"{nameof(broadbandJourneyService)} is null");
            Guard.Against<ArgumentException>(logger == null, $"{nameof(logger)} is null");
            Guard.Against<ArgumentException>(sessionManager == null, $"{nameof(sessionManager)} is null");
            Guard.Against<ArgumentException>(broadbandManager == null, $"{nameof(broadbandManager)} is null");
            _broadbandJourneyService = broadbandJourneyService;
            _logger = logger;
            _sessionManager = sessionManager;
            _broadbandManager = broadbandManager;
        }

        public SummaryViewModel GetSummaryViewModel()
        {
            Customer customer = _broadbandJourneyService.GetBroadbandJourneyDetails().Customer;
            Guard.Against<ArgumentException>(customer == null, $"{nameof(customer)} is null");

            try
            {
                var yourPriceModel = _sessionManager.GetSessionDetails<YourPriceViewModel>(SessionKeys.YourPriceDetails);
                // ReSharper disable once PossibleNullReferenceException
                double installationFee = _broadbandManager.GetInstallationFee();
                double equipmentCharge = _broadbandManager.GetEquipmentChargeFee();

                var model = new SummaryViewModel
                {
                    Name = $"{customer.PersonalDetails.Title} {customer.PersonalDetails.FirstName} {customer.PersonalDetails.LastName}",
                    DateOfBirth = customer.PersonalDetails.DateOfBirth,
                    Email = customer.ContactDetails.EmailAddress,
                    Address = customer.SelectedAddress.FormattedAddress,
                    PostCode = customer.SelectedAddress.Postcode,
                    AccountName = customer.DirectDebitDetails.AccountName,
                    AccountNumber = customer.DirectDebitDetails.AccountNumber,
                    SortCode = customer.DirectDebitDetails.SortCode,
                    HasCliNumber = !string.IsNullOrWhiteSpace(customer.CliNumber),
                    YourPriceViewModel = yourPriceModel,
                    BackChevronViewModel = new BackChevronViewModel
                    {
                        ActionName = "BankDetails",
                        ControllerName = "BankDetails",
                        TitleAttributeText = Common_Resources.BackButtonAlt
                    },
                    InstallationParagraph = string.Format(Summary_Resources.InstallationParagraph, installationFee),
                    TermsAndConditionsPdfLinks = _broadbandManager.GetTermsAndConditionPdfWithLinks(customer.SelectedProductGroup),
                    BankDetailsModal =
                        new ConfirmationModalViewModel
                        {
                            ModalId = "BankDetailsModal",
                            FirstMessage = Summary_Resources.BankDetailsModalMessage,
                            RedirectUrl = "/broadband/bank-details",
                            CloseButtonAlt = Common_Resources.ModalCloseButtonAlt,
                            CloseButtonLabel = Common_Resources.ModalCloseButtonText,
                            CancelButtonText = Common_Resources.ModalCancelButtonText,
                            CancelButtonTextAlt = Common_Resources.ModalCancelButtonAlt,
                            ButtonText = Summary_Resources.BankDetailsModalButtonText,
                            ButtonTextAlt = Summary_Resources.BankDetailsModalButtonAlt
                        },
                    PersonalDetailsModal =
                        new ConfirmationModalViewModel
                        {
                            ModalId = "PersonalDetailsModal",
                            FirstMessage = Summary_Resources.PersonalDetailsModalMessage,
                            RedirectUrl = "/broadband/personal-details",
                            CloseButtonAlt = Common_Resources.ModalCloseButtonAlt,
                            CloseButtonLabel = Common_Resources.ModalCloseButtonText,
                            CancelButtonText = Common_Resources.ModalCancelButtonText,
                            CancelButtonTextAlt = Common_Resources.ModalCancelButtonAlt,
                            ButtonText = Summary_Resources.PersonalDetailsModalButtonText,
                            ButtonTextAlt = Summary_Resources.PersonalDetailsModalButtonAlt
                        },
                    PackageDetailsModal = new ConfirmationModalViewModel
                    {
                        ModalId = "PackageDetailsModal",
                        FirstMessage = Summary_Resources.PackageDetailsModalMessage,
                        RedirectUrl = "/broadband/selected-package",
                        CloseButtonAlt = Common_Resources.ModalCloseButtonAlt,
                        CloseButtonLabel = Common_Resources.ModalCloseButtonText,
                        CancelButtonText = Common_Resources.ModalCancelButtonText,
                        CancelButtonTextAlt = Common_Resources.ModalCancelButtonAlt,
                        ButtonText = Summary_Resources.PackageDetailsModalButtonText,
                        ButtonTextAlt = Summary_Resources.PackageDetailsModalButtonAlt
                    },
                    CancellationChargesParagraph2 = string.Format(AvailablePackages_Resources.CancellationParagraph2, equipmentCharge)
                };

                bool isFixAndFibreV3 = customer.SelectedProductGroup == BroadbandProductGroup.FixAndFibreV3;
                bool isFixAndFibrePlus = customer.SelectedProductGroup == BroadbandProductGroup.FixAndFibrePlus;

                model.CancellationChargesParagraph2 = string.Format(AvailablePackages_Resources.CancellationParagraph2, equipmentCharge);

                model.TermsAndConditionsParagraph1 = isFixAndFibreV3 ? AvailablePackages_Resources.TermsAndConditionsParagraph1FixAndFibreV3 : AvailablePackages_Resources.TermsAndConditionsParagraph1;
                model.TermsAndConditionsParagraph2 = isFixAndFibreV3 ? AvailablePackages_Resources.TermsAndConditionsParagraph2FixAndFibreV3 : AvailablePackages_Resources.TermsAndConditionsParagraph2;
                
                if (isFixAndFibreV3)
                {
                    model.TermsAndConditionsParagraph3 = string.Format(AvailablePackages_Resources.TermsAndConditionsParagraph3FixAndFibre, "18", "£28");
                }
                else if (isFixAndFibrePlus)
                {
                    model.TermsAndConditionsParagraph3 = string.Format(AvailablePackages_Resources.TermsAndConditionsParagraph3FixAndFibre, "18", "£32");
                }
                else
                {
                    model.TermsAndConditionsParagraph3 = AvailablePackages_Resources.TermsAndConditionsParagraph3;
                }

                model.TermsAndConditionsParagraph4 = isFixAndFibreV3 ? AvailablePackages_Resources.TermsAndConditionsParagraph4FixAndFibre : string.Empty;

                model.CancellationParagraph3 = isFixAndFibreV3 ? AvailablePackages_Resources.CancellationParagraph3FixAndFibreV3 : AvailablePackages_Resources.CancellationParagraph3;
                    
                model.ShowCancellationParagraph4 = !isFixAndFibreV3;
                model.Accordion7Paragraph1 = Summary_Resources.Accordion7Paragraph1;

                return model;
            }
            catch (Exception ex)
            {
                const string errorMessage = "Exception occured with summary page";
                _logger.Error(errorMessage, ex);
                throw new Exception(errorMessage, ex);
            }
        }

        public DirectDebitMandateViewModel GetPrintMandateViewModel()
        {
            Customer customer = _broadbandJourneyService.GetBroadbandJourneyDetails().Customer;
            Guard.Against<ArgumentException>(customer == null, $"{nameof(customer)} is null");

            // ReSharper disable once PossibleNullReferenceException
            DirectDebitDetails directDebitDetails = customer.DirectDebitDetails;
            return DirectDebitMapper.GetMandateViewModel(directDebitDetails, ProductType.Phone);
        }

        public SummaryViewModel PopulateSummaryViewModel(SummaryViewModel model)
        {
            model.BackChevronViewModel = new BackChevronViewModel
            {
                ActionName = "BankDetails",
                ControllerName = "BankDetails",
                TitleAttributeText = Common_Resources.BackButtonAlt
            };

            return model;
        }
    }
}