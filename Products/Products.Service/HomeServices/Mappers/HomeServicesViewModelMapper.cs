namespace Products.Service.HomeServices.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Core.Configuration.Settings;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Model.Common;
    using Model.Constants;
    using Model.Enums;
    using Model.HomeServices;
    using WebModel.Resources.Common;
    using WebModel.Resources.HomeServices;
    using WebModel.ViewModels.Common;
    using WebModel.ViewModels.HomeServices;

    public class HomeServicesViewModelMapper
    {
        private readonly IConfigManager _configManager;
        private readonly ISessionManager _sessionManager;
        private readonly IConfigurationSettings _settings;

        public HomeServicesViewModelMapper(ISessionManager sessionManager, IConfigManager configManager, IConfigurationSettings settings)
        {
            Guard.Against<ArgumentException>(sessionManager == null, $"{nameof(sessionManager)} is null");
            Guard.Against<ArgumentException>(configManager == null, $"{nameof(configManager)} is null");
            Guard.Against<ArgumentException>(settings == null, $"{nameof(settings)} is null");

            _sessionManager = sessionManager;
            _configManager = configManager;
            _settings = settings;
        }

        public CoverDetailsHeaderViewModel CoverDetailsHeaderViewModel(HomeServicesCustomer homeServicesCustomer)
        {
            Product selectedProduct = homeServicesCustomer.GetSelectedProduct();
            Guard.Against<Exception>(selectedProduct == null,
                $"Home Services selected product object is null for product code - {homeServicesCustomer.SelectedProductCode}");

            return new CoverDetailsHeaderViewModel
            {
                // ReSharper disable once PossibleNullReferenceException
                DisplayName = selectedProduct.Description,
                MonthlyCost = selectedProduct.MonthlyCost.ToString("C"),
                OfferText = selectedProduct.FullOfferText,
                YearlyCost = homeServicesCustomer.GetFormattedTotalYearlyProductCost(),
                HasOffers = !string.IsNullOrEmpty(selectedProduct.FullOfferText?.Trim()),
                IsLandLord = homeServicesCustomer.IsLandlord,
                BackToHomeUrl = _configManager.GetAppSetting(homeServicesCustomer.IsLandlord ? "HomeServicesLandlordHubUrl" : "HomeServicesHubUrl"),
                HasCoverbullet = !string.IsNullOrEmpty(selectedProduct.CoverBulletText?.Trim()),
                CoverbulletText = selectedProduct.CoverBulletText
            };
        }

        public PostcodeViewModel GetEnterPostcodeViewModel(HomeServicesCustomerType customerType, AddressTypes addressType)
        {
            var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);

            return new PostcodeViewModel()
            {
                CustomerType = customerType,
                AddressTypes = addressType,
                HeaderText = GetPostcodeHeaderText(customerType, addressType),
                ParagraphText = GetPostcodeParagraphText(customerType, addressType),
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = addressType == AddressTypes.Billing ? "SelectAddress" : "Postcode",
                    ControllerName = "HomeServices",
                    TitleAttributeText = Resources.BackButtonAlt
                },
                Postcode = addressType == AddressTypes.Billing ? homeServicesCustomer?.BillingPostcode : homeServicesCustomer?.CoverPostcode
            };
        }

        private string GetPostcodeHeaderText(HomeServicesCustomerType customerType, AddressTypes addressType)
        {
            if (customerType == HomeServicesCustomerType.Residential && addressType == AddressTypes.Cover)
            {
                return EnterPostcode_Resources.HeaderResidential;
            }

            if (customerType == HomeServicesCustomerType.Landlord && addressType == AddressTypes.Cover)
            {
                return EnterPostcode_Resources.HeaderLandlordCover;
            }

            if (customerType == HomeServicesCustomerType.Landlord && addressType == AddressTypes.Billing)
            {
                return EnterPostcode_Resources.HeaderLandlordBilling;
            }

            return string.Empty;
        }

        private string GetPostcodeParagraphText(HomeServicesCustomerType customerType, AddressTypes addressType)
        {
            if (customerType == HomeServicesCustomerType.Residential && addressType == AddressTypes.Cover)
            {
                return EnterPostcode_Resources.ParagraphResidential;
            }

            if (customerType == HomeServicesCustomerType.Landlord && addressType == AddressTypes.Cover)
            {
                return EnterPostcode_Resources.ParagraphLandlordCover;
            }

            if (customerType == HomeServicesCustomerType.Landlord && addressType == AddressTypes.Billing)
            {
                return EnterPostcode_Resources.ParagraphLandlordBilling;
            }

            return string.Empty;
        }

        public YourCoverBasketViewModel GetYourCoverBasketViewModel(HomeServicesCustomer homeServicesCustomer)
        {
            Product selectedProduct = homeServicesCustomer.GetSelectedProduct();
            List<ProductExtra> selectedProductExtras = homeServicesCustomer.GetSelectedExtras();

            Guard.Against<Exception>(selectedProduct == null,
                $"Home Services selected product object is null for product code - {homeServicesCustomer.SelectedProductCode}");

            return new YourCoverBasketViewModel
            {
                // ReSharper disable once PossibleNullReferenceException
                ExcessCost = selectedProduct.Excess.ToPounds(),
                HasExcess = homeServicesCustomer.HasExcesses(),
                OffersAmount = selectedProduct.OfferSummary?.Amount.ToPounds(),
                OffersExist = selectedProduct.OfferSummary != null && !string.IsNullOrEmpty(selectedProduct.OfferSummary.Description),
                OffersText = selectedProduct.OfferSummary?.Description,
                HasSelectedExtras = selectedProductExtras != null && selectedProductExtras.Any(),
                ProductMonthlyCost = selectedProduct.MonthlyCost.ToString("C"),
                ProductName = selectedProduct.Description,
                SelectedExtras =
                    selectedProductExtras?.Select(extra => new SelectedExtra { MonthlyCost = extra.Cost.ToString("C"), Name = extra.Name }).ToList() ??
                    new List<SelectedExtra>(),
                TotalMonthlyCost = homeServicesCustomer.GetTotalMonthlyCostWithExtras().ToString("C"),
                YearlyCost = homeServicesCustomer.GetFormattedTotalYearlyCostWithExtras(),
                ContractDuration = selectedProduct.ContractLength.ToString(CultureInfo.InvariantCulture)
            };
        }

        public CoverDetailsViewModel GetCoverDetailsViewModel(HomeServicesCustomer homeServicesCustomer)
        {
            var viewModel = new CoverDetailsViewModel
            {
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = homeServicesCustomer.IsLandlord ? "LandlordPostcode" : "Postcode",
                    ControllerName = "HomeServices",
                    TitleAttributeText = Resources.BackButtonAlt,
                    RouteValues = new { productcode = homeServicesCustomer.SelectedProductCode }
                },
                CoverDetailsHeaderViewModel = CoverDetailsHeaderViewModel(homeServicesCustomer),
                WhatsIncluded = homeServicesCustomer.AvailableProduct?.WhatsIncluded ?? new List<string>(),
                WhatsExcluded = homeServicesCustomer.AvailableProduct?.WhatsExcluded ?? new List<string>(),
                ProductExtrasAvailable = homeServicesCustomer.IsProductExtrasAvailable(),
                ProductExtras = homeServicesCustomer.IsProductExtrasAvailable()
                    ? homeServicesCustomer.AvailableProduct?.Extras
                        .Select(extra => new ProductExtrasViewModel
                        {
                            MonthlyCost = extra.Cost.ToPounds(),
                            ProductCode = extra.ProductCode,
                            ProductTagLine = extra.ProductTagLine,
                            ProductName = extra.Name,
                            IsSelected = homeServicesCustomer.SelectedExtraCodes != null && homeServicesCustomer.SelectedExtraCodes.Contains(extra.ProductCode),
                            WhatsExcluded = extra.WhatsExcluded ?? new List<string>(),
                            WhatsIncluded = extra.WhatsIncluded ?? new List<string>()
                        }).ToList()
                    : new List<ProductExtrasViewModel>(),
                AccordionViewModel = new AccordionViewModel
                {
                    IsLandLord = homeServicesCustomer.IsLandlord,
                    ProductPDFs = GetProductPDFs(homeServicesCustomer.SelectedProductCode),
                    ExtraProductPDFs = homeServicesCustomer.IsProductExtrasAvailable() ? GetExtraProductPDFs(homeServicesCustomer.SelectedProductCode) : new List<ProductPDFViewModel>()
                }
            };

            List<double> excessesValues = homeServicesCustomer.GetExcesses();

            viewModel.HasExcess = homeServicesCustomer.HasExcesses();
            viewModel.SingleExcessAmount = excessesValues.Count == 1 && excessesValues.First() > 0 ? excessesValues.First().ToPounds() : string.Empty;
            viewModel.ExcessesRadioButtonList = excessesValues.Count > 1
                ? GetCoverDetailsExcesses(homeServicesCustomer.AvailableProduct, homeServicesCustomer.SelectedProductCode)
                : null;
            return viewModel;
        }

        private List<ProductPDFViewModel> GetProductPDFs(string selectedProductCode)
        {
            var pdfs = new List<ProductPDFViewModel>();
            switch (selectedProductCode)
            {
                case "BOBC":
                    pdfs.Add(GetProductPDFViewModel("Breakdown"));
                    break;
                case "BOHC":
                    pdfs.Add(GetProductPDFViewModel("Breakdown"));
                    break;
                case "BC":
                    pdfs.Add(GetProductPDFViewModel("Policy"));
                    pdfs.Add(GetProductPDFViewModel("BC50"));
                    break;
                case "BC50":
                    pdfs.Add(GetProductPDFViewModel("Policy"));
                    pdfs.Add(GetProductPDFViewModel("BC"));
                    break;
                case "HC":
                    pdfs.Add(GetProductPDFViewModel("Policy"));
                    pdfs.Add(GetProductPDFViewModel("HC50"));
                    break;
                case "HC50":
                    pdfs.Add(GetProductPDFViewModel("Policy"));
                    pdfs.Add(GetProductPDFViewModel("HC"));
                    break;
                case "LANDHC":
                    pdfs.Add(GetProductPDFViewModel("Policy"));
                    break;
                case "LANDBC":
                    pdfs.Add(GetProductPDFViewModel("Policy"));
                    break;
                case "EC":
                    pdfs.Add(GetProductPDFViewModel("Policy"));
                    break;
            }

            pdfs.Add(GetProductPDFViewModel(selectedProductCode));

            return pdfs;
        }

        private List<ProductPDFViewModel> GetExtraProductPDFs(string selectedProductCode)
        {
            var pdfs = new List<ProductPDFViewModel>();
            switch (selectedProductCode)
            {
                case "BOBC":
                    pdfs.Add(GetProductPDFViewModel("EC"));
                    break;
                case "BOHC":
                    pdfs.Add(GetProductPDFViewModel("EC"));
                    break;
                case "BC":
                    pdfs.Add(GetProductPDFViewModel("EC"));
                    break;
                case "BC50":
                    pdfs.Add(GetProductPDFViewModel("EC"));
                    break;
                case "HC":
                    pdfs.Add(GetProductPDFViewModel("EC"));
                    break;
                case "HC50":
                    pdfs.Add(GetProductPDFViewModel("EC"));
                    break;
                case "LANDBC":
                    pdfs.Add(GetProductPDFViewModel("EC"));
                    break;
                case "LANDHC":
                    pdfs.Add(GetProductPDFViewModel("EC"));
                    break;
            }

            return pdfs;
        }

        private ProductPDFViewModel GetProductPDFViewModel(string key)
        {
            ProductPDFViewModel retVal = null;
            PDFSettings pdfSettings = _settings.HomeServicesSettings.PDFs.FirstOrDefault(x => x.Key == key);

            if (pdfSettings != null)
            {
                retVal = MapPDFSettingsToProductPDFViewModel(pdfSettings);
            }

            return retVal;
        }

        private RadioButtonList GetCoverDetailsExcesses(ProductGroup availableProduct, string selectedProductCode)
        {
            var radioButtonList = new RadioButtonList { Items = new List<RadioButton>(), SelectedValue = "selected-excess-product-code" };
            var intLinkedList = new LinkedList<Product>(availableProduct.Products.OrderBy(c => c.Excess));

            foreach (Product product in intLinkedList)
            {
                LinkedListNode<Product> previous = intLinkedList.Find(product)?.Previous;

                    radioButtonList.Items.Add(new RadioButton
                    {
                        Value = product.ProductCode,
                        Checked = selectedProductCode.ToUpper() == product.ProductCode.ToUpper(),
                        DisplayText = $"{product.Excess.ToPounds()}{CoverDetails_Resources.RadioHeading1}",
                        DescriptiveText = product.Excess > 0
                            ? GetDescriptiveText(product.Excess, product.MonthlyCost, previous?.Value?.MonthlyCost)
                            : CoverDetails_Resources.RadioDescription1,
                        AriaDescription = string.Format(CoverDetails_Resources.RadioAriaDescription1, product.Excess.ToPounds()),
                        RightHandText = product.UpsellOfferText
                    });
            }

            return radioButtonList;
        }

        private string GetDescriptiveText(double excess, double currentProductCost, double? previousProductCost)
        {
            if (previousProductCost.HasValue)
            {
                double? difference = previousProductCost - currentProductCost;
                return string.Format(CoverDetails_Resources.RadioDescription2, excess.ToPounds(), difference.Value.ToString("C"));
            }

            return string.Empty;
        }

        private CoverSummaryViewModel GetCoverSummaryViewModel(HomeServicesCustomer homeServicesCustomer)
        {
            Product selectedProduct = homeServicesCustomer.GetSelectedProduct();
            Guard.Against<Exception>(selectedProduct == null,
                $"Home Services selected product object is null for product code - {homeServicesCustomer.SelectedProductCode}");

            return new CoverSummaryViewModel
            {
                // ReSharper disable once PossibleNullReferenceException
                ContractLength = $"{selectedProduct.ContractLength.ToNumber()}{Summary_Resources.ContractLengthSuffix}",
                ContractStartDate = Summary_Resources.ContractStartDate,
                ExcessAmount = selectedProduct.Excess.ToPounds(),
                Extras = homeServicesCustomer.GetSelectedExtras().Select(extra =>
                    new ExtraViewModel
                    {
                        ExtraMonthlyCost = extra.Cost.ToPounds(),
                        ExtraYearlyCost = (extra.Cost * 12).ToPounds(),
                        ExtraName = extra.Name
                    }).ToList(),
                HasOffers = selectedProduct.OfferSummary != null && !string.IsNullOrEmpty(selectedProduct.OfferSummary.Description),
                CoverMonthlyPaymentAmount = selectedProduct.MonthlyCost.ToString("C"),
                SelectedProductName = selectedProduct.Description,
                TotalMonthlyCost = homeServicesCustomer.GetTotalMonthlyCostWithExtras().ToString("C"),
                TotalYearlyProductCost = homeServicesCustomer.GetFormattedTotalYearlyProductCost(),
                YearlyTotalCost = homeServicesCustomer.GetFormattedTotalYearlyCostWithExtras(),
                OfferHeader = selectedProduct.FullOfferText,
                OfferParagraph = selectedProduct.OfferSummaryText
            };
        }

        public SummaryViewModel GetSummaryViewModel(HomeServicesCustomer homeServicesCustomer, string homeServicesHubUrl)
        {
            return new SummaryViewModel
            {
                DirectDebitPaymentDay = homeServicesCustomer.DirectDebitDetails.DirectDebitPaymentDate,
                DirectDebitAccountName = homeServicesCustomer.DirectDebitDetails.AccountName,
                DirectDebitAccountNumber = homeServicesCustomer.DirectDebitDetails.AccountNumber,
                DirectDebitSortCode = homeServicesCustomer.DirectDebitDetails.SortCode,
                CustomerFormattedName = homeServicesCustomer.PersonalDetails.FormattedName,
                DateOfBirth = homeServicesCustomer.PersonalDetails.DateOfBirth,
                Address = homeServicesCustomer.IsLandlord
                    ? homeServicesCustomer.SelectedBillingAddress.FullAddress(homeServicesCustomer.BillingPostcode)
                    : homeServicesCustomer.SelectedCoverAddress.FullAddress(homeServicesCustomer.CoverPostcode),
                ContactNumber = homeServicesCustomer.ContactDetails.ContactNumber,
                EmailAddress = homeServicesCustomer.ContactDetails.EmailAddress,
                IsLandLord = homeServicesCustomer.IsLandlord,
                CoverAddress = homeServicesCustomer.IsLandlord
                    ? homeServicesCustomer.SelectedCoverAddress.FullAddress(homeServicesCustomer.CoverPostcode)
                    : string.Empty,
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = "BankDetails",
                    ControllerName = "HomeServices",
                    TitleAttributeText = Resources.BackButtonAlt
                },
                BankDetailsModal = new ConfirmationModalViewModel
                {
                    ModalId = "BankDetailsModal",
                    FirstMessage = Summary_Resources.BankDetailsModalMessage,
                    RedirectUrl = "/home-services-signup/bank-details",
                    CloseButtonAlt = Common_Resources.ModalCloseButtonAlt,
                    CloseButtonLabel = Common_Resources.ModalCloseButtonText,
                    CancelButtonText = Common_Resources.ModalCancelButtonText,
                    CancelButtonTextAlt = Summary_Resources.ModalCancelButtonAlt,
                    ButtonText = Summary_Resources.BankDetailsModalButtonText,
                    ButtonTextAlt = Summary_Resources.BankDetailsModalButtonAlt
                },
                PersonalDetailsModal = new ConfirmationModalViewModel
                {
                    ModalId = "PersonalDetailsModal",
                    FirstMessage = Summary_Resources.PersonalDetailsModalMessage,
                    RedirectUrl = "/home-services-signup/personal-details",
                    CloseButtonAlt = Common_Resources.ModalCloseButtonAlt,
                    CloseButtonLabel = Common_Resources.ModalCloseButtonText,
                    CancelButtonText = Common_Resources.ModalCancelButtonText,
                    CancelButtonTextAlt = Summary_Resources.ModalCancelButtonAlt,
                    ButtonText = Summary_Resources.PersonalDetailsModalButtonText,
                    ButtonTextAlt = Summary_Resources.PersonalDetailsModalButtonAlt
                },
                CoverDetailsModal = new ConfirmationModalViewModel
                {
                    ModalId = "CoverDetailsModal",
                    FirstMessage = Summary_Resources.CoverDetailsModalMessage,
                    RedirectUrl = homeServicesHubUrl,
                    CloseButtonAlt = Common_Resources.ModalCloseButtonAlt,
                    CloseButtonLabel = Common_Resources.ModalCloseButtonText,
                    CancelButtonText = Common_Resources.ModalCancelButtonText,
                    CancelButtonTextAlt = Summary_Resources.ModalCancelButtonAlt,
                    ButtonText = Summary_Resources.CoverDetailsModalButtonText,
                    ButtonTextAlt = Summary_Resources.CoverDetailsModalButtonAlt
                },
                SupplyAddressModal = new ConfirmationModalViewModel
                {
                    ModalId = "SupplyAddressModal",
                    FirstMessage = Summary_Resources.SupplyAddressDetailsModalMessage,
                    RedirectUrl = "/home-services-signup/select-address",
                    CloseButtonAlt = Common_Resources.ModalCloseButtonAlt,
                    CloseButtonLabel = Common_Resources.ModalCloseButtonText,
                    CancelButtonText = Common_Resources.ModalCancelButtonText,
                    CancelButtonTextAlt = Summary_Resources.ModalCancelButtonAlt,
                    ButtonText = Summary_Resources.SupplyAddressModalButtonText,
                    ButtonTextAlt = Summary_Resources.SupplyAddressModalButtonAlt
                },
                AccordionViewModel = new AccordionViewModel
                {
                    ProductPDFs = GetProductPDFs(homeServicesCustomer.SelectedProductCode),
                    ExtraProductPDFs = homeServicesCustomer.IsProductExtrasAvailable() ? GetExtraProductPDFs(homeServicesCustomer.SelectedProductCode): new List<ProductPDFViewModel>(),
                    IsLandLord = homeServicesCustomer.IsLandlord
                },
                CoverSummaryViewModel = GetCoverSummaryViewModel(homeServicesCustomer)
            };
        }

        private static ProductPDFViewModel MapPDFSettingsToProductPDFViewModel(PDFSettings pdfSettings)
        {
            return new ProductPDFViewModel
            {
                Url = pdfSettings.FilePath,
                Description = pdfSettings.DisplayName,
                AccText = pdfSettings.AccText
            };
        }
    }
}