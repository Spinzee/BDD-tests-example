namespace Products.Web.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;

    // hat tip: https://cpratt.co/html-editorfor-and-htmlattributes/
    public static class HtmlHelperExtensions
    {
        public static IDictionary<string, object> MergeHtmlAttributes(this HtmlHelper helper, object htmlAttributesObject, object defaultHtmlAttributesObject)
        {
            // attributes with following names will append new items
            // others will replace values set in EditorTemplate with those passed from view
            var concatKeys = new[] { "class" };

            var htmlAttributesDict = htmlAttributesObject as IDictionary<string, object>;
            var defaultHtmlAttributesDict = defaultHtmlAttributesObject as IDictionary<string, object>;

            RouteValueDictionary htmlAttributes = htmlAttributesDict != null
                ? new RouteValueDictionary(htmlAttributesDict)
                : HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributesObject);

            RouteValueDictionary defaultHtmlAttributes = defaultHtmlAttributesDict != null
                ? new RouteValueDictionary(defaultHtmlAttributesDict)
                : HtmlHelper.AnonymousObjectToHtmlAttributes(defaultHtmlAttributesObject);

            foreach (KeyValuePair<string, object> item in htmlAttributes)
                if (concatKeys.Contains(item.Key))
                {
                    defaultHtmlAttributes[item.Key] = defaultHtmlAttributes[item.Key] != null
                        ? $"{defaultHtmlAttributes[item.Key]} {item.Value}"
                        : item.Value;
                }
                else
                {
                    defaultHtmlAttributes[item.Key] = item.Value;
                }

            return defaultHtmlAttributes;
        }
    }
}