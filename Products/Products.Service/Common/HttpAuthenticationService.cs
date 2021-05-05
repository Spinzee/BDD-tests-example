using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace Products.Service.Common
{
    public class HttpAuthenticationService : IAuthenticationService
    {
        public void CreateGenericPrincipalWithRole()
        {
            HttpCookie authorisationCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authorisationCookie != null)
            {
                FormsAuthenticationTicket formsAuthenticationTicket = FormsAuthentication.Decrypt(authorisationCookie.Value);

                if (formsAuthenticationTicket != null)
                {
                    string[] splitData = formsAuthenticationTicket.UserData.Split(new[] { ';' });

                    if (splitData.Length == 2)
                    {
                        string roles = splitData[0];
                        string userId = splitData[1];

                        AddUserPrincipalToContext(formsAuthenticationTicket.Name, new Guid(userId), new[] { roles });
                    }
                }
            }
        }

        private void AddUserPrincipalToContext(string email, Guid userId, string[] roles)
        {
            var userPrincipal = new MyAcountsPrincipal(new GenericIdentity(email), roles, userId);
            HttpContext.Current.User = userPrincipal;
        }
    }

    public class MyAcountsPrincipal : GenericPrincipal
    {
        public MyAcountsPrincipal(IIdentity identity, string[] roles, Guid userId)
            : base(identity, roles)
        {
            UserId = userId;
        }

        public Guid UserId { get; private set; }
    }
}
