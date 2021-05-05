namespace Products.Model.Common
{
    public class ConfirmationEmailParameters
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SelectedProductTitle { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string FixNProtectCoverName { get; set; }
        public string FixNFibreHeader { get; set; }
    }
}
