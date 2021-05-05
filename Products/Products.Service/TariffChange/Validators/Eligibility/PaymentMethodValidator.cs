using Products.Model.TariffChange.Customers;
using System.Collections.Generic;
using System;
using Products.Model.Enums;

namespace Products.Service.TariffChange.Validators.Eligibility
{
    public class PaymentMethodValidator : IAccountEligibilityValidator
    {
        public TariffChangeEligibilityCheckType falloutCheck
        {
            get
            {
                return TariffChangeEligibilityCheckType.GetEnergyDataEligibility;
            }
        }

        public FalloutReasonResult Validate(Dictionary<string, object> variables)
        {
            var falloutReason = new FalloutReasonResult();
            if (!variables.ContainsKey("PaymentMethod"))
            {
                falloutReason.FalloutDescription = "Cannot determine customer's payment method.";
                falloutReason.FalloutReason = FalloutReason.Indeterminable;
                return falloutReason;
            }

            if (!variables.ContainsKey("PaymentMethodFromGetEnergyData"))
            {
                falloutReason.FalloutReason = FalloutReason.None;
                return falloutReason;
            }

            if (variables["PaymentMethod"].ToString() == "SOR")
            {
                falloutReason.FalloutDescription = $"Ineligible payment method: {variables["PaymentMethod"]}.";
                falloutReason.FalloutReason = FalloutReason.PaymentMethodIneligible;
                return falloutReason;
            }

            string collectionDay = string.Empty;
            if (variables.ContainsKey("CollectionDay") && variables["CollectionDay"] != null)
            {
                collectionDay = variables["CollectionDay"].ToString();
            }

            switch (variables["PaymentMethodFromGetEnergyData"].ToString())
            {
                case "DDB":
                    falloutReason.FalloutDescription = $"Ineligible payment method: {variables["PaymentMethodFromGetEnergyData"]}.";
                    falloutReason.FalloutReason = string.IsNullOrEmpty(collectionDay) ? FalloutReason.PaymentMethodIneligible : FalloutReason.None;
                    return falloutReason;
                case "PPT":
                case "EXC":
                case "PGO":
                case "STO":
                    falloutReason.FalloutDescription = $"Ineligible payment method: {variables["PaymentMethodFromGetEnergyData"]}.";
                    falloutReason.FalloutReason = FalloutReason.PaymentMethodIneligible;
                    return falloutReason;
                case "QC":
                case "DDV":
                default:
                    falloutReason.FalloutReason = FalloutReason.None;
                    return falloutReason;
            }
        }
    }
}