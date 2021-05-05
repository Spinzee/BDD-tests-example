namespace Products.WebModel.ViewModels.Energy
{
    public class TariffTickUspViewModel
    {
        public TariffTickUspViewModel(string header, string description, int displayOrder)
        {
            Header = header;
            Description = description;
            DisplayOrder = displayOrder;
        }
        
        public string Header { get; }
        public string Description { get;}
        public int DisplayOrder { get; }
    }
}
