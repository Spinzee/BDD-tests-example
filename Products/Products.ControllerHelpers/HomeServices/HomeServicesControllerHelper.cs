namespace Products.ControllerHelpers.HomeServices
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Infrastructure.Logging;
    using Model.Common;
    using Model.Constants;
    using Model.Enums;
    using Model.HomeServices;
    using Service.Common;
    using Service.Common.Mappers;
    using Service.HomeServices;
    using Service.HomeServices.Mappers;
    using WebModel.Resources.Common;
    using WebModel.Resources.HomeServices;
    using WebModel.ViewModels.Common;
    using WebModel.ViewModels.HomeServices;

    public class HomeServicesControllerHelper : IHomeServicesControllerHelper
    {
        private readonly IAddressService _addressService;
        private readonly IBankValidationService _bankValidationService;
        private readonly IConfigManager _configManager;
        private readonly IContextManager _contextManager;
        private readonly ICustomerAlertService _customerAlertService;
        private readonly HomeServicesViewModelMapper _homeServicesViewModelMapper;
        private readonly ILogger _logger;
        private readonly IPostcodeCheckerService _postcodeCheckerService;
        private readonly IProductService _productService;
        private readonly ISessionManager _sessionManager;
        private readonly IStepCounterService _stepCounterService;
        private readonly ISummaryService _summaryService;

        public HomeServicesControllerHelper(ISessionManager sessionManager,
            ILogger logger,
            IBankValidationService bankValidationService,
            IPostcodeCheckerService postcodeCheckerService,
            IConfigManager configManager,
            IProductService productService,
            IAddressService addressService,
            HomeServicesViewModelMapper homeServicesViewModelMapper,
            ISummaryService summaryService,
            ICustomerAlertService customerAlertService,
            IContextManager contextManager,
            IStepCounterService stepCounterService)
        {
            Guard.Against<ArgumentException>(sessionManager == null, $"{nameof(sessionManager)} is null");
            Guard.Against<ArgumentException>(logger == null, $"{nameof(logger)} is null");
            Guard.Against<ArgumentException>(postcodeCheckerService == null, $"{nameof(postcodeCheckerService)} is null");
            Guard.Against<ArgumentException>(configManager == null, $"{nameof(configManager)} is null");
            Guard.Against<ArgumentException>(productService == null, $"{nameof(productService)} is null");
            Guard.Against<ArgumentException>(addressService == null, $"{nameof(addressService)} is null");
            Guard.Against<ArgumentException>(homeServicesViewModelMapper == null, $"{nameof(homeServicesViewModelMapper)} is null");
            Guard.Against<ArgumentException>(summaryService == null, $"{nameof(summaryService)} is null");
            Guard.Against<ArgumentException>(customerAlertService == null, $"{nameof(customerAlertService)} is null");
            Guard.Against<ArgumentException>(contextManager == null, $"{nameof(contextManager)} is null");
            Guard.Against<ArgumentException>(stepCounterService == null, $"{nameof(stepCounterService)} is null");

            _sessionManager = sessionManager;
            _logger = logger;
            _bankValidationService = bankValidationService;
            _postcodeCheckerService = postcodeCheckerService;
            _configManager = configManager;
            _productService = productService;
            _addressService = addressService;
            _homeServicesViewModelMapper = homeServicesViewModelMapper;
            _summaryService = summaryService;
            _customerAlertService = customerAlertService;
            _contextManager = contextManager;
            _stepCounterService = stepCounterService;
        }

        public PostcodeViewModel GetEnterPostcodeViewModel(HomeServicesCustomerType customerType, AddressTypes addressType)
        {
            return _homeServicesViewModelMapper.GetEnterPostcodeViewModel(customerType, addressType);
        }

        public void SavePostcode(PostcodeViewModel viewModel)
        {
            var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            Guard.Against<Exception>(homeServicesCustomer == null, "Home Services customer session object is null");

            if (!string.IsNullOrEmpty(viewModel.Postcode))
            {
                if (viewModel.AddressTypes == AddressTypes.Cover)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    homeServicesCustomer.CoverPostcode = viewModel.Postcode?.Trim().ToUpper();
                }
                else
                {
                    // ReSharper disable once PossibleNullReferenceException
                    homeServicesCustomer.BillingPostcode = viewModel.Postcode?.Trim().ToUpper();
                }
            }

            _sessionManager.SetSessionDetails(SessionKeys.HomeServicesCustomer, homeServicesCustomer);
        }

        public bool IsLandlord()
        {
            var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            Guard.Against<Exception>(homeServicesCustomer == null, "Home Services customer session object is null");

            // ReSharper disable once PossibleNullReferenceException
            return homeServicesCustomer.IsLandlord;
        }

        public void SaveProductCode(string productCode)
        {
            var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            Guard.Against<Exception>(homeServicesCustomer == null, "Home Services customer session object is null");

            if (!string.IsNullOrEmpty(productCode))
            {
                // ReSharper disable once PossibleNullReferenceException
                homeServicesCustomer.SelectedProductCode = productCode.Trim().ToUpper();
            }

            _sessionManager.SetSessionDetails(SessionKeys.HomeServicesCustomer, homeServicesCustomer);
        }

        public async Task<bool?> IsProductAvailable(PostcodeViewModel model)
        {
            try
            {
                var homeServicesCustomer = new HomeServicesCustomer { IsLandlord = model.CustomerType == HomeServicesCustomerType.Landlord };

                _sessionManager.SetSessionDetails(SessionKeys.HomeServicesCustomer, homeServicesCustomer);

                if (homeServicesCustomer.IsLandlord)
                {
                    return await _productService.GetHomeServicesLandlordProduct(model.Postcode?.Trim(), model.ProductCode);
                }

                return await _productService.GetHomeServicesResidentialProduct(model.Postcode?.Trim(), model.ProductCode);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occured while getting customer Home Services Product - {ex.Message}", ex);
            }

            return null;
        }

        public CoverDetailsViewModel GetCoverDetailsViewModel()
        {
            var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            Guard.Against<Exception>(homeServicesCustomer == null, "Home Services customer session object is null");

            return _homeServicesViewModelMapper.GetCoverDetailsViewModel(homeServicesCustomer);
        }

        public PersonalDetailsViewModel GetPersonalDetailsViewModel()
        {
            var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            Guard.Against<Exception>(homeServicesCustomer == null, "Home Services customer session object is null");

            return new PersonalDetailsViewModel
            {
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = "CoverDetails",
                    ControllerName = "HomeServices",
                    TitleAttributeText = Resources.BackButtonAlt
                },
                Titles = homeServicesCustomer?.PersonalDetails?.Title.ToEnum<Titles>(),
                FirstName = homeServicesCustomer?.PersonalDetails?.FirstName,
                LastName = homeServicesCustomer?.PersonalDetails?.LastName,
                DateOfBirth = homeServicesCustomer?.PersonalDetails?.DateOfBirth,
                DateOfBirthDay = homeServicesCustomer?.PersonalDetails?.DateOfBirth.Split('/')[0],
                DateOfBirthMonth = homeServicesCustomer?.PersonalDetails?.DateOfBirth.Split('/')[1],
                DateOfBirthYear = homeServicesCustomer?.PersonalDetails?.DateOfBirth.Split('/')[2],
                // ReSharper disable once PossibleNullReferenceException
                IsScottishPostcode = _postcodeCheckerService.IsScottishPostcode(homeServicesCustomer.CoverPostcode)
            };
        }

        public void SavePersonalDetailsViewModel(PersonalDetailsViewModel personalDetailsViewModel)
        {
            var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            Guard.Against<Exception>(homeServicesCustomer == null, "Home Services customer session object is null");

            // ReSharper disable once PossibleNullReferenceException
            homeServicesCustomer.PersonalDetails = new PersonalDetails
            {
                FirstName = personalDetailsViewModel.FirstName,
                LastName = personalDetailsViewModel.LastName,
                Title = personalDetailsViewModel.Titles.ToString(),
                DateOfBirth = personalDetailsViewModel.DateOfBirth
            };
            _sessionManager.SetSessionDetails(SessionKeys.HomeServicesCustomer, homeServicesCustomer);
        }

        public async Task<SelectAddressViewModel> GetSelectAddressViewModel(AddressTypes addressType)
        {
            return await _addressService.GetSelectAddressViewModel(addressType);
        }

        public void SetManualAddress(SelectAddressViewModel model)
        {
            _addressService.SetManualAddress(model);
        }

        public async Task<bool> SetSelectedAddressByMoniker(SelectAddressViewModel model)
        {
            return await _addressService.SetSelectedAddressByMoniker(model);
        }

        public ContactDetailsViewModel GetContactDetailsViewModel()
        {
            var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            Guard.Against<Exception>(homeServicesCustomer == null, "Home Services Customer session object is null");

            return new ContactDetailsViewModel
            {
                BackChevronViewModel = new BackChevronViewModel
                {
                    // ReSharper disable once PossibleNullReferenceException
                    ActionName = homeServicesCustomer.IsLandlord ? "SelectBillingAddress" : "SelectAddress",
                    ControllerName = "HomeServices",
                    TitleAttributeText = Resources.BackButtonAlt
                },
                ConfirmEmailAddress = homeServicesCustomer.ContactDetails?.EmailAddress,
                EmailAddress = homeServicesCustomer.ContactDetails?.EmailAddress,
                ContactNumber = homeServicesCustomer.ContactDetails?.ContactNumber,
                IsMarketingConsentChecked = homeServicesCustomer.ContactDetails?.MarketingConsent == true
            };
        }

        public void SetContactDetails(ContactDetailsViewModel contactDetailsViewModel)
        {
            var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            Guard.Against<Exception>(homeServicesCustomer == null, "Home Services Customer session object is null");

            // ReSharper disable once PossibleNullReferenceException
            homeServicesCustomer.ContactDetails = new ContactDetails
            {
                ContactNumber = contactDetailsViewModel.ContactNumber,
                EmailAddress = contactDetailsViewModel.EmailAddress,
                MarketingConsent = contactDetailsViewModel.IsMarketingConsentChecked
            };
        }

        public BankDetailsViewModel GetBankDetailsViewModel()
        {
            var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            Guard.Against<ArgumentException>(homeServicesCustomer == null, "Home Services Customer object is null");

            Product selectedProduct = homeServicesCustomer.GetSelectedProduct();
            Guard.Against<ArgumentException>(selectedProduct == null, "Home Services selected Product object is null");

            var viewModel = new BankDetailsViewModel
            {
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = "ContactDetails",
                    ControllerName = "HomeServices",
                    TitleAttributeText = Resources.BackButtonAlt
                },
                ProductData = new List<ProductsDataViewModel>()
            };

            viewModel.ProductData.Add(new ProductsDataViewModel
            {
                // ReSharper disable once PossibleNullReferenceException
                Amount = selectedProduct.MonthlyCost.ToString("C"),
                ProductName = selectedProduct.Description
            });

            viewModel.ProductData.AddRange(homeServicesCustomer.GetSelectedExtras().Select(p => new ProductsDataViewModel
            {
                Amount = p.Cost.ToString("C"),
                ProductName = p.Name
            }).ToList());

            return viewModel;
        }

        public void ProcessBankDetails(BankDetailsViewModel model)
        {
            var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            if (homeServicesCustomer == null)
            {
                throw new Exception("Home Services Customer object is null");
            }

            homeServicesCustomer.BankServiceRetryCount++;

            if (homeServicesCustomer.BankServiceRetryCount > 3)
            {
                model.IsRetryExceeded = true;
                return;
            }

            BankDetails bankDetails = _bankValidationService.GetBankDetails(model.SortCode, model.AccountNumber);

            if (bankDetails == null)
            {
                model.BankDetailsIsValid = false;
                model.IsRetry = true;
                model.IsRetryExceeded = false;
            }
            else
            {
                SaveBankDetails(model, homeServicesCustomer, bankDetails);
                model.BankDetailsIsValid = true;
                model.IsRetry = false;
                homeServicesCustomer.BankServiceRetryCount = 0;
            }
        }

        public DirectDebitMandateViewModel GetPrintMandateViewModel()
        {
            var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            Guard.Against<Exception>(homeServicesCustomer == null, "Customer object is null in session");

            // ReSharper disable once PossibleNullReferenceException
            return DirectDebitMapper.GetMandateViewModel(homeServicesCustomer.DirectDebitDetails, ProductType.HomeServices);
        }

        public SummaryViewModel GetSummaryViewModel()
        {
            var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            Guard.Against<Exception>(homeServicesCustomer == null, "Customer object is null in session");

            string homeServicesHubUrl = _configManager.GetAppSetting("HomeServicesHubUrl");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(homeServicesHubUrl), "Home Services Hub Url not in config");

            return _homeServicesViewModelMapper.GetSummaryViewModel(homeServicesCustomer, homeServicesHubUrl);
        }

        public async Task ConfirmSale()
        {
            await _summaryService.ConfirmSale();
        }

        public ConfirmationViewModel ConfirmationViewModel()
        {
            var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            Guard.Against<Exception>(homeServicesCustomer == null, "Customer object is null in session");

            // ReSharper disable once PossibleNullReferenceException
            Product selectedProduct = homeServicesCustomer.AvailableProduct.Products.FirstOrDefault(p => p.ProductCode == homeServicesCustomer.SelectedProductCode);

            Guard.Against<Exception>(selectedProduct == null, "Selected product object is null in session");
            var confirmationViewModel = new ConfirmationViewModel
            {
                // ReSharper disable once PossibleNullReferenceException
                ProductName = selectedProduct.Description,
                ProductExtras = homeServicesCustomer.GetSelectedExtras().Select(x => x.Name).ToList(),
                IsLandLord = homeServicesCustomer.IsLandlord,
                DataLayer = GetDataLayer()
            };

            _sessionManager.ClearSession();

            return confirmationViewModel;
        }

        public bool IsNorthernIrelandPostcode(string postcode)
        {
            return _postcodeCheckerService.IsNorthernIrelandPostcode(postcode.Trim());
        }

        public YourCoverBasketViewModel GetYourCoverBasket()
        {
            var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            Guard.Against<Exception>(homeServicesCustomer == null, "Home Services customer session object is null");

            return _homeServicesViewModelMapper.GetYourCoverBasketViewModel(homeServicesCustomer);
        }

        public bool UpdateExcessProductCode(string productCode)
        {
            try
            {
                var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
                Guard.Against<Exception>(homeServicesCustomer == null, "Home Services customer session object is null");
                // ReSharper disable once PossibleNullReferenceException
                bool productAvailable = homeServicesCustomer.AvailableProduct.Products.Select(p => p.ProductCode).Contains(productCode);
                if (productAvailable)
                {
                    homeServicesCustomer.SelectedProductCode = productCode;
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception while updating the product code on excess change", ex);
            }

            return false;
        }

        public CoverDetailsHeaderViewModel GetYourCoverHeader()
        {
            try
            {
                var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
                Guard.Against<Exception>(homeServicesCustomer == null, "Home Services customer session object is null");

                return _homeServicesViewModelMapper.CoverDetailsHeaderViewModel(homeServicesCustomer);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception while refreshing Your Cover Header", ex);
            }

            return null;
        }

        public YourCoverBasketViewModel GetYourCoverBasketAjax()
        {
            try
            {
                var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
                Guard.Against<Exception>(homeServicesCustomer == null, "Home Services customer session object is null");

                return _homeServicesViewModelMapper.GetYourCoverBasketViewModel(homeServicesCustomer);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception while refreshing Your Cover Basket", ex);
            }

            return null;
        }

        public bool UpdateExtraProductCode(string productCode)
        {
            try
            {
                var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
                Guard.Against<Exception>(homeServicesCustomer == null, "Home Services customer session object is null");

                // ReSharper disable once PossibleNullReferenceException
                bool productAvailable = homeServicesCustomer.AvailableProduct.Extras.Select(p => p.ProductCode).Contains(productCode);

                if (productAvailable)
                {
                    if (homeServicesCustomer.SelectedExtraCodes.Contains(productCode))
                    {
                        homeServicesCustomer.SelectedExtraCodes.Remove(productCode);
                    }
                    else
                    {
                        homeServicesCustomer.SelectedExtraCodes.Add(productCode);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception while updating selected extras", ex);
            }

            return false;
        }

        public async Task<bool> IsCustomerAlert()
        {
            return await _customerAlertService.IsCustomerAlert(_configManager.GetAppSetting("HomeServicesCustomerAlertName"));
        }

        public Dictionary<string, string> GetDataLayer()
        {
            string productcode = _contextManager.GetQueryStringValueFromContext("productcode");
            string url = _contextManager.GetRawUrl();
            string affiliateCampaignCode = _configManager.GetAppSetting("AffiliateCampaignCode");

            var homeServicesCustomer = _sessionManager.GetOrDefaultSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            Product selectedProduct = homeServicesCustomer.GetSelectedProduct();

            var dataLayer = new Dictionary<string, string>
            {
                { DataLayer_Resources.ProductType, homeServicesCustomer.IsLandlord || url.Contains("enter-cover-postcode") ? "Landlord" : "Residential" },
                { DataLayer_Resources.ProductCode, !string.IsNullOrEmpty(productcode) ? productcode : homeServicesCustomer.SelectedProductCode },
                { DataLayer_Resources.ProductName, selectedProduct?.Description ?? string.Empty },
                { DataLayer_Resources.MonthlyCost, selectedProduct != null ? homeServicesCustomer.GetTotalMonthlyCostWithExtras().ToString("C") : string.Empty },
                { DataLayer_Resources.YearlyCost, selectedProduct != null ? homeServicesCustomer.GetFormattedTotalYearlyCostWithExtras() : string.Empty },
                { DataLayer_Resources.Excess, selectedProduct?.Excess.ToString("C") ?? string.Empty },
                { DataLayer_Resources.Extras, string.Join(",", homeServicesCustomer.SelectedExtraCodes) },
                { DataLayer_Resources.ContractLength, selectedProduct?.ContractLength.ToString(CultureInfo.InvariantCulture) ?? string.Empty },
                { DataLayer_Resources.MarketingConsent, homeServicesCustomer.ContactDetails?.MarketingConsent.ToString() ?? string.Empty },
                { DataLayer_Resources.SaleId, string.Join(",", homeServicesCustomer.ApplicationIds) },
                { DataLayer_Resources.AffiliateSale, !string.IsNullOrEmpty(homeServicesCustomer.CampaignCode) ? (homeServicesCustomer.CampaignCode == affiliateCampaignCode).ToString() : string.Empty },
                { DataLayer_Resources.AffiliateId, homeServicesCustomer.MigrateAffiliateId ?? string.Empty },
                { DataLayer_Resources.MembershipId, homeServicesCustomer.MigrateMemberid ?? string.Empty }
            };
            return dataLayer;
        }

        public string GetStepCounter(string actionName)
        {
            return _stepCounterService.GetStepCounter(actionName);
        }

        private static void SaveBankDetails(BankDetailsViewModel model, HomeServicesCustomer homeServicesCustomer, BankDetails bankDetails)
        {
            homeServicesCustomer.DirectDebitDetails = new DirectDebitDetails
            {
                AccountName = model.AccountHolder,
                AccountNumber = model.AccountNumber,
                SortCode = model.SortCode,
                DirectDebitPaymentDate = int.Parse(model.DirectDebitDate),
                BankName = bankDetails.BankName,
                BankAddressLine1 = bankDetails.BankAddress.BankAddressLine1Field,
                BankAddressLine2 = bankDetails.BankAddress.BankAddressLine2Field,
                BankAddressLine3 = bankDetails.BankAddress.BankAddressLine3Field,
                Postcode = bankDetails.BankAddress.BankPostcodeField
            };
        }
    }
}