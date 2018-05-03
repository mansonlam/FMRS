using FMRS.Common.DataSource;
using FMRS.Service;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace FMRS.Helper
{
    public static class HtmlHelper 
    {
        public static IHtmlContent HelloWorldHTMLString(this IHtmlHelper htmlHelper)
            => new HtmlString("<strong>Hello World</strong>");

        public static String HelloWorldString(this IHtmlHelper htmlHelper)
            => "<strong>Hello World</strong>";


        
    }
}
