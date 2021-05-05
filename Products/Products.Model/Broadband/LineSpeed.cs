namespace Products.Model.Broadband
{
    using Core;

    public abstract class LineSpeed
    {
        public abstract BroadbandType Type { get; }

        public abstract string FormattedLineSpeed { get; }
    }
}