namespace Products.WebModel.ViewModels.Energy
{
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using Model.Enums;
    using Resources.Energy;

    public class AvailableTariffsViewModel
    {
        private const string TariffPartialName = "_ChosenTariff";
        private const string BundlePartialName = "_ChosenBundle";

        public string Postcode { get; set; }

        public bool HasE7Meter { get; set; }

        public string HeaderText { get; set; }

        public string HeaderParagraph { get; set; }

        public string SubHeaderText { get; set; }

        public string SubHeaderParagraph { get; set; }

        public IEnumerable<TariffsViewModel> EnergyTariffs { get; set; }

        public IEnumerable<TariffsViewModel> BundleTariffs { get; set; }

        public IEnumerable<TariffsViewModel> AllTariffs { get; set; }

        public PaymentMethod SelectedPaymentMethod { get; set; }

        public TariffsViewModel ChosenTariff { get; set; }

        public bool HasChosenTariff { get; set; }

        public bool IsChosenTariffAvailable => ChosenTariff != null;

        public Dictionary<string, string> DataLayer { get; set; }

        public string BannerClass { get; set; }
        
        public string SwitchStepPartial { get; set; }

        public ModalTitleAndBody LoadingModal { get; set; }

        public string ChosenTariffPartialName => ChosenTariff == null ? string.Empty : ChosenTariff.IsBundle ? BundlePartialName : TariffPartialName;

        public bool ShowTabs { get; set; }

        public bool HasBundleTariffs => BundleTariffs?.Count() > (IsChosenTariffAvailable && ChosenTariff.IsBundle ? 1 : 0);

        public string FirstTabLabel { get; set; }

        public string FirstTabShowTariffSelector { get; set; }

        public string FirstTabHideTariffSelector { get; set; }

        public string FirstTabActiveClass { get; set; }

        public string MiddleTabLabel { get; set; }

        public string MiddleTabShowTariffSelector { get; set; }

        public string MiddleTabHideTariffSelector { get; set; }

        public string LastTabLabel { get; set; }

        public string LastTabShowTariffSelector { get; set; }

        public string LastTabActiveClass { get; set; }

        public bool ShowYouMightBeInterestedInText => !string.IsNullOrEmpty(YouMightBeInterestedInText);

        public string YouMightBeInterestedInText { get; set; }

        public string ShowBundleContent { get; set; }

        public string ShowEnergyContent { get; set; }

        public string ChosenTariffType => ChosenTariff == null ? string.Empty : ChosenTariff.IsBundle ? AvailableTariffs_Resources.BundleTabName : AvailableTariffs_Resources.EnergyTabName;

        public string DisplayType { get; set; }

        public string InitialShowTariffs { get; set; }

        public string InitialHideTariffs { get; set; }
    }
}