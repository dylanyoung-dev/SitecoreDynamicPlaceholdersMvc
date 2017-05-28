using Sitecore.Mvc.Helpers;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Foundation.DynamicPlaceholders.Helpers
{
    public static class SitecoreHelperExtensions
    {
        public static IHtmlString DynamicPlaceholder(this SitecoreHelper sitecoreHelper, string placeholderName)
        {
            Rendering currentRendering = RenderingContext.Current.Rendering;

            return sitecoreHelper.Placeholder(string.Format("{0}|{1}", placeholderName, currentRendering.UniqueId.ToString()));
        }
    }
}