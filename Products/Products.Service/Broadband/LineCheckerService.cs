namespace Products.Service.Broadband
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Common;
    using Core;
    using Infrastructure;
    using Infrastructure.Logging;
    using Managers;
    using Mappers;
    using Model.Broadband;
    using Model.Common;
    using Model.Constants;
    using ServiceWrapper.BroadbandProductsService;
    using ServiceWrapper.NewBTLineAvailabilityService;
    using WebModel.Resources.Broadband;
    using WebModel.ViewModels.Broadband;
    using WebModel.ViewModels.Common;
    using LineSpeed = Model.Broadband.LineSpeed;
    using Tariff = Model.Broadband.Tariff;

    public class LineCheckerService : ILineCheckerService
    {
        private readonly IBroadbandJourneyService _broadbandJourneyService;
        private readonly IBroadbandProductsServiceWrapper _broadbandProductsServiceWrapper;
        private readonly IConfigManager _configManager;
        private readonly ICustomerAlertService _customerAlertService;
        private readonly ILogger _logger;
        private readonly IBroadbandManager _manager;
        private readonly INewBTLineAvailabilityServiceWrapper _newBtLineAvailabilityServiceWrapper;
        private readonly IPostcodeCheckerService _postcodeCheckerService;
        private readonly ISessionManager _sessionManager;

        public LineCheckerService(IBroadbandProductsServiceWrapper broadbandProductsServiceWrapper,
            IBroadbandManager manager,
            IBroadbandJourneyService broadbandJourneyService,
            ISessionManager sessionManager,
            IPostcodeCheckerService postcodeCheckerService,
            ILogger logger,
            IConfigManager configManager,
            ICustomerAlertService customerAlertService,
            INewBTLineAvailabilityServiceWrapper newBtLineAvailabilityServiceWrapper)
        {
            Guard.Against<ArgumentException>(broadbandProductsServiceWrapper == null, $"{nameof(broadbandProductsServiceWrapper)} is null");
            Guard.Against<ArgumentException>(manager == null, $"{nameof(manager)} is null");
            Guard.Against<ArgumentException>(broadbandJourneyService == null, $"{nameof(broadbandJourneyService)} is null");
            Guard.Against<ArgumentException>(sessionManager == null, $"{nameof(sessionManager)} is null");
            Guard.Against<ArgumentException>(postcodeCheckerService == null, $"{nameof(postcodeCheckerService)} is null");
            Guard.Against<ArgumentException>(customerAlertService == null, $"{nameof(customerAlertService)} is null");
            Guard.Against<ArgumentException>(configManager == null, $"{nameof(configManager)} is null");
            Guard.Against<ArgumentException>(logger == null, $"{nameof(logger)} is null");
            Guard.Against<ArgumentException>(newBtLineAvailabilityServiceWrapper == null, $"{nameof(newBtLineAvailabilityServiceWrapper)} is null");

            _broadbandProductsServiceWrapper = broadbandProductsServiceWrapper;
            _manager = manager;
            _broadbandJourneyService = broadbandJourneyService;
            _sessionManager = sessionManager;
            _postcodeCheckerService = postcodeCheckerService;
            _logger = logger;
            _configManager = configManager;
            _customerAlertService = customerAlertService;
            _newBtLineAvailabilityServiceWrapper = newBtLineAvailabilityServiceWrapper;
        }

        public bool IsNorthernIrelandPostcode(string postcode)
        {
            return _postcodeCheckerService.IsNorthernIrelandPostcode(postcode);
        }

        public async Task<List<AddressViewModel>> GetAddresses()
        {
            Customer customer = _broadbandJourneyService.GetBroadbandJourneyDetails().Customer;
            Guard.Against<ArgumentException>(customer == null, $"{nameof(customer)} is null");
            // ReSharper disable once PossibleNullReferenceException
            string postCode = customer.PostcodeEntered;

            try
            {
                List<BTAddress> response = await _broadbandProductsServiceWrapper.GetAddressesForPostcode(postCode);
                return LineCheckerMapper.MapBTAddressListToAddressViewModelList(response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occured in broadband product service for {postCode} {ex.Message}", ex);
            }

            return new List<AddressViewModel>();
        }

        public async Task<bool?> IsProductAvailable(SelectAddressViewModel selectAddressViewModel, List<AddressViewModel> addresses)
        {
            var selectedAddress = new AddressViewModel();
            BroadbandJourneyDetails broadbandJourneyDetails = _broadbandJourneyService.GetBroadbandJourneyDetails();
            Guard.Against<ArgumentException>(broadbandJourneyDetails.Customer == null, $"{nameof(broadbandJourneyDetails.Customer)} is null");

            // ReSharper disable once PossibleNullReferenceException
            if (broadbandJourneyDetails.Customer.IsUserFromHubPage && broadbandJourneyDetails.Customer.SelectedProductGroup == BroadbandProductGroup.FixAndFibreV2)
            {
                return null;
            }

            try
            {
                selectedAddress = addresses.Find(a => a.Id == selectAddressViewModel.SelectedAddressId);
                BTAddress address = LineCheckerMapper.MapAddressViewModelToBTAddress(selectedAddress);

                List<BroadbandProduct> listOfProducts = await GetBroadbandProducts(selectedAddress, broadbandJourneyDetails.Customer);

                if (listOfProducts.Any(x => x.IsAvailable && x.BroadbandType == BroadbandType.Fibre))
                {
                    listOfProducts = listOfProducts.Where(f => f.BroadbandType != BroadbandType.ADSL).ToList();
                }

                if (listOfProducts.Any())
                {
                    _sessionManager.SetSessionDetails("broadbandProducts", listOfProducts);
                    // ReSharper disable once PossibleNullReferenceException
                    broadbandJourneyDetails.Customer.SelectedAddress = address;

                    _broadbandJourneyService.SetBroadbandJourneyDetails(broadbandJourneyDetails);
                    BroadbandProduct broadbandProduct = listOfProducts
                        .Where(p => p.IsAvailable)
                        .FirstOrDefault(p => p.TalkProducts.Any(t => t.ProductCode == broadbandJourneyDetails.Customer.SelectedProductCode));

                    if (broadbandProduct != null && broadbandJourneyDetails.Customer.IsUserFromHubPage)
                    {
                        broadbandJourneyDetails.Customer.SelectedProduct = broadbandProduct;
                        return true;
                    }

                    if (broadbandJourneyDetails.Customer.SelectedProductGroup == BroadbandProductGroup.FixAndFibreV3)
                    {
                        return null;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occured in broadband product service {selectedAddress.Postcode} {ex.Message}", ex);
            }

            return null;
        }

        public bool IsOpenReachFallout(int selectedAddressId, List<AddressViewModel> addresses)
        {
            BroadbandJourneyDetails broadbandJourneyDetails = _broadbandJourneyService.GetBroadbandJourneyDetails();
            Guard.Against<ArgumentException>(broadbandJourneyDetails.Customer == null, $"{nameof(broadbandJourneyDetails.Customer)} is null");

            bool fallout;

            AddressViewModel selectedAddress = addresses.Find(a => a.Id == selectedAddressId);
            BTAddress address = LineCheckerMapper.MapAddressViewModelToBTAddress(selectedAddress);

            OpenReachData openReachResponse = new OpenReachData();

            try
            {
                // ReSharper disable once PossibleNullReferenceException
                openReachResponse = _newBtLineAvailabilityServiceWrapper.Newbtlineavailability(address, broadbandJourneyDetails.Customer.CliNumber);
            }
            catch (Exception)
            {
                openReachResponse = new OpenReachData {
                    LineavailabilityFlags = new LineAvailability { BackOfficeFile = true, Fallout = false, InstallLine = false },
                    LineStatus = LineStatus.NewConnection, AddressLineKey = "OPENREACHFALLOUTTEMPORARY"};

                //_logger.Error($"Exception occured in NewBtLineAvailabilityService -  {address.FormattedAddressLine1}  - {address.Postcode} - {ex.Message}", ex);
            }

            fallout = openReachResponse.LineavailabilityFlags.Fallout;

            if (!fallout)
            {
                _sessionManager.SetSessionDetails(SessionKeys.OpenReachResponse, openReachResponse);
                if (!string.IsNullOrEmpty(openReachResponse.CLI))
                {
                    broadbandJourneyDetails.Customer.CliNumber = openReachResponse.CLI;
                    broadbandJourneyDetails.Customer.IsSSECustomerCLI = true;
                }
                else
                {
                    broadbandJourneyDetails.Customer.CliNumber = broadbandJourneyDetails.Customer.OriginalCliEntered;
                    broadbandJourneyDetails.Customer.IsSSECustomerCLI = false;
                }

                broadbandJourneyDetails.Customer.ApplyInstallationFee = openReachResponse.LineavailabilityFlags.InstallLine;
                _broadbandJourneyService.SetBroadbandJourneyDetails(broadbandJourneyDetails);
            }

            return fallout;
        }


        public SelectAddressViewModel GetSelectAddressViewModel(List<AddressViewModel> addresses)
        {
            Customer customer = _broadbandJourneyService.GetBroadbandJourneyDetails().Customer;
            Guard.Against<ArgumentException>(customer == null, $"{nameof(customer)} is null");

            return new SelectAddressViewModel
            {
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = "LineChecker",
                    ControllerName = "LineChecker",
                    RouteValues = customer?.SelectedProductCode != null ? new { productcode = customer.SelectedProductCode } : null,
                    TitleAttributeText = Common_Resources.BackButtonAlt
                },
                Addresses = addresses,
                // ReSharper disable once PossibleNullReferenceException
                SelectedAddressId = customer.SelectedAddress?.Id ?? 0,
                LoadingModal = new ModalTitleAndBody
                {
                    Message = SelectAddress_Resources.LoadingPopupBody,
                    Title = string.Empty
                }
            };
        }

        public LineCheckerViewModel GetLineCheckerViewModel(string productCode)
        {
            Customer customer = _broadbandJourneyService.GetBroadbandJourneyDetails()?.Customer;

            return new LineCheckerViewModel
            {
                PhoneNumber = customer?.OriginalCliEntered,
                PostCode = customer?.PostcodeEntered,
                ProductCode = productCode
            };
        }

        public void SetInformationPassedByHub(string productCode)
        {
            BroadbandJourneyDetails broadbandJourneyDetails = _broadbandJourneyService.GetBroadbandJourneyDetails();
            broadbandJourneyDetails.Customer = broadbandJourneyDetails.Customer ?? new Customer();
            broadbandJourneyDetails.Customer.SelectedProductCode = productCode;
            broadbandJourneyDetails.Customer.SelectedProductGroup = _manager.BroadbandProductGroup(productCode);
            broadbandJourneyDetails.Customer.IsUserFromHubPage = !string.IsNullOrWhiteSpace(productCode);
            _broadbandJourneyService.SetBroadbandJourneyDetails(broadbandJourneyDetails);
        }

        public async Task<bool> IsCustomerAlert()
        {
            return await _customerAlertService.IsCustomerAlert(_configManager.GetAppSetting("BroadbandCustomerAlertName"));
        }

        public CannotCompleteOnlineViewModel GetCannotCompleteOnlineViewModel()
        {
            Customer customer = _broadbandJourneyService.GetBroadbandJourneyDetails().Customer;
            Guard.Against<ArgumentException>(customer == null, $"{nameof(customer)} is null");

            return new CannotCompleteOnlineViewModel
            {
                // ReSharper disable once PossibleNullReferenceException
                IsFixAndFibreV3 = customer.SelectedProductGroup == BroadbandProductGroup.FixAndFibreV3
            };
        }

        public void SetLineCheckerDetails(LineCheckerViewModel lineCheckerViewModel)
        {
            Guard.Against<ArgumentException>(lineCheckerViewModel == null, $"{nameof(lineCheckerViewModel)} is null");
            BroadbandJourneyDetails broadbandJourneyDetails = _broadbandJourneyService.GetBroadbandJourneyDetails();
            Guard.Against<ArgumentException>(broadbandJourneyDetails.Customer == null, $"{nameof(broadbandJourneyDetails.Customer)} is null");

            // ReSharper disable once PossibleNullReferenceException
            string cliNumber = string.IsNullOrWhiteSpace(lineCheckerViewModel.PhoneNumber) ? null : Regex.Replace(lineCheckerViewModel.PhoneNumber, @"\s+", "");
            // ReSharper disable once PossibleNullReferenceException
            broadbandJourneyDetails.Customer.CliNumber = cliNumber;
            broadbandJourneyDetails.Customer.PostcodeEntered = lineCheckerViewModel.PostCode?.Trim();
            broadbandJourneyDetails.Customer.OriginalCliEntered = cliNumber;
            _broadbandJourneyService.SetBroadbandJourneyDetails(broadbandJourneyDetails);
        }

        private async Task<List<BroadbandProduct>> GetBroadbandProducts(AddressViewModel selectedAddress, Customer broadbandCustomer)
        {
            BTAddress addressRequest = LineCheckerMapper.MapAddressViewModelToBTAddress(selectedAddress);

            Task<List<Tariff>> allProductsTask = _broadbandProductsServiceWrapper.GetAllTariffs("SSE");
            Task<BroadbandTariffsForAddress> availableProductsTask = _broadbandProductsServiceWrapper.GetAvailableTariffs("SSE", addressRequest, broadbandCustomer.CliNumber);

            BroadbandTariffsForAddress availableProducts = await availableProductsTask;
            if (!availableProducts.Tariffs.Any())
            {
                return new List<BroadbandProduct>();
            }

            List<Tariff> allTariffs = await allProductsTask;
            IEnumerable<Tariff> unavailableTariffs = allTariffs.Where(x => availableProducts.Tariffs.All(y => y.ProductCode != x.ProductCode));

            List<BroadbandProduct> allProducts = GetAvailableProducts(availableProducts);
            allProducts = RemoveInvalidProducts(allProducts, broadbandCustomer.SelectedProductGroup);
            allProducts.AddRange(GetUnavailableProducts(unavailableTariffs));

            return allProducts;
        }

        private List<BroadbandProduct> RemoveInvalidProducts(List<BroadbandProduct> availableProducts, BroadbandProductGroup selectedProductGroup)
        {
            availableProducts.ForEach(x => x.TalkProducts.RemoveAll(p => p.BroadbandProductGroup != selectedProductGroup || p.BroadbandProductGroup == BroadbandProductGroup.NotAvailableOnline));
            availableProducts.Where(x => x.TalkProducts.Count == 0).ToList().ForEach(y => y.IsAvailable = false);

            return availableProducts;
        }

        private List<BroadbandProduct> GetAvailableProducts(BroadbandTariffsForAddress tariffsForAddress)
        {
            var availableProducts = new List<BroadbandProduct>();

            foreach (Tariff tariff in tariffsForAddress.Tariffs)
            {
                BroadbandType broadbandType = _manager.GetBroadbandType(tariff.BroadbandCode);
                BroadbandProduct broadbandProduct = availableProducts.FirstOrDefault(p => p.BroadbandType == broadbandType);

                var talkProduct = new TalkProduct
                {
                    ProductCode = tariff.ProductCode,
                    ProductName = tariff.TariffName,
                    TalkCode = tariff.TalkCode,
                    Prices = tariff.Prices,
                    BroadbandProductGroup = _manager.BroadbandProductGroup(tariff.ProductCode)
                };

                if (broadbandProduct != null)
                {
                    broadbandProduct.TalkProducts.Add(talkProduct);
                    continue;
                }

                broadbandProduct = new BroadbandProduct
                {
                    TalkProducts = new List<TalkProduct> { talkProduct },
                    IsAvailable = true,
                    BroadbandType = broadbandType,
                    ProductOrder = (int) broadbandType,
                    LineSpeed = GetLineSpeed(broadbandType, tariffsForAddress.LineSpeeds)
                };

                availableProducts.Add(broadbandProduct);
            }

            return availableProducts;
        }

        private List<BroadbandProduct> GetUnavailableProducts(IEnumerable<Tariff> unavailableTariffs)
        {
            var unavailableProducts = new List<BroadbandProduct>();

            foreach (Tariff tariff in unavailableTariffs)
            {
                BroadbandType broadbandType = _manager.GetBroadbandType(tariff.BroadbandCode);
                BroadbandProduct broadbandProduct = unavailableProducts.FirstOrDefault(p => p.BroadbandType == broadbandType);

                if (broadbandProduct == null)
                {
                    unavailableProducts.Add(new BroadbandProduct
                    {
                        IsAvailable = false,
                        BroadbandType = broadbandType,
                        ProductOrder = (int) broadbandType
                    });
                }
            }

            return unavailableProducts;
        }

        private LineSpeed GetLineSpeed(BroadbandType broadbandType, IEnumerable<LineSpeed> lineSpeeds)
        {
            switch (broadbandType)
            {
                case BroadbandType.ADSL:
                    LineSpeed adslLineSpeed = lineSpeeds.FirstOrDefault(x => x.Type == BroadbandType.ADSL);
                    return adslLineSpeed;
                case BroadbandType.Fibre:
                case BroadbandType.FibrePlus:
                    LineSpeed fiberLineSpeed = lineSpeeds.FirstOrDefault(x => x.Type == BroadbandType.Fibre);
                    return fiberLineSpeed;
                default:
                    return null;
            }
        }
    }
}