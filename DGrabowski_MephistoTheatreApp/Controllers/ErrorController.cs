using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DGrabowski_MephistoTheatreApp.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
            [HandleError]
            public ActionResult NotFound()
            {
                LogError("404 - Not Found");
                return View();
            }
            [HandleError]
            public ActionResult InternalErrorServer()
            {
                LogError("500 - Internal Server Error");
                return View();
            }
            [HandleError]
            public ActionResult Forbidden()
            {
                LogError("403 - Forbidden");
                return View();
            }
            [HandleError]
            public ActionResult Unauthorized()
            {
                LogError("401 - Unauthorized");
                return View();
            }
            private void LogError(string message)
            {
                Console.WriteLine($"Error Occurred: {message}");
            }
    }
}