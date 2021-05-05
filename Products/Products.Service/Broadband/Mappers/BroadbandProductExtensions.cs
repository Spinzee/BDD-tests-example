namespace Products.Service.Broadband.Mappers
{
    using System;
    using System.Configuration;
    using System.Linq;
    using Core;
    using Model.Broadband;
    using WebModel.Resources.Broadband;
    using WebModel.ViewModels.Broadband;
    using WebModel.ViewModels.Broadband.Extensions;

    public static class BroadbandProductExtensions
    {
        private static readonly string FibreSpeedCap = ConfigurationManager.AppSettings["FibreSpeedCap"];
        private static readonly string FibrePlusSpeedCap = ConfigurationManager.AppSettings["FibrePlusSpeedCap"];
        private static readonly string FibreUploadSpeedCap = ConfigurationManager.AppSettings["FibreUploadSpeedCap"];
        private static readonly string FibrePlusUploadSpeedCap = ConfigurationManager.AppSettings["FibrePlusUploadSpeedCap"];

        public static string GetButtonText(this AvailableProductViewModel product) =>
            string.Format(ProductDetails_Resources.ButtonTextPattern, product.BroadbandType.GetTitle(BroadbandProductGroup.None));

        public static string GetButtonAltText(this AvailableProductViewModel product) =>
            string.Format(ProductDetails_Resources.ButtonAltTextPattern, product.BroadbandType.GetTitle(BroadbandProductGroup.None));

        public static double GetPhoneCost(this TalkProduct product) =>
            product.Prices.Where(p => p.FeatureCode == FeatureCodes.MonthlyTalkCharge)
                .Select(p => p.Price).Sum();

        public static double GetBroadbandCost(this TalkProduct product) =>
            product.Prices.Where(p => p.FeatureCode == FeatureCodes.MonthlyBroadbandCharge || p.FeatureCode == FeatureCodes.MonthlyLineRental)
                .Select(p => p.Price).Sum();

        public static TalkProduct GetSelectedTalkProduct(this BroadbandProduct product, string productCode) =>
            product.TalkProducts.FirstOrDefault(t => t.ProductCode == productCode);

        public static void SetLineSpeedCap(BroadbandProduct product)
        {
            string speedCap = "0";
            string uploadSpeedCap = "0";

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (product.BroadbandType == BroadbandType.Fibre)
            {
                speedCap = FibreSpeedCap;
                uploadSpeedCap = FibreUploadSpeedCap;
            }

            if (product.BroadbandType == BroadbandType.FibrePlus)
            {
                speedCap = FibrePlusSpeedCap;
                uploadSpeedCap = FibrePlusUploadSpeedCap;
            }

            var lineSpeed = (FibreLineSpeeds) product.LineSpeed;
            if (lineSpeed?.MaxDownload != null &&
                Convert.ToInt32(lineSpeed.MaxDownload) > Convert.ToInt32(speedCap))
            {
                lineSpeed.MaxDownload = speedCap;
            }

            if (lineSpeed?.MinDownload != null &&
                Convert.ToInt32(lineSpeed.MinDownload) > Convert.ToInt32(speedCap))
            {
                lineSpeed.MinDownload = speedCap;
            }

            if (lineSpeed?.MaxUpload != null &&
                Convert.ToInt32(lineSpeed.MaxUpload) > Convert.ToInt32(uploadSpeedCap))
            {
                lineSpeed.MaxUpload = uploadSpeedCap;
            }

            if (lineSpeed?.MinUpload != null &&
                Convert.ToInt32(lineSpeed.MinUpload) > Convert.ToInt32(uploadSpeedCap))
            {
                lineSpeed.MinUpload = uploadSpeedCap;
            }
        }

        public static string GetFormattedLineSpeeds(string linespeed)
        {
            string lineSpeed = linespeed.Remove(linespeed.Length - 3);
            return string.IsNullOrEmpty(lineSpeed) ? "0" : lineSpeed;
        }
    }
}