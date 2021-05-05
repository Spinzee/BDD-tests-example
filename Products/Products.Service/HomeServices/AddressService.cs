using Products.Infrastructure;
using Products.Infrastructure.Logging;
using Products.Model.Common;
using Products.Model.Constants;
using Products.Model.Enums;
using Products.Model.HomeServices;
using Products.ServiceWrapper.QASService;
using Products.WebModel.Resources.Common;
using Products.WebModel.Resources.HomeServices;
using Products.WebModel.ViewModels.Common;
using Products.WebModel.ViewModels.HomeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Products.Service.HomeServices
{
    public class AddressService : IAddressService
    {
        private readonly ISessionManager _sessionManager;
        private readonly ILogger _logger;
        private readonly IQASServiceWrapper _qasServiceWrapper;
        private readonly IConfigManager _configManager;

        public AddressService(ISessionManager sessionManager,
            IQASServiceWrapper qasServiceWrapper,
            ILogger logger,
            IConfigManager configManager)
        {
            Guard.Against<ArgumentException>(sessionManager == null, $"{nameof(sessionManager)} is null");
            Guard.Against<ArgumentException>(logger == null, $"{nameof(logger)} is null");
            Guard.Against<ArgumentException>(qasServiceWrapper == null, $"{nameof(qasServiceWrapper)} is null");
            Guard.Against<ArgumentException>(configManager == null, $"{nameof(configManager)} is null");

            _qasServiceWrapper = qasServiceWrapper;
            _sessionManager = sessionManager;
            _logger = logger;
            _configManager = configManager;
        }

        public async Task<SelectAddressViewModel> GetSelectAddressViewModel(AddressTypes addressType)
        {
            var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            Guard.Against<Exception>(homeServicesCustomer == null, "Home Services customer session object is null");

            List<KeyValuePair<string, string>> addresses = new List<KeyValuePair<string, string>>();

            var selectAddressPostCode = addressType == AddressTypes.Cover ? homeServicesCustomer.CoverPostcode : homeServicesCustomer.BillingPostcode;
            var qasEnabled = Convert.ToBoolean(_configManager.GetAppSetting("QASEnabled"));
            if (qasEnabled)
            {
                try
                {
                    addresses = await _qasServiceWrapper.GetAddressByPostcode(selectAddressPostCode);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Exception while calling QAS for postcode {selectAddressPostCode}", ex);
                }
            }

            var hasValidAddress = addresses.Any(a => !string.IsNullOrEmpty(a.Key));
            var selectAddressViewModel = new SelectAddressViewModel
            {
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = addressType == AddressTypes.Billing ? "LandlordBillingPostcode" : "PersonalDetails",
                    ControllerName = "HomeServices",
                    TitleAttributeText = Resources.BackButtonAlt
                },
                Addresses = addresses.Select(p => new KeyValuePair<string, string>(p.Key, p.Value.ToLower())).ToList(),
                Postcode = selectAddressPostCode,
                HasValidAddress = hasValidAddress,
                IsManual = !hasValidAddress,
                AddressType = addressType,
                QASEnabled = qasEnabled
            };

            if (selectAddressViewModel.HasValidAddress)
            {
                if (selectAddressViewModel.Addresses.Count > 1)
                {
                    selectAddressViewModel.HeaderText = GetSelectAddressHeaderText(homeServicesCustomer.IsLandlord, addressType);
                    selectAddressViewModel.ParaText = GetSelectAddressParagraphText(homeServicesCustomer.IsLandlord, addressType);
                    selectAddressViewModel.SubHeaderText = $"{SelectAddressCommon_Resources.SubHeader} {selectAddressPostCode}";
                }
                else
                {
                    selectAddressViewModel.HeaderText = SelectAddressCommon_Resources.SingleAddressHeader;
                }
            }
            else
            {
                selectAddressViewModel.HeaderText = GetSelectAddressHeaderText(homeServicesCustomer.IsLandlord, addressType);
                selectAddressViewModel.SubHeaderText = $"{SelectAddressCommon_Resources.SubHeader} {selectAddressPostCode}";
            }

            SetAddressDetails(addressType, homeServicesCustomer, selectAddressViewModel);

            return selectAddressViewModel;
        }

        private static void SetAddressDetails(AddressTypes addressType, HomeServicesCustomer homeServicesCustomer, SelectAddressViewModel selectAddressViewModel)
        {
            if (addressType == AddressTypes.Cover && homeServicesCustomer.SelectedCoverAddress != null)
            {
                if (!string.IsNullOrEmpty(homeServicesCustomer.SelectedCoverAddress.Moniker))
                {
                    selectAddressViewModel.SelectedAddressId = homeServicesCustomer.SelectedCoverAddress.Moniker;
                }
                else
                {
                    selectAddressViewModel.IsManual = true;
                    selectAddressViewModel.PropertyNumber = homeServicesCustomer.SelectedCoverAddress.HouseName;
                    selectAddressViewModel.AddressLine1 = homeServicesCustomer.SelectedCoverAddress.AddressLine1;
                    selectAddressViewModel.AddressLine2 = homeServicesCustomer.SelectedCoverAddress.AddressLine2;
                    selectAddressViewModel.Town = homeServicesCustomer.SelectedCoverAddress.Town;
                    selectAddressViewModel.County = homeServicesCustomer.SelectedCoverAddress.County;
                }
            }

            if (addressType == AddressTypes.Billing && homeServicesCustomer.SelectedBillingAddress != null)
            {
                if (!string.IsNullOrEmpty(homeServicesCustomer.SelectedBillingAddress.Moniker))
                {
                    selectAddressViewModel.SelectedAddressId = homeServicesCustomer.SelectedBillingAddress.Moniker;
                }
                else
                {
                    selectAddressViewModel.IsManual = true;
                    selectAddressViewModel.PropertyNumber = homeServicesCustomer.SelectedBillingAddress.HouseName;
                    selectAddressViewModel.AddressLine1 = homeServicesCustomer.SelectedBillingAddress.AddressLine1;
                    selectAddressViewModel.AddressLine2 = homeServicesCustomer.SelectedBillingAddress.AddressLine2;
                    selectAddressViewModel.Town = homeServicesCustomer.SelectedBillingAddress.Town;
                    selectAddressViewModel.County = homeServicesCustomer.SelectedBillingAddress.County;
                }
            }
        }

        private string GetSelectAddressHeaderText(bool isLandlord, AddressTypes addressType)
        {
            if (!isLandlord && addressType == AddressTypes.Cover)
            {
                return SelectAddress_Resources.HeaderResidential;
            }

            if (isLandlord && addressType == AddressTypes.Cover)
            {
                return SelectAddress_Resources.HeaderLandlordCover;
            }

            if (isLandlord && addressType == AddressTypes.Billing)
            {
                return SelectAddress_Resources.HeaderLandlordBilling;
            }

            return string.Empty;
        }

        private string GetSelectAddressParagraphText(bool isLandlord, AddressTypes addressType)
        {
            if (!isLandlord && addressType == AddressTypes.Cover)
            {
                return SelectAddress_Resources.ParagraphResidential;
            }

            if (isLandlord && addressType == AddressTypes.Cover)
            {
                return SelectAddress_Resources.ParagraphLandlordCover;
            }

            if (isLandlord && addressType == AddressTypes.Billing)
            {
                return SelectAddress_Resources.ParagraphLandlordBilling;
            }

            return string.Empty;
        }

        public void SetManualAddress(SelectAddressViewModel model)
        {
            var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            Guard.Against<ArgumentException>(homeServicesCustomer == null, "Home Services Customer session object is null");

            if (model.AddressType == AddressTypes.Cover)
            {
                homeServicesCustomer.SelectedCoverAddress = new QasAddress
                {
                    HouseName = model.PropertyNumber.Trim(),
                    AddressLine1 = model.AddressLine1.Trim(),
                    AddressLine2 = !string.IsNullOrWhiteSpace(model.AddressLine2) ? model.AddressLine2.Trim() : string.Empty,
                    Town = model.Town.Trim(),
                    County = !string.IsNullOrWhiteSpace(model.County) ? model.County.Trim() : string.Empty,
                    Moniker = null
                };
            }
            else if (model.AddressType == AddressTypes.Billing)
            {
                homeServicesCustomer.SelectedBillingAddress = new QasAddress
                {
                    HouseName = model.PropertyNumber.Trim(),
                    AddressLine1 = model.AddressLine1.Trim(),
                    AddressLine2 = !string.IsNullOrWhiteSpace(model.AddressLine2) ? model.AddressLine2.Trim() : string.Empty,
                    Town = model.Town.Trim(),
                    County = !string.IsNullOrWhiteSpace(model.County) ? model.County.Trim() : string.Empty,
                    Moniker = null
                };
            }
        }

        public async Task<bool> SetSelectedAddressByMoniker(SelectAddressViewModel model)
        {
            var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            Guard.Against<ArgumentException>(homeServicesCustomer == null, "Home Services Customer session object is null");

            try
            {
                var qasAddress = await _qasServiceWrapper.GetAddressByMoniker(model.SelectedAddressId);

                if (qasAddress != null)
                {
                    if (model.AddressType == AddressTypes.Cover)
                    {
                        homeServicesCustomer.SelectedCoverAddress = qasAddress;
                        homeServicesCustomer.SelectedCoverAddress.Moniker = model.SelectedAddressId;
                    }
                    else if (model.AddressType == AddressTypes.Billing)
                    {
                        homeServicesCustomer.SelectedBillingAddress = qasAddress;
                        homeServicesCustomer.SelectedBillingAddress.Moniker = model.SelectedAddressId;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception while calling QAS for postcode {homeServicesCustomer.CoverPostcode}", ex);
            }

            return false;
        }
    }
}
