namespace Products.ServiceWrapper.EnergyProductService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure.Extensions;
    using Sse.Retail.Energy.Products.Client.Models;
    using static Model.Enums.TariffTypeExtensions;

    public static class EnergyProductMapper
    {
        public static List<Model.Energy.Product> ToProducts(ProductsResult productsResult)
        {
            return productsResult?.Products?.Select(p => new Model.Energy.Product
            {
                ExitFee1 = p.Product?.ExitFee1,
                ProjectedYearlyCost = p.ProjectedYearlyCost,
                ServicePlanId = p.Product?.ServicePlanId,
                ServicePlanInvoiceDescription = p.Product?.ServicePlanInvoiceDescription,
                TariffType = GetTariffType(p.Product?.TariffType),
                EndOfTariffDate = p.Product?.EndOfTariffDate,
                PriceGuaranteeDate = p.Product?.PriceGuaranteeDate,
                UnitRate1InclVAT = p.Product?.UnitRate1InclVat,
                UnitRate2InclVAT = p.Product?.UnitRate2InclVat,
                StandingCharge = p.Product?.StandingChargeInclVat,
                RateCodeStandardDescription = p.Product?.RateCodeStandardDescription,
                LoyaltyBenefits = p.Product?.LoyaltyBenefits,
                EndOfTariffDateDescription = p.Product?.TariffEndDateDescription,
                PriceGuaranteeDateDescription = p.Product?.PriceGuaranteeDateDescription,
                DirectDebitDiscount = p.Discount
            }).ToList();
        }

        public static List<Model.Energy.Product> ToProducts(EnergyProductsResult productsResult)
        {
            return productsResult?.Results?.Select(product => new Model.Energy.Product
            {
                ExitFee1 = product?.ExitFee1,             
                ServicePlanId = product?.ServicePlanId,
                ServicePlanInvoiceDescription = product?.ServicePlanInvoiceDescription,
                TariffType = GetTariffType(product?.TariffType),
                EndOfTariffDate = product?.EndOfTariffDate,
                PriceGuaranteeDate = product?.PriceGuaranteeDate,
                UnitRate1InclVAT = product?.UnitRate1InclVat,
                UnitRate1ExVAT = product?.UnitRate1ExcVat,
                UnitRate1InvoiceDescription = product?.UnitRate1SpcoBillingDesc,
                UnitRate2InclVAT = product?.UnitRate2InclVat,
                UnitRate2ExVAT = product?.UnitRate2ExcVat,
                UnitRate2InvoiceDescription = product?.UnitRate2SpcoBillingDesc,
                UnitRate3InclVAT = product?.UnitRate3InclVAT,
                UnitRate3ExVAT = product?.UnitRate3ExcVAT,
                UnitRate3InvoiceDescription = product?.UnitRate3SpcoBillingDesc,
                UnitRate4InclVAT = product?.UnitRate4InclVAT,
                UnitRate4ExVAT = product?.UnitRate4ExcVAT,
                UnitRate4InvoiceDescription = product?.UnitRate4SpcoBillingDesc,
                StandingCharge = product?.StandingChargeInclVat,
                StandingChargeExVAT = product?.StandingChargeExcVat,
                RateCode = (int?)product?.RateCode,
                RateCodeStandardDescription = product?.RateCodeStandardDescription,
                LoyaltyBenefits = product?.LoyaltyBenefits,
                EndOfTariffDateDescription = product?.TariffEndDateDescription,
                PriceGuaranteeDateDescription = product?.PriceGuaranteeDateDescription,
                IsBundle = product?.IsBundle,
                EffectiveDate = product?.EffectiveDate.TryParseDateTime() ?? DateTime.MinValue
            }).ToList();
        }
    }
}
