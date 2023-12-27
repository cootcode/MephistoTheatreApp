using DGrabowski_MephistoTheatreApp.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;
using System.Web.Services.Description;

[assembly: OwinStartupAttribute(typeof(DGrabowski_MephistoTheatreApp.Startup))]
namespace DGrabowski_MephistoTheatreApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }


    }

}
