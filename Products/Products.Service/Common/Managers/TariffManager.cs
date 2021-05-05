namespace Products.Service.Common.Managers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Core;
    using Core.Configuration.Settings;
    using Exceptions;
    using Infrastructure;
    using Model.Energy;
    using Model.Energy.TariffPdfConfiguration;
    using Model.Energy.TariffPdfsConfiguration;
    using Model.Energy.TariffTickUspConfiguration;
    using WebModel.Resources.Common;
    using WebModel.Resources.Energy;
    using WebModel.ViewModels.Common;

    public class TariffManager : ITariffManager
    {
        private const string SectionGroupPath = "tariffManagement";
        private const string TariffTickUspSectionName = "tariffTickUsp";
        private const string TariffPdfsSectionName = "tariffPdfs";

        private readonly IConfigManager _configManager;
        private readonly TariffPdfsSection _tariffPdfsSection;
        private readonly TariffTickUspSection _tickUspSection;
        private readonly IConfigurationSettings _settings;

        public TariffManager(IConfigManager configManager, IConfigurationSettings settings)
        {
            Guard.Against<ArgumentException>(configManager == null, $"{nameof(configManager)} is null");
            Guard.Against<ArgumentException>(settings == null, $"{nameof(settings)} is null");
            _configManager = configManager;
            _settings = settings;
            // ReSharper disable once PossibleNullReferenceException
            _tickUspSection = configManager.GetConfigSectionGroup<TariffTickUspSection>(TariffTickUspSectionName);
            _tariffPdfsSection = configManager.GetConfigSectionGroup<TariffPdfsSection>(TariffPdfsSectionName);
        }

        public string GetTagline(string tariffName) => _configManager.GetValueForKeyFromSection(SectionGroupPath, "availableTariffTaglines", tariffName);

        public List<string> GetPdfLinks(string tariffName) => PdfsForTariff(tariffName).Select(pdf => pdf.FilePath).ToList();

        public IEnumerable<TermsAndConditionsPdfLink> GetTermsAndConditionsPdfs(string tariffName, TariffGroup tariffGroup, List<CMSEnergyContent> cmsEnergyContents)
        {
            if (tariffGroup == TariffGroup.None)
            {
                return cmsEnergyContents?.FirstOrDefault(x => string.Equals(x.TariffNameWithoutTariffWording, tariffName, StringComparison.InvariantCultureIgnoreCase))
                    ?.PDFList.Select(p => new TermsAndConditionsPdfLink
                    {
                        DisplayName = p.Name,
                        Link = p.Path,
                        Title = p.Title,
                        IsEnergy = true,
                    }) ?? new List<TermsAndConditionsPdfLink>();
            }

            string GetDisplayName(string pdfFilePath) =>
                tariffGroup == TariffGroup.FixAndProtect || tariffGroup == TariffGroup.FixAndFibre
                    ? Path.GetFileNameWithoutExtension(pdfFilePath)
                    : string.Format(Summary_Resources.TermsAndConditionsPDFText, tariffName);

            return PdfsForTariff(tariffName).Select(pdf => new TermsAndConditionsPdfLink()
            {
                DisplayName = string.IsNullOrEmpty(pdf.DisplayName) ? GetDisplayName(pdf.FilePath) : pdf.DisplayName,
                Link = pdf.FilePath,
                IsEnergy = pdf.IsEnergy,
                IsBroadbandBundle = pdf.IsBroadband,
                IsHomeServicesBundlePdf = pdf.IsHomeServices,
                Title = string.IsNullOrEmpty(pdf.AccText) ? string.Format(Summary_Resources.TermsAndConditionsPDFAlt, tariffName) : pdf.AccText
            });
        }

        public TariffGroup GetTariffGroup(string servicePlanId)
        {
            var retVal = TariffGroup.None;
            servicePlanId = servicePlanId ?? string.Empty;
            if (_settings.TariffGroupSettings.Keys.Contains(servicePlanId))
            {
                retVal = _settings.TariffGroupSettings[servicePlanId];
            }

            return retVal;
        }

        public string GetSpecialTariffCardText(TariffGroup tariffGroup) => Tariff_Resources.ResourceManager.GetString($"{tariffGroup}TariffCardText");

        IEnumerable<TariffTickUsp> ITariffManager.GetTariffTickUsp(string servicePlanId)
        {
            servicePlanId = servicePlanId ?? string.Empty;
            if (_tickUspSection == null || string.IsNullOrWhiteSpace(servicePlanId) || string.IsNullOrEmpty(servicePlanId))
            {
                return new List<TariffTickUsp>();
            }

            TariffGroupConfigElement[] tariffs = _tickUspSection
                .TariffGroups
                .Cast<TariffGroupConfigElement>()
                .Where(tariff => tariff.ServicePlanIds.Contains(servicePlanId))
                .ToArray();

            switch (tariffs.Length)
            {
                case 0:
                    return new List<TariffTickUsp>();

                case 1:
                    return tariffs
                        .Single()
                        .TickUsps
                        .Cast<TickUspConfigElement>()
                        .Select(t => new TariffTickUsp(t.Header, t.Description, t.DisplayOrder))
                        .ToList();

                default:
                    throw new DuplicateTariffTickUspFoundException(servicePlanId);
            }
        }

        public bool IsSmart(string servicePlanId)
        {
            List<string> servicePlanIds = _settings.TariffManagementSettings.SmartTariffSettings.ServicePlanIds;
            return servicePlanIds?.Any(x => string.Equals(x.ToLower(), servicePlanId.ToLower(), StringComparison.InvariantCultureIgnoreCase)) ?? false;
        }

        private IEnumerable<PdfConfigElement> PdfsForTariff(string tariffName)
        {
            if (_tariffPdfsSection == null || string.IsNullOrEmpty(tariffName) || string.IsNullOrWhiteSpace(tariffName))
            {
                return new List<PdfConfigElement>();
            }

            TariffConfigElement[] tariffConfigElements = _tariffPdfsSection
                .Tariffs
                .Cast<TariffConfigElement>()
                .Where(tariff => tariff.Name.Equals(tariffName))
                .ToArray();

            switch (tariffConfigElements.Length)
            {
                case 0:
                    return new List<PdfConfigElement>();
                case 1:
                    return
                        tariffConfigElements
                            .Single()
                            .Pdfs
                            .Cast<PdfConfigElement>();
                default:
                    throw new ArgumentException($"Possibly duplicate Tariffs defined in ${TariffPdfsSectionName}' section for tariff '{tariffName}'");
            }
        }
    }
}