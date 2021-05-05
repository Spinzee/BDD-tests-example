namespace Products.Service.TariffChange.Mappers
{
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Infrastructure.Extensions;
    using Model.Energy;
    using Tariff = Model.TariffChange.Tariffs.Tariff;

    public class TariffMapper
    {
        public static List<Tariff> MapTariffsToCMSEnergyContentTariffs(List<Tariff> tariffs, List<CMSEnergyContent> cmsEnergyContents, string servicePlanId)
        {
            var tariffsInCMS = new List<Tariff>();

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (Tariff tariff in tariffs)
            {
                if (cmsEnergyContents.Any(cmsEnergyContent => tariff.DisplayName == cmsEnergyContent.TariffNameWithoutTariffWording))
                {
                    tariffsInCMS.Add(tariff);
                }
                else
                {
                    if (tariff.ElectricityDetails?.ServicePlanId.RemoveSpacesAndConvertToUpper() == servicePlanId.RemoveSpacesAndConvertToUpper() ||
                        tariff.GasDetails?.ServicePlanId.RemoveSpacesAndConvertToUpper() == servicePlanId.RemoveSpacesAndConvertToUpper() ||
                        tariff.TariffGroup != TariffGroup.None)
                    {
                        tariffsInCMS.Add(tariff);
                    }
                }
            }

            return tariffsInCMS;
        }

        public static List<Tariff> MapTariffsToFilteredFixAndDriveTariffs(bool isDualFuel, List<Tariff> tariffs)
        {
            var filteredTariffs = new List<Tariff>();

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (Tariff tariff in tariffs)
            {
                if (tariff.TariffGroup == TariffGroup.FixAndDrive)
                {
                    if (isDualFuel)
                    {
                        filteredTariffs.Add(tariff);
                    }
                }
                else
                {
                    filteredTariffs.Add(tariff);
                }
            }

            return filteredTariffs;
        }
    }
}