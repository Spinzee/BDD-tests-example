namespace Products.WebModel.ViewModels.Broadband
{
    using Core;
    using Extensions;

    public class AvailableProductViewModel
    {        
        public BroadbandType BroadbandType { get; set; }

        public string BroadbandTypeDescription => BroadbandType.GetModalContent();

        public string Title { get; set; }

        public string SubTitle => BroadbandType.GetSubTitle();

        public string FormattedLineSpeed { get; set; }

        public bool IsAvailable { get; set; }

        public double Price { get; set; }

        public string FormattedPriceFullValue { get; set; }

        public string SelectedTalkName { get; set; }

        public string SelectedTalkProductCode { get; set; }

        public string FormattedPricePenceValue { get; set; }

        public bool AddOnSelected { get; set; }

        public string TalkCode { get; set; }
    }
}