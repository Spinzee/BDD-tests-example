namespace Products.Model.Broadband
{
    using Core;

    public class ADSLLineSpeeds : LineSpeed
    {
        public override BroadbandType Type => BroadbandType.ADSL;

        public string Min { get; set; }

        public string Max { get; set; }
        
        public override string FormattedLineSpeed => Max?.Remove(Max.Length - 3) ?? "";
    }
}