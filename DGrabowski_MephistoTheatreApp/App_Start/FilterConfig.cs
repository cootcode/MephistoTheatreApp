using System.Web;
using System.Web.Mvc;

namespace DGrabowski_MephistoTheatreApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
