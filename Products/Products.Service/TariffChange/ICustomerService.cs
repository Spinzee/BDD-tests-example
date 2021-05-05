using Products.Model.TariffChange.Customers;

namespace Products.Service.TariffChange
{
    public interface ICustomerService
    {
        Customer GetCustomerDetails(CustomerAccount customerAccount);
        bool CheckCustomerHasBeenSet();
        bool CheckEmailAddressIsNotNull();
    }
}