using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonoIndication
{
    public static class Helpers
    {
        public static MvcHtmlString Signal(this HtmlHelper html)
        {
            var svg_attr = new
            {
                version = "1.1",
                @class = "iconic iconic-signal",
                xmlns = "http://www.w3.org/2000/svg",
                x = "0px",
                y = "0px",
                width = "50px",
                height = "50px",
                viewBox = "0 0 11.26 8",
                style = "display:inline-block;"

            };
            TagBuilder svg = new TagBuilder("svg");
            svg.MergeAttribute("xmlns:xlink", "http://www.w3.org/1999/xlink");
            svg.MergeAttribute("enable-background", "new 0 0 11.26 8");
            svg.MergeAttribute("xml:space", "preserve");
            svg.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(svg_attr));

            TagBuilder title = new TagBuilder("title");
            title.SetInnerText("Значек отображает статус подключения к программе-модулю отправки GSM-сообщений. (Зеленый - 'OK', Красный - 'Не подключен', Чёрный - 'Значение не определено')");

            TagBuilder path = new TagBuilder("path");
            path.AddCssClass("iconic-signal-base");
            path.MergeAttribute("d", "M6.337,7.247c-0.391-0.391-1.023-0.391-1.414,0l0.708,0.708L6.337,7.247z");

            TagBuilder g = new TagBuilder("g");
            g.AddCssClass("iconic-signal-wave");


            TagBuilder path_inner1 = new TagBuilder("path");
            path_inner1.AddCssClass("iconic-signal-wave-inner");
            path_inner1.MergeAttribute("fill", "none");
            path_inner1.MergeAttribute("stroke", "#000000");
            path_inner1.MergeAttribute("stroke-miterlimit", "10");
            path_inner1.MergeAttribute("d", "M7.62,5.966c-1.098-1.098-2.88-1.098-3.977,0");

            TagBuilder path_inner2 = new TagBuilder("path");
            path_inner2.AddCssClass("iconic-signal-wave-middle");
            path_inner2.MergeAttribute("fill", "none");
            path_inner2.MergeAttribute("stroke", "#000000");
            path_inner2.MergeAttribute("stroke-miterlimit", "10");
            path_inner2.MergeAttribute("d", "M9.31,4.275C7.278,2.244,3.984,2.245,1.952,4.276");

            TagBuilder path_inner3 = new TagBuilder("path");
            path_inner3.AddCssClass("iconic-signal-wave-outer");
            path_inner3.MergeAttribute("fill", "none");
            path_inner3.MergeAttribute("stroke", "#000000");
            path_inner3.MergeAttribute("stroke-miterlimit", "10");
            path_inner3.MergeAttribute("d", "M10.9,2.684c-2.911-2.911-7.629-2.911-10.54,0");

            g.InnerHtml = path_inner1.ToString(TagRenderMode.SelfClosing) +
                          path_inner2.ToString(TagRenderMode.SelfClosing) +
                          path_inner3.ToString(TagRenderMode.SelfClosing);
            svg.InnerHtml = title.ToString() + path.ToString(TagRenderMode.SelfClosing) + g.ToString();
            return MvcHtmlString.Create(svg.ToString());
        }

        /* Render svg element
         * 
         * 
         * <svg version="1.1" class="iconic iconic-signal iconic-signal-weak" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" width="50px" height="50px" viewBox="0 0 11.26 8" enable-background="new 0 0 11.26 8" xml:space="preserve" style="display:inline-block">
            <path class="iconic-signal-base" d="M6.337,7.247c-0.391-0.391-1.023-0.391-1.414,0l0.708,0.708L6.337,7.247z"/>
              <g class="iconic-signal-wave">
                <path class="iconic-signal-wave-inner" fill="none" stroke="#000000" stroke-miterlimit="10" d="M7.62,5.966c-1.098-1.098-2.88-1.098-3.977,0"/>
                <path class="iconic-signal-wave-middle" fill="none" stroke="#000000" stroke-miterlimit="10" d="M9.31,4.275C7.278,2.244,3.984,2.245,1.952,4.276"/>
                <path class="iconic-signal-wave-outer" fill="none" stroke="#000000" stroke-miterlimit="10" d="M10.9,2.684c-2.911-2.911-7.629-2.911-10.54,0"/>
              </g>
           </svg>
         * 
         * */

    }
}