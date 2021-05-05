namespace Products.WebModel.ViewModels.Common
{
    public class BroadbandPackageSpeedViewModel
    {
        public string PostCode { get; set; }
        public bool ShowUploadSpeed { get; set; }
        public string MaxDownload { get; set; }
        public string MaxUpload { get; set; }
        public string MinDownload { get; set; }
        public string MinUpload { get; set; }
        public string PackageDescription { get; set; }
        public int MaximumSpeedCap { get; set; }
        public bool ShowPostcodeText { get; set; }
        public bool ShowSpeedRangeText { get; set; }

        public bool ShowHeaderText { get; set; } = true;
        public string HeaderText { get; set; }
    }
}