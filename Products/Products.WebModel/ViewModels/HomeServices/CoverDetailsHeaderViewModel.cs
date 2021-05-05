namespace Products.WebModel.ViewModels.HomeServices
{
    public class CoverDetailsHeaderViewModel
    {
        public string DisplayName { get; set; }
        public string MonthlyCost { get; set; }
        public string YearlyCost { get; set; }
        public string OfferText { get; set; }
        public bool HasOffers { get; set; }
        public bool IsLandLord { get; set; }
        public string BackToHomeUrl { get; set; }
        public bool HasCoverbullet { get; set; }
        public string CoverbulletText { get; set; }
    }
}
