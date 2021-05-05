namespace Products.Web.Helpers
{
    using System;
    using System.Text.RegularExpressions;
    using System.Web;

    public static class HtmlHelpers
    {
        /// <summary>
        ///     Takes an input string, and if it detects a decimal, wraps it in a span with the supplied class name
        ///     <para>e.g. £123 => £123</para>
        ///     <para>e.g. £123.12 => £123[span class="name"].12[/span]</para>
        /// </summary>
        public static IHtmlString HandlePenceSuperscript(string price, string superscriptClass)
        {
            if (price.IndexOf(".", StringComparison.Ordinal) < 0)
            {
                return new HtmlString(HttpUtility.HtmlEncode(price));
            }

            return new HtmlString(HttpUtility.HtmlEncode(price).Replace(".", "<span class=" + superscriptClass + ">.") + "</span>");
        }

        /// <summary>
        ///     If it finds a string in the form "v[0-9]?"
        /// </summary>
        public static IHtmlString HandleVersionSuperscript(string name, string superscriptClass)
        {
            name = name?.Trim();

            var rgx = new Regex(@"v[0-9]+$");
            // ReSharper disable once AssignNullToNotNullAttribute
            if (!rgx.IsMatch(name))
            {
                return new HtmlString(HttpUtility.HtmlEncode(name));
            }

            string encodedName = HttpUtility.HtmlEncode(name);
            int vIndex = encodedName.LastIndexOf("v", StringComparison.Ordinal);

            return new HtmlString(
                encodedName.Substring(0, vIndex)
                + "<span class=" + superscriptClass + ">"
                + encodedName.Substring(vIndex)
                + "</span>"
            );
        }

        public static bool IsPostLogin(dynamic postLogin)
        {
            if (postLogin == null)
            {
                return false;
            }

            bool.TryParse(postLogin.ToString(), out bool isPostLogin);
            return isPostLogin;
        }
    }
}