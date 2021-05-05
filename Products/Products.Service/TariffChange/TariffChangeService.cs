namespace Products.Service.TariffChange
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using System.Web;
    using Common;
    using Core;
    using Infrastructure;
    using Mappers;
    using Model.Enums;
    using Products.Model.TariffChange.Customers;
    using Products.Model.TariffChange.Enums;
    using Products.WebModel.Resources.TariffChange;
    using Products.WebModel.ViewModels.Common;
    using Products.WebModel.ViewModels.TariffChange;

    public class TariffChangeService : ITariffChangeService
    {
        private readonly IJourneyDetailsService _journeyDetailsService;
        private readonly ICustomerService _customerService;
        private readonly IConfigManager _configManager;
        private readonly ICustomerAlertService _customerAlertService;
        private readonly IGoogleReCaptchaService _googleReCaptchaService;
        private readonly IContextManager _contextManager;
        private readonly CustomerAccountService _customerAccountService;

        public TariffChangeService(
            IJourneyDetailsService journeyDetailsService,
            ICustomerService customerService,
            IConfigManager configManager,
            ICustomerAlertService customerAlertService,
            IGoogleReCaptchaService googleReCaptchaService,
            IContextManager contextManager,
            CustomerAccountService customerAccountService)
        {
            Guard.Against<ArgumentNullException>(journeyDetailsService == null, "journeyDetailsService is null");
            Guard.Against<ArgumentNullException>(customerService == null, "customerService is null");
            Guard.Against<ArgumentNullException>(configManager == null, "configManager is null");
            Guard.Against<ArgumentNullException>(customerAlertService == null, "customerAlertService is null");
            Guard.Against<ArgumentNullException>(googleReCaptchaService == null, "googleReCaptchaService is null");
            Guard.Against<ArgumentNullException>(contextManager == null, "contextManager is null");
            Guard.Against<ArgumentNullException>(customerAccountService == null, "customerAccountService is null");

            _journeyDetailsService = journeyDetailsService;
            _customerService = customerService;
            _configManager = configManager;
            _customerAlertService = customerAlertService;
            _googleReCaptchaService = googleReCaptchaService;
            _contextManager = contextManager;
            _customerAccountService = customerAccountService;
        }

        public void ClearJourneyDetails()
        {
            _journeyDetailsService.ClearJourneyDetails();
        }

        public CustomerEligibilityViewModel GetCustomerEligibilityViewModel()
        {
            var customerEligibilityViewModel = new CustomerEligibilityViewModel();

            try
            {
                CustomerAccount customerAccount = _journeyDetailsService.GetCustomerAccount();
                if (customerAccount == null)
                {
                    _journeyDetailsService.ClearJourneyDetails();
                    customerEligibilityViewModel.AccountNumber = null;
                    return customerEligibilityViewModel;
                }

                customerEligibilityViewModel.AccountNumber = customerAccount.SiteDetails.AccountNumber;
                Customer customer = _customerService.GetCustomerDetails(customerAccount);

                FalloutReasonResult falloutAction = customer.FalloutReasons.OrderByDescending(f => f.FalloutReason).FirstOrDefault();
                if (falloutAction == null)
                {
                    _journeyDetailsService.SetCustomer(customer);
                }
                else
                {
                    customerEligibilityViewModel.FalloutReasons = customer.FalloutReasons;
                    customerEligibilityViewModel.FalloutReasonResult = falloutAction;
                }

                customerEligibilityViewModel.IsPostLogin = _journeyDetailsService.GetCustomerJourney() !=
                                                           CTCJourneyType.PreLogIn;

                return customerEligibilityViewModel;
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception occured, Account = {customerEligibilityViewModel.AccountNumber}, checking account eligibility.", ex);
            }

        }

        public AcquisitionJourneyViewModel GetAcquisitionJourneyViewModel()
        {
            string url = _configManager.GetAppSetting("RedirectionUrl");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(url), "RedirectionUrl is missing in web.config");
            CustomerAccount customerAccount = _journeyDetailsService.GetCustomerAccount();
            string postcode = customerAccount?.SiteDetails?.PostCode;
            _journeyDetailsService.ClearJourneyDetails();

            var viewModel = new AcquisitionJourneyViewModel
            {
                PostCode = postcode,
                Url = url
            };

            return viewModel;
        }

        public GetCustomerEmailViewModel GetCustomerEmailViewModel()
        {
            Customer customer = _journeyDetailsService.GetCustomer();
            if (customer?.CustomerSelectedTariff == null)
            {
                _journeyDetailsService.ClearJourneyDetails();
                return null;
            }

            var customerEmailViewModel = new GetCustomerEmailViewModel
            {
                EmailAddress = customer.EmailAddress,
                ConfirmEmailAddress = customer.EmailAddress,
                DataLayer = GetDataLayer(),
                ProgressBarViewModel = new ProgressBarViewModel
                {
                    Sections = new List<ProgressBarSection>
                    {
                        new ProgressBarSection
                        {
                            Text = ProgressBar_Resource.UserDetailsSectionHeader,
                            Status = ProgressBarStatus.Done
                        },
                        new ProgressBarSection
                        {
                            Text = ProgressBar_Resource.TariffChoiceSectionHeader,
                            Status = ProgressBarStatus.Done
                        },
                        new ProgressBarSection
                        {
                            Text = ProgressBar_Resource.EmailAddressSectionHeader,
                            Status = ProgressBarStatus.Active
                        },
                        new ProgressBarSection
                        {
                            Text = ProgressBar_Resource.SummarySectionHeader,
                            Status = ProgressBarStatus.Awaiting
                        }
                    }
                }
            };

            return customerEmailViewModel;
        }

        public ConfirmationViewModel GetConfirmationViewModel()
        {
            var viewModel = new ConfirmationViewModel();
            Customer customer = _journeyDetailsService.GetCustomer();
            CTCJourneyType customerJourneyType = _journeyDetailsService.GetCustomerJourney();
            CustomerAccount customerAccount = _journeyDetailsService.GetCustomerAccount();

            if (customer?.CustomerSelectedTariff == null || customer.CustomerSelectedTariff.EffectiveDate == DateTime.MinValue)
            {
                _journeyDetailsService.ClearJourneyDetails();
                return null;
            }

            viewModel.DataLayer = GetDataLayer();
            viewModel.CustomerEmailAddress = customer.EmailAddress;
            viewModel.CTCJourneyType = customerJourneyType;
            viewModel.BulletList = new List<string>();
            _journeyDetailsService.ClearJourneyDetails();

            if ((customer.CustomerSelectedTariff?.IsSmartTariff ?? false)  && !(customerAccount?.IsSmart ?? false))
            {
                viewModel.BulletList.Add(Confirmation_Resources.SmartBullet);
            }

            if (customer.CustomerSelectedTariff?.IsFollowOnTariff == true)
            {
                viewModel.Header = Confirmation_Resources.FollowOnHeader;
                viewModel.BulletList.Add(Confirmation_Resources.FollowOnBullet1);
                viewModel.BulletList.Add(Confirmation_Resources.FollowOnBullet2);
                return viewModel;
            }
            viewModel.Header = Confirmation_Resources.Header;
            viewModel.Paragraph = string.Format(Confirmation_Resources.Paragraph, customer.EmailAddress);

            if (viewModel.CTCJourneyType != CTCJourneyType.PreLogIn)
            {
                viewModel.BulletList.Add(Confirmation_Resources.BulletPostLogin);
            }

            viewModel.ShowSmartBookingLink = customerAccount != null && (!customerAccount.IsSmart && customerAccount.IsSmartEligible) && customer.CustomerSelectedTariff.TariffGroup != TariffGroup.FixAndFibre;
            viewModel.ShowTelcoLink = customer.CustomerSelectedTariff.TariffGroup == TariffGroup.FixAndFibre;
            viewModel.IsSmartCustomer = customerAccount?.IsSmart ?? false;

            return viewModel;
        }

        public async Task<ConfirmDetailsViewModel> GetConfirmDetailsViewModel()
        {
            var model = new ConfirmDetailsViewModel
            {
                CTCJourneyType = CTCJourneyType.PreLogIn
            };

            try
            {
                bool hasActiveCustomerAlert = await _customerAlertService.IsCustomerAlert(_configManager.GetAppSetting("CustomerAlertName"));
                if (hasActiveCustomerAlert)
                {
                    model.CustomerAlertActive = true;
                    return model;
                }

                if (HttpContext.Current != null)
                {
                    IPrincipal user = HttpContext.Current.User;
                    if (user.Identity.IsAuthenticated)
                    {
                        List<string> customerAccountNumbers = await _customerAccountService.GetUserAccountsByLoginName(user.Identity.Name);
                        if (customerAccountNumbers.Count == 0)
                        {
                            model.CTCJourneyType = CTCJourneyType.PostLogInWithNoAccounts;
                        }
                        else
                        {
                            IList<CustomerAccount> customerAccounts = _customerAccountService.GetCustomerAccount(customerAccountNumbers);

                            IList<CustomerAccount> distinctCustomerAccounts = customerAccounts.GroupBy(s => s.SiteDetails.SiteId).Select(g => g.First()).ToList();

                            int siteCount = distinctCustomerAccounts.Count;

                            if (siteCount > 1)
                            {
                                model.CTCJourneyType = CTCJourneyType.PostLogInWithMultipleSites;

                                var multiSite = new MultiSiteAddressesViewModel();
                                var addresses = new MultiSiteAddressViewModel
                                {
                                    Items = new List<RadioButton>(),
                                    SelectedValue = "SelectedSiteId"
                                };

                                foreach (CustomerAccount account in distinctCustomerAccounts)
                                {
                                    addresses.Items.Add
                                    (
                                        new RadioButton
                                        {
                                            Value = account.SiteDetails.SiteId.ToString(),
                                            DisplayText = account.SiteDetails.Address
                                        }
                                    );
                                }

                                addresses.Items.First().Checked = true;
                                multiSite.Addresses = addresses;
                                model.MultiSiteAddressesViewModel = multiSite;

                                model.ProgressBarViewModel = new ProgressBarViewModel
                                {
                                    Sections = new List<ProgressBarSection>
                                    {
                                        new ProgressBarSection
                                        {
                                            Text = ProgressBar_Resource.SelectAddressSectionHeader,
                                            Status = ProgressBarStatus.Active
                                        },
                                        new ProgressBarSection
                                        {
                                            Text = ProgressBar_Resource.TariffChoiceSectionHeader,
                                            Status = ProgressBarStatus.Awaiting
                                        },
                                        new ProgressBarSection
                                        {
                                            Text = ProgressBar_Resource.SummarySectionHeader,
                                            Status = ProgressBarStatus.Awaiting
                                        }
                                    }

                                };
                                _journeyDetailsService.SetMultipleCustomerAccounts(customerAccounts);
                            }
                            else
                            {
                                if (siteCount == 1)
                                {
                                    CustomerAccount account = customerAccounts.OrderBy(o => o.SiteDetails.ServiceStatusType).FirstOrDefault();
                                    if (account != null)
                                    {
                                        model.CTCJourneyType = CTCJourneyType.PostLogInWithSingleSite;
                                        model.HasMultipleServices = account.SiteDetails.HasMultipleServices;
                                        model.IsValidForPostCode = account.SiteDetails.IsValidForPostCode(customerAccounts[0].SiteDetails.PostCode);
                                        _journeyDetailsService.SetCustomerAccount(account);
                                    }
                                }
                                else
                                {
                                    model.CTCJourneyType = CTCJourneyType.PostLogInWithNoAccounts;
                                }
                            }
                        }
                    }
                }

                _journeyDetailsService.SetCustomerJourney(model.CTCJourneyType);

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception occured in GetConfirmDetailsViewModel method.", ex);
            }
        }

        public ConfirmDetailsViewModel ValidateCustomer(IdentifyCustomerViewModel viewModel)
        {
            var model = new ConfirmDetailsViewModel();
            try
            {
                CustomerAccount customerAccount = _customerAccountService.GetCustomerAccount(viewModel.AccountNumber.Trim());

                if (customerAccount.SiteDetails.HasMultipleServices)
                {
                    model.HasMultipleServices = true;
                }

                if (customerAccount.SiteDetails.IsValidForPostCode(viewModel.PostCode))
                {
                    model.IsValidForPostCode = true;
                    _journeyDetailsService.SetCustomerAccount(customerAccount);
                }

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception occured, Account = {viewModel.AccountNumber}, Postcode = {viewModel.PostCode}, attempting to validate for customer tariff change, customer identification.", ex);
            }

        }

        public GoogleCaptchaViewModel GetGoogleCaptchaViewModel()
        {
            string googleReCaptchaConfig = _configManager.GetAppSetting("SupressGoogleReCaptcha");
            bool supressGoogleReCaptcha = false;
            if (!string.IsNullOrEmpty(googleReCaptchaConfig))
            {
                supressGoogleReCaptcha = Convert.ToBoolean(googleReCaptchaConfig);
            }

            var viewModel = new GoogleCaptchaViewModel
            {
                SuppressGoogleCaptcha = supressGoogleReCaptcha
            };
            if (!supressGoogleReCaptcha)
            {
                viewModel.GoogleCaptchaPublicKey = _configManager.GetAppSetting("PublicKey") ?? string.Empty;
            }

            return viewModel;
        }

        public GoogleCaptchaViewModel CheckGoogleCaptchaViewModel()
        {
            var viewModel =
                new GoogleCaptchaViewModel
                {
                    IsValidReCaptcha =
                        _googleReCaptchaService.ValidateReCaptcha(
                            _contextManager.HttpContext.Request["g-recaptcha-response"])
                };

            return viewModel;
        }

        public ConfirmDetailsViewModel ValidateMultiSiteCustomer(int selectedSiteId)
        {

            IList<CustomerAccount> customerAccounts = _journeyDetailsService.GetCustomerAccounts();
            CustomerAccount selectedCustomerAccount = customerAccounts
                .OrderBy(o => o.SiteDetails.ServiceStatusType)
                .FirstOrDefault(s => s.SiteDetails.SiteId == selectedSiteId);

            var model = new ConfirmDetailsViewModel();

            if (selectedCustomerAccount != null)
            {
                if (selectedCustomerAccount.SiteDetails.HasMultipleServices)
                {
                    model.HasMultipleServices = true;
                }

                _journeyDetailsService.SetCustomerAccount(selectedCustomerAccount);

            }

            _journeyDetailsService.ClearMultipleCustomerAccounts();

            return model;
        }

        public ConfirmAddressViewModel GetConfirmAddressViewModel()
        {
            CustomerAccount customerAccount = null;
            var model = new ConfirmAddressViewModel();

            try
            {
                customerAccount = _journeyDetailsService.GetCustomerAccount();
                if (customerAccount == null)
                {
                    _journeyDetailsService.ClearJourneyDetails();
                    model.IsCustomerAccountSet = false;
                    return model;
                }

                model.IsCustomerAccountSet = true;
                model.Greeting = GetGreeting();
                model.FormattedAddress = customerAccount.SiteDetails.Address;
                model.CTCJourneyType = _journeyDetailsService.GetCustomerJourney();

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception occured, Account = {customerAccount?.SiteDetails?.AccountNumber}, attempting to get account holder name for customer tariff change, confirm customer details.", ex);
            }
        }

        public TariffSummaryViewModel GetTariffSummaryViewModel()
        {
            Customer customer = null;

            try
            {
                customer = _journeyDetailsService.GetCustomer();
                customer.SetSummaryDetails();
                _journeyDetailsService.SetCustomer(customer);

                CTCJourneyType customerJourneyType = _journeyDetailsService.GetCustomerJourney();

                TariffSummaryViewModel model = SummaryViewModelMapper.MapSummary(customer, customerJourneyType);
                model.DataLayer = GetDataLayer();

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception occured, Accounts = { string.Join(", ", customer.CustomerAccountNumbers()) }, showing Summary page.", ex);
            }
        }

        public IdentifyCustomerViewModel GetUnAuthenticatedIdentifyViewModel()
        {
            var viewModel = new IdentifyCustomerViewModel
            {
                GoogleCaptchaViewModel = GetGoogleCaptchaViewModel(),
                LoginRedirectUrl =
                    $"{_configManager.GetAppSetting("PostLoginLoginLinkUrl")}?returnUrl={HttpUtility.UrlEncode(_configManager.GetAppSetting("PostLoginLoginRedirectUrl"))}"
            };

            return viewModel;
        }


        private string GetGreeting()
        {
            int hour = DateTime.Now.Hour;

            if (hour < 12)
            {
                return ConfirmDetails_Resources.HeaderMorning;
            }

            if (hour < 18)
            {
                return ConfirmDetails_Resources.HeaderAfternoon;
            }

            return ConfirmDetails_Resources.HeaderEvening;
        }

        public void SetFollowOnAsSelectedTariff()
        {
            Customer customer = _journeyDetailsService.GetCustomer();

            customer.CustomerSelectedTariff = customer.FollowOnTariff;
            customer.CustomerSelectedTariff.IsFollowOnTariff = true;

            _journeyDetailsService.SetCustomer(customer);
        }

        public Dictionary<string, string> GetDataLayer()
        {
            Customer customer = _journeyDetailsService.GetCustomer();
            CustomerAccount customerAccount = _journeyDetailsService.GetCustomerAccount();
            Guard.Against<ArgumentException>(customer == null, "Customer is null");

            return DataLayerMapper.GetDataLayerDictionary(customer, customerAccount);
        }
    }
}
