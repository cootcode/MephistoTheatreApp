using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DGrabowski_MephistoTheatreApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "PostSearch",
                url: "Post/Search",
                defaults: new { controller = "Posts", action = "Search" }
            );
            routes.MapRoute(
                name: "SubmitComment",
                url: "Post/SubmitComment",
                defaults: new { controller = "Posts", action = "SubmitComment" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
