namespace Products.Service.Broadband.Mappers
{
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Model.Broadband;
    using WebModel.Resources.Broadband;
    using WebModel.ViewModels.Broadband;
    using WebModel.ViewModels.Broadband.Extensions;
    using WebModel.ViewModels.Common;

    public static class AvailablePackageMapper
    {
        public static AvailableProductViewModel ToAvailableProductViewModel(this BroadbandProduct product, bool isSSECustomer, string talkCode)
        {
            TalkProduct talkProduct = product.TalkProducts?.FirstOrDefault(t => t.TalkCode == talkCode);

            if (!product.IsAvailable)
            {
                return new AvailableProductViewModel
                {
                    IsAvailable = product.IsAvailable,
                    BroadbandType = product.BroadbandType,
                    Title = product.BroadbandType.GetTitle(talkProduct?.BroadbandProductGroup ?? BroadbandProductGroup.None)
                };
            }

            if (product.BroadbandType == BroadbandType.Fibre || product.BroadbandType == BroadbandType.FibrePlus)
            {
                BroadbandProductExtensions.SetLineSpeedCap(product);
            }

            BroadbandPrice broadbandPrice = talkProduct?.Prices.FirstOrDefault(t => t.FeatureCode == FeatureCodes.HeadlinePricePaperlessBilling);

            string[] monthlyCost = (broadbandPrice?.Price ?? 0).ToString("C").Split('.');
            string formattedPricePenceVal = monthlyCost.Length > 1 ? monthlyCost[1] == "00" ? string.Empty : $".{monthlyCost[1]}" : string.Empty;

            return new AvailableProductViewModel
            {
                BroadbandType = product.BroadbandType,
                IsAvailable = product.IsAvailable,
                FormattedLineSpeed = product.LineSpeed?.FormattedLineSpeed ?? "",
                Price = broadbandPrice?.Price ?? 0,
                SelectedTalkName = talkProduct?.TalkCode.GetTelName(),
                TalkCode = talkProduct?.TalkCode,
                AddOnSelected = talkProduct?.TalkCode != TalkCodes.LineRentalOnly,
                SelectedTalkProductCode = talkProduct?.ProductCode,
                FormattedPriceFullValue = monthlyCost[0],
                FormattedPricePenceValue = formattedPricePenceVal,
                Title = product.BroadbandType.GetTitle(talkProduct?.BroadbandProductGroup ?? BroadbandProductGroup.None)
            };
        }

        public static List<TalkProductViewModel> ToTalkProductViewModel(this BroadbandProduct broadbandProduct)
        {
            return broadbandProduct.TalkProducts
                .OrderBy(prod => prod.Prices.FirstOrDefault(p => p.FeatureCode == FeatureCodes.MonthlyTalkCharge)?.Price ?? 0)
                .Select(t => t.ToTalkProduct())
                .ToList();
        }

        public static TalkProductViewModel ToTalkProduct(this TalkProduct talkProduct)
        {
            return new TalkProductViewModel
            {
                TalkCode = talkProduct.TalkCode,
                TalkProductName = talkProduct.TalkCode?.GetTelName(),
                TalkProductDescription = talkProduct.TalkCode?.GetTelDescription(),
                ProductCode = talkProduct.ProductCode,
                ModalViewModel = PopulateModalDetails(talkProduct),
                Price = talkProduct.GetMonthlyTalkCost()
            };
        }

        public static TalkProductModalViewModel PopulateModalDetails(TalkProduct talkProduct)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (talkProduct.TalkCode)
            {
                case TalkCodes.LineRentalOnly:
                    return new TalkProductModalViewModel
                    {
                        ProductCode = talkProduct.ProductCode,
                        ModalTitle = SelectedPackageModal_Resources.LineRentalTitle,
                        ModalParagraph = SelectedPackageModal_Resources.LineRentalParagraph,
                        ListItems = new List<string>
                        {
                            SelectedPackageModal_Resources.LocalNatCalls15p,
                            SelectedPackageModal_Resources.UKMobCalls18p,
                            SelectedPackageModal_Resources.CallCharge23,
                            SelectedPackageModal_Resources.Call084522p,
                            SelectedPackageModal_Resources.Call087028p,
                            SelectedPackageModal_Resources.AccessCharge,
                            SelectedPackageModal_Resources.InternatCalls20p,
                            SelectedPackageModal_Resources.PremiumNumberExclusion
                        },
                        LineFeatureLinkPrefix = SelectedPackageModal_Resources.LineFeatureLinkPrefix,
                        LineFeatureLinkUrl = SelectedPackageModal_Resources.LineFeatureLinkUrl,
                        LineFeatureLinkAltText = SelectedPackageModal_Resources.LineFeatureLinkAltText,
                        LineFeatureLinkText = SelectedPackageModal_Resources.LineFeatureLinkText,
                        LineFeatureLinkSuffix = SelectedPackageModal_Resources.LineFeatureLinkSuffix,
                        MoreInformationPrefix = SelectedPackageModal_Resources.MoreInformationPrefix,
                        MoreInformationLinkUrl = SelectedPackageModal_Resources.MoreInformationLinkUrl,
                        MoreInformationLinkAltText = SelectedPackageModal_Resources.MoreInformationLinkAltText,
                        MoreInformationLinkText = SelectedPackageModal_Resources.MoreInformationLinkText
                    };
                case TalkCodes.AnytimeLandline:
                    return new TalkProductModalViewModel
                    {
                        ProductCode = talkProduct.ProductCode,
                        ModalTitle = SelectedPackageModal_Resources.AnytimeLandlineTitle,
                        ModalParagraph = SelectedPackageModal_Resources.AnytimeLandlineParagraph,
                        ListItems = new List<string>
                        {
                            SelectedPackageModal_Resources.LocalNatCalls15p70mins,
                            SelectedPackageModal_Resources.UKMobCalls18p70mins,
                            SelectedPackageModal_Resources.CallCharge23Long,
                            SelectedPackageModal_Resources.Call084522p,
                            SelectedPackageModal_Resources.Call087028p,
                            SelectedPackageModal_Resources.AccessCharge,
                            SelectedPackageModal_Resources.InternatCalls20p,
                            SelectedPackageModal_Resources.PremiumNumberExclusion
                        },
                        LineFeatureLinkPrefix = SelectedPackageModal_Resources.LineFeatureLinkPrefix,
                        LineFeatureLinkUrl = SelectedPackageModal_Resources.LineFeatureLinkUrl,
                        LineFeatureLinkAltText = SelectedPackageModal_Resources.LineFeatureLinkAltText,
                        LineFeatureLinkText = SelectedPackageModal_Resources.LineFeatureLinkText,
                        LineFeatureLinkSuffix = SelectedPackageModal_Resources.LineFeatureLinkSuffix,
                        MoreInformationPrefix = SelectedPackageModal_Resources.MoreInformationPrefix,
                        MoreInformationLinkUrl =  SelectedPackageModal_Resources.MoreInformationLinkUrl,
                        MoreInformationLinkAltText = SelectedPackageModal_Resources.MoreInformationLinkAltText,
                        MoreInformationLinkText = SelectedPackageModal_Resources.MoreInformationLinkText
                    };
                case TalkCodes.AnytimePlus:
                    return new TalkProductModalViewModel
                    {
                        ProductCode = talkProduct.ProductCode,
                        ModalTitle = SelectedPackageModal_Resources.AnytimePlusTitle,
                        ModalParagraph = SelectedPackageModal_Resources.AnytimePlusParagraph,
                        ListItems = new List<string>
                        {
                            SelectedPackageModal_Resources.LocalNatCalls15p70mins,
                            SelectedPackageModal_Resources.UKMobCalls18p70mins,
                            SelectedPackageModal_Resources.CallCharge23Long,
                            SelectedPackageModal_Resources.Call084522p,
                            SelectedPackageModal_Resources.Call087028p,
                            SelectedPackageModal_Resources.AccessCharge,
                            SelectedPackageModal_Resources.InternatCals20p70mins,
                            SelectedPackageModal_Resources.PremiumNumberExclusion
                        },
                        LineFeatureLinkPrefix = SelectedPackageModal_Resources.LineFeatureLinkPrefix,
                        LineFeatureLinkUrl = SelectedPackageModal_Resources.LineFeatureLinkUrl,
                        LineFeatureLinkAltText = SelectedPackageModal_Resources.LineFeatureLinkAltText,
                        LineFeatureLinkText = SelectedPackageModal_Resources.LineFeatureLinkText,
                        LineFeatureLinkSuffix = SelectedPackageModal_Resources.LineFeatureLinkSuffix,
                        MoreInformationPrefix = SelectedPackageModal_Resources.MoreInformationPrefix,
                        MoreInformationLinkUrl = SelectedPackageModal_Resources.MoreInformationLinkUrl,
                        MoreInformationLinkAltText = SelectedPackageModal_Resources.MoreInformationLinkAltText,
                        MoreInformationLinkText = SelectedPackageModal_Resources.MoreInformationLinkText
                    };
                case TalkCodes.EveningAndWeekend:
                    return new TalkProductModalViewModel
                    {
                        ProductCode = talkProduct.ProductCode,
                        ModalTitle = SelectedPackageModal_Resources.EveningAndWeekendTitle,
                        ModalParagraph = SelectedPackageModal_Resources.EveningAndWeekendParagraph,
                        ListItems = new List<string>
                        {
                            SelectedPackageModal_Resources.LocalNatCalls15pNotInclusive,
                            SelectedPackageModal_Resources.UKMobCalls18pNotInclusive,
                            SelectedPackageModal_Resources.CallCharge23Long,
                            SelectedPackageModal_Resources.Call084522p,
                            SelectedPackageModal_Resources.Call087028p,
                            SelectedPackageModal_Resources.AccessCharge,
                            SelectedPackageModal_Resources.InternatCalls20p,
                            SelectedPackageModal_Resources.PremiumNumberExclusion
                        },
                        LineFeatureLinkPrefix = SelectedPackageModal_Resources.LineFeatureLinkPrefix,
                        LineFeatureLinkUrl = SelectedPackageModal_Resources.LineFeatureLinkUrl,
                        LineFeatureLinkAltText = SelectedPackageModal_Resources.LineFeatureLinkAltText,
                        LineFeatureLinkText = SelectedPackageModal_Resources.LineFeatureLinkText,
                        LineFeatureLinkSuffix = SelectedPackageModal_Resources.LineFeatureLinkSuffix,
                        MoreInformationPrefix = SelectedPackageModal_Resources.MoreInformationPrefix,
                        MoreInformationLinkUrl = SelectedPackageModal_Resources.MoreInformationLinkUrl,
                        MoreInformationLinkAltText = SelectedPackageModal_Resources.MoreInformationLinkAltText,
                        MoreInformationLinkText = SelectedPackageModal_Resources.MoreInformationLinkText
                    };
            }

            return new TalkProductModalViewModel();
        }
    }
}