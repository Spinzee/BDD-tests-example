namespace Products.WebModel.ViewModels.Broadband.Extensions
{
    using Products.Model.Broadband;
    using Products.WebModel.Resources.Broadband;

    public static class TalkCodeExtensions
    {
        public static string GetTelDescription(this string talkCode)
        {
            switch (talkCode)
            {
                case TalkCodes.LineRentalOnly:
                    return SelectedPackage_Resources.Option1Info;
                case TalkCodes.AnytimeLandline:
                    return SelectedPackage_Resources.Option2Info;
                case TalkCodes.AnytimePlus:
                    return SelectedPackage_Resources.Option3Info;
                case TalkCodes.EveningAndWeekend:
                    return SelectedPackage_Resources.Option4Info;
            }

            return string.Empty;
        }

        public static string GetTelName(this string talkCode)
        {
            switch (talkCode)
            {
                case TalkCodes.LineRentalOnly:
                    return ProductDetails_Resources.TalkCodeLineRental;
                case TalkCodes.AnytimeLandline:
                    return ProductDetails_Resources.TalkCodeAnytime;
                case TalkCodes.AnytimePlus:
                    return ProductDetails_Resources.TalkCodeAnytimePlus;
                case TalkCodes.EveningAndWeekend:
                    return ProductDetails_Resources.TalkCodeEveningWeekend;
            }

            return string.Empty;
        }
    }
}
