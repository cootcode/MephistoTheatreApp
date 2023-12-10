using Microsoft.Owin;
using Owin;

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
