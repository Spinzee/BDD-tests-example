// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Products.Web.DependencyResolution
{
    using System;
    using System.Configuration;
    using ControllerHelpers;
    using ControllerHelpers.HomeServices;
    using Core.Configuration.Settings;
    using log4net.Config;
    using ControllerHelpers.Energy;
    using Infrastructure;
    using Infrastructure.GuidEncryption;
    using Infrastructure.Logging;
    using Repository;
    using Repository.Broadband;
    using Repository.Common;
    using Repository.Energy;
    using Repository.HomeServices;
    using Repository.TariffChange;
    using Service.Broadband;
    using Service.Broadband.Managers;
    using Service.Broadband.Mappers;
    using Service.Common;
    using Service.Common.Managers;
    using Service.Energy;
    using Service.Energy.Mappers;
    using Service.HomeServices;
    using Service.Security;
    using Service.TariffChange;
    using Service.TariffChange.Validators;
    using Service.TariffChange.Validators.Eligibility;
    using ServiceWrapper.AnnualEnergyReviewService;
    using ServiceWrapper.BankDetailsService;
    using ServiceWrapper.BroadbandProductsService;
    using ServiceWrapper.CAndCService;
    using ServiceWrapper.ContentManagementService;
    using ServiceWrapper.EnergyProductService;
    using ServiceWrapper.EnergyProjectionService;
    using ServiceWrapper.GoogleReCaptchaService;
    using ServiceWrapper.ManageCustomerInformationService;
    using ServiceWrapper.NewBTLineAvailabilityService;
    using ServiceWrapper.PersonalProjectionService;
    using Helpers;
    using ServiceWrapper.BundleTariffService;
    using ServiceWrapper.HomeServicesProductService;
    using ServiceWrapper.QASService;
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;
    using Core;

    public class DefaultRegistry : Registry
    {
        public DefaultRegistry(IConfigurationSettings configurationSettings)
        {
            Scan(
                scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                    scan.With(new ControllerConvention());
                });

            // Configuration
            For<IConfigurationSettings>().Use(configurationSettings);

            // Logger setup
            XmlConfigurator.Configure();
            For<ILogger>().Use<Log4NetLogger>();
            For<IConfigManager>().Use<ConfigManager>();
            For<IContextManager>().Use<ContextManager>();

            var AllowEconomyMultiRateMetersValue = ConfigurationManager.AppSettings["AllowEconomyMultiRateMeters"];

            if (string.IsNullOrEmpty(AllowEconomyMultiRateMetersValue))
            {
                throw new Exception("Missing AllowEconomyMultiRateMeters in web.config");
            }

            if (!bool.TryParse(AllowEconomyMultiRateMetersValue, out bool AllowEconomyMultiRateMeters))
            {
                throw new Exception("AllowEconomyMultiRateMeters is non-boolean in web.config");
            }

            var AllowRenewalsValue = ConfigurationManager.AppSettings["AllowRenewals"];

            if (string.IsNullOrEmpty(AllowRenewalsValue))
            {
                throw new Exception("Missing AllowRenewals in web.config");
            }

            if (!bool.TryParse(AllowRenewalsValue, out bool AllowRenewals))
            {
                throw new Exception("AllowRenewals is non-boolean in web.config");
            }

            var AllowOnlylc31aAnnualConsumptionValue = ConfigurationManager.AppSettings["AllowOnlylc31aAnnualConsumption"];

            if (string.IsNullOrEmpty(AllowOnlylc31aAnnualConsumptionValue))
            {
                throw new Exception("Missing AllowOnlylc31aAnnualConsumption in web.config");
            }

            if (!bool.TryParse(AllowOnlylc31aAnnualConsumptionValue, out bool AllowOnlylc31aAnnualConsumption))
            {
                throw new Exception("AllowOnlylc31aAnnualConsumption is non-boolean in web.config");
            }

            var AllowCollectionDayValidatorValue = ConfigurationManager.AppSettings["AllowCollectionDayValidator"];

            if (string.IsNullOrEmpty(AllowCollectionDayValidatorValue))
            {
                throw new Exception("Missing AllowCollectionDayValidator in web.config");
            }

            if (!bool.TryParse(AllowCollectionDayValidatorValue, out bool AllowCollectionDayValidator))
            {
                throw new Exception("AllowCollectionDayValidator is non-boolean in web.config");
            }

            //Singleton references
            For<IBroadbandApplicationService>().Use<BroadbandApplicationService>().Singleton();
            For<Service.Broadband.ISummaryService>().Use<Service.Broadband.SummaryService>().Singleton();
            For<Service.Broadband.IConfirmationEmailService>().Use<Service.Broadband.ConfirmationEmailService>().Singleton();
            For<IMembershipEmailService>().Use<MembershipEmailService>().Singleton();
            For<IBroadbandProductsServiceWrapper>().Use<BroadbandProductsServiceWrapper>().Singleton();
            For<ILineCheckerService>().Use<LineCheckerService>().Singleton();
            For<IBankDetailsService>().Use<BankDetailsService>().Singleton();
            For<IBankDetailsServiceWrapper>().Use<BankDetailsServiceWrapper>().Singleton();
            For<IBroadbandJourneyService>().Use<BroadbandJourneyService>().Singleton();
            For<IExistingCustomerService>().Use<ExistingCustomerService>().Singleton();
            For<IManageCustomerInformationServiceWrapper>().Use<ManageCustomerInformationServiceWrapper>().Singleton();
            For<IManageCustomerInformationServiceClientFactory>().Use<ManageCustomerInformationServiceClientFactory>().Singleton();
            For<ICustomerAccountService>().Use<CustomerAccountService>().Singleton();
            For<ICustomerService>().Use<CustomerService>().Singleton();
            For<ICustomerProfileService>().Use<CustomerProfileService>(); //.Singleton();
            For<IProfileService>().Use<ProfileService>().Singleton();
            For<IProfileRepository>().Use<ProfileRepository>(); //.Singleton();
            For<ITariffRepository>().Use<TariffRepository>(); //.Singleton();
            For<IAnnualEnergyReviewServiceWrapper>().Use<AnnualEnergyReviewServiceWrapper>().Singleton();
            For<IAnnualEnergyReviewServiceClientFactory>().Use<AnnualEnergyReviewServiceClientFactory>().Singleton();
            For<ISessionManager>().Use<SessionManager>().Singleton();
            For<IUtilityService>().Use<UtilityService>().Singleton();
            For<IAvailableTariffService>().Use<AvailableTariffService>(); //.Singleton();
            For<Service.TariffChange.ITariffService>().Use<Service.TariffChange.TariffService>().Singleton();
            For<Service.Energy.ITariffService>().Use<Service.Energy.TariffService>().Singleton();
            For<IEnergyAlertService>().Use<EnergyAlertService>().Singleton();

            For<ITariffChangeSessionService>().Use<TariffChangeSessionService>().Singleton();
            For<IJourneyDetailsService>().Use<JourneyDetailsService>().Singleton();
            For<IGoogleReCaptchaService>().Use<GoogleReCaptchaService>().Singleton();
            For<IApplicationDataMapper>().Use<ApplicationDataMapper>().Singleton();            
            For<ITariffMapper>().Use<TariffMapper>().Singleton();

            For<ICheckDigitValidator>().Use<CheckDigitValidator>().Singleton();
            For<IAccountEligibilityChecker>().Use<AccountEligibilityChecker>().Singleton();
            For<IAccountEligibilityValidator>().Use<GreenDealValidator>().Singleton();
            For<IAccountEligibilityValidator>().Use<CombinedHeatingAndPowerValidator>().Singleton();
            For<IAccountEligibilityValidator>().Use<LastBillDateValidator>().Singleton();
            For<IAccountEligibilityValidator>().Use<PaymentPlanSpecialInterestValidator>().Singleton();
            For<IAccountEligibilityValidator>().Use<SameBrandAtSameSiteValidator>().Singleton();
            For<IAccountEligibilityValidator>().Use<BillNotInExceptionValidator>().Singleton();
            For<IAccountEligibilityValidator>().Use<TariffChangeInProgressValidator>().Singleton();
            For<IAccountEligibilityValidator>().Use<MandSBrandValidator>().Singleton();
            For<IAccountEligibilityValidator>().Use<AtlanticBrandValidator>().Singleton();
            For<IAccountEligibilityValidator>().Use<ZeroAnnualCostValidator>().Singleton();
            For<IAccountEligibilityValidator>().Use<MAndSTariffNameValidator>().Singleton();
            For<IAccountEligibilityValidator>().Use<PredictAndFixTariffValidator>().Singleton();

            For<IQuoteControllerHelper>().Use<QuoteControllerHelper>();
            For<ITariffsControllerHelper>().Use<TariffsControllerHelper>();
            For<ISignUpControllerHelper>().Use<SignUpControllerHelper>();

            if (AllowOnlylc31aAnnualConsumption)
            {
                For<IAccountEligibilityValidator>().Use<Lc31AAnnualConsumptionTypeValidator>().Singleton();
            }

            if (AllowEconomyMultiRateMeters)
            {
                For<IAccountEligibilityValidator>().Use<MultiRateMeterRegistersValidator>().Singleton();
            }
            else
            {
                For<IAccountEligibilityValidator>().Use<AcquisitionValidator>().Singleton();
                For<IAccountEligibilityValidator>().Use<SingleMeterRegisterValidator>().Singleton();
            }

            if (AllowRenewals == false)
            {
                For<IAccountEligibilityValidator>().Use<RenewalsValidator>().Singleton();
            }
            For<IAccountEligibilityValidator>().Use<PaymentMethodValidator>().Singleton();

            if (AllowCollectionDayValidator == true)
            {
                For<IAccountEligibilityValidator>().Use<DirectDebitCollectionDayValidator>().Singleton();
            }
            For<IPasswordService>().Use<PasswordService>().Singleton();
            For<IActivationEmailService>().Use<ActivationEmailService>().Singleton();
            For<IEmailManager>().Use<EmailManager>().Singleton();
            For<IContentRepository>().Use<ContentRepository>().Singleton();
            For<IGuidEncrypter>().Use<GuidEncrypter>().Singleton();
            For<IPersonalProjectionServiceWrapper>().Use<PersonalProjectionServiceWrapper>().Singleton();
            For<IPersonalProjectionServiceClientFactory>().Use<PersonalProjectionServiceClientFactory>().Singleton();
            For<ICustomerAlertRepository>().Use<CustomerAlertRepository>().Singleton();
            For<ICustomerAlertService>().Use<CustomerAlertService>().Singleton();
            For<ITariffManager>().Use<TariffManager>().Singleton();
            For<IBroadbandManager>().Use<BroadbandManager>().Singleton();

            For<IDatabaseHelper>().Use<DatabaseHelper>().Singleton();
            For<IBroadbandSalesRepository>().Use<BroadBandSalesRepository>().Singleton();
            For<ICryptographyService>().Use<CryptographyService>().Singleton();
            For<IPostcodeCheckerService>().Use<PostcodeCheckerService>().Singleton();

            For<IConfirmationService>().Use<ConfirmationService>().Singleton();
            For<ICustomerDetailsService>().Use<CustomerDetailsService>().Singleton();
            For<IOnlineCreationService>().Use<OnlineCreationService>().Singleton();
            For<IPackageService>().Use<PackageService>().Singleton();

            For<IGoogleReCaptchaServiceWrapper>().Use<GoogleReCaptchaServiceWrapper>().Singleton();
            For<ITariffChangeService>().Use<TariffChangeService>().Singleton();

            For<INewBTLineAvailabilityServiceWrapper>().Use<NewBTLineAvailabilityServiceWrapper>().Singleton();
            For<INewBTLineAvailabilityServiceFactory>().Use<NewBTLineAvailabilityServiceFactory>().Singleton();

            // Energy Journey Services
            For<Service.Energy.IConfirmationEmailService>().Use<Service.Energy.ConfirmationEmailService>().Singleton();
            For<Service.Energy.ISummaryService>().Use<Service.Energy.SummaryService>().Singleton();
            For<IBankValidationService>().Use<BankValidationService>();
            For<IEnergyProjectionServiceWrapper>().Use<EnergyProjectionServiceWrapper>();
            For<IQASServiceWrapper>().Use<QASServiceWrapper>();
            For<ICacheService>().Use<CacheManager>().Singleton();
            For<IEnergyProductServiceWrapper>().Use<EnergyProductServiceWrapper>();
            For<IBundleTariffServiceWrapper>().Use<BundleTariffServiceWrapper>();

            For<IEnergySalesRepository>().Use<EnergySalesRepository>();
            For<ISalesRepository>().Use<SalesRepository>();
            For<IEnergyApplicationDataMapper>().Use<EnergyApplicationDataMapper>();
            For<ICampaignManager>().Use<CampaignManager>();
            For<ICAndCServiceWrapper>().Use<CAndCServiceWrapper>();

            //Home Services Journey
            For<IHomeServicesControllerHelper>().Use<HomeServicesControllerHelper>().Singleton();
            For<IHomeServicesProductServiceWrapper>().Use<HomeServicesProductServiceWrapper>();
            For<IProductService>().Use<ProductService>();
            For<IAddressService>().Use<AddressService>();
            For<Service.HomeServices.ISummaryService>().Use<Service.HomeServices.SummaryService>();
            For<IHomeServicesSalesRepository>().Use<HomeServicesSalesRepository>();
            For<Service.HomeServices.IConfirmationEmailService>().Use<Service.HomeServices.ConfirmationEmailService>();
            For<IStepCounterService>().Use<StepCounterService>();

            //Common
            For<IBroadbandProductsService>().Use<BroadbandProductsService>();
            For<IContentManagementAPIClient>().Use<ContentManagementAPIClient>();

            For<IContentManagementControllerHelper>().Use<ContentManagementControllerHelper>();

            var webClient = (Model.Configuration.WebClientConfigurationSection)ConfigurationManager.GetSection("webClientConfiguration");
            if(webClient == null)
            {
                throw new Exception("Missing Web Client Config in web.config");
            }

            string webClientBaseUrl = webClient.BaseUrl;
            string webClientVersion = webClient.Version;
            if (string.IsNullOrEmpty(webClientBaseUrl) || string.IsNullOrEmpty(webClientVersion))
            {
                throw new Exception("Missing Web Client Config in web.config");
            }

            WebClientHelper.Instance.BaseUrl = webClientBaseUrl;
            WebClientHelper.Instance.Version = webClientVersion;
            
            For<WebClientData>().Add(new WebClientData(webClientBaseUrl));
        }
    }
}