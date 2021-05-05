using Products.Web.Attributes;
using System.Web.Mvc;

namespace Products.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomHandleErrorAttribute());
            filters.Add(new CustomRequireHttpsAttribute());
        }
    }
}