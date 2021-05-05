namespace Products.Service.Broadband
{
    using System;
    using Common;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Model.Broadband;
    using Model.Common;
    using Model.Constants;
    using WebModel.Resources.Broadband;
    using WebModel.ViewModels.Broadband;
    using WebModel.ViewModels.Common;

    public class CustomerDetailsService : ICustomerDetailsService
    {
        private readonly IBroadbandJourneyService _broadbandJourneyService;
        private readonly IPostcodeCheckerService _postcodeCheckerService;
        private readonly ISessionManager _sessionManager;

        public CustomerDetailsService(ISessionManager sessionManager, IBroadbandJourneyService broadbandJourneyService,
            IPostcodeCheckerService postcodeCheckerService)
        {
            Guard.Against<ArgumentException>(sessionManager == null, $"{nameof(sessionManager)} is null");
            Guard.Against<ArgumentException>(broadbandJourneyService == null, $"{nameof(broadbandJourneyService)} is null");
            Guard.Against<ArgumentException>(postcodeCheckerService == null, $"{nameof(postcodeCheckerService)} is null");

            _sessionManager = sessionManager;
            _broadbandJourneyService = broadbandJourneyService;
            _postcodeCheckerService = postcodeCheckerService;
        }

        public PersonalDetailsViewModel GetPersonalDetailsViewModel()
        {
            Customer customer = _broadbandJourneyService.GetBroadbandJourneyDetails().Customer;
            Guard.Against<ArgumentException>(customer == null, $"{nameof(customer)} is null");
            // ReSharper disable once PossibleNullReferenceException
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(customer.PostcodeEntered), $"{nameof(customer.PostcodeEntered)} is null");

            var personalDetailsViewModel = new PersonalDetailsViewModel
            {
                YourPriceViewModel = _sessionManager.GetSessionDetails<YourPriceViewModel>("yourPriceDetails"),
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = "TransferYourNumber",
                    ControllerName = "CustomerDetails",
                    TitleAttributeText = Common_Resources.BackButtonAlt
                },
                IsScottishPostcode = _postcodeCheckerService.IsScottishPostcode(customer.PostcodeEntered)
            };

            PersonalDetails personalDetails = customer.PersonalDetails;

            if (personalDetails != null)
            {
                personalDetailsViewModel.Titles = personalDetails.Title.ToEnum<Titles>();
                personalDetailsViewModel.FirstName = personalDetails.FirstName;
                personalDetailsViewModel.LastName = personalDetails.LastName;
                personalDetailsViewModel.DateOfBirth = personalDetails.DateOfBirth;
                personalDetailsViewModel.DateOfBirthDay = personalDetails.DateOfBirth.Split('/')[0];
                personalDetailsViewModel.DateOfBirthMonth = personalDetails.DateOfBirth.Split('/')[1];
                personalDetailsViewModel.DateOfBirthYear = personalDetails.DateOfBirth.Split('/')[2];
            }

            return personalDetailsViewModel;
        }

        public void SetPersonalDetailsViewModel(PersonalDetailsViewModel personalDetailsViewModel)
        {
            try
            {
                BroadbandJourneyDetails broadbandJourneyDetails = _broadbandJourneyService.GetBroadbandJourneyDetails();
                Guard.Against<ArgumentException>(broadbandJourneyDetails.Customer == null, $"{nameof(broadbandJourneyDetails.Customer)} is null");
                var personDetails = new PersonalDetails
                {
                    Title = personalDetailsViewModel.Titles.ToString(),
                    FirstName = personalDetailsViewModel.FirstName,
                    LastName = personalDetailsViewModel.LastName,
                    DateOfBirth =
                        $"{personalDetailsViewModel.DateOfBirthDay}/{personalDetailsViewModel.DateOfBirthMonth}/{personalDetailsViewModel.DateOfBirthYear}"
                };

                // ReSharper disable once PossibleNullReferenceException
                broadbandJourneyDetails.Customer.PersonalDetails = personDetails;
                _broadbandJourneyService.SetBroadbandJourneyDetails(broadbandJourneyDetails);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception occured with storing personal details", ex);
            }
        }

        public ContactDetailsViewModel GetContactDetailsViewModel()
        {
            Customer customer = _broadbandJourneyService.GetBroadbandJourneyDetails().Customer;
            Guard.Against<ArgumentException>(customer == null, $"{nameof(customer)} is null");

            var contactDetailsViewModel = new ContactDetailsViewModel
            {
                YourPriceViewModel = _sessionManager.GetSessionDetails<YourPriceViewModel>("yourPriceDetails"),
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = "PersonalDetails",
                    ControllerName = "CustomerDetails",
                    TitleAttributeText = Common_Resources.BackButtonAlt
                }
            };

            // ReSharper disable once PossibleNullReferenceException
            ContactDetails contactDetails = customer.ContactDetails;

            if (contactDetails != null)
            {
                contactDetailsViewModel.ContactNumber = contactDetails.ContactNumber;
                contactDetailsViewModel.EmailAddress = contactDetails.EmailAddress;
                contactDetailsViewModel.ConfirmEmailAddress = contactDetails.EmailAddress;
                contactDetailsViewModel.IsMarketingConsentChecked = contactDetails.MarketingConsent;
            }

            return contactDetailsViewModel;
        }

        public void SetContactDetailsViewModel(ContactDetailsViewModel contactDetailsViewModel, Guid userId)
        {
            BroadbandJourneyDetails broadbandJourneyDetails = _broadbandJourneyService.GetBroadbandJourneyDetails();
            Guard.Against<ArgumentException>(broadbandJourneyDetails.Customer == null, $"{nameof(broadbandJourneyDetails.Customer)} is null");

            var contactDetails = new ContactDetails
            {
                EmailAddress = contactDetailsViewModel.EmailAddress,
                ContactNumber = NumberFormatter.ToDigitsOnly(contactDetailsViewModel.ContactNumber),
                MarketingConsent = contactDetailsViewModel.IsMarketingConsentChecked
            };

            // ReSharper disable once PossibleNullReferenceException
            broadbandJourneyDetails.Customer.UserId = userId;
            broadbandJourneyDetails.Customer.ContactDetails = contactDetails;
            _broadbandJourneyService.SetBroadbandJourneyDetails(broadbandJourneyDetails);
        }

        public TransferYourNumberViewModel GetTransferYourNumberViewModel()
        {
            Customer customer = _broadbandJourneyService.GetBroadbandJourneyDetails().Customer;
            Guard.Against<ArgumentException>(customer == null, $"{nameof(customer)} is null");

            return new TransferYourNumberViewModel
            {
                YourPriceViewModel = _sessionManager.GetSessionDetails<YourPriceViewModel>(SessionKeys.YourPriceDetails),
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = "SelectedPackage",
                    ControllerName = "Packages",
                    TitleAttributeText = Common_Resources.BackButtonAlt
                },
                // ReSharper disable once PossibleNullReferenceException
                PhoneNumber = customer.IsSSECustomerCLI
                    ? string.Empty
                    : string.IsNullOrWhiteSpace(customer.CliNumber) ? customer.OriginalCliEntered : customer.CliNumber,
                KeepExistingNumber = customer.KeepExistingNumber,
                IsReadOnly = !string.IsNullOrWhiteSpace(customer.OriginalCliEntered),
                IsSSECustomerCLI = customer.IsSSECustomerCLI
            };
        }

        public void SetTransferYourNumberViewModel(TransferYourNumberViewModel model)
        {
            BroadbandJourneyDetails broadbandJourneyDetails = _broadbandJourneyService.GetBroadbandJourneyDetails();
            Guard.Against<ArgumentNullException>(broadbandJourneyDetails.Customer == null, $"{nameof(broadbandJourneyDetails.Customer)} is null");

            // ReSharper disable once PossibleNullReferenceException
            broadbandJourneyDetails.Customer.CliNumber = GetPhoneNumber(model.KeepExistingNumber, model.PhoneNumber, broadbandJourneyDetails.Customer);
            broadbandJourneyDetails.Customer.KeepExistingNumber = model.KeepExistingNumber;
            broadbandJourneyDetails.Customer.TransferYourNumberIsSet = true;

            _broadbandJourneyService.SetBroadbandJourneyDetails(broadbandJourneyDetails);
        }

        private string GetPhoneNumber(bool keepExistingNumber, string phoneNumber, Customer customer)
        {
            if (keepExistingNumber)
            {
                if (customer.IsSSECustomerCLI)
                {
                    return customer.CliNumber;
                }

                return string.IsNullOrWhiteSpace(customer.OriginalCliEntered) ? phoneNumber?.Replace(" ", string.Empty) : customer.OriginalCliEntered;
            }

            return string.Empty;
        }

        public TransferYourNumberViewModel GetUpdatedTransferYourNumberViewModel(TransferYourNumberViewModel model)
        {
            Customer customer = _broadbandJourneyService.GetBroadbandJourneyDetails().Customer;
            Guard.Against<Exception>(customer == null, "broadbandJourneyDetails customer session object is null");
            var openReachResponse = _sessionManager.GetSessionDetails<OpenReachData>(SessionKeys.OpenReachResponse);            

            // ReSharper disable once PossibleNullReferenceException
            if (customer.IsSSECustomerCLI && model.KeepExistingNumber)
            {
                model.PhoneNumber = openReachResponse?.CLI;
            }

            model.YourPriceViewModel = _sessionManager.GetSessionDetails<YourPriceViewModel>(SessionKeys.YourPriceDetails);
            model.BackChevronViewModel = new BackChevronViewModel
            {
                ActionName = "SelectedPackage",
                ControllerName = "Packages",
                TitleAttributeText = Common_Resources.BackButtonAlt
            };
            model.IsReadOnly = !string.IsNullOrWhiteSpace(customer.OriginalCliEntered);
            model.IsSSECustomerCLI = customer.IsSSECustomerCLI;
            return model;
        }
    }
}