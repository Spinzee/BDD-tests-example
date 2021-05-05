namespace Products.WebModel.ViewModels.Broadband.Extensions
{
    using System;
    using Core;
    using Model.Broadband;
    using Resources.Broadband;

    public static class BroadbandTypeExtensions
    {
        public static string GetTitle(this BroadbandType broadbandType, BroadbandProductGroup broadbandProductGroup)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (broadbandType)
            {
                case BroadbandType.ADSL:
                    return ProductDetails_Resources.ADSLTitle;
                case BroadbandType.Fibre:
                    switch (broadbandProductGroup)
                    {
                        case BroadbandProductGroup.FixAndFibreV3:
                            return ProductDetails_Resources.FixAndFibreV3Title;
                        default:
                            return ProductDetails_Resources.FibreTitle;
                    }

                case BroadbandType.FibrePlus:
                    if (broadbandProductGroup == BroadbandProductGroup.FixAndFibrePlus)
                    {
                        return ProductDetails_Resources.FixAndFibrePlusTitle;
                    }

                    return ProductDetails_Resources.FibrePlusTitle;
            }

            throw new Exception(ProductDetails_Resources.InvalidBroadbandType);
        }

        public static string GetSubTitle(this BroadbandType broadbandType)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (broadbandType)
            {
                case BroadbandType.ADSL:
                    return ProductDetails_Resources.ADSLSubTitle;
                case BroadbandType.Fibre:
                    return ProductDetails_Resources.FibreSubTitle;
                case BroadbandType.FibrePlus:
                    return ProductDetails_Resources.FibrePlusSubTitle;
            }

            throw new Exception(ProductDetails_Resources.InvalidBroadbandType);
        }

        public static string GetModalContent(this BroadbandType broadbandType)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (broadbandType)
            {
                case BroadbandType.ADSL:
                    return ProductDetails_Resources.ADSLHelpText;
                case BroadbandType.Fibre:
                    return ProductDetails_Resources.FibreHelpText;
                case BroadbandType.FibrePlus:
                    return ProductDetails_Resources.FibrePlusHelpText;
            }

            throw new Exception(ProductDetails_Resources.InvalidBroadbandType);
        }
    }
}