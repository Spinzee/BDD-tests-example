using System.Collections.Generic;
using Model;

namespace Services
{
    public interface IProductService
    {
        IList<Service> GetListOfServices();
    }
}
