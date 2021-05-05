namespace Products.Model.Broadband
{
    using Core;

    public class FibreLineSpeeds : LineSpeed
    {
        public override BroadbandType Type => BroadbandType.Fibre;

        public string MinUpload { get; set; }

        public string MaxUpload { get; set; }

        public string MinDownload { get; set; }

        public string MaxDownload { get; set; }

        public override string FormattedLineSpeed => MaxDownload?.Remove(MaxDownload.Length - 3) ?? "";
    }
}