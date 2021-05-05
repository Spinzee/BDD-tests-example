namespace Products.WebModel.ViewModels.TariffChange
{
    using System;
    using System.Collections.Generic;
    using Model.Enums;

    public class TariffsViewModel : BaseViewModel
    {
        public CurrentTariffViewModel CurrentTariffViewModel { get; set; }

        public List<AvailableTariff> AvailableTariffs { get; set; }

        public FuelType FuelType { get; set; }

        public bool IsRenewal { get; set; }

        public bool IsImmediateRenewal { get; set; }

        public string RenewalDate { get; set; }

        public bool HasMultiRateMeter { get; set; }

        public bool IsCustomerFallout { get; set; }

        public int TariffsCount { get; set; }

        public string FuelTypeHeader { get; set; }

        public string NewTariffSubHeader { get; set; }

        public bool ShowMultiRateMessage { get; set; }

        public AvailableTariff FollowOnTariff { get; set; }

        public string NewTariffParagraph1 { get; set; }

        public string NewTariffStartMessage { get; set; }

        public string BulletText { get; set; }

        public DateTime TariffStartDate { get; set; }

        public string AccordionEPPContent { get; set; }

        public Dictionary<string, string> DataLayer { get; set; }
    }
}