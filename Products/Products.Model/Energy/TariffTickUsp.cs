namespace Products.Model.Energy
{
    public class TariffTickUsp
    {
        public TariffTickUsp(string header, string description, int displayOrder)
        {
            Header = header;
            Description = description;
            DisplayOrder = displayOrder;
        }

        public string Header { get; }

        public string Description { get; }

        public int DisplayOrder { get; }
    }
}
