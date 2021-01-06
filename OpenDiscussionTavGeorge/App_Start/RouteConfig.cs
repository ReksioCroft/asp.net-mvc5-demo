using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OpenDiscussionTavGeorge
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}/{criterion}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional, criterion=UrlParameter.Optional }
            );
        }
    }
}
