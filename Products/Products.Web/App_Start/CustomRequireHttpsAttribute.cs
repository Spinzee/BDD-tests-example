using System;
using System.Web.Mvc;

namespace Products.Web
{
    public class CustomRequireHttpsAttribute : RequireHttpsAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException(nameof(filterContext));
            }

            if (filterContext.HttpContext.Request.IsLocal)
            {
                // Don't require HTTPS for local requests.
                return;
            }
            base.OnAuthorization(filterContext);
        }

    }
}
