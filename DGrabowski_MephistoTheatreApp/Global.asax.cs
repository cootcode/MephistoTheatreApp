using DGrabowski_MephistoTheatreApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DGrabowski_MephistoTheatreApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Transfer the user to the appropriate custom error page
            HttpException lastErrorWrapper = Server.GetLastError() as HttpException;

            if (lastErrorWrapper.GetHttpCode() == 404)
            {
                Server.TransferRequest("~/Error/NotFound");
            }
            else if (lastErrorWrapper.GetHttpCode() == 500)
            {
                Server.TransferRequest("~/Error/InternalErrorServer");
            }
            else if (lastErrorWrapper.GetHttpCode() == 403)
            {
                Server.TransferRequest("~/Error/Forbidden");
            }
            else
            {
                Server.TransferRequest("~/Error/Unauthorized");
            }
        }
    }
}
