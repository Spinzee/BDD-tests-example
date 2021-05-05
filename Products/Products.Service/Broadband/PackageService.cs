namespace Products.Service.Broadband
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Infrastructure;
    using Infrastructure.Logging;
    using Managers;
    using Mappers;
    using Model.Constants;
    using Products.Model.Broadband;
    using Products.WebModel.Resources.Broadband;
    using Products.WebModel.ViewModels.Broadband;
    using Products.WebModel.ViewModels.Common;

    public class PackageService : IPackageService
    {
        private readonly IBroadbandJourneyService _broadbandJourneyService;
        private readonly IBroadbandManager _broadbandManager;
        private readonly IConfigManager _configManager;
        private readonly ILogger _logger;
        private readonly ISessionManager _sessionManager;

        public PackageService(IBroadbandJourneyService broadbandJourneyService,
            ISessionManager sessionManager,
            IConfigManager configManager,
            ILogger logger,
            IBroadbandManager broadbandManager)
        {
            Guard.Against<ArgumentException>(broadbandJourneyService == null, $"{nameof(broadbandJourneyService)} is null");
            Guard.Against<ArgumentException>(sessionManager == null, $"{nameof(sessionManager)} is null");
            Guard.Against<ArgumentException>(configManager == null, $"{nameof(configManager)} is null");
            Guard.Against<ArgumentException>(logger == null, $"{nameof(logger)} is null");
            Guard.Against<ArgumentException>(broadbandManager == null, $"{nameof(broadbandManager)} is null");

            _broadbandJourneyService = broadbandJourneyService;
            _sessionManager = sessionManager;
            _configManager = configManager;
            _logger = logger;
            _broadbandManager = broadbandManager;
        }

        public AvailableTalkPackagesViewModel GetAvailableTalkPackageViewModel()
        {
            Customer customer = _broadbandJourneyService.GetBroadbandJourneyDetails().Customer;
            Guard.Against<ArgumentNullException>(customer == null, "session object is null");

            var products = _sessionManager.GetSessionDetails<List<BroadbandProduct>>("broadbandProducts");
            // ReSharper disable once PossibleNullReferenceException
            string selectedTalkCode = string.IsNullOrWhiteSpace(customer.SelectedTalkCode) ? TalkCodes.LineRentalOnly : customer.SelectedTalkCode;
            BroadbandProduct firstProduct = products.FirstOrDefault();
            if (firstProduct != null)
            {
                List<TalkProductViewModel> talkProductViewModelList = firstProduct.ToTalkProductViewModel();
                talkProductViewModelList.Where(p => p.TalkCode == selectedTalkCode).ToList().ForEach(d => d.Selected = true);
                return new AvailableTalkPackagesViewModel { TalkProducts = talkProductViewModelList, IsTalkProductAvailable = talkProductViewModelList.Any() };
            }

            return new AvailableTalkPackagesViewModel { TalkProducts = new List<TalkProductViewModel>(), IsTalkProductAvailable = false };
        }

        public AvailablePackagesViewModel GetAvailablePackagesViewModel()
        {
            try
            {
                var model = new AvailablePackagesViewModel();
                Customer customer = _broadbandJourneyService.GetBroadbandJourneyDetails().Customer;
                Guard.Against<ArgumentNullException>(customer == null, "session object is null");

                var products = _sessionManager.GetSessionDetails<List<BroadbandProduct>>("broadbandProducts");
                // ReSharper disable once PossibleNullReferenceException
                string selectedTalkCode = string.IsNullOrWhiteSpace(customer.SelectedTalkCode) ? TalkCodes.LineRentalOnly : customer.SelectedTalkCode;

                model.PostCode = customer.SelectedAddress.Postcode ?? string.Empty;
                model.Products = products.OrderBy(p => p.ProductOrder)
                    .Select(broadbandProduct => broadbandProduct
                        .ToAvailableProductViewModel(customer.IsSSECustomer, selectedTalkCode))
                    .ToList();
                if (model.Products == null || !model.Products.Any(p => p.IsAvailable))
                {
                    return null;
                }

                PopulateAccordionViewModel(model);

                return model;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString(), ex);
                return null;
            }
        }

        public bool? GetBroadbandProductsForAvailablePackages(string selectedTalkProductCode, string talkCode)
        {
            var products = _sessionManager.GetSessionDetails<List<BroadbandProduct>>("broadbandProducts");
            BroadbandJourneyDetails broadbandJourneyDetails = _broadbandJourneyService.GetBroadbandJourneyDetails();
            Guard.Against<ArgumentNullException>(broadbandJourneyDetails.Customer == null, "session object is null");

            if (products == null || !products.Any())
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(talkCode))
            {
                BroadbandProduct selectedTalkCode = products.FirstOrDefault(x =>  x.TalkProducts != null && x.TalkProducts.Any(y => y.TalkCode == talkCode));
                if (selectedTalkCode == null)
                {
                    return null;
                }
            }

            BroadbandProduct broadbandProduct = products.FirstOrDefault(p => p.TalkProducts != null && p.TalkProducts.Any(t => t.ProductCode == selectedTalkProductCode));
            if (!string.IsNullOrWhiteSpace(selectedTalkProductCode))
            {
                if (broadbandProduct == null)
                {
                    return null;
                }
            }

            bool? result = selectedTalkProductCode == null;
            
            // ReSharper disable once PossibleNullReferenceException
            broadbandJourneyDetails.Customer.SelectedProductCode = selectedTalkProductCode;
            broadbandJourneyDetails.Customer.SelectedTalkCode = talkCode;
            broadbandJourneyDetails.Customer.SelectedProduct = broadbandProduct;
            _broadbandJourneyService.SetBroadbandJourneyDetails(broadbandJourneyDetails);

            return result;
        }

        public SelectedPackageViewModel GetSelectedPackageViewModel()
        {
            Customer customer = _broadbandJourneyService.GetBroadbandJourneyDetails().Customer;
            Guard.Against<ArgumentNullException>(customer == null, "session object is null");

            // ReSharper disable once PossibleNullReferenceException
            List<TermsAndConditionsPdfLink> termsAndConditionsPdfLinks = _broadbandManager.GetTermsAndConditionPdfWithLinks(customer.SelectedProductGroup);

            SelectedPackageViewModel model = SelectedPackageMapper.MapCustomerToSelectedPackageViewModel(customer, termsAndConditionsPdfLinks);
            double installationFee = _broadbandManager.GetInstallationFee();

            double equipmentCharge = _broadbandManager.GetEquipmentChargeFee();

            model.BackChevronViewModel = new BackChevronViewModel { TitleAttributeText = Common_Resources.BackButtonAlt };

            switch (customer?.SelectedProductGroup)
            {
                case BroadbandProductGroup.FixAndFibreV3:
                    model.TermsAndConditionsParagraph1 = AvailablePackages_Resources.TermsAndConditionsParagraph1FixAndFibreV3;
                    model.TermsAndConditionsParagraph2 = AvailablePackages_Resources.TermsAndConditionsParagraph2FixAndFibreV3;
                    model.TermsAndConditionsParagraph3 = string.Format(AvailablePackages_Resources.TermsAndConditionsParagraph3FixAndFibre,"18","£28");
                    model.TermsAndConditionsParagraph4 = AvailablePackages_Resources.TermsAndConditionsParagraph4FixAndFibre;
                    model.CancellationParagraph3 = AvailablePackages_Resources.CancellationParagraph3FixAndFibreV3;
                    model.BackChevronViewModel.ActionName = "SelectAddress";
                    model.BackChevronViewModel.ControllerName = "LineChecker";
                    break;
                case BroadbandProductGroup.FixAndFibrePlus:
                    model.TermsAndConditionsParagraph3 = string.Format(AvailablePackages_Resources.TermsAndConditionsParagraph3FixAndFibre, "18", "£32");
                    break;
                default:
                    model.CancellationChargesParagraph2 = string.Format(AvailablePackages_Resources.CancellationParagraph2, equipmentCharge);
                    model.TermsAndConditionsParagraph1 = AvailablePackages_Resources.TermsAndConditionsParagraph1;
                    model.TermsAndConditionsParagraph2 = AvailablePackages_Resources.TermsAndConditionsParagraph2;
                    model.TermsAndConditionsParagraph3 = AvailablePackages_Resources.TermsAndConditionsParagraph3;
                    model.TermsAndConditionsParagraph4 = string.Empty;
                    model.CancellationParagraph3 = $"{AvailablePackages_Resources.CancellationParagraph3}{AvailablePackages_Resources.CancellationParagraph4}";
                    model.ZeroPriceBullet1 = SelectedPackage_Resources.Bullet1;
                    model.BackChevronViewModel.ActionName = customer != null && customer.IsUserFromHubPage ? "SelectAddress" : "AvailablePackages";
                    model.BackChevronViewModel.ControllerName = customer != null && customer.IsUserFromHubPage ? "LineChecker" : "Packages";
                    model.LineFeatureDisclaimer = SelectedPackage_Resources.LineFeatureDisclaimer;
                    break;
            }
            double surcharge = _broadbandManager.GetSurcharge();
            double connectionFee = _broadbandManager.GetConnectionFee();
            double newLineInstallationFee = _broadbandManager.GetInstallationFee();
            model.InstallationFeeText = string.Format(SelectedPackage_Resources.InstallationText, installationFee);
            model.YourPriceViewModel = YourPriceViewModelMapper.MapCustomerToYourPriceViewModel(customer, surcharge, connectionFee, newLineInstallationFee);
            double dataLayerInstallationFee = customer != null && customer.ApplyInstallationFee ? installationFee : 0;
            model.DataLayerDictionary = GetDataLayerMappings(dataLayerInstallationFee);
            return model;
        }

        public SelectedPackageViewModel SetSelectedPackageViewModel(SelectedPackageViewModel model, string productCode)
        {
            var products = _sessionManager.GetSessionDetails<List<BroadbandProduct>>(SessionKeys.BroadbandProducts);

            if (products == null || !products.Any())
            {
                return null;
            }

            BroadbandJourneyDetails broadbandJourneyDetails = _broadbandJourneyService.GetBroadbandJourneyDetails();
            Guard.Against<ArgumentNullException>(broadbandJourneyDetails.Customer == null, "session object is null");

            BroadbandProduct selectedProduct = products.FirstOrDefault(p => p.TalkProducts.Any(t => t.ProductCode == productCode));
            if (broadbandJourneyDetails.Customer != null)
            {
                broadbandJourneyDetails.Customer.SelectedProduct = selectedProduct;
                broadbandJourneyDetails.Customer.SelectedProductCode = productCode;
                broadbandJourneyDetails.Customer.SelectedTalkCode = selectedProduct?.TalkProducts.FirstOrDefault(p => p.ProductCode == productCode)?.TalkCode;
                model.CliNumber = broadbandJourneyDetails.Customer.CliNumber;
                double surcharge = _broadbandManager.GetSurcharge();
                double connectionFee = _broadbandManager.GetConnectionFee();
                double newLineInstallationFee = _broadbandManager.GetInstallationFee();
                model.YourPriceViewModel = YourPriceViewModelMapper.MapCustomerToYourPriceViewModel(broadbandJourneyDetails.Customer, surcharge, connectionFee, newLineInstallationFee);
            }

            _sessionManager.SetSessionDetails(SessionKeys.YourPriceDetails, model.YourPriceViewModel);

            return model;
        }

        public int GetSpeedCap()
        {
            string cap = _configManager.GetAppSetting("FibrePlusSpeedCap");
            int.TryParse(cap.Substring(0, 2), out int speedCap);
            return speedCap;
        }

        private Dictionary<string, string> GetDataLayerMappings(double installationFee)
        {
            return new Dictionary<string, string>
            {
                { DataLayer_Resources.InstallationFeeAttribute, installationFee.ToString(CultureInfo.InvariantCulture) }
            };
        }

        private void PopulateAccordionViewModel(AvailablePackagesViewModel model)
        {
            double installationFee = _broadbandManager.GetInstallationFee();
            double equipmentCharge = _broadbandManager.GetEquipmentChargeFee();

            model.BroadbandChargesParagraph = string.Format(AvailablePackages_Resources.AccordionBroadbandChargesParagraph, installationFee);
            model.CancellationChargesParagraph2 = string.Format(AvailablePackages_Resources.CancellationParagraph2, equipmentCharge);
        }
    }
}