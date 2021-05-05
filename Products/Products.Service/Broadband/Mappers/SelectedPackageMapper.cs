namespace Products.Service.Broadband.Mappers
{
    using System.Collections.Generic;
    using Core;
    using Model.Broadband;
    using WebModel.ViewModels.Common;
    using Products.WebModel.ViewModels.Broadband;
    using Products.WebModel.ViewModels.Broadband.Extensions;

    public sealed class SelectedPackageMapper
    {
        public static SelectedPackageViewModel MapCustomerToSelectedPackageViewModel(Customer customer, List<TermsAndConditionsPdfLink> termsAndConditionsPdfLinks)
        {
            BroadbandProduct broadbandProduct = customer.SelectedProduct;
            BroadbandProductGroup broadbandProductGroup = customer.SelectedProduct.GetSelectedTalkProduct(customer.SelectedProductCode).BroadbandProductGroup;

            var model = new SelectedPackageViewModel
            {
                TalkProducts = broadbandProduct.ToTalkProductViewModel(),
                SelectedProductName = broadbandProduct.BroadbandType.GetTitle(broadbandProductGroup),
                SelectedProductDescription = broadbandProduct.BroadbandType.GetModalContent(),
                BroadbandType = broadbandProduct.BroadbandType,
                PostCode = customer.SelectedAddress.Postcode,
                SelectedTalkProductCode = customer.SelectedProductCode,
                CliNumber = customer.CliNumber,
                AvailablePackageLinkIsHidden = customer.SelectedProductGroup == BroadbandProductGroup.FixAndFibreV3,
                TermsAndConditionsPdfLinks = termsAndConditionsPdfLinks
            };

            if (broadbandProduct.BroadbandType == BroadbandType.ADSL)
            {
                var adslLineSpeeds = (ADSLLineSpeeds)broadbandProduct.LineSpeed;

                model.MaxDownload = BroadbandProductExtensions.GetFormattedLineSpeeds(adslLineSpeeds.Max);
                model.MinDownload = BroadbandProductExtensions.GetFormattedLineSpeeds(adslLineSpeeds.Min);
            }
            else
            {
                BroadbandProductExtensions.SetLineSpeedCap(broadbandProduct);
                var fibreLineSpeed = (FibreLineSpeeds)broadbandProduct.LineSpeed;

                model.MaxDownload = BroadbandProductExtensions.GetFormattedLineSpeeds(fibreLineSpeed.MaxDownload);
                model.MinDownload = BroadbandProductExtensions.GetFormattedLineSpeeds(fibreLineSpeed.MinDownload);
                model.MaxUpload = BroadbandProductExtensions.GetFormattedLineSpeeds(fibreLineSpeed.MaxUpload);
                model.MinUpload = BroadbandProductExtensions.GetFormattedLineSpeeds(fibreLineSpeed.MinUpload);
            }

            return model;
        }
    }
}