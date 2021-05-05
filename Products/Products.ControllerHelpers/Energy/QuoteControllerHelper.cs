namespace Products.ControllerHelpers.Energy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Infrastructure.Logging;
    using Model.Common;
    using Model.Constants;
    using Model.Energy;
    using Model.Enums;
    using Service.Common;
    using Service.Energy;
    using ServiceWrapper.CAndCService;
    using ServiceWrapper.EnergyProductService;
    using ServiceWrapper.QASService;
    using WebModel.Resources.Common;
    using WebModel.Resources.Energy;
    using WebModel.ViewModels.Common;
    using WebModel.ViewModels.Energy;
    using QASMapper = Mappers.QASMapper;

    public class QuoteControllerHelper : BaseEnergyControllerHelper, IQuoteControllerHelper
    {
        private readonly IPostcodeCheckerService _postcodeCheckerService;
        private readonly ILogger _logger;
        private readonly IQASServiceWrapper _qasServiceWrapper;
        private readonly ICAndCServiceWrapper _cAndCServiceWrapper;
        private readonly IEnergyAlertService _energyAlertService;
        private readonly IEnergyProductServiceWrapper _energyProductServiceWrapper;

        public QuoteControllerHelper(
            IPostcodeCheckerService postcodeCheckerService,
            IQASServiceWrapper qasServiceWrapper,
            ILogger logger,
            ICAndCServiceWrapper cAndCServiceWrapper,
            IEnergyAlertService energyAlertService,
            IEnergyProductServiceWrapper energyProductServiceWrapper,
            ISessionManager sessionManager,
            IConfigManager configManager)
            : base(sessionManager, configManager)
        {
            Guard.Against<ArgumentException>(postcodeCheckerService == null, $"{nameof(postcodeCheckerService)} is null");
            Guard.Against<ArgumentException>(qasServiceWrapper == null, $"{nameof(qasServiceWrapper)} is null");
            Guard.Against<ArgumentException>(logger == null, $"{nameof(logger)} is null");
            Guard.Against<ArgumentException>(cAndCServiceWrapper == null, $"{nameof(cAndCServiceWrapper)} is null");
            Guard.Against<ArgumentException>(energyAlertService == null, $"{nameof(energyAlertService)} is null");
            Guard.Against<ArgumentException>(energyProductServiceWrapper == null, $"{nameof(energyProductServiceWrapper)} is null");

            _postcodeCheckerService = postcodeCheckerService;
            _qasServiceWrapper = qasServiceWrapper;
            _logger = logger;
            _cAndCServiceWrapper = cAndCServiceWrapper;
            _energyAlertService = energyAlertService;
            _energyProductServiceWrapper = energyProductServiceWrapper;
        }

        public void SaveCustomer(PostcodeViewModel model)
        {
            var energyCustomer = SessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            if (model.OurPrices || energyCustomer == null)
            {
                energyCustomer = new EnergyCustomer();
            }

            energyCustomer.Postcode = model.PostCode.Trim().ToUpper();
            energyCustomer.CLIChoice.UserProvidedCLI = model.Landline?.Replace(" ", string.Empty);
            energyCustomer.IsBundlingJourney = model.IsBundle;
            SetChosenProductForCustomer(energyCustomer, model.ProductCode);

            ResetCustomer(energyCustomer);
        }

        public void SetShowCliInSession(bool showCli)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            energyCustomer.ShowCli = showCli;
            SaveEnergyCustomerInSession(energyCustomer);
        }

        public void SetBundlingJourney(bool isBundlingJourney)
        {
            var customer = new EnergyCustomer
            {
                IsBundlingJourney = isBundlingJourney
            };

            SaveEnergyCustomerInSession(customer);
        }

        public bool IsNorthernIrelandPostcode(string postcode)
        {
            return _postcodeCheckerService.IsNorthernIrelandPostcode(postcode.Trim());
        }

        public async Task<bool?> IsValidPostCode(string postcode)
        {
            try
            {
                string geoArea = await _energyProductServiceWrapper.GetGeoAreaForPostCode(postcode.Trim());
                return !string.IsNullOrEmpty(geoArea);
            }
            catch (Exception e)
            {
                _logger.Error("Postcode validation API check failed.", e);
            }

            return null;
        }

        public PostcodeViewModel GetEnergyPostcodeViewModel()
        {
            EnergyCustomer energyCustomer = SessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer) ?? new EnergyCustomer();
            SessionManager.SetSessionDetails(SessionKeys.EnergyCustomer, energyCustomer);

            return new PostcodeViewModel
            {
                PostCode = energyCustomer.Postcode,
                Landline = energyCustomer.CLIChoice.UserProvidedCLI,
                Header = Postcode_Resources.Header,
                SubmitButtonText = Resources.ButtonNextText,
                SubmitButtonTitle = Common_Resources.QuoteButtonNextAlt,
                ShowLandline = energyCustomer.ShowCli,
                IsBundle = energyCustomer.IsBundlingJourney,
                ProductCode = energyCustomer.ChosenProduct
            };
        }

        public async Task<bool> IsEnergyCustomerAlert()
        {
            return await _energyAlertService.IsEnergyCustomerAlert();
        }

        public async Task<bool> IsOurPricesCustomerAlert()
        {
            return await _energyAlertService.IsOurPricesCustomerAlert();
        }

        public SelectFuelViewModel GetSelectFuelViewModel()
        {
            return new SelectFuelViewModel
            {
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = "SelectAddress",
                    ControllerName = "Quote",
                    TitleAttributeText = Resources.BackButtonAlt
                }
            };
        }

        public SelectFuelViewModel SetSelectedFuel(SelectFuelViewModel model)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();

            // ReSharper disable once PossibleInvalidOperationException
            // ReSharper disable once PossibleNullReferenceException
            energyCustomer.SelectedFuelType = model.FuelType.Value;

            if (model.FuelType.Value == FuelType.Gas)
            {
                energyCustomer.SelectedElectricityMeterType = ElectricityMeterType.None;
            }

            CAndCRedirectRoute cAndCRedirectRoute = GetCAndCRedirectRoute(energyCustomer);

            if (energyCustomer.IsCAndCJourney())
            {
                if (cAndCRedirectRoute == CAndCRedirectRoute.Usage)
                {
                    energyCustomer.SelectedPaymentMethod = PaymentMethod.PayAsYouGo;
                    energyCustomer.SelectedBillingPreference = BillingPreference.Paper;
                }
                energyCustomer.SelectedElectricityMeterType = energyCustomer.GetElectricityMeterInformation()?.ElectricityMeterType ?? ElectricityMeterType.Standard;
            }

            return new SelectFuelViewModel
            {
                CAndCRedirectRoute = cAndCRedirectRoute
            };
        }

        public SelectPaymentMethodViewModel GetSelectPaymentMethodViewModel()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();

            var viewModel = new SelectPaymentMethodViewModel
            {
                PaymentMethods = new RadioButtonList
                {
                    Items = new List<RadioButton>(),
                    SelectedValue = "SelectedPaymentMethodId"
                },

                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = "SelectFuel",
                    ControllerName = "Quote",
                    TitleAttributeText = Resources.BackButtonAlt
                }
            };

            viewModel.PaymentMethods.Items.Add(new RadioButton
            {
                Value = PaymentMethod.MonthlyDirectDebit.ToString(),
                // ReSharper disable once PossibleNullReferenceException
                Checked = energyCustomer.SelectedPaymentMethod == PaymentMethod.MonthlyDirectDebit,
                DisplayText = PaymentMethod_Resources.RadioHeading1,
                DescriptiveText = PaymentMethod_Resources.RadioDescription1,
                AriaDescription = PaymentMethod_Resources.RadioAriaDescription1,
                RightHandTextWithLink = PaymentMethod_Resources.RightHandTextWithLink,
                RightHandTextWithLinkUrl = PaymentMethod_Resources.RightHandTextWithLinkUrl,
                RightHandTextWithLinkAlt = PaymentMethod_Resources.RightHandTextWithLinkAlt,
                ModalInfo = new Modal
                {
                    ModalId = "DDModalId",
                    ModalHeader = PaymentMethod_Resources.RadioModalHeader1,
                    ModalMessage = $"{string.Format(Common_Resources.DDPara1, Common_Resources.SingleFuelDiscount)}{Common_Resources.DDPara2}{Common_Resources.DDPara3}"
                }
            });
            viewModel.PaymentMethods.Items.Add(new RadioButton
            {
                Value = PaymentMethod.Quarterly.ToString(),
                Checked = energyCustomer.SelectedPaymentMethod == PaymentMethod.Quarterly,
                DisplayText = PaymentMethod_Resources.RadioHeading2,
                DescriptiveText = PaymentMethod_Resources.RadioDescription2,
                AriaDescription = PaymentMethod_Resources.RadioAriaDescription2
            });

            if (!energyCustomer.IsCAndCJourney() || energyCustomer.IsSmartMeterSmets2())
            {
                viewModel.PaymentMethods.Items.Add(new RadioButton
                {
                    Value = PaymentMethod.PayAsYouGo.ToString(),
                    Checked = energyCustomer.SelectedPaymentMethod == PaymentMethod.PayAsYouGo,
                    DisplayText = PaymentMethod_Resources.RadioHeading3,
                    DescriptiveText = PaymentMethod_Resources.RadioDescription3,
                    AriaDescription = PaymentMethod_Resources.RadioAriaDescription3
                });
            }

            return viewModel;
        }

        public void SaveSelectedPaymentMethod(SelectPaymentMethodViewModel viewModel)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();

            if (viewModel.SelectedPaymentMethodId != null)
            {
                energyCustomer.SelectedPaymentMethod = viewModel.SelectedPaymentMethodId.Value;
                energyCustomer.DirectDebitDetails = null;
            }

            SaveEnergyCustomerInSession(energyCustomer);
        }

        public bool ShowSmartMeterTypeQuestionInNonCAndCJourney()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            return energyCustomer.IsGasOnly() && !energyCustomer.IsCAndCJourney();
        }

        public bool ShowSmartFrequency()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            return energyCustomer.IsSmartMeter();
        }

        public bool ShowUsageScreenForCAndCJourneyNonSmartCustomer()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            return !energyCustomer.IsSmartMeter() && energyCustomer.IsCAndCJourney();
        }

        public void SetBillingPreference()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            energyCustomer.SelectedBillingPreference = energyCustomer.IsPrePay() ? BillingPreference.Paper : BillingPreference.Paperless;
            SaveEnergyCustomerInSession(energyCustomer);
        }

        public SelectMeterTypeViewModel GetSelectMeterTypeViewModel()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();

            var viewModel = new SelectMeterTypeViewModel
            {
                MeterTypes = new RadioButtonList
                {
                    Items = new List<RadioButton>(),
                    SelectedValue = nameof(SelectMeterTypeViewModel.SelectedElectricityMeterType)
                },
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = "PaymentMethod",
                    ControllerName = "Quote",
                    TitleAttributeText = Resources.BackButtonAlt
                }
            };

            viewModel.MeterTypes.Items.Add(new RadioButton
            {
                Value = ElectricityMeterType.Standard.ToString(),
                Checked = energyCustomer.SelectedElectricityMeterType == ElectricityMeterType.Standard,
                DisplayText = MeterType_Resources.RadioHeading1,
                DescriptiveText = MeterType_Resources.RadioDescription1,
                AriaDescription = MeterType_Resources.RadioAriaDescription1
            });
            viewModel.MeterTypes.Items.Add(new RadioButton
            {
                Value = ElectricityMeterType.Economy7.ToString(),
                Checked = energyCustomer.SelectedElectricityMeterType == ElectricityMeterType.Economy7,
                DisplayText = MeterType_Resources.RadioHeading2,
                DescriptiveText = MeterType_Resources.RadioDescription2,
                AriaDescription = MeterType_Resources.RadioAriaDescription2
            });
            viewModel.MeterTypes.Items.Add(new RadioButton
            {
                Value = ElectricityMeterType.Other.ToString(),
                Checked = energyCustomer.SelectedElectricityMeterType == ElectricityMeterType.Other,
                DisplayText = MeterType_Resources.RadioHeading3,
                DescriptiveText = MeterType_Resources.RadioDescription3,
                AriaDescription = MeterType_Resources.RadioAriaDescription3
            });

            return viewModel;
        }

        public SelectSmartMeterViewModel GetSelectSmartMeterViewModel()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();

            var viewModel = new SelectSmartMeterViewModel
            {
                SmartMeter = new RadioButtonList
                {
                    Items = new List<RadioButton>(),
                    SelectedValue = nameof(SelectSmartMeterViewModel.HasSmartMeter)
                },
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = energyCustomer.IsGasOnly() ? "PaymentMethod" : "MeterType",
                    ControllerName = "Quote",
                    TitleAttributeText = Resources.BackButtonAlt
                }
            };

            viewModel.SmartMeter.Items.Add(new RadioButton
            {
                Value = "true",
                // ReSharper disable once PossibleNullReferenceException
                Checked = energyCustomer.HasSmartMeter ?? false,
                DisplayText = SmartMeter_Resources.RadioHeading1,
                DescriptiveText = GetRadio1Description(energyCustomer),
                AriaDescription = SmartMeter_Resources.RadioAriaDescription1
            });
            viewModel.SmartMeter.Items.Add(new RadioButton
            {
                Value = "false",
                Checked = energyCustomer.HasSmartMeter.HasValue && !energyCustomer.HasSmartMeter.Value,
                DisplayText = SmartMeter_Resources.RadioHeading2,
                DescriptiveText = SmartMeter_Resources.RadioDescription2,
                AriaDescription = SmartMeter_Resources.RadioAriaDescription2
            });

            return viewModel;
        }

        public SmartMeterFrequencyViewModel GetSmartMeterFrequencyViewModel()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();

            var viewModel = new SmartMeterFrequencyViewModel
            {
                SmartMeterReadingFrequency = new RadioButtonList
                {
                    Items = new List<RadioButton>(),
                    SelectedValue = "SmartMeterFrequencyId"
                },
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = "PaymentMethod",
                    ControllerName = "Quote",
                    TitleAttributeText = Resources.BackButtonAlt
                },
                SmartMeterFrequencyEnabled = energyCustomer.AskSmartMeterFrequency()
            };

            viewModel.SmartMeterReadingFrequency.Items.Add(new RadioButton
            {
                Value = SmartMeterFrequency.HalfHourly.ToString(),
                Checked = energyCustomer.SmartMeterFrequency == SmartMeterFrequency.HalfHourly,
                DisplayText = SmartMeter_Resources.SmartMeterRadioHeading,
                RightHandText = SmartMeter_Resources.SmartMeterRadioDescription,
                AriaDescription = SmartMeter_Resources.SmartMeterRadioAriaDescription1,
                DataValRequiredIf = SmartMeter_Resources.SmartMeterFrequencyRequiredError,
                DataValRequiredIfDesiredValue = "true",
                DataValRequiredIfDependencyProperty = "SmartMeterFrequencyEnabled"
            });
            viewModel.SmartMeterReadingFrequency.Items.Add(new RadioButton
            {
                Value = SmartMeterFrequency.Daily.ToString(),
                Checked = energyCustomer.SmartMeterFrequency == SmartMeterFrequency.Daily,
                DisplayText = SmartMeter_Resources.SmartMeterRadioHeadingDaily,
                AriaDescription = SmartMeter_Resources.SmartMeterRadioAriaDescription2,
                DataValRequiredIf = SmartMeter_Resources.SmartMeterFrequencyRequiredError,
                DataValRequiredIfDesiredValue = "true",
                DataValRequiredIfDependencyProperty = "SmartMeterFrequencyEnabled"
            });
            viewModel.SmartMeterReadingFrequency.Items.Add(new RadioButton
            {
                Value = SmartMeterFrequency.Monthly.ToString(),
                Checked = energyCustomer.SmartMeterFrequency == SmartMeterFrequency.Monthly,
                DisplayText = SmartMeter_Resources.SmartMeterRadioHeadingMonthly,
                AriaDescription = SmartMeter_Resources.SmartMeterRadioAriaDescription3,
                DataValRequiredIf = SmartMeter_Resources.SmartMeterFrequencyRequiredError,
                DataValRequiredIfDesiredValue = "true",
                DataValRequiredIfDependencyProperty = "SmartMeterFrequencyEnabled"
            });

            return viewModel;
        }

        public void SetSelectedMeterType(SelectMeterTypeViewModel viewModel)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            // ReSharper disable once PossibleInvalidOperationException
            energyCustomer.SelectedElectricityMeterType = energyCustomer.HasElectricity() ? viewModel.SelectedElectricityMeterType.Value : ElectricityMeterType.None;
        }

        public void SetHasSmartMeter(SelectSmartMeterViewModel viewModel)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            // ReSharper disable once PossibleInvalidOperationException
            energyCustomer.HasSmartMeter = viewModel.HasSmartMeter.Value;
        }

        public bool IsAMeterTypeFallout(SelectMeterTypeViewModel model)
        {
            return model.SelectedElectricityMeterType == ElectricityMeterType.Other;
        }

        public void SaveSelectedSmartMeterFrequency(SmartMeterFrequencyViewModel viewModel)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();

            if (!string.IsNullOrWhiteSpace(viewModel.SmartMeterFrequencyId))
            {
                Enum.TryParse(viewModel.SmartMeterFrequencyId, out SmartMeterFrequency smartMeterFrequency);
                energyCustomer.SmartMeterFrequency = smartMeterFrequency;
                SaveEnergyCustomerInSession(energyCustomer);
            }
        }

        public async Task<SelectAddressViewModel> GetSelectAddressViewModel()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();

            var addresses = new List<KeyValuePair<string, string>>();
            var qasEnabled = Convert.ToBoolean(ConfigManager.GetAppSetting("QASEnabled"));

            if (qasEnabled)
            {
                try
                {
                    addresses = await _qasServiceWrapper.GetAddressByPostcode(energyCustomer.Postcode);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Exception while calling QAS for postcode {energyCustomer.Postcode}", ex);
                }
            }

            bool hasValidAddress = addresses.Any(a => !string.IsNullOrEmpty(a.Key));
            var selectAddressViewModel = new SelectAddressViewModel
            {
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = "EnterPostcode",
                    ControllerName = "Quote",
                    TitleAttributeText = Resources.BackButtonAlt
                },
                Addresses = addresses.Select(p => new KeyValuePair<string, string>($"{p.Key}~~{p.Value}", p.Value.ToLower())).ToList(),
                Postcode = energyCustomer.Postcode,
                HasValidAddress = hasValidAddress,
                IsManual = !hasValidAddress,
                QASEnabled = qasEnabled
            };

            if (selectAddressViewModel.HasValidAddress)
            {
                if (selectAddressViewModel.Addresses.Count > 1)
                {
                    selectAddressViewModel.HeaderText = SelectAddressCommon_Resources.Header;
                    selectAddressViewModel.ParaText = SelectAddressCommon_Resources.Paragraph;
                    selectAddressViewModel.SubHeaderText = $"{SelectAddressCommon_Resources.SubHeader} {energyCustomer.Postcode}";
                }
                else
                {
                    selectAddressViewModel.HeaderText = SelectAddressCommon_Resources.SingleAddressHeader;
                }
            }
            else
            {
                selectAddressViewModel.HeaderText = SelectAddressCommon_Resources.Header;
                selectAddressViewModel.SubHeaderText = $"{SelectAddressCommon_Resources.SubHeader} {energyCustomer.Postcode}";
            }

            if (energyCustomer.SelectedAddress != null)
            {
                if (!string.IsNullOrEmpty(energyCustomer.SelectedAddress.Moniker))
                {
                    selectAddressViewModel.SelectedAddressId = energyCustomer.SelectedAddress.Moniker;
                }
                else
                {
                    selectAddressViewModel.IsManual = true;
                    selectAddressViewModel.PropertyNumber = energyCustomer.SelectedAddress.HouseName;
                    selectAddressViewModel.AddressLine1 = energyCustomer.SelectedAddress.AddressLine1;
                    selectAddressViewModel.AddressLine2 = energyCustomer.SelectedAddress.AddressLine2;
                    selectAddressViewModel.Town = energyCustomer.SelectedAddress.Town;
                    selectAddressViewModel.County = energyCustomer.SelectedAddress.County;
                }
            }

            return selectAddressViewModel;
        }

        public async Task<bool> SetSelectedAddress(string addressMoniker, string addressPickListEntry)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();

            QasAddress qasAddress = null;

            try
            {
                qasAddress = await _qasServiceWrapper.GetAddressByMoniker(addressMoniker);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception while calling QAS for postcode {energyCustomer.Postcode}", ex);
            }

            if (qasAddress != null && energyCustomer != null)
            {
                energyCustomer.SelectedAddress = qasAddress;
                energyCustomer.SelectedAddress.Moniker = addressMoniker;
                energyCustomer.SelectedAddress.PicklistEntry = addressPickListEntry;
                SaveEnergyCustomerInSession(energyCustomer);
                return true;
            }

            return false;
        }

        public void SetManualAddress(SelectAddressViewModel viewModel)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();

            if (energyCustomer != null)
            {
                energyCustomer.SelectedAddress = QASMapper.MapSelectAddressViewModelToQasAddress(viewModel);
                ResetCustomer(energyCustomer);
                SaveEnergyCustomerInSession(energyCustomer);
            }
        }

        public async Task<bool> ProcessAddress()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            bool retVal = true;

            if (energyCustomer != null)
            {
                MeterDetail meterDetail = null;
                try
                {
                    meterDetail = await _cAndCServiceWrapper.GetMeterDetail(energyCustomer.Postcode, energyCustomer.SelectedAddress);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Exception while calling C&C service for postcode {energyCustomer.Postcode} and House Number {energyCustomer.SelectedAddress.HouseName}", ex);
                }

                energyCustomer.MeterDetail = meterDetail;
                if (energyCustomer.MeterDetail != null && string.IsNullOrEmpty(energyCustomer.MeterDetail?.GeographicalArea))
                {
                    retVal = false;
                }
            }

            return retVal;
        }

        public void ResetCustomer()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            ResetCustomer(energyCustomer);
        }

        public void SaveDetailsFromHub(ExistingCustomerViewModel viewModel)
        {
            var energyCustomer = new EnergyCustomer
            {
                IsBundlingJourney = viewModel.IsBundlingJourney,
                ShowCli = viewModel.ShowCli
            };
            SetChosenProductForCustomer(energyCustomer, viewModel.ProductCode);

            SaveEnergyCustomerInSession(energyCustomer);
        }

        private static void SetChosenProductForCustomer(EnergyCustomer energyCustomer, string productCode)
        {
            energyCustomer.ChosenProduct = productCode?.Replace('_', ' ').TrimEconomyWording();
        }

        private static CAndCRedirectRoute GetCAndCRedirectRoute(EnergyCustomer energyCustomer)
        {
            if (energyCustomer.IsCAndCJourney() && energyCustomer.SelectedFuelType != FuelType.Gas)
            {
                MeterInformation meterDetails = energyCustomer.GetElectricityMeterInformation();
                if (meterDetails?.ElectricityMeterType == ElectricityMeterType.Other)
                {
                    return CAndCRedirectRoute.MultiRateMeterFallout;
                }
            }

            if (energyCustomer.IsMeterDetailsPayGo() && !energyCustomer.IsSmartMeter())
            {
                return CAndCRedirectRoute.Usage;
            }

            return CAndCRedirectRoute.PaymentMethod;
        }

        private static string GetRadio1Description(EnergyCustomer energyCustomer)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (energyCustomer.SelectedPaymentMethod)
            {
                case PaymentMethod.MonthlyDirectDebit:
                case PaymentMethod.Quarterly:
                    return SmartMeter_Resources.RadioDescription1;
                case PaymentMethod.PayAsYouGo:
                    return SmartMeter_Resources.RadioDescription1PrePay;
                default:
                    return string.Empty;
            }
        }

        private void ResetCustomer(EnergyCustomer energyCustomer)
        {
            energyCustomer.UnavailableBundles = new List<string>();
            energyCustomer.SelectedBroadbandProduct = null;
            energyCustomer.SelectedBroadbandProductCode = null;
            energyCustomer.SelectedTariff = null;
            energyCustomer.SelectedBTAddress = null;
            SaveEnergyCustomerInSession(energyCustomer);
        }
    }
}